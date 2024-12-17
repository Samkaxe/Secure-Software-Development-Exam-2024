using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CreateMedicalRecordDTO
{
    [Required(ErrorMessage = "UserId is required.")]
    public Guid UserId { get; set; } 

    [Required(ErrorMessage = "RecordData is required.")]
    [StringLength(1000, ErrorMessage = "RecordData cannot exceed 1000 characters.")]
    public string RecordData { get; set; } 

    [Required(ErrorMessage = "EncryptionKey is required.")]
    [StringLength(256, MinimumLength = 32, ErrorMessage = "EncryptionKey must be between 32 and 256 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9!@#$%^&*()_+=\-{}\[\]:;""'<>.,?/|]{32,256}$", 
        ErrorMessage = "EncryptionKey must consist of alphanumeric characters and special characters.")]
    public string EncryptionKey { get; set; } 
}