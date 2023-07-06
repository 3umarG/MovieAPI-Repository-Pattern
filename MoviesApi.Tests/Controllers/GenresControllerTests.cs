using Castle.Core.Internal;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Movies.Core.DTOs;
using Movies.Core.Interfaces;
using Movies.Core.Models;
using MoviesApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using BadRequestResult = Microsoft.AspNetCore.Mvc.BadRequestResult;

namespace MoviesApi.Tests.Controllers
{
	public class GenresControllerTests
	{
		private readonly GenresController _controller;
		private readonly IUnitOfWork _unitOfWork;

		public GenresControllerTests()
		{
			_unitOfWork = A.Fake<IUnitOfWork>();
			_controller = new GenresController(_unitOfWork);
		}
		/*
		 * Naming your tests
				The name of your test should consist of three parts:

				The name of the method being tested.
				The scenario under which it's being tested.
				The expected behavior when the scenario is invoked.
		 * 
		 */
		[Fact]
		public async Task GetAllAsync_AcceptGenresList_ReturnOkObject()
		{
			/// A A A


			// Arrange : all fakes and moqs and setup all methods with return types.
			var genres = A.Fake<List<Genre>>();
			A.CallTo(() => _unitOfWork.Genres.GetAllAsync()).Returns(genres);

			// Act : extract the actual result and expected .
			var actionResult = await _controller.GetAllAsync();
			var okObjectResult = actionResult as OkObjectResult;
			var okObjectResultValue = okObjectResult!.Value as CustomResponse<List<Genre>>;

			var expectedResult = CustomResponse<List<Genre>>.CreateSuccessCustomResponse(200, genres);


			// Assert : compare the accepted result with the expected.
			Assert.NotNull(actionResult);
			Assert.NotNull(okObjectResult);
			Assert.NotNull(okObjectResultValue);
			okObjectResultValue.Should().BeEquivalentTo(expectedResult);

		}

		[Fact]
		public async Task GetAllAsync_AcceptEmpteGenresList_ReturnOkObject()
		{
			/// A A A


			// Arrange : all fakes and moqs and setup all methods with return types.
			A.CallTo(() => _unitOfWork.Genres.GetAllAsync()).Returns(new List<Genre> { });

			// Act : extract the actual result and expected .
			IActionResult actionResult = await _controller.GetAllAsync();
			OkObjectResult? okObjectResult = actionResult as OkObjectResult;
			CustomResponse<List<Genre>>? okObjectResultValue = okObjectResult!.Value as CustomResponse<List<Genre>>;

			CustomResponse<List<Genre>> expectedResult =
				CustomResponse<List<Genre>>.CreateSuccessCustomResponse(200, new List<Genre> { });


			// Assert : compare the accepted result with the expected.
			Assert.NotNull(actionResult);
			Assert.NotNull(okObjectResult);
			Assert.NotNull(okObjectResultValue);
			actionResult!.GetType()!.GetProperty("StatusCode")!.GetValue(actionResult).Should().Be(200);
			okObjectResultValue.Should().BeEquivalentTo(expectedResult);

		}

		[Fact]
		public async Task GetAllAsync_ThrowException_ReturnNotBadRequest()
		{
			// Arrange
			A.CallTo(() => _unitOfWork.Genres.GetAllAsync()).Throws(new Exception());

			// Act
			var actionResult = await _controller.GetAllAsync();
			var badRequestObjectResult = actionResult as BadRequestResult;

			// Assert
			Assert.NotNull(actionResult);
			Assert.NotNull(badRequestObjectResult);
		}


		[Fact]
		public async Task CreateGenreAsync_AcceptNullNameForGenre_ReturnBadRequest()
		{
			// Arrange
			var genreDto = new GenreRequestDto
			{
				Name = string.Empty
			};



			// Act
			var actionResult = await _controller.CreateGenreAsync(genreDto);
			var badRequestObjectResult = actionResult as BadRequestObjectResult;
			var badRequestValue = badRequestObjectResult!.Value as CustomResponse<object>;

			var actualBadRequestResult =
				CustomResponse<object>.CreateFailureCustomResponse(
						(int)HttpStatusCode.BadRequest,
						new List<string> { "You should provide Name for Genre " }
					);


			// Assert
			actionResult.Should().NotBeNull();
			badRequestObjectResult.Should().NotBeNull();
			badRequestValue.Should().NotBeNull();
			badRequestValue.Should().BeEquivalentTo(actualBadRequestResult);
		}

		[Fact]
		public async Task CreatGenreAsync_AddVlidGenre_ReturnCreatedObjectResult()
		{
			// Arrange
			var genreDto = new GenreRequestDto
			{
				Name = "Foo"
			};

			// Act
			var actionResult = await _controller.CreateGenreAsync(genreDto);
			var createObjectResult = actionResult as ObjectResult;
			var objectResultContent = createObjectResult!.Value as CustomResponse<Genre>;

			var expectedObjectResultContent = CustomResponse<Genre>.CreateSuccessCustomResponse(

				(int)HttpStatusCode.Created,
				new Genre
				{
					Name = genreDto.Name!
				}
			);


			// Assert
			actionResult.Should().NotBeNull();
			createObjectResult.Should().NotBeNull();
			createObjectResult.GetType()!.GetProperty("StatusCode")!.GetValue(actionResult).Should().Be((int)HttpStatusCode.Created);

			objectResultContent.Should().NotBeNull();
			objectResultContent.Should().BeEquivalentTo(expectedObjectResultContent);
		}

		[Fact]
		public async Task UpdateGenreAsync_ProvideGenreNameEmptyOrNull_ReturnBadResponse()
		{
			// Arrange
			var genreDto = new GenreRequestDto
			{
				Name = string.Empty
			};


			// Act
			var actionResult = await _controller.UpdateAsync(1, genreDto);
			var badRequestObjectResult = actionResult as BadRequestObjectResult;
			var badRequestValue = badRequestObjectResult!.Value as CustomResponse<object>;

			var actualBadRequestResult =
				CustomResponse<object>.CreateFailureCustomResponse(
						(int)HttpStatusCode.BadRequest,
						new List<string> { "You should provide Genre name for update" }
					);


			// Assert
			actionResult.Should().NotBeNull();
			badRequestObjectResult.Should().NotBeNull();
			badRequestValue.Should().NotBeNull();
			badRequestValue.Should().BeEquivalentTo(actualBadRequestResult);
		}

		[Fact]
		public async Task UpdateGenreAsync_ReturnNullGenre_ReturnNotFoundResponse()
		{
			// Arrange
			Genre? emptyGenre = null;
			A.CallTo(
				() => _unitOfWork.Genres.GetByExpressionAsync(A<Expression<Func<Genre, bool>>>.Ignored))
				.Returns(emptyGenre);
			var genreDto = new GenreRequestDto { Name = "Foo" };

			// Act
			var actionResult = await _controller.UpdateAsync(1, genreDto);
			var objectResult = actionResult as NotFoundObjectResult;
			var objectResultContent = objectResult!.Value as CustomResponse<object>;

			var actualObjectResultContent = CustomResponse<object>
						.CreateFailureCustomResponse(404, new List<string> { "The Genre ID was not found" });


			// Assert
			Assert.NotNull(actionResult);
			Assert.NotNull(objectResult);
			Assert.NotNull(objectResultContent);
			objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
			objectResultContent.Should().BeEquivalentTo(actualObjectResultContent);


		}

		[Fact]
		public async Task UpdateGenreAsync_ReturnGenre_ReturnOkObjectResultResponse()
		{
			// Arrange
			var genre = A.Fake<Genre>();
			A.
			  CallTo(() => _unitOfWork.Genres.GetByExpressionAsync(A<Expression<Func<Genre, bool>>>.Ignored))
			  .Returns(genre);

			// by usin FakeItEasy
			var dto = A.Fake<GenreRequestDto>();
			dto.Name = "Foo";

			// Act
			var actionResult = await _controller.UpdateAsync(1, dto);
			var objectResult = actionResult as ObjectResult;
			var contentObjectResult = objectResult!.Value as CustomResponse<Genre>;
			var actualContent = CustomResponse<Genre>.CreateSuccessCustomResponse(200, genre);

			// Assert
			actionResult.Should().NotBeNull();
			objectResult.Should().NotBeNull();
			contentObjectResult.Should().NotBeNull();
			objectResult.StatusCode.Should().Be(200);
			contentObjectResult.Should().BeEquivalentTo(actualContent);
		}
	}
}
