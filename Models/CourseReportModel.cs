
using System.ComponentModel.DataAnnotations;

public class CourseReportModel {

    [Required]
    public string? Lessonoftheday {get;set;}
    public string? CourseId { get; set; }
    public string? CourseReportId { get; set; } 
    [Required]
    public bool classWorkonExBook {get;set;} = false;
    [Required]
    public int classWorkonTextBookPage {get;set;}
    [Required]
    public bool HomeWorkonExBook {get;set;} = false;
    [Required]
    public int HomeWorkonTextBookPage {get;set;} 
    public string? CreatedBy {get;set;}
    public string? CourseName { get;set;}

}