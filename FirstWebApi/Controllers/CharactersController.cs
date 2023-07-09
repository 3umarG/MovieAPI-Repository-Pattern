using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Core.DTOs;
using Movies.Core.Interfaces;
using System;

namespace MoviesApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CharactersController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private IResponseFactory _successFactory;
		private IResponseFactory _failureFactory;
		private readonly IMapper _mapper;

		public CharactersController(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllAsync()
		{
			var characters = await _unitOfWork.Characters.GetAllAsync();
			_successFactory = new SuccessResponseFactory<List<CharacterResponseDto>>(200, _mapper.Map<List<CharacterResponseDto>>(characters));
			return Ok(_successFactory.Create());
		}


		[HttpPost]
		public async Task<IActionResult> AddCharacterAsync([FromForm] CharacterRequestDto dto)
		{
			var ch = _mapper.Map<Character>(dto);

			var addedRows = await _unitOfWork.Characters.AddAsync(
					ch
				);
			_unitOfWork.Complete();

			if (addedRows > 0)
			{
				_successFactory = new SuccessResponseFactory<CharacterResponseDto>(
						200,
						_mapper.Map<CharacterResponseDto>(ch)
				);
				return Ok(_successFactory.Create());
			}
			return BadRequest();
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCharacterAsync(int id, [FromForm] CharacterRequestDto dto)
		{
			var ch = await _unitOfWork.Characters.GetByIdAsync(id);
			if (ch is null)
			{
				_failureFactory = new FailureResponseFactory(404, "Not Found Character with the provided ID");
				return NotFound(_failureFactory.Create());
			}

			// Before use AutoMapper :
			if (dto.FirstName != null || dto.LastName != null)
			{
				if (dto.FirstName != null)
					ch.CharacterName.FirstName = dto.FirstName;

				if (dto.LastName != null)
					ch.CharacterName.LastName = dto.LastName;
			}

			if (dto.BirthDate is not null)
				ch.BirthDate = dto.BirthDate.Value;

			// with using AutoMapper :
			// BUG : with using this approach , you create new instance and instead of updating it , you will insert new one. 
			//ch = _mapper.Map<Character>(dto);

			var updatedCh = _unitOfWork.Characters.Update(ch);
			if (updatedCh is null)
			{
				return BadRequest();
			}

			_successFactory = new SuccessResponseFactory<CharacterResponseDto>(200, _mapper.Map<CharacterResponseDto>(ch));

			return Ok(_successFactory.Create());
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			var ch = await _unitOfWork.Characters.GetByIdAsync(id);
			if (ch is null)
			{
				_failureFactory = new FailureResponseFactory(404, "Not found Character with the provided id");
				return NotFound(_failureFactory.Create());

			}
			_unitOfWork.Characters.Delete(ch);
			_successFactory = new SuccessResponseFactory<CharacterResponseDto>(200, _mapper.Map<CharacterResponseDto>(ch));
			return Ok(_successFactory.Create());
		}

		[HttpPost("AddCharacterToMovieWithSalary")]
		public async Task<IActionResult> AddCharacterWithMovie(int movieId, int characterId, double salary)
		{
			var result = await _unitOfWork.Characters.AddCharacterToMovieWithSalary(characterId, movieId, salary);

			if (result is null)
			{
				_failureFactory = new FailureResponseFactory(400, "Make sure of the MovieId and CharacterId are exist");
				return BadRequest(_failureFactory.Create());
			}

			var responseDto = _mapper.Map<CharacterWithMovieResponseDto>(result);

			_successFactory = new SuccessResponseFactory<CharacterWithMovieResponseDto>(200, responseDto);
			return Ok(_successFactory.Create());
		}


		[HttpGet("CharacterWithAllMovies/{id}")]
		public async Task<IActionResult> GetCharacterWithAllMoviesAsync(int id)
		{
			// with all includes
			Character? ch = await _unitOfWork.Characters.GetCharacterWithAllMoviesAsync(id);

			if (ch is null)
			{
				_failureFactory = new FailureResponseFactory(404, "There is no Character with provided Id");
				return NotFound(_failureFactory.Create());
			}

			#region Return only movies without character data
			//var moviesDto = 
			//	movies
			//	.Select(
			//		m => new MovieResponseDto
			//		{
			//			ID = m.ID,
			//			Genre = new GenreResponseDto
			//			{
			//				Id = m.Genre.ID,
			//				Name = m.Genre.Name,
			//			},
			//			Rate = m.Rate,
			//			StoryLine = m.StoryLine,
			//			Title = m.Title,
			//			Year = m.Year
			//		}
			//	);
			#endregion

			#region Before Using AutoMapper
			//var movies = ch.CharacterActInMovies.Select(cm =>cm.Movie).ToList();
			//var moviesDto = _mapper.Map<List<MovieResponseDto>>(movies);
			//var dto = new CharacterWithAllMoviesResponseDto
			//{
			//	Id = ch.ID,
			//	FirstName = ch.CharacterName.FirstName,
			//	LastName = ch.CharacterName.LastName,
			//	BirthDate = ch.BirthDate,
			//	Movies = moviesDto
			//};
			#endregion

			_successFactory = new SuccessResponseFactory<CharacterWithAllMoviesResponseDto>(
						200,
						_mapper.Map<CharacterWithAllMoviesResponseDto>(ch)
			);
			return Ok(_successFactory.Create());
		}

		[HttpGet("MovieWithAllCharacters/{id}")]
		public async Task<IActionResult> GetMovieWithAllCharactersAsync(int id)
		{
			var movie = await _unitOfWork.Characters.GetMovieWithAllCharactersAsync(id);

			if (movie is null)
			{
				_failureFactory = new FailureResponseFactory(404, "There is no Movie with provided Id");
				return NotFound(_failureFactory.Create());
			}

			var charactersDto = movie.CharacterActInMovies.Select(
					cm => new CharacterResponseDto
					{
						Id = cm.Character.ID,
						BirthDate = cm.Character.BirthDate,
						FirstName = cm.Character.CharacterName.FirstName,
						LastName = cm.Character.CharacterName.LastName,
					}).ToList();
			var movieResultDto = new MovieWithAllCharacterResponseDto
			{
				ID = movie.ID,
				Rate = movie.Rate,
				Genre = new GenreResponseDto
				{
					Id = movie.Genre.ID,
					Name = movie.Genre.Name
				},
				Title = movie.Title,
				StoryLine = movie.StoryLine,
				Year = movie.Year,
				Characters = charactersDto
			};

			_successFactory = new SuccessResponseFactory<MovieWithAllCharacterResponseDto>(200, movieResultDto);
			return Ok(_successFactory.Create());
		}


		[HttpPut("UpdateSalaryForCharacter")]
		public async Task<IActionResult> UpdateSalaryOfCharacter(int characterId, int movieId, double salary)
		{
			try
			{

				var character = await _unitOfWork.Characters.UpdateSalaryForCharacterInMovieAsync(characterId, movieId, salary);

				if (character is null)
				{
					_failureFactory = new FailureResponseFactory(404, "Failed");
					return NotFound(_failureFactory.Create());
				}

				_successFactory = new SuccessResponseFactory<CharacterWithMovieResponseDto>
					(200, new CharacterWithMovieResponseDto
					{
						MovieId = character.MovieID,
						CharacterId = character.CharacterID,
						Salary = character.Salary
					});
				return Ok(_successFactory.Create());
			}
			catch (Exception ex)
			{
				_failureFactory = new FailureResponseFactory(400, ex.Message);
				return BadRequest(_failureFactory.Create());
			}


		}
	}
}
