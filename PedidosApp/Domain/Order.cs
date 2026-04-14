namespace PedidosApp.Domain
{
    public class Order
    {
        public Guid Id { get; private set; }
        public string Cliente { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataPedido { get; private set; }

        public Order(string cliente, decimal valor)
        {
            if (string.IsNullOrWhiteSpace(cliente))
            {
                throw new ArgumentException("Cliente é obrigatório");
            }
            if (valor <= 0)
            {
                throw new ArgumentException("Valor deve ser maior que zero");
            }

            Id = Guid.NewGuid();
            DataPedido = DateTime.Now;
            Valor = valor;
            Cliente = cliente;

        }
    }
}
