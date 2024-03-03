using AutoMapper;
using CardService.Application.Common.Helpers;
using CardService.Application.Common.Interfaces;
using CardService.Application.Common.Mapper;
using CardService.Application.UseCases.Card.Handlers;
using CardService.Application.UseCases.Card.Queries;
using CardService.Domain.Entities;
using CardService.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace CardService.UnitTests.Features
{
    [TestFixture]
    public class GetSingleCardQueryHandlerTests
    {
        private IMapper _mapper;
        private Mock<ILogger<GetSingleCardQueryHandler>> _loggerMock;
        private Mock<IUnitOfWork> _uowMock;
        private GetSingleCardQueryHandler _handler;
        private CancellationToken cancellationToken = new CancellationToken();
        private Mock<IRepository<CardEntity>> _repositoryMock;

        [SetUp]
        public void Setup()
        {
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _loggerMock = new Mock<ILogger<GetSingleCardQueryHandler>>();
            _uowMock = new Mock<IUnitOfWork>();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            _mapper = mockMapper.CreateMapper();

            _repositoryMock = new Mock<IRepository<CardEntity>>();
            _uowMock.Setup(u => u.Repository<CardEntity>()).Returns(_repositoryMock.Object);

            _handler = new GetSingleCardQueryHandler(_loggerMock.Object, _mapper, _uowMock.Object);
        }

        [Test]
        public async Task GetSingleCardQueryHandler_ShouldReturn_CardNotExist()
        {
            // Arrange
            var query = new GetSingleCardQuery { UserId = 1, CardId = 1 };
            // Mocking FindAsync to return null (card not found)
            _uowMock.Setup(uow => uow.Repository<CardEntity>().FindAsync(
                It.IsAny<Expression<Func<CardEntity, bool>>>(),
                It.IsAny<Expression<Func<CardEntity, object>>[]>()))
                .ReturnsAsync((CardEntity)null);
            // Act
            var response = await _handler.Handle(query, cancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.IsSuccessful);
            Assert.That(response.Message, Is.EqualTo(BaseStrings.CARD_NOT_EXIST));
        }

        [Test]
        public async Task CreateCardCommandHandler_ShouldReturn_SuccessResponse()
        {
            // Arrange
            var query = new GetSingleCardQuery { UserId = 1, CardId = 1 };
            var sampleCard = new CardEntity
            {
                Status = Status.ToDo,
                Name = "Example Name",
                Color = "#FFFFFF",
                DateCreated = new DateTime(2022, 6, 15)
            };

            _uowMock.Setup(uow => uow.Repository<CardEntity>().FindAsync(
                It.IsAny<Expression<Func<CardEntity, bool>>>(), // Mock any filter
                It.IsAny<Expression<Func<CardEntity, object>>[]>()))
                .ReturnsAsync(sampleCard);

            // Act
            var response = await _handler.Handle(query, cancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessful);
            Assert.That(response.Message, Is.EqualTo(BaseStrings.SUCCESSFUL));
        }
    }
}
