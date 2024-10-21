using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class CourseReport {
   [Key]
   public  string Id { get; set; }  = Guid.NewGuid().ToString();   

   public string? Lessonoftheday {get;set;}

   public bool? classWorkonExBook {get;set;}
   public int classWorkonTextBookPage {get;set;}
   public bool? HomeWorkonExBook {get;set;}
   public int HomeWorkonTextBookPage {get;set;} 
   
   public DateTime? CreatedAt {get;set;}
    
   // change from teacherId to CreatedBy
   public string? CreatedBy {get;set;}

   // Foreign keys
   public string? CourseId {get;set;}
   // Navigation Property
   public virtual Course? Course {get;set;}
   
}