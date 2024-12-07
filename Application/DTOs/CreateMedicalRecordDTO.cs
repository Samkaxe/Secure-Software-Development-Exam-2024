namespace Application.DTOs;

public class CreateMedicalRecordDTO
{
    public Guid UserId { get; set; } 
    public string RecordData { get; set; } 
    public string EncryptionKey { get; set; } 
}