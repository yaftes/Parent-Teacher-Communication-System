using System.ComponentModel.DataAnnotations;
public class Course {
    [Key]
    public string Id {get;set;} = Guid.NewGuid().ToString();
    
    [Required]
    public string? Name {get;set;}

    // Navigation Properties
    public virtual List<CourseReport>? CourseReports {get;set;}
    public virtual List<ApplicationUser>? ApplicationUsers {get;set;}

}