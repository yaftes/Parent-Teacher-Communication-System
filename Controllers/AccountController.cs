using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


public class AccountController : Controller {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly RoleManager<ApplicationRole> roleManager; 
    private readonly ApplicationDbContext dbContext;

    
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext dbContext){
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.dbContext = dbContext;

        }



    [HttpGet]
    public IActionResult Login(){
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserLoginModel Model){
        if(ModelState.IsValid){
            var User = await userManager.FindByNameAsync(Model.UserName);
            if(User != null){
             var result = await signInManager.PasswordSignInAsync(User,Model.Password,Model.RememberMe,false);
             if(result.Succeeded){
                if(await userManager.IsInRoleAsync(User,"Admin")){
                    return RedirectToAction("DashBoard","Admin");
                }
                else if(await userManager.IsInRoleAsync(User,"Teacher")){
                    return RedirectToAction("Courses","CourseReport");
                }
                else if(await userManager.IsInRoleAsync(User,"Student")){
                    return RedirectToAction("StudentReport","StudentPerformance");

                }
                
             }
             else{
                ModelState.AddModelError("Password","Incorrect Password");
                return View();
             }
            }
            else{
                ModelState.AddModelError(string.Empty,"You don't have an account");
                return View();
            }
        }
        return View(Model);
    }

    public async Task<IActionResult> Logout(){
        await signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile(){
        var curruser = await userManager.GetUserAsync(User);
        if(curruser is null){
            return NotFound("No user Found");
        }
        ProfileViewModel Model = new ProfileViewModel();
        Model.ConfirmPassword = string.Empty;
        Model.NewPassword = string.Empty;   
        Model.User = curruser ?? new ApplicationUser();
        return View(Model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Profile(ProfileViewModel Model){
        var curruser = await userManager.GetUserAsync(User);
        if(ModelState.IsValid){
          if(curruser != null){
           var result = await userManager.ChangePasswordAsync(curruser,Model.OldPassword ?? "",Model.NewPassword ?? "");
           if(result.Succeeded){
            ViewBag.Success = "Success";
            return RedirectToAction("Login");
           }
           else{
            foreach(var error in result.Errors){
                ModelState.AddModelError(string.Empty, error.Description);  
            };
           }
         }
        } 
        Model.User = curruser ;
        return View(Model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetProfilePicture(string UserName) {
        if(string.IsNullOrEmpty(UserName)){
            return NotFound("User Not Found"); }
            
            var currentUser = await userManager.FindByNameAsync(UserName);
            if (currentUser == null)
            {
                return NotFound("User not found.");
            }

            if (currentUser.ProfilePicture == null){

                string defaultImagePath;

                if(await userManager.IsInRoleAsync(currentUser,"Student")){
                defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets","img" ,"avatars", "student.jpg");
                }
                else if(await userManager.IsInRoleAsync(currentUser,"Teacher")){
                    defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets","img" ,"avatars", "Teacher.jpg");
                }
                else{
                    defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets","img" ,"avatars", "admin.jpg");
                }
                var defaultImage = await System.IO.File.ReadAllBytesAsync(defaultImagePath);

                return File(defaultImage, "image/jpg");
            }
            string mimeType;
            if(currentUser.ContentType == "image/jpeg"){
                mimeType =  "image/jpeg";
            }
            else{
                mimeType = "image/png";
            }
            
            return File(currentUser.ProfilePicture, mimeType);
        }

}