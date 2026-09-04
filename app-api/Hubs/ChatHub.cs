using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private readonly IChatService chatService;

    public ChatHub(IChatService chatService)
    {
        this.chatService = chatService;
    }
    public async Task SendMessage(ClientMessage message)
    {
        var responses = chatService.ChatAsync(message.message, Guid.Parse(message.documentId), Context.ConnectionAborted);

        await foreach (var response in responses)
        {    
            await Clients.All.SendAsync("ReceiveMessage", new {id = Guid.NewGuid(), sender = "AI", message = response.Response, done = response.Done}, Context.ConnectionAborted);
        }
    }
}

public class ClientMessage
{
    public string id {get; set;}
    public string sender {get; set;}
    public string message {get; set;}
    public string documentId {get; set;}
}