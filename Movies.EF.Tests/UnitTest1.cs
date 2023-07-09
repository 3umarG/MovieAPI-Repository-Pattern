using FakeItEasy;
using FirstWebApi.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Movies.Core.Interfaces;
using Movies.Core.Models;
using System.Linq.Expressions;

namespace Movies.EF.Tests
{
	public class CharactersRepositoryTests
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ApplicationDbContext _context;

		public CharactersRepositoryTests()
		{
			_context = A.Fake<ApplicationDbContext>();
			_unitOfWork = A.Fake<IUnitOfWork>();
		}

		[Fact]
		public void UpdateSalaryForCharacterInMovieAsync_AcceptNotExistsCharacterId_ThrowException()
		{
			// Arrange
			int characterId = 1;
			int movieId = 2;
			double salary = 555555;
			A.CallTo(() => _unitOfWork.Characters.AnyAsync(A<Expression<Func<Character, bool>>>.Ignored)).Returns(false);


			// Act
			Func<Task> f = async () => await _unitOfWork
										.Characters
										.UpdateSalaryForCharacterInMovieAsync(characterId, movieId, salary);


			// Assert
			f.Should().ThrowAsync<Exception>().WithMessage("There is no Character with provided ID");
		}

		[Fact]
		public void UpdateSalaryForCharacterInMovieAsync_AcceptNotExistsMovieId_ThrowException()
		{
			// Arrange
			int characterId = 1;
			int movieId = 2;
			double salary = 555555;
			A.CallTo(() => _unitOfWork.Characters.AnyAsync(A<Expression<Func<Character, bool>>>.Ignored)).Returns(true);
			A.CallTo(() => _unitOfWork.Movies.AnyAsync(A<Expression<Func<Movie, bool>>>.Ignored)).Returns(false);


			// Act
			Func<Task> f = async () => await _unitOfWork
										.Characters
										.UpdateSalaryForCharacterInMovieAsync(characterId, movieId, salary);


			// Assert
			f.Should().ThrowAsync<Exception>().WithMessage("There is no Movie with provided ID");
		}


		[Fact]
		public void UpdateSalaryForCharacterInMovieAsync_AcceptWrongCharacterWithMovie_ThrowException()
		{
			// Arrange
			int characterId = 1;
			int movieId = 2;
			double salary = 555555;
			A.CallTo(() => _unitOfWork.Characters.AnyAsync(A<Expression<Func<Character, bool>>>.Ignored)).Returns(true);
			A.CallTo(() => _unitOfWork.Movies.AnyAsync(A<Expression<Func<Movie, bool>>>.Ignored)).Returns(true);
			A.CallTo(() => _unitOfWork.CharactersInMovies.AnyAsync(A<Expression<Func<CharacterInMovie, bool>>>.Ignored)).Returns(false);


			// Act
			Func<Task> f = async () => await _unitOfWork
										.Characters
										.UpdateSalaryForCharacterInMovieAsync(characterId, movieId, salary);


			// Assert
			f.Should().ThrowAsync<Exception>().WithMessage("The provided Character doesn't work on the provided Movie");
		}


		[Fact]
		public  void UpdateSalaryForCharacterInMovieAsync_AcceptNegativeSalary_ThrowException()
		{
			// Arrange
			int characterId = 1;
			int movieId = 2;
			double salary = -1;
			A.CallTo(() => _unitOfWork.Characters.AnyAsync(A<Expression<Func<Character, bool>>>.Ignored)).Returns(true);
			A.CallTo(() => _unitOfWork.Movies.AnyAsync(A<Expression<Func<Movie, bool>>>.Ignored)).Returns(true);
			A.CallTo(() => _unitOfWork.CharactersInMovies.AnyAsync(A<Expression<Func<CharacterInMovie, bool>>>.Ignored)).Returns(true);

			// Act
			Func<Task> f = async () => await _unitOfWork
											.Characters
											.UpdateSalaryForCharacterInMovieAsync(characterId, movieId, salary);


			// Assert
			f.Should().ThrowAsync<Exception>().WithMessage("The Salary must be Positive value");
		}

		#region The Best Case for the method
		/*
		[Fact]
		public async void UpdateSalaryForCharacterInMovieAsync_AcceptRightParameters_ReturnUpdatedObject()
		{
			// Arrange
			int characterId = 1;
			int movieId = 1;
			double salary = 88;
			A.CallTo(() => _unitOfWork.Characters.AnyAsync(A<Expression<Func<Character, bool>>>.Ignored)).Returns(true);
			A.CallTo(() => _unitOfWork.Movies.AnyAsync(A<Expression<Func<Movie, bool>>>.Ignored)).Returns(true);
			A.CallTo(() => _unitOfWork.CharactersInMovies.AnyAsync(A<Expression<Func<CharacterInMovie, bool>>>.Ignored)).Returns(true);


			/// Cannot fake extension methods
			//var characterInMovie = A.Fake<CharacterInMovie>();
			//A.CallTo(() => _context.CharactersInMovies
			//			.FirstOrDefaultAsync(A<Expression<Func<CharacterInMovie, bool>>>.Ignored, A<CancellationToken>.Ignored))
			//			.Returns(characterInMovie);

			// Act
			var actualResult = await _unitOfWork
										.Characters
										.UpdateSalaryForCharacterInMovieAsync(characterId, movieId, salary);
			

			// Assert
			actualResult.Should().NotBeNull();
			actualResult.Salary.Should().Be(salary);
		}
		*/
		#endregion

	}
}