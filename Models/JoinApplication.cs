using System.ComponentModel.DataAnnotations;

public class JoinApplication
{
    public int Id { get; set; }
    [Required, StringLength(100)]
    public string Name { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }
    [Required, StringLength(200)]
    public string Address { get; set; }
    [StringLength(200)]
    public string Address2 { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required, Phone]
    public string Phone { get; set; }
    [StringLength(100)]
    public string Occupation { get; set; }
    [Required]
    public string ArtworkFilePath { get; set; }
    public DateTime SubmissionDate { get; set; } = DateTime.Now;
}