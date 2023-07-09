using AutoMapper;
using Movies.Core.Interfaces;

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
				return new OkObjectResult(_successFactory.Create());
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

                //var response = CustomResponse<List<Genre>>.CreateSuccessCustomResponse(200, data);
				return  Ok(data);
			}
			catch
			{
				return BadRequest();
			}
		}
		*/
		#endregion



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
				return new BadRequestObjectResult(_failureFactory.Create());
			}
			else
			{
				var g = new Genre { Name = genre.Name! };
				//await _context.AddAsync(g);
				await _unitOfWork.Genres.AddAsync(g);
				_unitOfWork.Complete();

				// Only one message
				//return Content(HttpStatusCode.Created.ToString(), "Created Genre Succfully");

				_successFactory = new SuccessResponseFactory<Genre>(201, g);
				return new ObjectResult(_successFactory.Create())
				{ StatusCode = StatusCodes.Status201Created };


				//return new OkObjectResult(new CustomOkResponse<Genre>() { Data = g , StatusCode = 201});
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync(int id, GenreRequestDto dto)
		{
			if (dto.Name.IsNullOrEmpty())
			{
				_failureFactory = new FailureResponseFactory(400, "You should provide Genre name for update");
				return new BadRequestObjectResult(_failureFactory.Create());
			}

			//var genre = await _context.Genres.FirstOrDefaultAsync(g => g.ID == id);
			var genre = await _unitOfWork.Genres.GetByExpressionAsync(G => G.ID == (byte)id);
			if (genre is null)
			{
				_failureFactory = new FailureResponseFactory(404, "The Genre ID was not found");
				return new NotFoundObjectResult(_failureFactory.Create());
			}

			genre.Name = dto.Name!;
			_unitOfWork.Complete();

			_successFactory = new SuccessResponseFactory<GenreResponseDto>(200, _mapper.Map<GenreResponseDto>(genre));
			return new OkObjectResult(_successFactory.Create());
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			//var genre = await _context.Genres.FirstOrDefaultAsync(g => g.ID == id);
			var genre = await _unitOfWork.Genres.GetByExpressionAsync(G => G.ID == (byte)id);
			if (genre is null)
			{
				_failureFactory = new FailureResponseFactory(404, "There is No Genre with the provided Id");
				return new NotFoundObjectResult(_failureFactory.Create());
			}

			_unitOfWork.Genres.Delete(genre);
			_unitOfWork.Complete();

			_successFactory = new SuccessResponseFactory<GenreResponseDto>(200, _mapper.Map<GenreResponseDto>(genre));
			return new OkObjectResult(_successFactory.Create());
		}
	}
}
