using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class AnnouncementController : Controller{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly RoleManager<ApplicationRole> roleManager; 
    private readonly ApplicationDbContext dbContext;
    public AnnouncementController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext dbContext){
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.dbContext = dbContext; 
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create(){
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(AnnouncementModel Model,IFormFile? image){
            if(ModelState.IsValid){
                var curruser = await userManager.GetUserAsync(User);
               Announcement announcement = new Announcement();

               
               announcement.Title = Model.Title;
               announcement.Description = Model.Description;    
               announcement.CreatedAt = DateTime.Now; 
               announcement.CreatedBy = curruser.FirstName + " " + curruser.LastName;
               if(image != null && image.Length > 0){
                using(var memorystream = new MemoryStream()){
                    await image.CopyToAsync(memorystream);
                    announcement.image = memorystream.ToArray();
                    announcement.ContentType = image.ContentType;
                }
               }  
               
               await dbContext.Announcement.AddAsync(announcement);
               await dbContext.SaveChangesAsync();
               return RedirectToAction("Announcements");
               
               
            }
            return View(Model);
    
        }

        [HttpGet]
        public IActionResult Announcements(string? time){
            var announcements =  dbContext.Announcement.OrderBy(a => a.CreatedAt);
            List<Announcement> announcement = new List<Announcement>();
            DateTime today = DateTime.Now;

            if(announcements != null){

                if(time == "Today"){
                    announcements = announcements.Where(r => r.CreatedAt <= today).OrderBy( c => c.CreatedAt);
                }
                else if (time == "Week"){
                    announcements = announcements.Where(r => r.CreatedAt >= today.AddDays(-7)).OrderBy( c => c.CreatedAt);
                }
                else if (time == "Month"){
                    announcements = announcements.Where(r => r.CreatedAt >= today.AddDays(-30)).OrderBy( c => c.CreatedAt);
                }
                announcement = announcements.ToList();

            }
            return View(announcement);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(string Id){
            if(string.IsNullOrEmpty(Id)){
                return NotFound("Not Found");
            }  
            var announcement = await dbContext.Announcement.FirstOrDefaultAsync(a => a.Id == Id);  
            AnnouncementModel announcementModel = new AnnouncementModel();
            announcementModel.Title = announcement?.Title;
            announcementModel.Description = announcement?.Description;
            return View(announcementModel); 

        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(string Id,AnnouncementModel Model,IFormFile image){
            var announcement = await dbContext.Announcement.FirstOrDefaultAsync(a => a.Id == Id);   
            if(announcement != null){

                announcement.Title = Model.Title;
                announcement.Description = Model.Description; 
                announcement.CreatedAt = DateTime.Now;

                if(image != null && image.Length > 0){
                    using(var ms = new MemoryStream()){
                       await image.CopyToAsync(ms);
                       announcement.image = ms.ToArray();
                       announcement.ContentType= image.ContentType;
                    }
                }  

                dbContext.Announcement.Update(announcement);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Announcements");

            }
            return View();
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(string Id){
            if(string.IsNullOrEmpty(Id)){
                return NotFound("Not Found");
            }
            var announcement = await dbContext.Announcement.FirstOrDefaultAsync(a => a.Id == Id);
            if(announcement != null){
                dbContext.Announcement.Remove(announcement);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Announcements");
        }

        public async Task<IActionResult> GetImage(string Id){
            if(string.IsNullOrEmpty(Id)){
                return NotFound("Not Found");
            }
            var annoucement = await dbContext.Announcement.FirstOrDefaultAsync(a => a.Id == Id);
            if(annoucement != null && annoucement.image != null){

            string mimeType;
            if(annoucement.ContentType == "image/jpeg"){
                mimeType =  "image/jpeg";
            }
            else{
                mimeType = "image/png";
            }

            return File(annoucement.image,mimeType);
        }
        return NotFound("Image Not Found");
    }

        


}