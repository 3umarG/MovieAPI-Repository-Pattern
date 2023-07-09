using AutoMapper;
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
		private readonly IMapper _mapper;

		public GenresControllerTests()
		{
			_mapper = A.Fake<IMapper>();
			_unitOfWork = A.Fake<IUnitOfWork>();
			_controller = new GenresController(_unitOfWork , _mapper);
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
			var okObjectResultValue = okObjectResult!.Value as SuccessResponse<List<GenreResponseDto>>;

			var successFactory = new SuccessResponseFactory<List<GenreResponseDto>>(200, _mapper.Map<List<GenreResponseDto>>(genres));
			var expectedResult = successFactory.Create();


			// Assert : compare the accepted result with the expected.
			Assert.NotNull(actionResult);
			Assert.NotNull(okObjectResult);
			Assert.NotNull(okObjectResultValue);
			okObjectResultValue.Should().BeEquivalentTo(expectedResult);

		}

		[Fact]
		public async Task GetAllAsync_AcceptEmptyGenresList_ReturnOkObject()
		{
			/// A A A


			// Arrange : all fakes and moqs and setup all methods with return types.
			A.CallTo(() => _unitOfWork.Genres.GetAllAsync()).Returns(new List<Genre> { });

			// Act : extract the actual result and expected .
			var actionResult = await _controller.GetAllAsync();
			var okObjectResult = actionResult as OkObjectResult;
			var okObjectResultValue = okObjectResult!.Value as SuccessResponse<List<GenreResponseDto>>;

			var successFactory = new SuccessResponseFactory<List<GenreResponseDto>>(200 , _mapper.Map<List<GenreResponseDto>>(new() { }));
			var expectedResult = successFactory.Create();

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
			var badRequestValue = badRequestObjectResult!.Value as IResponse;

			var failureFactory = new FailureResponseFactory(400, "You should provide Name for Genre");
			var actualBadRequestResult = failureFactory.Create();


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
			var objectResultContent = createObjectResult!.Value as SuccessResponse<Genre>;

			var successFactory = new SuccessResponseFactory<Genre>(201, _mapper.Map<Genre>(genreDto));
			var expectedObjectResultContent = successFactory.Create();

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
			var badRequestValue = badRequestObjectResult!.Value as IResponse;

			var failureFactory = new FailureResponseFactory(400, "You should provide Genre name for update");
			var actualBadRequestResult = failureFactory.Create();


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
			var objectResultContent = objectResult!.Value as IResponse;

			var failureFact = new FailureResponseFactory(404, "The Genre ID was not found");
			var actualObjectResultContent = failureFact.Create();



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
			var contentObjectResult = objectResult!.Value as SuccessResponse<GenreResponseDto>;

			var successFactory = new SuccessResponseFactory<GenreResponseDto>(200, _mapper.Map<GenreResponseDto>(genre)); 
			var actualContent = successFactory.Create();

			// Assert
			actionResult.Should().NotBeNull();
			objectResult.Should().NotBeNull();
			contentObjectResult.Should().NotBeNull();
			objectResult.StatusCode.Should().Be(200);
			contentObjectResult.Should().BeEquivalentTo(actualContent);
		}

		[Fact]
		public async Task DeleteGenreAsync_AcceptNotFoundGenreId_ReturnNotFoundObjectResultResponse()
		{
			// Arrange
			Genre? genre = null;
			A.CallTo(() => _unitOfWork
									.Genres
									.GetByExpressionAsync(A<Expression<Func<Genre, bool>>>.Ignored))
									.Returns(genre);

			// Act
			var actionResult = await _controller.DeleteAsync(1);
			var objectResult = actionResult as NotFoundObjectResult;
			var contentResult = objectResult!.Value as IResponse;

			var failureFactory = new FailureResponseFactory(404, "There is No Genre with the provided Id");
			var expectedContentResult = failureFactory.Create();
			var expectedStatusCode = (int)HttpStatusCode.NotFound;


			// Assert
			actionResult.Should().NotBeNull();
			objectResult.Should().NotBeNull();
			contentResult.Should().NotBeNull();
			contentResult.Should().BeEquivalentTo(expectedContentResult);
			objectResult.StatusCode.Should().Be(expectedStatusCode);
		}


		[Fact]
		public async Task DeleteGenreAsync_DeleteExistGenre_ReturnOkObjectResultResponse()
		{
			// Arrange
			var genre = A.Fake<Genre>();
			genre.Name = "Foo";
			A.CallTo(() => _unitOfWork
									.Genres
									.GetByExpressionAsync(A<Expression<Func<Genre, bool>>>.Ignored))
									.Returns(genre);

			// Act
			var actionResult = await _controller.DeleteAsync(1);
			var objectResult = actionResult as OkObjectResult;
			var contentResult = objectResult!.Value as SuccessResponse<GenreResponseDto>;

			var successFactory = new SuccessResponseFactory<GenreResponseDto>(200, _mapper.Map<GenreResponseDto>(genre));
			var expectedContentResult = successFactory.Create();
			var expectedStatusCode = (int)HttpStatusCode.OK;


			// Assert
			actionResult.Should().NotBeNull();
			objectResult.Should().NotBeNull();
			contentResult.Should().NotBeNull();
			contentResult.Should().BeEquivalentTo(expectedContentResult);
			objectResult.StatusCode.Should().Be(expectedStatusCode);

		}
	}
}
