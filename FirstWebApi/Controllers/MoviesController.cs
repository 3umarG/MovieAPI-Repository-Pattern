

using Movies.Core.DTOs;
using Movies.Core.Interfaces;

namespace MoviesApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MoviesController : ControllerBase
	{
		private readonly string[] _allowedImageExtensions = { ".jpg", ".png" };
		private readonly long _maxPosterLengthInByte = 1048576;


		private readonly IUnitOfWork _unitOfWork;
		public MoviesController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}


		[HttpGet]
		public IActionResult GetAll()
		{
			var movies = _unitOfWork.Movies.GetAllAsync<MovieResponseDto>(
					M => new MovieResponseDto()
					{
						ID = M.ID,
						Rate = M.Rate,
						Title = M.Title,
						Year = M.Year,
						Genre = new GenreResponseDto { Id = M.Genre.ID, Name = M.Genre.Name },
						StoryLine = M.StoryLine
					},
					new string[] { "Genre" }
				);
			return SuccessObjectResult<List<MovieResponseDto>>(movies, (int)HttpStatusCode.OK);
		}

		[HttpGet("{id:int}")]
		public IActionResult GetMovieByIdAsync(int id)
		{
			var movie = _unitOfWork.Movies.GetByExpressionWithInclude<MovieResponseDto>(
					M => new MovieResponseDto
					{
						ID = M.ID,
						Genre = new GenreResponseDto
						{
							Id = M.Genre.ID,
							Name = M.Genre.Name
						},
						Rate = M.Rate,
						StoryLine = M.StoryLine,
						Title = M.Title,
						Year = M.Year
					},
					M => M.ID == id,
					new string[] { "Genre" }

				);
			//var movieDto = new MovieResponseDto();
			if (movie is null)
			{
				return new NotFoundObjectResult(
					new CustomResponse<object>()
					{
						//Errors = new List<string> { "The provided Movide ID was not exists" },
						Message = "The provided Movie ID was not exists",
						StatusCode = (int)HttpStatusCode.NotFound,
						Status = false,

					}

				);
			}

			return SuccessObjectResult<MovieResponseDto>(movie, (int)HttpStatusCode.OK);
		}


		[HttpGet("GetMoviesByGenreId")]
		public async Task<IActionResult> GetMoviesByGenreId(byte id)
		{
			//var isValidGenre = await _context.Genres.AnyAsync(G => G.ID == id);
			var isValidGenre = await _unitOfWork.Genres.AnyAsync(G => G.ID == id);
			if (!isValidGenre)
			{
				return new NotFoundObjectResult(
						new CustomResponse<object>()
						{
							StatusCode = (int)HttpStatusCode.NotFound,
							Message = "Failure",
							Status = false,
							Errors = new List<string> { "The Provided Genre ID was not exists" }
						}
					)
				{ StatusCode = (int)HttpStatusCode.NotFound };
			}

			var movies = _unitOfWork.Movies.GetAllByExpressionWithInclude<MovieResponseDto>(
			M => new MovieResponseDto {
				ID = M.ID,
				Genre = new GenreResponseDto
				{
					Id = M.Genre.ID,
					Name = M.Genre.Name
				},
				Rate = M.Rate,
				StoryLine = M.StoryLine,
				Title = M.Title,
				Year = M.Year
			},
			M => M.Genre.Id == id,
			new string[] { "Genre" });
			return SuccessObjectResult<List<MovieResponseDto>>(movies, (int)HttpStatusCode.OK);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromForm] MovieRequestDto dto)
		{
			var errors = new List<string>();
			await CreateMovieErrorsHandler(dto, errors);

			if (errors.Count > 0)
			{
				return HandleBadRequestError(errors);
			}

			using var dataStream = new MemoryStream();
			await dto.Poster!.CopyToAsync(dataStream);
			var movie = new Movie
			{
				GenreID = dto.GenreID!.Value,
				Poster = dataStream.ToArray(),
				Rate = dto.Rate!.Value,
				StoryLine = dto.StoryLine!,
				Title = dto.Title,
				Year = dto.Year!.Value,
			};
			await _unitOfWork.Movies.AddAsync(movie);
			_unitOfWork.Complete();

			return SuccessObjectResult<Movie>(movie, (int)HttpStatusCode.Created);
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromForm] MovieRequestDto dto)
		{
			//var movie = await _context.Movies.Include(M => M.Genre).FirstOrDefaultAsync(M => M.ID == id);
			var movie = await _unitOfWork.Movies.GetByIdAsync(id);
			if (movie is null)
			{
				return HandleBadRequestError(new List<string> { $"There is no Movie with ID : {id}" });
			}
			var errors = new List<string>();
			await CreateMovieErrorsHandler(dto, errors);

			if (errors.Count > 0)
			{
				return HandleBadRequestError(errors);
			}

			if (dto.Poster != null)
			{
				using var dataStream = new MemoryStream();
				await dto.Poster!.CopyToAsync(dataStream);

				movie.Poster = dataStream.ToArray();
			}

			movie.StoryLine = dto.StoryLine!;
			movie.Year = dto.Year!.Value;
			movie.GenreID = dto.GenreID!.Value;
			movie.Rate = dto.Rate!.Value;
			movie.Title = dto.Title!;

			_unitOfWork.Movies.Update(movie);

			return SuccessObjectResult(new MovieResponseDto()
			{
				ID = id,
				Title = movie.Title,
				Genre = new GenreResponseDto
				{
					Id = movie.GenreID,
					Name = movie.Genre.Name
				},
				Rate = movie.Rate,
				StoryLine = movie.StoryLine,
				Year = movie.Year,
			}, 200);

		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var movie = await _unitOfWork.Movies.GetByIdAsync(id);
			if (movie is null)
				return new NotFoundObjectResult(new CustomResponse<object>()
				{
					Errors = new List<string> { $"There is no Movies with the id : {id}" },
					Message = "Failure",
					Status = false,
					StatusCode = (int)HttpStatusCode.NotFound
				});



			//_context.Movies.Remove(movie);
			_unitOfWork.Movies.Delete(movie);
			_unitOfWork.Complete();
			return SuccessObjectResult<Movie>(movie, 200);
		}










		private static ObjectResult SuccessObjectResult<T>(T data, int statusCode)
		{
			return new ObjectResult(new CustomResponse<T>()
			{
				StatusCode = statusCode,
				Data = data,
				Message = "Success",
			})
			{ StatusCode = statusCode };
		}

		private async Task CreateMovieErrorsHandler(MovieRequestDto dto, List<string> errors)
		{
			if (dto is null)
			{
				errors.Add("Please Provide you Movie info");
				return;
			}
			//var isValidGenre = await _context.Genres.AnyAsync(G => G.ID == dto.GenreID);
			var isValidGenre = await _unitOfWork.Genres.AnyAsync(G => G.ID == dto.GenreID);
			if (dto.Poster != null)
			{
				if (dto.Poster.Length > _maxPosterLengthInByte)
				{
					errors.Add("The Poster size is too big , it shouldn't exceed 1MB");
				}
				else if (!_allowedImageExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
				{
					errors.Add("You Poster image should be .png / .jpg Only");
				}
			}

			if (dto.Title.IsNullOrEmpty())
			{
				errors.Add(HandleErrorMessage("Title"));
			}

			if (dto.Rate is null)
			{
				errors.Add(HandleErrorMessage("Rate"));
			}
			else if (dto.Rate.Value < 0 || dto.Rate.Value > 10)
			{
				errors.Add("Invalid Rate , the Rate should be between 0 - 10");
			}

			if (dto.GenreID is null)
			{
				errors.Add("You should provide GenreId for the Movie");
			}
			else if (dto.GenreID <= 0)
			{
				errors.Add("The Genre ID should be more than 0 ");
			}
			else if (!isValidGenre)
			{
				errors.Add($"The Provided Genre ID : {dto.GenreID} doesn't exists !!");
			}

		}

		private string HandleErrorMessage(string name)
		{
			return $"You should send {name}";
		}

		private static ObjectResult HandleBadRequestError(List<string> errors)
		{
			return new BadRequestObjectResult(new CustomResponse<object>
			{
				Message = "Failure",
				Status = false,
				StatusCode = (int)HttpStatusCode.BadRequest,
				Errors = errors
			});
		}
	}


}
