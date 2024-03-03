using AutoMapper;
using CardService.Application.Common.Enums;
using CardService.Application.Common.Helpers;
using CardService.Application.Common.Interfaces;
using CardService.Application.Common.Mapper;
using CardService.Application.UseCases.Card.Commands;
using CardService.Application.UseCases.Card.Handlers;
using CardService.Application.UseCases.Card.Queries;
using CardService.Application.UseCases.Card.Validators;
using CardService.Domain.Entities;
using CardService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CardService.UnitTests.Features
{
    [TestFixture]
    public class GetCardsQueryHandlerTests
    {
        private IMapper _mapper;
        private Mock<ILogger<GetCardsQueryHandler>> _loggerMock;
        private Mock<IUnitOfWork> _uowMock;
        private GetCardsQueryHandler _handler;
        private CancellationToken cancellationToken = new CancellationToken();
        private GetCardsQueryValidator _validator;
        private Mock<IRepository<CardEntity>> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _validator = new GetCardsQueryValidator();
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _loggerMock = new Mock<ILogger<GetCardsQueryHandler>>();
            _uowMock = new Mock<IUnitOfWork>();
            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            _mapper = mockMapper.CreateMapper();

            _handler = new GetCardsQueryHandler(_loggerMock.Object, _mapper, _uowMock.Object);

            _repositoryMock = new Mock<IRepository<CardEntity>>();
            _repositoryMock.Setup(r => r.Exist(It.IsAny<Expression<Func<CardEntity, bool>>>())).Returns(true);
            _repositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<CardEntity, bool>>>())).ReturnsAsync(new CardEntity { Name = "Test" });
            _uowMock.Setup(u => u.Repository<CardEntity>()).Returns(_repositoryMock.Object);
            _uowMock.Setup(uow => uow.Complete(cancellationToken)).Returns(Task.FromResult(true)).Verifiable();
        }

        [Test]
        public async Task GetCardsQueryHandler_NoFilters_ShouldReturn_CardNotExist()
        {
            // Arrange
            var request = new GetCardsQuery { /* Initialize request without any filters */ };
            var cancellationToken = new CancellationToken();

            var cards = new List<CardEntity>();

            var mockQueryable = cards.AsQueryable();
            var mockDbSet = new Mock<DbSet<CardEntity>>();
            mockDbSet.As<IQueryable<CardEntity>>().Setup(m => m.Provider).Returns(mockQueryable.Provider);
            mockDbSet.As<IQueryable<CardEntity>>().Setup(m => m.Expression).Returns(mockQueryable.Expression);
            mockDbSet.As<IQueryable<CardEntity>>().Setup(m => m.ElementType).Returns(mockQueryable.ElementType);
            mockDbSet.As<IQueryable<CardEntity>>().Setup(m => m.GetEnumerator()).Returns(mockQueryable.GetEnumerator());

            _uowMock.Setup(uow => uow.Repository<CardEntity>().FilterAsNoTracking(
                It.IsAny<Expression<Func<CardEntity, bool>>>(), // Mock any filter
                null, null, null, It.IsAny<Func<IQueryable<CardEntity>, IIncludableQueryable<CardEntity, object>>>()))
            .Returns(cards.AsQueryable());

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Message, Is.EqualTo(BaseStrings.CARD_NOT_EXIST));
            Assert.That(result.ResponseCode, Is.EqualTo(ResponseCodes.BAD_REQUEST));
        }

        [Test]
        public async Task GetCardsQueryHandler_NoFilters_ShouldReturn_AllCards()
        {
            // Arrange
            var request = new GetCardsQuery { /* Initialize request without any filters */ };
            var cancellationToken = new CancellationToken();

            var cards = new List<CardEntity>
            {
                new CardEntity { Id = 1, Name = "Test1" },
                new CardEntity { Id = 2, Name = "Test2" },
                new CardEntity { Id = 3, Name = "Test3" },
            };
            var mockQueryable = cards.AsQueryable();
            var mockDbSet = new Mock<DbSet<CardEntity>>();
            mockDbSet.As<IQueryable<CardEntity>>().Setup(m => m.Provider).Returns(mockQueryable.Provider);
            mockDbSet.As<IQueryable<CardEntity>>().Setup(m => m.Expression).Returns(mockQueryable.Expression);
            mockDbSet.As<IQueryable<CardEntity>>().Setup(m => m.ElementType).Returns(mockQueryable.ElementType);
            mockDbSet.As<IQueryable<CardEntity>>().Setup(m => m.GetEnumerator()).Returns(mockQueryable.GetEnumerator());

            _uowMock.Setup(uow => uow.Repository<CardEntity>().FilterAsNoTracking(
            It.IsAny<Expression<Func<CardEntity, bool>>>(),
            null, null, null,
            It.IsAny<Func<IQueryable<CardEntity>, IIncludableQueryable<CardEntity, object>>>()))
            .Returns(cards.AsQueryable());

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.ResponseCode, Is.EqualTo(ResponseCodes.SUCCESS));
        }

        [Test]
        public async Task GetCardsQueryHandler_WithFilters_ShouldReturn_FilteredCards()
        {
            // Arrange
            var request = new GetCardsQuery
            {
                Status = "ToDo", // Example filter criteria
                Name = "Test Name",
                Color = "#99DDFF",
                From = new DateTime(2022, 1, 1), // Example date range
                To = new DateTime(2022, 12, 31)
            };
            var cancellationToken = new CancellationToken();

            var cards = new List<CardEntity>
            {
                // Sample cards that match the filter criteria
                new CardEntity { Status = Status.ToDo, Name = "Test Name", Color = "#99DDFF", DateCreated = new DateTime(2022, 1, 1) },
                new CardEntity { Status = Status.ToDo, Name = "Test Card", Color = "#FFFFFF", DateCreated = new DateTime(2022, 6, 15) }
            }.AsQueryable();

            _uowMock.Setup(uow => uow.Repository<CardEntity>().FilterAsNoTracking(
                It.IsAny<Expression<Func<CardEntity, bool>>>(), // Mock any filter
                null, null, null, It.IsAny<Func<IQueryable<CardEntity>, IIncludableQueryable<CardEntity, object>>>()))
            .Returns(cards.AsQueryable());

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.TotalCount, Is.EqualTo(1));
            Assert.IsTrue(result.Data.All(c => c.Status == Status.ToDo));
            Assert.IsTrue(result.Data.Exists(c => c.Name == "Test Name"));
            Assert.IsTrue(result.Data.Exists(c => c.Color == "#99DDFF"));
            Assert.IsTrue(result.Data.All(c => c.CreatedOn >= new DateTime(2022, 1, 1) && c.CreatedOn <= new DateTime(2022, 12, 31)));
        }
    }
}
