﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Core.Interfaces;

namespace MoviesApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CharactersController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public CharactersController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[HttpGet]
		public IActionResult GetAllAsync()
		{
			var characters = _unitOfWork.Characters.GetAllAsync(Ch => new CharacterResponseDto
			{
				Id = Ch.ID,
				FirstName = Ch.CharacterName.FirstName,
				LastName = Ch.CharacterName.LastName,
				BirthDate = Ch.BirthDate
			});
			return Ok(CustomResponse<List<CharacterResponseDto>>.CreateSuccessCustomResponse(200, characters));
		}


		[HttpPost]
		public async Task<IActionResult> AddCharacterAsync([FromForm] CharacterRequestDto dto)
		{
			var ch = new Character
			{
				CharacterName = new Name { FirstName = dto.FirstName, LastName = dto.LastName! },
				BirthDate = dto.BirthDate
			};
			var addedRows = await _unitOfWork.Characters.AddAsync(
					ch
				);
			_unitOfWork.Complete();

			if (addedRows > 0)
			{
				return Ok(
						CustomResponse<CharacterResponseDto>.CreateSuccessCustomResponse(200,
						new CharacterResponseDto
						{
							Id = ch.ID,
							FirstName = ch.CharacterName.FirstName,
							LastName = ch.CharacterName.LastName,
							BirthDate = ch.BirthDate
						})
					);
			}
			return BadRequest();
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCharacterAsync(int id, [FromForm] CharacterRequestDto dto)
		{
			var ch = await _unitOfWork.Characters.GetByIdAsync(id);
			if (ch is null)
			{
				return NotFound(CustomResponse<object>.CreateFailureCustomResponse(404, new List<string> { "Not Found Character with the provided ID" }));
			}

			ch.CharacterName = new Name { FirstName = dto.FirstName, LastName = dto.LastName! };
			ch.BirthDate = dto.BirthDate;

			var updatedCh = _unitOfWork.Characters.Update(ch);
			if (updatedCh is null)
			{
				return BadRequest();
			}

			return Ok(
						CustomResponse<CharacterResponseDto>.CreateSuccessCustomResponse(200,
						new CharacterResponseDto
						{
							Id = ch.ID,
							FirstName = ch.CharacterName.FirstName,
							LastName = ch.CharacterName.LastName,
							BirthDate = ch.BirthDate
						})
				);
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			var ch = await _unitOfWork.Characters.GetByIdAsync(id);
			if (ch is null)
				return NotFound(CustomResponse<object>
					.CreateFailureCustomResponse(
						404,
						new List<string> { "Not found Character with the provided id" }));

			_unitOfWork.Characters.Delete(ch);
			return Ok(
				CustomResponse<CharacterResponseDto>
				.CreateSuccessCustomResponse(200,
							new CharacterResponseDto
							{
								Id = ch.ID,
								FirstName = ch.CharacterName.FirstName,
								LastName = ch.CharacterName.LastName,
								BirthDate = ch.BirthDate
							}));
		}

		[HttpPost("AddCharacterToMovieWithSalary")]
		public async Task<IActionResult> AddCharacterWithMovie(int movieId, int characterId, double salary)
		{
			var result = await _unitOfWork.Characters.AddCharacterToMovieWithSalary(characterId, movieId, salary);

			if (result is null)
			{
				return BadRequest(CustomResponse<object>.CreateFailureCustomResponse(400, new List<string> { "Make sure of the MovieId and CharacterId are exist" }));
			}

			var responseDto = new CharacterWithMovieResponseDto
			{
				CharacterId = result.CharacterID,
				MovieId = result.MovieID,
				Salary = result.Salary

			};
			return Ok(CustomResponse<CharacterWithMovieResponseDto>
						.CreateSuccessCustomResponse(200, responseDto));
		}


		[HttpGet("CharacterWithAllMovies/{id}")]
		public async Task<IActionResult> GetCharacterWithAllMoviesAsync(int id)
		{
			var ch = await _unitOfWork.Characters.GetCharacterWithAllMoviesAsync(id);

			if (ch is null)
				return NotFound(CustomResponse<object>.CreateFailureCustomResponse(404, new List<string> { "There is no Character with provided Id" }));

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

			var moviesDto = ch.CharacterActInMovies.Select(cm => new MovieResponseDto
			{
				ID = cm.Movie.ID,
				Rate = cm.Movie.Rate,
				StoryLine = cm.Movie.StoryLine,
				Title = cm.Movie.Title,
				Year = cm.Movie.Year,
				Genre = new GenreResponseDto
				{
					Id = cm.Movie.Genre.ID,
					Name = cm.Movie.Genre.Name,
				}
			}).ToList();
			var dto = new CharacterWithAllMoviesResponseDto
			{
				Id = ch.ID,
				FirstName = ch.CharacterName.FirstName,
				LastName = ch.CharacterName.LastName,
				BirthDate = ch.BirthDate,
				Movies = moviesDto
			};
			return Ok(CustomResponse<CharacterWithAllMoviesResponseDto>.CreateSuccessCustomResponse(
					200,
					dto
				));
		}

		[HttpGet("MovieWithAllCharacters/{id}")]
		public async Task<IActionResult> GetMovieWithAllCharactersAsync(int id)
		{
			var movie = await _unitOfWork.Characters.GetMovieWithAllCharactersAsync(id);

			if (movie is null)
				return NotFound(CustomResponse<object>.CreateFailureCustomResponse(404,
					new List<string> { "There is no Genre with provided Id" }));

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

			return Ok(movieResultDto);
		}


		[HttpPut("UpdateSalaryForCharacter")]
		public async Task<IActionResult> UpdateSalaryOfCharacter(int characterId, int movieId, double salary)
		{
			try
			{

				var character = await _unitOfWork.Characters.UpdateSalaryForCharacterInMovieAsync(characterId, movieId, salary);

				if (character is null)
					return NotFound();

				return Ok(CustomResponse<CharacterWithMovieResponseDto>.CreateSuccessCustomResponse(200,

					new CharacterWithMovieResponseDto
					{
						MovieId = character.MovieID,
						CharacterId = character.CharacterID,
						Salary = character.Salary
					}
					));
			}
			catch (Exception ex)
			{
				return BadRequest(CustomResponse<object>.CreateFailureCustomResponse(400, new List<string> { ex.Message }));
			}


		}
	}
}
