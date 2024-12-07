namespace Application.DTOs;

public class MedicalRecordDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string RecordData { get; set; } 
    public DateTime CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; } 
}