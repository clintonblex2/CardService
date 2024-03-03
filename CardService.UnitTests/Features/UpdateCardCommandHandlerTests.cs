using AutoMapper;
using CardService.Application.Common.Helpers;
using CardService.Application.Common.Interfaces;
using CardService.Application.Common.Mapper;
using CardService.Application.UseCases.Card.Commands;
using CardService.Application.UseCases.Card.Handlers;
using CardService.Application.UseCases.Card.Validators;
using CardService.Domain.Entities;
using CardService.Domain.Enums;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace CardService.UnitTests.Features
{
    [TestFixture]
    public class UpdateCardCommandHandlerTests
    {
        private IMapper _mapper;
        private Mock<ILogger<UpdateCardCommandHandler>> _loggerMock;
        private Mock<IUnitOfWork> _uowMock;
        private UpdateCardCommandHandler _handler;
        private CancellationToken cancellationToken = new CancellationToken();
        private UpdateCardCommandValidator _validator;
        private Mock<IRepository<CardEntity>> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _validator = new UpdateCardCommandValidator(_uowMock.Object);
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _loggerMock = new Mock<ILogger<UpdateCardCommandHandler>>();
            _uowMock = new Mock<IUnitOfWork>();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            _mapper = mockMapper.CreateMapper();
            _handler = new UpdateCardCommandHandler(_loggerMock.Object, _uowMock.Object, _mapper);

            _repositoryMock = new Mock<IRepository<CardEntity>>();
            _repositoryMock.Setup(r => r.Exist(It.IsAny<Expression<Func<CardEntity, bool>>>())).Returns(true);
            _uowMock.Setup(u => u.Repository<CardEntity>()).Returns(_repositoryMock.Object);
        }

        [Test]
        public void UpdateCardCommandHandler_ShouldReturn_RequiredCardId()
        {
            // Arrange
            var command = new UpdateCardCommand { Name = "Test" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == nameof(UpdateCardCommand.CardId) && e.ErrorMessage == BaseStrings.CARD_ID_REQUIRED));
        }

        [Test]
        public void UpdateCardCommandHandler_ShouldReturn_RequiredCardName()
        {
            // Arrange
            var command = new UpdateCardCommand { CardId = 1 };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == nameof(UpdateCardCommand.Name) && e.ErrorMessage == BaseStrings.CARD_NAME_REQUIRED));
        }

        [Test]
        public void UpdateCardCommandHandler_ShouldReturn_RequiredCardStatus()
        {
            // Arrange
            var command = new UpdateCardCommand { Name = "Test", CardId = 1 };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == nameof(UpdateCardCommand.Status) && e.ErrorMessage == BaseStrings.CARD_STATUS_REQUIRED));
        }

        [Test]
        public void UpdateCardCommandHandler_ShouldReturn_InvalidCardStatus()
        {
            // Arrange
            var command = new UpdateCardCommand { Name = "Test", CardId = 1, Status = "To Do" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == nameof(UpdateCardCommand.Status) && e.ErrorMessage == BaseStrings.INVALID_CARD_STATUS));
        }

        [Test]
        public void UpdateCardCommandHandler_ShouldPass_ColorFormatValidation()
        {
            // Arrange
            var command = new UpdateCardCommand { CardId = 1, Status = "ToDo", Name = "Test Card", Color = "#FFFFFF" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Color);
        }

        [Test]
        public void UpdateCardCommandHandler_ShouldReturn_InvalidColorFormat()
        {
            // Arrange
            var command = new UpdateCardCommand { CardId = 1, Status = "ToDo", Name = "Test Card", Color = "InvalidColor" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Color)
                .WithErrorMessage(BaseStrings.INVALID_COLOR_FORMAT);
        }

        [Test]
        public void UpdateCardCommandHandler_ShouldReturn_CardNotExist()
        {
            // Arrange
            var command = new UpdateCardCommand { CardId = 1, Status = "ToDo", Name = "Test Card", Color = "#FFFFFF" };

            _repositoryMock.Setup(r => r.Exist(It.IsAny<Expression<Func<CardEntity, bool>>>())).Returns(false);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CardId)
                .WithErrorMessage(BaseStrings.CARD_NOT_EXIST);
        }

        [Test]
        public void UpdateCardCommandHandler_ShouldReturn_CardAlreadyExist()
        {
            // Arrange
            var command = new UpdateCardCommand { CardId = 1, Status = "ToDo", Name = "Test Card", Color = "#FFFFFF" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage(BaseStrings.CARD_ALREADY_EXIST);
        }

        [Test]
        public async Task UpdateCardCommandHandler_ShouldReturn_SuccessResponse()
        {
            // Arrange
            var request = new UpdateCardCommand { CardId = 1, Status = "ToDo", Name = "Test Card", Color = "#FFFFFF" };

            var sampleCard = new CardEntity
            {
                Status = Status.ToDo,
                Name = "Example Name",
                Color = "#FFFFFF",
                DateCreated = new DateTime(2022, 6, 15)
            };

            _uowMock.Setup(uow => uow.Repository<CardEntity>().FindAsync(
                It.IsAny<Expression<Func<CardEntity, bool>>>()))
                .ReturnsAsync(sampleCard);

            _uowMock.Setup(uow => uow.Complete(cancellationToken)).Returns(Task.FromResult(true)).Verifiable();

            // Act
            var response = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessful);
            Assert.That(response.Message, Is.EqualTo(BaseStrings.SUCCESSFUL_CARD_UPDATE));
            _uowMock.Verify(uow => uow.Complete(cancellationToken), Times.Once);
        }
    }
}
