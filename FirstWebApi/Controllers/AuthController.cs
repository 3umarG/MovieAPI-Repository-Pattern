using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Core.Interfaces;
using Movies.Core.Models.Factories;

namespace MoviesApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly IMapper _mapper;
		private readonly IResponseFactory _successFactory;
		private readonly IResponseFactory _failureFactory;
		private readonly IResponseFactory _unAuthorizedFactory;


		public AuthController(IAuthService authService, IMapper mapper)
		{
			_authService = authService;
			_mapper = mapper;
			_unAuthorizedFactory = new UnAuthorizedFailureResponseFactory();
		}
	}
}
