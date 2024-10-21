

public class CourseClassDetail {

    public string? className { get; set; }  

    public string? CourseName {get;set;}
    
    public List<Course>? Courses { get; set; } = new List<Course>();
    public List<Class> Classes { get; set; } = new List<Class>();

}