

using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class ChatHub : Hub {
    private readonly ApplicationDbContext dbContext;
    private readonly UserManager<ApplicationUser> userManager;  
    public ChatHub(ApplicationDbContext dbContext,UserManager<ApplicationUser> userManager) {
        this.dbContext = dbContext; 
        this.userManager = userManager;
    }

    



    public async Task SendMessage(string SenderId,string ReceiverId,string Message){

        var message = new ChatMessage();
        message.SenderId = SenderId;
        message.ReceiverId = ReceiverId;
        message.Message = Message;  
        message.CreaetedAt = DateTime.Now;  

        await dbContext.ChatMessage.AddAsync(message);
        await dbContext.SaveChangesAsync();

       await Clients.User(ReceiverId).SendAsync("ReceiveMessage",SenderId,Message);    
    }
}