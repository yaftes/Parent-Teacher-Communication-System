

using System.ComponentModel.DataAnnotations;

public class AnnouncementModel {
    [Required]
    public string? Title { get; set; }
    [Required]
    [StringLength(300)]
    public string? Description { get; set; }    
}