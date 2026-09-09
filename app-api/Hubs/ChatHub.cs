using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private readonly IChatService chatService;

    public ChatHub(IChatService chatService)
    {
        this.chatService = chatService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;

        Console.WriteLine("USER ID:");
        Console.WriteLine(userId);

        if (string.IsNullOrWhiteSpace(userId))
        {
            Context.Abort();
            return;
        }

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            $"user:{userId}");

        await base.OnConnectedAsync();
    }
    public async Task SendMessage(ClientMessage message)
    {
        var userId = Context.UserIdentifier;

        if(string.IsNullOrWhiteSpace(userId))
        {
            Context.Abort();
            return;
        }

        var responses = chatService.ChatAsync(message.message, Guid.Parse(message.documentId), Context.ConnectionAborted);

        await foreach (var response in responses)
        {
            await Clients.Group($"user:{userId}").SendAsync("ReceiveMessage", new { id = Guid.NewGuid(), sender = "AI", message = response.Response, done = response.Done }, Context.ConnectionAborted);
        }
    }
}

public class ClientMessage
{
    public string id { get; set; }
    public string sender { get; set; }
    public string message { get; set; }
    public string documentId { get; set; }
}