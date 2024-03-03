using CardService.Application.Common.Helpers;
using CardService.Application.Common.Interfaces;
using CardService.Application.UseCases.Card.Commands;
using CardService.Application.UseCases.Card.Handlers;
using CardService.Application.UseCases.Card.Validators;
using CardService.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace CardService.UnitTests.Features
{
    [TestFixture]
    public class DeleteCardCommandHandlerTests
    {
        private Mock<ILogger<DeleteCardCommandHandler>> _loggerMock;
        private Mock<IUnitOfWork> _uowMock;
        private DeleteCardCommandHandler _handler;
        private CancellationToken cancellationToken = new CancellationToken();
        private DeleteCardCommandValidator _validator;
        private Mock<IRepository<CardEntity>> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _validator = new DeleteCardCommandValidator(_uowMock.Object);
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _loggerMock = new Mock<ILogger<DeleteCardCommandHandler>>();
            _uowMock = new Mock<IUnitOfWork>();

            _handler = new DeleteCardCommandHandler(_loggerMock.Object, _uowMock.Object);

            _repositoryMock = new Mock<IRepository<CardEntity>>();
            _repositoryMock.Setup(r => r.Exist(It.IsAny<Expression<Func<CardEntity, bool>>>())).Returns(true);
            _repositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<CardEntity, bool>>>())).ReturnsAsync(new CardEntity { Name = "Test" });
            _uowMock.Setup(u => u.Repository<CardEntity>()).Returns(_repositoryMock.Object);
            _uowMock.Setup(uow => uow.Complete(cancellationToken)).Returns(Task.FromResult(true)).Verifiable();
        }

        [Test]
        public void DeleteCardCommandHandler_ShouldReturn_CardIdRequired()
        {
            // Arrange
            var command = new DeleteCardCommand { UserId = 1 };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.IsFalse(result.IsValid); // Ensure validation fails
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == nameof(DeleteCardCommand.CardId) && e.ErrorMessage == BaseStrings.CARD_ID_REQUIRED));
        }

        [Test]
        public void DeleteCardCommandHandler_ShouldReturn_CardNotExist()
        {
            // Arrange
            var command = new DeleteCardCommand { UserId = 1, CardId = -1 };

            _repositoryMock.Setup(r => r.Exist(It.IsAny<Expression<Func<CardEntity, bool>>>())).Returns(false);

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.IsFalse(result.IsValid); // Ensure validation fails
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == nameof(DeleteCardCommand.CardId) && e.ErrorMessage == BaseStrings.CARD_NOT_EXIST));
        }


        [Test]
        public async Task DeleteCardCommandHandler_ShouldReturn_SuccessResponse()
        {
            // Arrange
            var request = new DeleteCardCommand { UserId = 1, CardId = 1 };

            _uowMock.Setup(u => u.Repository<CardEntity>()).Returns(_repositoryMock.Object);

            // Act
            var response = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessful);
            Assert.That(response.Message, Is.EqualTo(BaseStrings.SUCCESSFUL_CARD_DELETE));
            _uowMock.Verify(uow => uow.Complete(cancellationToken), Times.Once);
        }

    }
}
