using Core.Entites;

namespace Infrastructure.DataAccessInterfaces;

public interface IMedicalRecordRepository
{
    Task<MedicalRecord> GetByIdAsync(Guid id);
    Task<IEnumerable<MedicalRecord>> GetAllByUserIdAsync(Guid userId);
    Task<MedicalRecord> AddAsync(MedicalRecord medicalRecord);
    Task<MedicalRecord> UpdateAsync(MedicalRecord medicalRecord);
    Task DeleteAsync(MedicalRecord medicalRecord);
    Task SaveChangesAsync();
}