using Movies.Core.Interfaces;

namespace MoviesApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GenresController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public GenresController(IUnitOfWork unitOfWork)
		{
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
				return new OkObjectResult(CustomResponse<List<Genre>>.CreateSuccessCustomResponse(200, data));
			}
			catch
			{
				return BadRequest();
			}
		}

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

				return new BadRequestObjectResult(
						CustomResponse<object>.CreateFailureCustomResponse((int)HttpStatusCode.BadRequest, new List<string> { "You should provide Name for Genre " })
					);
			}
			else
			{
				var g = new Genre { Name = genre.Name! };
				//await _context.AddAsync(g);
				await _unitOfWork.Genres.AddAsync(g);
				_unitOfWork.Complete();

				// Only one message
				//return Content(HttpStatusCode.Created.ToString(), "Created Genre Succfully");

				return new ObjectResult(
					CustomResponse<Genre>.CreateSuccessCustomResponse(
						(int)HttpStatusCode.Created,
						g
					))
				{ StatusCode = StatusCodes.Status201Created };


				//return new OkObjectResult(new CustomOkResponse<Genre>() { Data = g , StatusCode = 201});
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync(int id, GenreRequestDto dto)
		{
			if (dto.Name.IsNullOrEmpty())
			{
				return new BadRequestObjectResult(
					CustomResponse<object>
						.CreateFailureCustomResponse(400, new List<string> { "You should provide Genre name for update" })
					);
			}

			//var genre = await _context.Genres.FirstOrDefaultAsync(g => g.ID == id);
			var genre = await _unitOfWork.Genres.GetByExpressionAsync(G => G.ID == (byte)id);
			if (genre is null)
			{
				return new NotFoundObjectResult(
						CustomResponse<object>
						.CreateFailureCustomResponse(404, new List<string> { "The Genre ID was not found"})
					);
			}

			genre.Name = dto.Name!;
			_unitOfWork.Complete();


			return new OkObjectResult(CustomResponse<Genre>.CreateSuccessCustomResponse(200 ,genre));
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			//var genre = await _context.Genres.FirstOrDefaultAsync(g => g.ID == id);
			var genre = await _unitOfWork.Genres.GetByExpressionAsync(G => G.ID == (byte)id);
			if (genre is null)
			{
				return new NotFoundObjectResult(
					CustomResponse<object>.CreateFailureCustomResponse(404, new List<string> { "There is No Genre with the provided Id"}));
			}

			_unitOfWork.Genres.Delete(genre);
			_unitOfWork.Complete();

			return new OkObjectResult(
				CustomResponse<Genre>.CreateSuccessCustomResponse(200, genre));
		}
	}
}
