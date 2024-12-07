namespace Application.DTOs;

public class UpdateMedicalRecordDTO
{
    public string RecordData { get; set; } 
    public string EncryptionKey { get; set; } 
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; 
}