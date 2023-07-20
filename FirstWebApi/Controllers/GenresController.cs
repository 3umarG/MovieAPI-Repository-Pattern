using AutoMapper;
using Movies.Core.Interfaces;
using Movies.Core.Models.Factories;
using Movies.Core.Models.Responses;
using Newtonsoft.Json.Linq;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class GenresController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		// will initialized for every action result.
		private IResponseFactory _successFactory;
		private IResponseFactory _failureFactory;
		private readonly IMapper _mapper;

		public GenresController(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_mapper = mapper;
			_unitOfWork = unitOfWork;
		}

		[ProducesResponseType(StatusCodes.Status200OK , Type = typeof(SuccessResponse<List<GenreResponseDto>>))]
		[HttpGet]
		public async Task<IActionResult> GetAllAsync()
		{
			try
			{
				//return new OkObjectResult(new CustomResponse<List<Genre>>()
				//{
				//	Message = "Get All Genres Successfully .",
				//	StatusCode = 200,
				//	Data = await _unitOfWork.Genres.GetAllAsync()
				//});

				var data = await _unitOfWork.Genres.GetAllAsync();
				var dataDto = _mapper.Map<List<GenreResponseDto>>(data);
				_successFactory = new SuccessResponseFactory<List<GenreResponseDto>>(200, dataDto);
				return new OkObjectResult(_successFactory.CreateResponse());
			}
			catch
			{
				return BadRequest();
			}
		}




		#region Get All Genres with their Movies End point response error.
		/*
		[HttpGet("AllGenresWithTheirMovies")]
		public IActionResult GetAllGenresWithTheirMovies()
		{
			try
			{
				//return new OkObjectResult(new CustomResponse<List<Genre>>()
				//{
				//	Message = "Get All Genres Successfully .",
				//	StatusCode = 200,
				//	Data = await _unitOfWork.Genres.GetAllAsync()
				//});

				var data = _unitOfWork.Genres.QueryableOf().Include(G => G.Movies).ToList();

                //var response = CustomResponse<List<Genre>>.CreateResponseSuccessCustomResponse(200, data);
				return  Ok(data);
			}
			catch
			{
				return BadRequest();
			}
		}
		*/
		#endregion



		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GenreResponseDto))]
		[ProducesResponseType(StatusCodes.Status400BadRequest , Type = typeof(FailureResponse))]
		[HttpPost]
		public async Task<IActionResult> CreateGenreAsync(GenreRequestDto genre)
		{
			if (genre.Name.IsNullOrEmpty())
			{

				//return new BadRequestObjectResult(new CustomResponse<object>()
				//{
				//	Status = false,
				//	StatusCode = 404,
				//	Message = "Genre Name Should be specified !!",
				//});
				_failureFactory = new FailureResponseFactory(400, "You should provide Name for Genre");
				return new BadRequestObjectResult(_failureFactory.CreateResponse());
			}
			else
			{
				var g = new Genre { Name = genre.Name! };
				//await _context.AddAsync(g);
				await _unitOfWork.Genres.AddAsync(g);
				_unitOfWork.Complete();

				// Only one message
				//return Content(HttpStatusCode.CreateResponsed.ToString(), "CreateResponsed Genre Succfully");

				_successFactory = new SuccessResponseFactory<GenreResponseDto>(201, _mapper.Map<GenreResponseDto>(g));
				return new ObjectResult(_successFactory.CreateResponse())
				{ StatusCode = StatusCodes.Status201Created };


				//return new OkObjectResult(new CustomOkResponse<Genre>() { Data = g , StatusCode = 201});
			}
		}


		[ProducesResponseType(StatusCodes.Status200OK ,Type = typeof(SuccessResponse<GenreResponseDto>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest ,Type = typeof(FailureResponse))]
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync(int id, GenreRequestDto dto)
		{
			if (dto.Name.IsNullOrEmpty())
			{
				_failureFactory = new FailureResponseFactory(400, "You should provide Genre name for update");
				return new BadRequestObjectResult(_failureFactory.CreateResponse());
			}

			//var genre = await _context.Genres.FirstOrDefaultAsync(g => g.ID == id);
			var genre = await _unitOfWork.Genres.GetByExpressionAsync(G => G.ID == (byte)id);
			if (genre is null)
			{
				_failureFactory = new FailureResponseFactory(404, "The Genre ID was not found");
				return new NotFoundObjectResult(_failureFactory.CreateResponse());
			}

			genre.Name = dto.Name!;
			_unitOfWork.Complete();

			_successFactory = new SuccessResponseFactory<GenreResponseDto>(200, _mapper.Map<GenreResponseDto>(genre));
			return new OkObjectResult(_successFactory.CreateResponse());
		}




		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<GenreResponseDto>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(FailureResponse))]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			//var genre = await _context.Genres.FirstOrDefaultAsync(g => g.ID == id);
			var genre = await _unitOfWork.Genres.GetByExpressionAsync(G => G.ID == (byte)id);
			if (genre is null)
			{
				_failureFactory = new FailureResponseFactory(404, "There is No Genre with the provided Id");
				return new NotFoundObjectResult(_failureFactory.CreateResponse());
			}

			_unitOfWork.Genres.Delete(genre);
			_unitOfWork.Complete();

			_successFactory = new SuccessResponseFactory<GenreResponseDto>(200, _mapper.Map<GenreResponseDto>(genre));
			return new OkObjectResult(_successFactory.CreateResponse());
		}
	}
}
