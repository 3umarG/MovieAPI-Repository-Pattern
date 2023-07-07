using Microsoft.AspNetCore.Http;
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
		public async Task<IActionResult> GetAllAsync()
		{
			var characters = await _unitOfWork.Characters.GetAllAsync();
			return Ok(characters);
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
				return Ok(new CharacterResponseDto
				{
					Id = ch.ID,
					FirstName = ch.CharacterName.FirstName,
					LastName = ch.CharacterName.LastName,
					BirthDate = ch.BirthDate
				});
			}
			return BadRequest();
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCharacterAsync(int id, [FromForm] CharacterRequestDto dto)
		{
			var ch = await _unitOfWork.Characters.GetByIdAsync(id);
			if (ch is null)
			{
				return NotFound();
			}

			ch.CharacterName = new Name { FirstName = dto.FirstName, LastName = dto.LastName! };
			ch.BirthDate = dto.BirthDate;

			var updatedCh = _unitOfWork.Characters.Update(ch);
			if (updatedCh is null)
			{
				return BadRequest();
			}

			return Ok(new CharacterResponseDto
			{
				Id = ch.ID,
				FirstName = ch.CharacterName.FirstName,
				LastName = ch.CharacterName.LastName,
				BirthDate = ch.BirthDate
			});
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			var ch = await _unitOfWork.Characters.GetByIdAsync(id);
			if (ch is null)
				return NotFound();

			_unitOfWork.Characters.Delete(ch);
			return Ok(new CharacterResponseDto
			{
				Id = ch.ID,
				FirstName  = ch.CharacterName.FirstName,
				LastName = ch.CharacterName.LastName,
				BirthDate = ch.BirthDate
			});
		}
	}
}
