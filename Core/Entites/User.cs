namespace Core.Entites;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; } 
    public string PasswordSalt { get; set; } 
    public Role Role { get; set; } 
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
}