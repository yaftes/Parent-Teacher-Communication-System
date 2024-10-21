using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Authorize]
public class ChatController : Controller{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly ApplicationDbContext dbContext;
    public ChatController(

        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext dbContext){

        this.userManager = userManager;
        this.signInManager = signInManager;
        this.roleManager = roleManager;
        this.dbContext = dbContext;
    }

    public async Task<IActionResult> Users()
    {
        var curruser = await userManager.GetUserAsync(User);
        var allusers = await userManager.Users.Where(u => u != curruser).ToListAsync();
        List<ApplicationUser> users = [];

        if(curruser != null){

            if(await userManager.IsInRoleAsync(curruser,"Student")){
                foreach(var user in allusers){
                    if(await userManager.IsInRoleAsync(user,"Admin") || await userManager.IsInRoleAsync(user,"Teacher")){
                        users.Add(user);
                    }
                }
            }

            else if(await userManager.IsInRoleAsync(curruser,"Teacher")){
                foreach(var user in allusers){
                    if(await userManager.IsInRoleAsync(user,"Admin") || await userManager.IsInRoleAsync(user,"Student")){
                        users.Add(user);
                    }
                }
            }
            
            else {
                users = allusers;
            }
        }

        return View(users);
    }


   public async Task<IActionResult> ChatRoom(string  ReceiverId) {

    if (string.IsNullOrEmpty(ReceiverId)){
        return BadRequest("User ID is required");
    }
    var checkreceiver = userManager.Users.Any(u => u.Id == ReceiverId);

    if(!checkreceiver){
        return NotFound("No User Found");
    }
    var currentUserId = userManager.GetUserId(User);

    if( currentUserId == null ){
        return NotFound("No User Found");
    }
    var chatHistory = await dbContext.ChatMessage
        .Where(m => (m.SenderId == currentUserId.ToString() && m.ReceiverId == ReceiverId) || 
                     (m.SenderId == ReceiverId && m.ReceiverId == currentUserId.ToString()))
        .OrderBy(m => m.CreaetedAt)
        .ToListAsync();

    ViewData["ReceiverId"] = ReceiverId;

    return View(chatHistory);

    }

}
