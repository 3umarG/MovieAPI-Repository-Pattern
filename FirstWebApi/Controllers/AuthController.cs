using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Core.Interfaces;
using Movies.Core.Models.Auth;
using Movies.Core.Models.Factories;
using Serilog;

namespace MoviesApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly IMapper _mapper;
		private IResponseFactory _successFactory;
		private IResponseFactory _failureFactory;
		private readonly IResponseFactory _unAuthorizedFactory;
		private readonly ILogger<AuthController> _logger;


		public AuthController(IAuthService authService, IMapper mapper, ILogger<AuthController> logger)
		{
			_authService = authService;
			_mapper = mapper;
			_unAuthorizedFactory = new UnAuthorizedFailureResponseFactory();
			_logger = logger;
		}


		[HttpPost("register")]
		public async Task<IActionResult> RegisterAsync(UserRegisterDto dto)
		{
			var result = await _authService.RegisterAsync(dto);

			if (!result.IsAuthed)
			{

				_failureFactory = new FailureResponseFactory(400, result.Message);
				return BadRequest(_failureFactory.CreateResponse());
			}

			SetRefreshTokenToCookies(result.RefreshToken!, result.RefreshTokenExpiration);
			_successFactory = new SuccessResponseFactory<AuthModel>(200, result);
			return Ok(_successFactory.CreateResponse());
		}


		[HttpPost("login")]
		public async Task<IActionResult> LoginAsync(UserLoginDto dto)
		{
			var result = await _authService.LoginAsync(dto);

			if (!result.IsAuthed)
			{
				_failureFactory = new FailureResponseFactory((int)HttpStatusCode.Unauthorized, result.Message);
				_logger.LogInformation("UnAuthorized Login Access !!", _failureFactory.CreateResponse());
				return Unauthorized(_failureFactory.CreateResponse());
			}

			SetRefreshTokenToCookies(result.RefreshToken, result.RefreshTokenExpiration);
			_successFactory = new SuccessResponseFactory<AuthModel>(200, result);
			return Ok(_successFactory.CreateResponse());
		}


		[HttpGet("refresh-token")]
		public async Task<IActionResult> RefreshTokenAsync()
		{
			var token = Request.Cookies["refreshToken"];

			if (token is null)
			{
				_failureFactory = new FailureResponseFactory(400, "You should provide refreshToken !!");
				return BadRequest(_failureFactory.CreateResponse());
			}

			var result = await _authService.RefreshTokenAsync(token);

			if (!result.IsAuthed)
			{
				var response = _unAuthorizedFactory.CreateResponse();
				response.Message = result.Message;
				return Unauthorized(response);
			}

			SetRefreshTokenToCookies(result.RefreshToken, result.RefreshTokenExpiration);
			_successFactory = new SuccessResponseFactory<AuthModel>(200, result);
			return Ok(_successFactory.CreateResponse());
		}


		[HttpGet("revoke-token")]
		public async Task<IActionResult> RevokeTokenAsync()
		{
			var token = Request.Cookies["refreshToken"];

			if (token is null)
			{
				return Unauthorized(_unAuthorizedFactory.CreateResponse());
			}

			if (!await _authService.RevokeTokenAsync(token))
			{
				_failureFactory = new FailureResponseFactory(400, "Something went wrong when revoking the token");
				return BadRequest(_failureFactory.CreateResponse());
			}

			return NoContent();
		}


		private void SetRefreshTokenToCookies(string refreshToken, DateTime expiresOn)
		{
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Expires = expiresOn
			};

			Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
		}
	}
}
