async function createOrder() {
    const cliente = document.getElementById("cliente").value.trim();
    const valor = document.getElementById("valor").value;
    const status = document.getElementById("createOrderStatus");

    status.innerText = "";

    if (!cliente || !valor ) {
        status.innerText = "Preencha todos os campos.";
        return;
    }

    const order = {
        cliente: cliente,
        valor: parseFloat(valor)
    };

    try {
        console.log("enviado pedido...");

        const response = await fetch("/orders", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(order)
        });

        if (!response.ok) {
            if (data.errors) {
                status.innerText = data.errors.join(", ");
            } else {
                status.innerText = data.message || "Erro ao criar pedido.";
            }
            return;
        }

        closeCreateOrderModal();

        document.getElementById("cliente").value = "";
        document.getElementById("valor").value = "";

        await loadOrders();

    } catch (error) {
        status.innerText = "Não foi possível salvar o pedido.";
    }
}

function openCreateOrderModal() {
    const modal = document.getElementById("createOrderModal");
    const status = document.getElementById("createOrderStatus");

    if (status) status.innerText = "";
    modal.classList.add("active");
}

function closeCreateOrderModal() {
    const modal = document.getElementById("createOrderModal");
    modal.classList.remove("active");
}

window.addEventListener("click", function (event) {
    const modal = document.getElementById("createOrderModal");

    if (event.target === modal) {
        closeCreateOrderModal();
    }
});