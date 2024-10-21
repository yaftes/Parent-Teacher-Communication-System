using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[Authorize]
public class CourseReportController : Controller {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly RoleManager<ApplicationRole> roleManager; 
    private readonly ApplicationDbContext dbContext;
    public CourseReportController(
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
        public async Task<IActionResult> Courses(){
            var courses = await dbContext.Course.ToListAsync();
            if(!courses.IsNullOrEmpty()){
                return View(courses);
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Reports(string CourseId,string? time){

            if(string.IsNullOrEmpty(CourseId)){
                return BadRequest("Please Select Course");
            }

            var coursereports = dbContext.CourseReport.Where(c => c.CourseId == CourseId).OrderBy( c => c.CreatedAt);
            ReportCourseDetail report = new ReportCourseDetail(){
                Id = CourseId,
            };
            DateTime today = DateTime.Now;

            if(!coursereports.IsNullOrEmpty()){

                if(time == "Today"){
                    coursereports = coursereports.Where(r => r.CreatedAt <= today).OrderBy( c => c.CreatedAt);
                }
                else if (time == "Week"){
                    coursereports = coursereports.Where(r => r.CreatedAt >= today.AddDays(-7)).OrderBy( c => c.CreatedAt);
                }
                else if (time == "Month"){
                    coursereports = coursereports.Where(r => r.CreatedAt >= today.AddDays(-30)).OrderBy( c => c.CreatedAt);
                }
                report.CourseReports = await coursereports.ToListAsync();

            }
            return View(report);
        }



        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public IActionResult Create(string CourseId){
            if(string.IsNullOrEmpty(CourseId)){
               return BadRequest("Please Select Course or invalid Id for course");
            }
            var coursereport = new CourseReportModel();
            coursereport.CourseId = CourseId;
            return View(coursereport);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> Create(string CourseId ,CourseReportModel Model){
            if(string.IsNullOrEmpty(CourseId)){
                return BadRequest("Invalid Input");
            }
            var currentUser = await userManager.GetUserAsync(User);
            if(ModelState.IsValid){
                var coursereport = new CourseReport(){
                     Lessonoftheday = Model.Lessonoftheday,

                     classWorkonExBook = Model.classWorkonExBook,
                     classWorkonTextBookPage = Model.classWorkonTextBookPage,

                     HomeWorkonExBook = Model.HomeWorkonExBook,
                     HomeWorkonTextBookPage = Model.HomeWorkonTextBookPage,

                     CreatedAt = DateTime.Now,
                     CreatedBy =  currentUser?.FirstName + " " +  currentUser?.LastName,
                     CourseId = CourseId,
                };

             await dbContext.CourseReport.AddAsync(coursereport);
             await dbContext.SaveChangesAsync();

             return RedirectToAction("Reports","CourseReport", new {CourseId});
            }
            return View(Model);
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public IActionResult Edit(string CourseReportId,string CourseId){
            if(string.IsNullOrEmpty(CourseId)){
                return BadRequest("Please select Course First");
            }    
            var coursereport = dbContext.CourseReport.FirstOrDefault(c => c.Id == CourseReportId);

            if(coursereport!=null){
                CourseReportModel model = new CourseReportModel(){
                    CourseId = CourseId,
                    CourseReportId = CourseReportId,
                    Lessonoftheday = coursereport.Lessonoftheday,
                    HomeWorkonExBook = coursereport.HomeWorkonExBook ?? false,
                    HomeWorkonTextBookPage = coursereport.HomeWorkonTextBookPage,
                    classWorkonExBook = coursereport.classWorkonExBook ?? false,
                    classWorkonTextBookPage = coursereport.classWorkonTextBookPage,
                };
                return View(model);
            }
            return NotFound("No Course Report Found with this Id");
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public IActionResult Edit(string CourseId,string CourseReportId,CourseReportModel Model){
            var coursereport = dbContext.CourseReport.FirstOrDefault(c => c.Id == CourseReportId);
            if(coursereport!=null){
                coursereport.Lessonoftheday = Model.Lessonoftheday;
                coursereport.classWorkonExBook = Model.classWorkonExBook;
                coursereport.classWorkonTextBookPage = Model.classWorkonTextBookPage;
                coursereport.HomeWorkonExBook = Model.HomeWorkonExBook;
                coursereport.HomeWorkonTextBookPage = Model.HomeWorkonTextBookPage;
            dbContext.CourseReport.Update(coursereport);
            dbContext.SaveChanges();
            }
             return RedirectToAction("Reports","CourseReport",new {CourseId});
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public IActionResult Delete(string CourseId,string CourseReportId){
            if(string.IsNullOrEmpty(CourseId) || string.IsNullOrEmpty(CourseReportId)){
                return BadRequest("No Course Found to Delete");
            }  
           var coursereport =  dbContext.CourseReport.FirstOrDefault(c => c.Id == CourseReportId);

           if(coursereport != null){
            dbContext.CourseReport.Remove(coursereport);
            dbContext.SaveChanges();
            TempData["DeletedReport"] = "Report Deleted";
           }
           return RedirectToAction("Reports","CourseReport",new {CourseId});
       
        }

    
        

        
}  