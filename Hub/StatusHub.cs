

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

public class StatusHub : Hub {

    private readonly UserManager<ApplicationUser> userManager;

    public StatusHub(UserManager<ApplicationUser> userManager){
        this.userManager = userManager; 
    }
            public override async Task OnConnectedAsync(){
            var curruser = await userManager.GetUserAsync(Context.User);
            if(curruser != null){
                curruser.Status = true;
                await userManager.UpdateAsync(curruser);
                await Clients.All.SendAsync("UserStatusChanged", curruser.Id, true);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception){
            var curruser = await userManager.GetUserAsync(Context.User);
            if(curruser != null){
                curruser.Status = false;
                await userManager.UpdateAsync(curruser);
                await Clients.All.SendAsync("UserStatusChanged", curruser.Id, false);
            }
            await base.OnDisconnectedAsync(exception);
        }

}