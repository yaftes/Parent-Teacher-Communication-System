

using System.ComponentModel.DataAnnotations;

public class UserChangePasswordModel : UserLoginModel {
    [Required]
    [Compare("Password")]
    public virtual required string  ConfirmPassword {get;set;}

}

