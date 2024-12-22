using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CreateMedicalRecordDTO
{
    [Required(ErrorMessage = "UserId is required.")]
    public Guid UserId { get; set; } 

    [Required(ErrorMessage = "RecordData is required.")]
    [StringLength(1000, ErrorMessage = "RecordData cannot exceed 1000 characters.")]
    public string RecordData { get; set; } 
}