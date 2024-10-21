
using System.ComponentModel.DataAnnotations;

public class UserLoginModel {
    [Required]
    public required string UserName { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
    public bool RememberMe {get;set;} = false;
}