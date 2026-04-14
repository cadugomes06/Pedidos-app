using System.ComponentModel.DataAnnotations;

namespace PedidosApp.DTO
{    public class CreateOrderDTO
    {
        [Required]
        public required string Cliente { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }
    }
}
