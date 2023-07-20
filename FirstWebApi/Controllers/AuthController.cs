using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Core.Interfaces;
using Movies.Core.Models.Auth;
using Movies.Core.Models.Factories;

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


		public AuthController(IAuthService authService, IMapper mapper)
		{
			_authService = authService;
			_mapper = mapper;
			_unAuthorizedFactory = new UnAuthorizedFailureResponseFactory();
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

			_successFactory = new SuccessResponseFactory<AuthModel>(200, result);
			return Ok(_successFactory.CreateResponse());
		}
	}
}
