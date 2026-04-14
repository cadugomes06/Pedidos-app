using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using PedidosApp.Domain;
using PedidosApp.DTO;
using PedidosApp.Infrastructure;
using PedidosApp.Service.Interface;
using System.Text.Json;

namespace PedidosApp.Service.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProducer<string, string> _producer;
        private readonly IConfiguration _configuration;
        public OrderService(ApplicationDbContext context,
            IProducer<string, string> producer,
            IConfiguration configuration)
        {
            _context = context;
            _producer = producer;
            _configuration = configuration;
        }


        public async Task<ApiResponseDTO<object>> CreateAsync(CreateOrderDTO dto)
        {
            try
            {
                var order = new Order(dto.Cliente, dto.Valor);

                var topic = _configuration["Kafka:Topic"];

                if (string.IsNullOrWhiteSpace(topic))
                {
                    return new ApiResponseDTO<object>
                    {
                        Success = false,
                        Message = "Tópico Kafka não configurado."
                    };
                }

                var message = new Message<string, string>
                {
                    Key = order.Id.ToString(),
                    Value = JsonSerializer.Serialize(order)
                };

                await _producer.ProduceAsync(topic, message);

                return new ApiResponseDTO<object>
                {
                    Success = true,
                    Message = "Pedido enviado para processamento.",
                    Data = new { orderId = order.Id }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "Erro ao criar pedido.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponseDTO<List<Order>>> GetAllAsync()
        {
            var orders = await _context.Orders.ToListAsync();

            if (!orders.Any())
            {
                return new ApiResponseDTO<List<Order>>
                {
                    Success = true,
                    Message = "Nenhum pedido encontrado.",
                    Data = new List<Order>()
                };
            }

            return new ApiResponseDTO<List<Order>>
            {
                Success = true,
                Data = orders
            };
        }

        public async Task<ApiResponseDTO<Order>> GetByIdAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return new ApiResponseDTO<Order>
                {
                    Success = false,
                    Message = $"Pedido com id:{id} não encontrado."
                };
            }

            return new ApiResponseDTO<Order>
            {
                Success = true,
                Data = order
            };
        }


    }
}
