async function loadOrders() {
    const status = document.getElementById("status");
    const table = document.getElementById("ordersTable");
    const tbody = document.getElementById("ordersBody");
    const emptyState = document.getElementById("emptyState");

    status.innerText = "Carregando pedidos...";
    table.style.display = "none";
    emptyState.style.display = "none";
    tbody.innerHTML = "";

    try {
        const response = await fetch("/orders");

        if (!response.ok) {
            throw new Error("Erro ao buscar pedidos.");
        }

        const result = await response.json();
        const orders = result.data;


        if (!orders || orders.length === 0) {
            status.innerText = "";
            emptyState.style.display = "block";
            return;
        }

        orders.forEach(order => {
            const row = document.createElement("tr");
            row.classList.add("clickable-row");
            row.onclick = () => viewOrderDetails(order.id);

            const dataFormatada = order.dataPedido
                ? new Date(order.dataPedido).toLocaleString("pt-BR")
                : "-";

            const valorFormatado = order.valor !== undefined && order.valor !== null
                ? Number(order.valor).toLocaleString("pt-BR", {
                    style: "currency",
                    currency: "BRL"
                })
                : "-";

            row.innerHTML = `
                        <td>${order.id ?? "-"}</td>
                        <td>${order.cliente ?? "-"}</td>
                        <td>${valorFormatado}</td>
                        <td>${dataFormatada}</td>
                        <td><span class="badge">Disponível</span></td>
                    `;

            tbody.appendChild(row);
        });

        status.innerText = `${orders.length} pedido(s) encontrado(s).`;
        table.style.display = "table";
    } catch (error) {
        status.innerText = "Não foi possível carregar os pedidos.";
    }
}

document.addEventListener("DOMContentLoaded", loadOrders);
