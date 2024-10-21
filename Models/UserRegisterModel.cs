using System.ComponentModel.DataAnnotations;
public class UserRegisterModel {
  [Required]
  public string? FirstName {get;set;}
  [Required]
  public string? LastName {get;set;}
  [Required]
  public string? UserName {get;set;}
  [Required]
  public string? Class {get;set;}
  [Required]
  public string? ClassRollNumber {get;set;}
  [Required]
  public string? Kebele {get;set;}
  [Required]
  public string? Section {get;set;}
  [Required]
  public string? SubCity {get;set;}
  [Required]
  public string DateofBirth {get;set;} = string.Empty;

  [Required]
  public string? HomeAddress {get;set;}
  [Required]
  public string? FatherName {get;set;}
  [Required]
  public string? FatherEducationQualification {get;set;}
  [Required]
  public string? FatherOccupation {get;set;}
  [Required]
  public string? FatherPhoneNumber {get;set;}
  [Required]
  public string? MotherName {get;set;}
  [Required]
  public string? MotherEducationQualification {get;set;}
  [Required]
  public string? MotherOccupation {get;set;}
  [Required]
  public string? MotherPhoneNumber {get;set;}

  public List<Class>? classes {get;set;}


}