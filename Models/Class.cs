

public class Class {
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    // nav
    public List<ClassTeacher>? ClassTeacher {get;set;}

}