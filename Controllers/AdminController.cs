using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[Authorize(Roles = "Admin")]
public class AdminController : Controller {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly RoleManager<ApplicationRole> roleManager; 
    private readonly ApplicationDbContext dbContext;
    public AdminController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext dbContext){
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.dbContext = dbContext;

        }
        public async Task<IActionResult> DashBoard(){
            var dashboard = new DashboardReport();
            dashboard.Classes = await dbContext.Class.ToListAsync() ?? new List<Class>();
            dashboard.Courses = await dbContext.Course.ToListAsync() ?? new List<Course>();
            dashboard.Teachers = new List<ApplicationUser>();
            dashboard.Students = new List<ApplicationUser>();
            var users = await dbContext.Users.Include(u => u.ClassTeacher).ThenInclude(u => u.Class).ToListAsync() ?? new List<ApplicationUser>();

            foreach(var user in users){
                if(await userManager.IsInRoleAsync(user,"Teacher")){
                    dashboard?.Teachers.Add(user);
                }
                else if(await userManager.IsInRoleAsync(user,"Student")){
                    dashboard?.Students.Add(user);
                }
            }
            return View(dashboard);
        }


        [HttpGet]
        public IActionResult RegisterStudent(){
            var classes = dbContext.Class.ToList();
            var usermodel = new UserRegisterModel();
            usermodel.classes = classes ?? new List<Class>();
            return View(usermodel);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterStudent(UserRegisterModel Model,IFormFile? image){
            if(ModelState.IsValid){
                
                ApplicationUser user = new ApplicationUser(){
                    FirstName = Model.FirstName,
                    LastName = Model.LastName,
                    UserName = Model.UserName,
                    CreatedAt = DateTime.Now,
                    Email = string.Empty,
                    Class = Model.Class,
                    Section = Model.Section,
                    ClassRollNumber = Model.ClassRollNumber,
                    HomeAddress = Model.HomeAddress,
                    SubCity = Model.SubCity,
                    Kebele = Model.Kebele,
                    DateofBirth = DateTime.Parse(Model.DateofBirth),

                    FatherName = Model.FatherName,
                    FatherEducationQualification = Model.FatherEducationQualification,
                    FatherOccupation = Model.FatherOccupation,
                    FatherPhoneNumber = Model.FatherPhoneNumber,

                    MotherName = Model.MotherName,
                    MotherEducationQualification = Model.MotherEducationQualification,
                    MotherOccupation = Model.MotherOccupation,
                    MotherPhoneNumber = Model.MotherPhoneNumber,
                };

                if(image != null && image.Length > 0){
                    using(var ms = new  MemoryStream()){
                        await image.CopyToAsync(ms);
                        user.ProfilePicture = ms.ToArray();
                        user.ContentType = image.ContentType;
                    }
                }

                var password = Password.GeneratePassword();
                var result = await userManager.CreateAsync(user,password);
                if(result.Succeeded){
                    ViewBag.Password = password;  
                    ViewBag.Success = "New Student Registered";
                    await userManager.AddToRoleAsync(user,"Student");
                }
                else{
                    foreach(var error in result.Errors){
                        ModelState.AddModelError(string.Empty,error.Description);
                    }
                }
            }
            var classes = dbContext.Class.ToList() ?? new List<Class>();
            Model.classes = classes;
            return View(Model);
        }


    [HttpGet]
    public IActionResult RegisterTeacher(){
         var classes = dbContext.Class.ToList();
         var tr = new TeacherRegisterModel();
         tr.Classes = classes ?? new List<Class>();
        return View(tr);  
    }
    

    [HttpPost]
    public async Task<IActionResult> RegisterTeacher(TeacherRegisterModel Model,IFormFile? image) {

        if (ModelState.IsValid) {
            ApplicationUser User = new ApplicationUser() {
                FirstName = Model.FirstName,
                LastName = Model.LastName,
                Email = string.Empty,
                UserName = Model.UserName,
                HomeAddress = Model.HomeAddress,
                SubCity = Model.SubCity,
                Kebele = Model.Kebele,
                PhoneNumber = Model.PhoneNumber,
                DateofBirth = DateTime.Parse(Model.DateofBirth),
            };

            if(image != null && image.Length > 0){
                    using(var ms = new  MemoryStream()){
                        await image.CopyToAsync(ms);
                        User.ProfilePicture = ms.ToArray();
                        User.ContentType = image.ContentType;
                    }
                }
            var password = Password.GeneratePassword();
            var result = await userManager.CreateAsync(User,password);

            if (result.Succeeded) {
               if(Model.Class != null && Model.Class.Count() > 0){
                foreach(var classId in Model.Class){
                   var classteacher = new ClassTeacher();
                   classteacher.ClassId = classId;
                   classteacher.TeacherId = User.Id;
                   dbContext.ClassTeacher.Add(classteacher);
                }
                dbContext.SaveChanges();
               }
                await userManager.AddToRoleAsync(User,"Teacher");
                ViewBag.Password = password;
                ViewBag.Success = "New Teacher Registered";
            } 
            else {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }
        Model.Classes = dbContext.Class.ToList() ?? new List<Class>();
        return View(Model);
    }



        [HttpGet]
        public IActionResult AddCourse() {
            var courses = dbContext.Course.ToList();
            var ccd = new CourseClassDetail();
            ccd.Courses = courses ?? new List<Course>();
            return View(ccd);
        }

        [HttpPost]
        public async Task<IActionResult> AddCourse(CourseClassDetail Model) {
            if(ModelState.IsValid){
                var check = dbContext.Course.Any(c => c.Name == Model.CourseName);
                if(!check){
                var course = new Course(); 
                course.Name = Model.CourseName;
                var result = await dbContext.Course.AddAsync(course);
                await dbContext.SaveChangesAsync();
                ViewBag.Success = "New Course Added";
                }
                else{
                    ModelState.AddModelError("CourseName","Already Exists");
                }  
            }
            var courses = await dbContext.Course.ToListAsync();
            var ccd = new CourseClassDetail();
            ccd.Courses = courses ?? new List<Course>();
            return View(ccd);
        }

        [HttpPost]
        public IActionResult DeleteCourse(string CourseId){
            if(string.IsNullOrEmpty(CourseId)){
                return BadRequest("Bad");
            }
            var course = dbContext.Course.FirstOrDefault(c => c.Id == CourseId);    
            if(course != null){
                dbContext.Course.Remove(course);
                dbContext.SaveChanges();
                TempData["DeleteCourse"] = $"{course.Name} Course Deleted";
                
            }
            return RedirectToAction("AddCourse");
        }



        // Class
        [HttpGet]
        public IActionResult AddClass(){
            var classes = dbContext.Class.ToList();
            var ccd = new CourseClassDetail();
            ccd.Classes = classes ?? new List<Class>();
            return View(ccd);
        }


        [HttpPost]
        public async Task<IActionResult> AddClass(CourseClassDetail Model){
            if(ModelState.IsValid){
                var check = dbContext.Class.Any(c => c.Name == Model.className);
                if(!check){
                var clas= new Class(); 
                clas.Name = Model.className;
                await dbContext.Class.AddAsync(clas);
                await dbContext.SaveChangesAsync();
                ViewBag.Success = $"Class{clas.Name}  Added";
                }
                else{
                ModelState.AddModelError("className","Already Exists");
                }
        
            }
            var classes = dbContext.Class.ToList();
            var ccd = new CourseClassDetail();
            ccd.Classes = classes ?? new List<Class>();
            return View(ccd);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteClass(string ClassId){

            if(string.IsNullOrEmpty(ClassId)){
                return BadRequest("Bad Request");
            }
            var Class = dbContext.Class.FirstOrDefault(c => c.Id == ClassId);
            if(Class!=null){
                dbContext.Class.Remove(Class);
                TempData["DeleteClass"] = $"class {Class.Name}  Deleted";
                userManager.Users.Where(st => st.Class == Class.Name).ExecuteUpdate(st => st.SetProperty(s => s.Class, s => null));
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("AddClass");
        }


            [HttpPost]
            public async Task<IActionResult> ResetPassword(string UserName){
                if(!UserName.IsNullOrEmpty()){
                    var user = await userManager.FindByNameAsync(UserName);
                    if(user != null){
                        await userManager.RemovePasswordAsync(user);
                        var password = Password.GeneratePassword();
                        await userManager.AddPasswordAsync(user,password);
                        TempData["password"] = password;
                        if(await userManager.IsInRoleAsync(user,"Student")){
                            return RedirectToAction("EditStudentProfile","Admin",new {UserName});
                        }
                        return RedirectToAction("EditTeacherProfile","Admin",new {UserName});
                        
                    }
                    else{
                        ModelState.AddModelError(string.Empty,"No User Found");
                        return NotFound("User Found");
                    }
                }      
                return BadRequest("Bad Request");
            }



            [HttpGet]
            public async Task<IActionResult>  EditStudentProfile(string UserName){
                if(string.IsNullOrEmpty(UserName)){
                    return NotFound("User Not Found");
                }
                var user = await userManager.FindByNameAsync(UserName);
                UserRegisterModel model = new UserRegisterModel();
                if(user != null){
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Class = user.Class;
                model.Kebele = user.Kebele;
                model.SubCity = user.SubCity;
                model.Section = user.Section;
                model.ClassRollNumber = user.ClassRollNumber;
                model.UserName = user.UserName;
                model.DateofBirth = user.DateofBirth?.ToString("yyyy-MM-dd") ?? "2000-01-02";


                model.FatherPhoneNumber = user.FatherPhoneNumber;
                model.FatherName = user.FatherName;
                model.FatherEducationQualification = user.FatherEducationQualification;
                model.FatherOccupation = user.FatherOccupation;

                model.MotherName = user.MotherName;
                model.MotherPhoneNumber = user.MotherPhoneNumber;
                model.MotherOccupation = user.MotherOccupation;
                model.MotherEducationQualification = user.MotherEducationQualification;

                model.HomeAddress = user.HomeAddress;
                model.classes = dbContext.Class.ToList() ?? new List<Class>();
                }
                ViewBag.Password = TempData["password"] ?? string.Empty;
                return View(model);
            }

            [HttpPost]
            public async Task<IActionResult>  EditStudentProfile(UserRegisterModel model,string UserName){

                if (ModelState.IsValid){
                    if(string.IsNullOrEmpty(UserName)){
                        return NotFound("User Not Found");
                    }
                    var user = await userManager.FindByNameAsync(UserName);
                    if (user != null){
                    user.FirstName = model.FirstName ;
                    user.LastName = model.LastName;
                    user.Class = model.Class;
                    user.Kebele = model.Kebele;
                    user.SubCity = model.SubCity;
                    user.Section = model.Section;
                    user.ClassRollNumber = model.ClassRollNumber;
                    user.UserName = model.UserName ;
                    user.DateofBirth = DateTime.Parse(model.DateofBirth);


                    user.FatherPhoneNumber =  model.FatherPhoneNumber;
                    user.FatherName =  model.FatherName;
                    user.FatherEducationQualification = model.FatherEducationQualification;
                    user.FatherOccupation =  model.FatherOccupation;

                    user.MotherName = model.MotherName;
                    user.MotherPhoneNumber = model.MotherPhoneNumber;
                    user.MotherOccupation =  model.MotherOccupation;
                    user.MotherEducationQualification = model.MotherEducationQualification;


                    user.HomeAddress = model.HomeAddress;
                    
                    var result = await userManager.UpdateAsync(user);
                    if(result.Succeeded){
                        ViewBag.Success = "Student Info Updated";
                        return RedirectToAction("DashBoard");
                    }
                    else{
                        foreach(var error in result.Errors){
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

                }
                model.classes = dbContext.Class.ToList() ?? new List<Class>();
                return View(model);
            }

            [HttpGet]
            public async Task<IActionResult>  EditTeacherProfile(string UserName){
                
                if(string.IsNullOrEmpty(UserName)){
                    return BadRequest("Bad Request");
                }
                var user = await userManager.FindByNameAsync(UserName);
                if(user == null){
                    return NotFound("Teacher Not Found");
                }

                TeacherRegisterModel model = new TeacherRegisterModel();
                model.FirstName = user.FirstName;
                model.LastName = user.LastName; 
                model.UserName = user.UserName;
                model.PhoneNumber = user.PhoneNumber;
                model.Kebele = user.Kebele;
                model.SubCity = user.SubCity;
                model.HomeAddress = user.HomeAddress;
                model.DateofBirth = user.DateofBirth.ToString() ?? string.Empty;
                model.Classes = dbContext.Class.ToList() ?? new List<Class>();
                return View(model);

            }

            [HttpPost]
            public async Task<IActionResult>  EditTeacherProfile(TeacherRegisterModel Model,string UserName){
                if(ModelState.IsValid){
                var user = await userManager.FindByNameAsync(UserName);
                if(user != null){
                    user.FirstName = Model.FirstName;
                    user.LastName = Model.LastName;
                    user.Email = string.Empty;
                    user.UserName = Model.UserName;
                    user.HomeAddress = Model.HomeAddress;
                    user.SubCity = Model.SubCity;
                    user.Kebele = Model.Kebele;
                    user.PhoneNumber = Model.PhoneNumber;
                    user.DateofBirth = DateTime.Parse(Model.DateofBirth);

                    var result = await userManager.UpdateAsync(user);
                    if(result.Succeeded){
                        var classteacher = dbContext.ClassTeacher.Where(ct => ct.TeacherId == user.Id).ExecuteDelete();
                        foreach(var clas in Model.Class){
                            var ct = new ClassTeacher();
                            ct.ClassId = clas;
                            ct.TeacherId = user.Id;
                            dbContext.ClassTeacher.Add(ct);
                        }
                        dbContext.SaveChanges();
                        ViewBag.Success = "Teacher Info Updated";
                        return RedirectToAction("DashBoard");
                    }
                }
            }
            Model.Classes = dbContext.Class.ToList() ?? new List<Class>();
            return View(Model);
                     
            }


            [HttpPost]
            public async Task<IActionResult> DeleteUser(string UserName){

                if(!string.IsNullOrEmpty(UserName)){
                    var user = await userManager.FindByNameAsync(UserName);

                    if(user != null){
                        await userManager.DeleteAsync(user);
                        dbContext.ChatMessage.Where(m => m.SenderId == user.Id).ExecuteDelete();
                        TempData["UserDeleted"] = $"{user.FirstName + " " + user.LastName} deleted";
                        return RedirectToAction("DashBoard");
                    }

                    else{
                        return NotFound("No User Found");
                    }
                } 

                return BadRequest("User Not Found");   
             }
        

         


}