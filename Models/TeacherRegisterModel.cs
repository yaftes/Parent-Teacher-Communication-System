using System.ComponentModel.DataAnnotations;
public class TeacherRegisterModel {

  [Required]
  public string? FirstName {get;set;}
  [Required]
  public string? LastName {get;set;}
  [Required]
  public string? UserName {get;set;}
  [Required]
  public string? Kebele {get;set;}
  [Required]
  public string? SubCity {get;set;}

  [Required]
  public string? PhoneNumber {get;set;}

  [Required]
  public string DateofBirth {get;set;} = string.Empty;

  [Required]
  public string? HomeAddress {get;set;}



  [Required]
  public List<string> Class {get;set;} = new List<string>();


  public List<Class>? Classes {get;set;} = new List<Class>(); 



}