using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
public class ApplicationUser : IdentityUser {

  public string? FirstName {get;set;}

  public string? LastName {get;set;}

  public string? Class {get;set;}

  public string? ClassRollNumber {get;set;}

  public string? Kebele {get;set;}

  public string? Section {get;set;}

  public string? SubCity {get;set;}

  public DateTime? DateofBirth {get;set;}
  public DateTime? CreatedAt {get;set;}
  public string? HomeAddress {get;set;}
  public string? FatherName {get;set;}
  public string? FatherEducationQualification {get;set;}
  public string? FatherOccupation {get;set;}

  public string? FatherPhoneNumber {get;set;}

  public string? MotherName {get;set;}
  public bool Status {get;set;}

  public string? MotherEducationQualification {get;set;}

  public string? MotherOccupation {get;set;}

  public string? MotherPhoneNumber {get;set;}
  public byte[]? ProfilePicture {get;set;}

  public string? ContentType {get;set;}
  // navigation propertiess
  public virtual List<Course>? Courses {get;set;}
  public virtual List<StudentPerformanceReport>? StudentPerformanceReport {get;set;}
  public virtual IEnumerable<ClassTeacher>? ClassTeacher {get;set;}
}