using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[Authorize]
public class StudentPerformanceController : Controller {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly RoleManager<ApplicationRole> roleManager; 
    private readonly ApplicationDbContext dbContext;
    public StudentPerformanceController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext dbContext){
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.dbContext = dbContext;
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<IActionResult> Students(){
            var users = await userManager.Users.ToListAsync();
            List<ApplicationUser> students = [];
            if(users is null || users.Count == 0){
                return View();
            }
            foreach(var user in users){
                if(await userManager.IsInRoleAsync(user,"Student")){
                    students.Add(user); 
                }
            }
            return View(students);
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<IActionResult> Reports(string UserName,string time){
            if (string.IsNullOrEmpty(UserName)) {
                return BadRequest("No User Found with this id or you don't have Authorization");
            }
            
            var user = await userManager.FindByNameAsync(UserName);

            if(user == null){
                return NotFound("No user Find with this UserName");
            }

            IQueryable<StudentPerformanceReport>? reports = dbContext.StudentPerformanceReport
                .Where(r => r.StudentId == user.Id);

            DateTime today = DateTime.Now;
            if(time == "Today"){
                reports = reports.Where(r => r.CreatedAt <= today);
            }
            else if( time == "Week"){
                reports = reports.Where(r => r.CreatedAt >= today.AddDays(-7));
            }
            else if (time == "Month"){
                reports = reports.Where(r => r.CreatedAt >= today.AddDays(-30));
            }
   

            ReportPerformanceDetail report = new ReportPerformanceDetail{
                UserName = UserName,
                PerformanceReport = await reports.ToListAsync() ?? new List<StudentPerformanceReport>() 
            };

            return View(report);
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public IActionResult Create(string UserName){

            if (string.IsNullOrEmpty(UserName)){
                return BadRequest("No User Selected");
            }
            var model = new StudentPerformanceReportModel();
            model.UserName = UserName;
            return View(model);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> Create(string UserName,StudentPerformanceReportModel Model){
            var currentuser = await userManager.GetUserAsync(User);
            if(string.IsNullOrEmpty(UserName)){
                return BadRequest("You Can't Create Without Selecting User");
            }
            var user = await userManager.FindByNameAsync(UserName);
            if(user != null){
                var report = new StudentPerformanceReport(){

                    CreatedBy =  currentuser?.FirstName + " " + currentuser?.LastName, 
                    CreatedAt = DateTime.Now,
                    StudentId = user.Id,
        
                    ActiveParticipation = Model.ActiveParticipation,
                    HavingFun = Model.HavingFun,
                    Formality = Model.Formality,    
                    Meal = Model.Meal,
             
                    Parentfollowup = Model.Parentfollowup,
                    Handwriting = Model.Handwriting,
                    Readingskill = Model.Readingskill,
                    Materialhandling = Model.Materialhandling,
                    TeacherComment = Model.TeacherComment,

                };
                dbContext.StudentPerformanceReport.Add(report);    
                dbContext.SaveChanges();
            }
            else{
                return NotFound("No User Found with This Id");
            }

            return RedirectToAction("Reports","StudentPerformance",new {UserName});
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<IActionResult> Edit(string Id,string UserName){

            if(Id == null){
                return BadRequest("No Report Found");
            }

            var report = await dbContext.StudentPerformanceReport.FirstOrDefaultAsync(r => r.Id == Id);
            var model = new StudentPerformanceReportModel();
            model.UserName = UserName;
            model.Parentfollowup = report?.Parentfollowup;
            model.Handwriting = report?.Handwriting;
            model.Readingskill = report?.Readingskill;
            model.Materialhandling = report?.Materialhandling;
            model.ActiveParticipation = report?.ActiveParticipation;
            model.HavingFun = report?.HavingFun;
            model.Formality = report?.Formality;
            model.Meal = report?.Meal;
            model.TeacherComment = report?.TeacherComment;

            return View(model);
        }
 
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> Edit(string UserName,string Id,StudentPerformanceReportModel Model){
            var report = await dbContext.StudentPerformanceReport.FirstOrDefaultAsync(r => r.Id == Id);

            if(string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Id)){
                return BadRequest("No User Selected");
            }

            if(ModelState.IsValid){
                if(report != null){
                report.ActiveParticipation = Model.ActiveParticipation;
                report.HavingFun = Model.HavingFun;
                report.Formality = Model.Formality;
                report.Meal = Model.Meal;
                report.Parentfollowup = Model.Parentfollowup;
                report.Handwriting = Model.Handwriting; 
                report.Readingskill = Model.Readingskill;
                report.Materialhandling = Model.Materialhandling;
                dbContext.StudentPerformanceReport.Update(report);
                await dbContext.SaveChangesAsync();
                }
            
            }
            return RedirectToAction("Reports","StudentPerformance",new{UserName});

        } 
        
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> Delete(string Id,string UserName){
            if(string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Id)){
                return BadRequest("No User Selected");
            }
            var report = await dbContext.StudentPerformanceReport.FirstOrDefaultAsync(r => r.Id == Id);
            if(report != null){
                dbContext.StudentPerformanceReport.Remove(report);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Reports","StudentPerformance",new { UserName });
        }
        

            [Authorize(Roles = "Student")]
            [HttpGet]
            public async Task<IActionResult> StudentReport(string? time) {

                var currentuser = await userManager.GetUserAsync(User);
                if(currentuser == null){
                    return NotFound("No logged In");
                }
                var reportdetails = new ReportPerformanceDetail();
                DateTime today = DateTime.Now;
                var reportsQuery = dbContext.StudentPerformanceReport.Where(r => r.StudentId == currentuser.Id);

                if(!reportsQuery.IsNullOrEmpty()){

                if (time == "Today")
                {
                    reportsQuery = reportsQuery.Where(r => r.CreatedAt <= today);
                }
                else if (time == "Week")
                {
                    reportsQuery = reportsQuery.Where(r => r.CreatedAt >= today.AddDays(-7));
                }
                else if (time == "Month")
                {
                    reportsQuery = reportsQuery.Where(r => r.CreatedAt >= today.AddDays(-30));
                }

                reportdetails.PerformanceReport = await reportsQuery.ToListAsync();

            }
             return View(reportdetails);
         }
        
         [Authorize(Roles = "Student")]
         [HttpPost]
         public async Task<IActionResult> ParentComment(string PerformanceId,string? Comment){

            if(string.IsNullOrEmpty(PerformanceId)){
                return BadRequest("Bad");
            }

            if(Comment is not null){
                var performancereport = dbContext.StudentPerformanceReport.FirstOrDefault(pr => pr.Id == PerformanceId);
                if (performancereport != null){
                    performancereport.ParentComment = Comment;
                    dbContext.StudentPerformanceReport.Update(performancereport);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("StudentReport");
                }
            }

            return NotFound("Not Found");

         }
           

} 