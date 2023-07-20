using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Core.DTOs;
using Movies.Core.Interfaces;
using Movies.Core.Models.Auth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IMapper _mapper;
		private readonly JWT _jwt;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AuthService(
			UserManager<ApplicationUser> userManager,
			IMapper mapper,
			IOptions<JWT>  jwt,
			RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_mapper = mapper;
			_jwt = jwt.Value;
			_roleManager = roleManager;
		}

		public async Task<AuthModel> LoginAsync(UserLoginDto dto)
		{
			var auth = new AuthModel();

			var user = await _userManager.FindByEmailAsync(dto.EmailOrUserName);

			if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
			{
				auth.Message = "Your Email or Password is not correct";
				return auth;
			}

			auth.UserName = user.UserName;
			auth.Email = user.Email;
			auth.IsAuthed = true;

			var jwtToken = await CreateJwtToken(user);
			auth.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
			auth.AccessTokenExpiration = jwtToken.ValidTo;



			var refreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);

			if (refreshToken is null)
			{
				refreshToken = GenerateRefreshToken();
				user.RefreshTokens.Add(refreshToken);
				await _userManager.UpdateAsync(user);
			}


			auth.RefreshToken = refreshToken.Token;
			auth.RefreshTokenExpiration = refreshToken.ExpiresOn;


			return auth;
		}

		public async Task<AuthModel> RefreshTokenAsync(string oldToken)
		{
			var auth = new AuthModel();

			var user = await _userManager
								.Users
								.FirstOrDefaultAsync(u =>
									u.RefreshTokens
									.Any(t => t.Token == oldToken)
								);

			if (user is null)
			{
				auth.Message = "Invalid Refresh Token";
				return auth;
			}

			var refreshToken = user.RefreshTokens.Single(t => t.Token == oldToken);

			if (!refreshToken.IsActive)
			{
				auth.Message = "Inactive Refresh Token";
				return auth;
			}

			// Active Refresh Token :

			// Revoke old one ...
			refreshToken.RevokedOn = DateTime.UtcNow;

			// create new refresh token
			var newRefreshToken = GenerateRefreshToken();
			user.RefreshTokens.Add(newRefreshToken);

			// update user
			await _userManager.UpdateAsync(user);

			// create new JWT Token 
			var newJwtToken = await CreateJwtToken(user);

			auth.Email = user.Email;
			auth.UserName = user.UserName;
			auth.IsAuthed = true;
			auth.RefreshToken = newRefreshToken.Token;
			auth.RefreshTokenExpiration = newRefreshToken.ExpiresOn;
			auth.Token = new JwtSecurityTokenHandler().WriteToken(newJwtToken);
			auth.AccessTokenExpiration = newJwtToken.ValidTo;



			return auth;
		}

		public async Task<AuthModel> RegisterAsync(UserRegisterDto dto, string? role = "user")
		{
			var authModel = new AuthModel();
			var user = await _userManager.FindByEmailAsync(dto.EmailOrUserName);

			if (user is not null)
			{
				authModel.IsAuthed = false;
				authModel.Message = "There is already User with same UserName or Email.";
				return authModel;
			}

			var appUser = _mapper.Map<ApplicationUser>(dto);
			appUser.Email = dto.EmailOrUserName;

			var result = await _userManager.CreateAsync(appUser, dto.Password);

			if (!result.Succeeded)
			{
				authModel.Message = "Something went wrong , please try again !!";
				return authModel;
			}

			await _userManager.AddToRoleAsync(appUser, role);
			await InitializeSuccessAuthModel(authModel, appUser);

			return authModel;
		}

		private async Task InitializeSuccessAuthModel(AuthModel authModel, ApplicationUser appUser)
		{
			var jwtToken = await CreateJwtToken(appUser);
			var refreshToken = GenerateRefreshToken();

			appUser.RefreshTokens.Add(refreshToken);
			await _userManager.UpdateAsync(appUser);

			authModel.IsAuthed = true;
			authModel.Email = appUser.Email;
			authModel.UserName = appUser.UserName;

			authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
			authModel.AccessTokenExpiration = jwtToken.ValidTo;
			authModel.RefreshToken = refreshToken.Token;
			authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
		}

		public async Task<bool> RevokeTokenAsync(string token)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

			if (user is null)
			{

				return false;
			}


			var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

			if (!refreshToken.IsActive)
			{
				return false;
			}

			refreshToken.RevokedOn = DateTime.UtcNow;
			await _userManager.UpdateAsync(user);


			return true;
		}

		private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
		{
			var userClaims = await _userManager.GetClaimsAsync(user);
			var roles = await _userManager.GetRolesAsync(user);
			var roleClaims = new List<Claim>();

			foreach (var role in roles)
				roleClaims.Add(new Claim("roles", role));

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim("uid", user.Id)
			}
			.Union(userClaims)
			.Union(roleClaims);

			var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
			var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

			var jwtSecurityToken = new JwtSecurityToken(
				issuer: _jwt.Issuer,
				audience: _jwt.Audience,
				claims: claims,
				expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
				signingCredentials: signingCredentials);

			return jwtSecurityToken;
		}

		private static RefreshToken GenerateRefreshToken()
		{
			var randomNumber = new byte[32];

			using var generator = new RNGCryptoServiceProvider();

			generator.GetBytes(randomNumber);

			return new RefreshToken
			{
				Token = Convert.ToBase64String(randomNumber),
				ExpiresOn = DateTime.UtcNow.AddDays(10),
				CreatedOn = DateTime.UtcNow
			};
		}

		public async Task<string> AddUserToRoleAsync(AddUserToRoleRequestDto dto)
		{
			// check for the existing user ...
			var user = await _userManager.FindByNameAsync(dto.EmailOrUserName);

			if (user is null)
				return $"There is no user called : {dto.EmailOrUserName}";

			// check for the role name ...
			var isValidRoleName = await _roleManager.RoleExistsAsync(dto.RoleName);

			if (!isValidRoleName)
				return $"There is no existing role called : {dto.RoleName}";

			// check for is that user already has this role or not ..
			if (await _userManager.IsInRoleAsync(user, dto.RoleName))
				return $"User : {dto.EmailOrUserName} is already assigned to {dto.RoleName} Role .";

			var result = await _userManager.AddToRoleAsync(user, dto.RoleName);

			if (!result.Succeeded)
				return "Something wen wrong";

			return string.Empty;
		}
	}
}
