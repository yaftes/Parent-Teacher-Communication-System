using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class StudentPerformanceReport {

    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString(); 
    
    public string? CreatedBy {get;set;}
    public DateTime? CreatedAt {get;set;}

    public string? ActiveParticipation {get;set;}
    public string? HavingFun {get;set;}
    public string? Formality {get;set;}
    public string? Meal {get;set;}
    
    public string? Parentfollowup {get;set;}
    public string? Handwriting {get;set;}
    public string? Readingskill {get;set;}
    public string? Materialhandling {get;set;}
    public string? TeacherComment {get;set;}
    public string? ParentComment {get;set;}

    // Foreign Key
    public string? StudentId { get; set;}
    // Navigation Properties
    public virtual ApplicationUser? ApplicationUser {get; set;}  


}