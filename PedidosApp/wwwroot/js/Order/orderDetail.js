async function viewOrderDetails(id) {
    const status = document.getElementById("detailsStatus");

    document.getElementById("detailId").innerText = "-";
    document.getElementById("detailCliente").innerText = "-";
    document.getElementById("detailValor").innerText = "-";
    document.getElementById("detailDataPedido").innerText = "-";
    status.innerText = "";

    openOrderDetailsModal();
    status.innerText = "Carregando detalhes do pedido...";

    try {
        const response = await fetch(`/orders/${id}`);
        const result = await response.json();

        if (!response.ok || !result.success) {
            status.innerText = result.message || "Não foi possível carregar o pedido.";
            return;
        }

        const order = result.data;

        document.getElementById("detailId").innerText = order.id ?? "-";
        document.getElementById("detailCliente").innerText = order.cliente ?? "-";
        document.getElementById("detailValor").innerText =
            order.valor !== undefined && order.valor !== null
                ? Number(order.valor).toLocaleString("pt-BR", {
                    style: "currency",
                    currency: "BRL"
                })
                : "-";

        document.getElementById("detailDataPedido").innerText =
            order.dataPedido
                ? new Date(order.dataPedido).toLocaleString("pt-BR")
                : "-";

        status.innerText = "";
    } catch (error) {
        status.innerText = "Erro ao buscar detalhes do pedido.";
        console.error(error);
    }
}

function openOrderDetailsModal() {
    const modal = document.getElementById("orderDetailsModal");
    const status = document.getElementById("detailsStatus");

    if (status) status.innerText = "";
    modal.classList.add("active");
}

function closeOrderDetailsModal() {
    const modal = document.getElementById("orderDetailsModal");
    modal.classList.remove("active");
}