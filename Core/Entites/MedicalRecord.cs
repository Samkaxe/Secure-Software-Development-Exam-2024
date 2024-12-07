namespace Core.Entites;

public class MedicalRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } 
    public User User { get; set; } 

    public string RecordData { get; set; } 
    public string EncryptionKey { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    public DateTime? UpdatedAt { get; set; } 
}