using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
		private readonly RoleManager<ApplicationUser> _roleManager;

		public AuthService(
			UserManager<ApplicationUser> userManager,
			IMapper mapper,
			JWT jwt,
			RoleManager<ApplicationUser> roleManager)
		{
			_userManager = userManager;
			_mapper = mapper;
			_jwt = jwt;
			_roleManager = roleManager;
		}

		public Task<AuthModel> LoginAsync(UserLoginDto dto)
		{
			throw new NotImplementedException();
		}

		public Task<AuthModel> RefreshTokenAsync(string oldRefreshToken)
		{
			throw new NotImplementedException();
		}

		public Task<AuthModel> RegisterAsAdmin(UserRegisterDto dto)
		{
			throw new NotImplementedException();
		}

		public async Task<AuthModel> RegisterAsync(UserRegisterDto dto)
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
			var result = await _userManager.CreateAsync(appUser , dto.Password);

			if (!result.Succeeded)
			{
				authModel.Message = "Something went wrong , please try again !!";
				return authModel;
			}

			await InitializeSuccessAuthModel(authModel, appUser);

			return authModel;
		}

		private async Task InitializeSuccessAuthModel(AuthModel authModel, ApplicationUser appUser)
		{
			var jwtToken = await CreateJwtToken(appUser);
			var refreshToken = GenerateRefreshToken();

			authModel.IsAuthed = true;
			authModel.Email = appUser.Email;
			authModel.UserName = appUser.UserName;

			authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
			authModel.AccessTokenExpiration = jwtToken.ValidTo;
			authModel.RefreshToken = refreshToken.Token;
			authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
		}

		public Task<bool> RevokeTokenAsync(string token)
		{
			throw new NotImplementedException();
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
	}
}
