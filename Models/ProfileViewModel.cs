


using System.ComponentModel.DataAnnotations;

public class  ProfileViewModel {
    [Required]
    public  string? OldPassword { get; set; }    
    [Required]
    public string? NewPassword { get; set;} 
    [Required]
    [Compare("NewPassword")]
    public string? ConfirmPassword { get; set;}

    public ApplicationUser? User {get;set;}
}