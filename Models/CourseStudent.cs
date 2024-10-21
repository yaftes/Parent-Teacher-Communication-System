using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class CourseStudent {
    [Key]
    public string? Id { get; set; } = Guid.NewGuid().ToString();    
    [ForeignKey("Course")]
    public string? CourseId {get; set;}
    [ForeignKey("ApplicationUser")]
    public string? StudentId { get; set;} 
    // Navigation Properties
    public virtual ApplicationUser? ApplicationUser { get; set; }
    public virtual Course? Course { get; set; } 
}