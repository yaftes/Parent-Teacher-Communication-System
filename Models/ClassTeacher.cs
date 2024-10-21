


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class ClassTeacher {

    public string? ClassId { get; set; } 
    public string? TeacherId { get; set;}   
    // Navigation
    public Class? Class {get;set;}
    public ApplicationUser? Teacher { get; set; }


}