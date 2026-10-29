using Microsoft.AspNetCore.SignalR;

public class PagamentoHub : Hub
{
    public async Task EntrarPedido(string pedidoId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, pedidoId);
    }
}