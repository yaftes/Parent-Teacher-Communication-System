using System.ComponentModel.DataAnnotations;
public class Announcement {
    [Key]
    public string Id {get; set;} = Guid.NewGuid().ToString();  
    public string? Title {get; set;}
    public string? Description {get; set;}
    public DateTime? CreatedAt {get; set;}
    public string? CreatedBy {get; set;}
    public string? ContentType {get;set;}
    public byte[]? image {get;set;}

   
}