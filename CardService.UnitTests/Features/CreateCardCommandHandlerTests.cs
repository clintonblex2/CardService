using AutoMapper;
using CardService.Application.Common.Helpers;
using CardService.Application.Common.Interfaces;
using CardService.Application.Common.Mapper;
using CardService.Application.UseCases.Card.Commands;
using CardService.Application.UseCases.Card.Handlers;
using CardService.Application.UseCases.Card.Validators;
using CardService.Domain.Entities;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace CardService.UnitTests.Features
{
    [TestFixture]
    public class CreateCardCommandHandlerTests
    {
        private IMapper _mapper;
        private Mock<ILogger<CreateCardCommandHandler>> _loggerMock;
        private Mock<IUnitOfWork> _uowMock;
        private CreateCardCommandHandler _handler;
        private CancellationToken cancellationToken = new CancellationToken();
        private CreateCardCommandValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new CreateCardCommandValidator(_uowMock.Object);
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _loggerMock = new Mock<ILogger<CreateCardCommandHandler>>();
            _uowMock = new Mock<IUnitOfWork>();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            _mapper = mockMapper.CreateMapper();
            _handler = new CreateCardCommandHandler(_loggerMock.Object, _uowMock.Object, _mapper);

            var repositoryMock = new Mock<IRepository<CardEntity>>();
            repositoryMock.Setup(r => r.Exist(It.IsAny<Expression<Func<CardEntity, bool>>>())).Returns(true);
            _uowMock.Setup(u => u.Repository<CardEntity>()).Returns(repositoryMock.Object);
        }

        [Test]
        public void CreateCardCommandHandler_ShouldReturn_RequiredCardName()
        {
            // Arrange
            var command = new CreateCardCommand { Name = "" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == nameof(CreateCardCommand.Name) && e.ErrorMessage == BaseStrings.CARD_NAME_REQUIRED));
        }

        [Test]
        public void CreateCardCommandHandler_ShouldPass_ColorFormatValidation()
        {
            // Arrange
            var command = new CreateCardCommand { Name = "Test Card", Color = "#FFFFFF" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Color);
        }

        [Test]
        public void CreateCardCommandHandler_ShouldReturn_InvalidColorFormat()
        {
            // Arrange
            var command = new CreateCardCommand { Name = "Test Card", Color = "InvalidColor" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Color)
                .WithErrorMessage(BaseStrings.INVALID_COLOR_FORMAT);
        }

        [Test]
        public void CreateCardCommandHandler_ShouldReturn_CardAlreadyExist()
        {
            // Arrange
            var command = new CreateCardCommand { UserId = 1, Name = "Existing Card" };

            var repositoryMock = new Mock<IRepository<CardEntity>>();
            repositoryMock.Setup(r => r.Exist(It.IsAny<Expression<Func<CardEntity, bool>>>())).Returns(true);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage(BaseStrings.CARD_ALREADY_EXIST);
        }

        [Test]
        public async Task CreateCardCommandHandler_ShouldReturn_SuccessResponse()
        {
            // Arrange
            var request = new CreateCardCommand { UserId = 1, Name = "Test" };

            _uowMock.Setup(uow => uow.Repository<CardEntity>().Insert(It.IsAny<CardEntity>())).Verifiable();
            _uowMock.Setup(uow => uow.Complete(cancellationToken)).Returns(Task.FromResult(true)).Verifiable();

            // Act
            var response = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessful);
            Assert.That(response.Message, Is.EqualTo(BaseStrings.SUCCESSFUL_CARD_CREATION));
            _uowMock.Verify(uow => uow.Repository<CardEntity>().Insert(It.IsAny<CardEntity>()), Times.Once);
            _uowMock.Verify(uow => uow.Complete(cancellationToken), Times.Once);
        }
    }
}
