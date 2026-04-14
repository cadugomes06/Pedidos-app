using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using PedidosApp.Domain;
using PedidosApp.DTO;
using PedidosApp.Infrastructure;
using PedidosApp.Service.Implementation;

namespace PedidosApp.Tests.Services
{
    public class OrderServiceTests
    {
        private static ApplicationDbContext CreateDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        private static Mock<IConfiguration> CreateConfigurationMock(string? topic)
        {
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Kafka:Topic"]).Returns(topic);
            return configurationMock;
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenTopicIsConfigured()
        {
            // Arrange
            var dbContext = CreateDbContext(Guid.NewGuid().ToString());
            var producerMock = new Mock<IProducer<string, string>>();
            var configurationMock = CreateConfigurationMock("orders-topic");

            producerMock
                .Setup(p => p.ProduceAsync(
                    It.IsAny<string>(),
                    It.IsAny<Message<string, string>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((DeliveryResult<string, string>)null!);

            var service = new OrderService(
                dbContext,
                producerMock.Object,
                configurationMock.Object);

            var dto = new CreateOrderDTO
            {
                Cliente = "Carlos",
                Valor = 150.75m
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Pedido enviado para processamento.", result.Message);
            Assert.NotNull(result.Data);

            producerMock.Verify(p => p.ProduceAsync(
                "orders-topic",
                It.IsAny<Message<string, string>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFailure_WhenTopicIsNotConfigured()
        {
            // Arrange
            var dbContext = CreateDbContext(Guid.NewGuid().ToString());
            var producerMock = new Mock<IProducer<string, string>>();
            var configurationMock = CreateConfigurationMock(null);

            var service = new OrderService(
                dbContext,
                producerMock.Object,
                configurationMock.Object);

            var dto = new CreateOrderDTO
            {
                Cliente = "Carlos",
                Valor = 100m
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Tópico Kafka não configurado.", result.Message);

            producerMock.Verify(p => p.ProduceAsync(
                It.IsAny<string>(),
                It.IsAny<Message<string, string>>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoOrdersExist()
        {
            // Arrange
            var dbContext = CreateDbContext(Guid.NewGuid().ToString());
            var producerMock = new Mock<IProducer<string, string>>();
            var configurationMock = CreateConfigurationMock("orders-topic");

            var service = new OrderService(
                dbContext,
                producerMock.Object,
                configurationMock.Object);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Nenhum pedido encontrado.", result.Message);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnOrders_WhenOrdersExist()
        {
            // Arrange
            var dbContext = CreateDbContext(Guid.NewGuid().ToString());

            dbContext.Orders.Add(new Order("Carlos", 100m));
            dbContext.Orders.Add(new Order("Maria", 250m));
            await dbContext.SaveChangesAsync();

            var producerMock = new Mock<IProducer<string, string>>();
            var configurationMock = CreateConfigurationMock("orders-topic");

            var service = new OrderService(
                dbContext,
                producerMock.Object,
                configurationMock.Object);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var dbContext = CreateDbContext(Guid.NewGuid().ToString());
            var order = new Order("Carlos", 99.90m);

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();

            var producerMock = new Mock<IProducer<string, string>>();
            var configurationMock = CreateConfigurationMock("orders-topic");

            var service = new OrderService(
                dbContext,
                producerMock.Object,
                configurationMock.Object);

            // Act
            var result = await service.GetByIdAsync(order.Id);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(order.Id, result.Data.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnFailure_WhenOrderDoesNotExist()
        {
            // Arrange
            var dbContext = CreateDbContext(Guid.NewGuid().ToString());
            var producerMock = new Mock<IProducer<string, string>>();
            var configurationMock = CreateConfigurationMock("orders-topic");

            var service = new OrderService(
                dbContext,
                producerMock.Object,
                configurationMock.Object);

            var id = Guid.NewGuid();

            // Act
            var result = await service.GetByIdAsync(id);

            // Assert
            Assert.False(result.Success);
            Assert.Equal($"Pedido com id:{id} não encontrado.", result.Message);
            Assert.Null(result.Data);
        }
    }
}