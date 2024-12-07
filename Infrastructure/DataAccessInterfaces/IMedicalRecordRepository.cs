using Core.Entites;

namespace Infrastructure.DataAccessInterfaces;

public interface IMedicalRecordRepository
{
    Task<MedicalRecord> GetByIdAsync(Guid id);
    Task<IEnumerable<MedicalRecord>> GetAllByUserIdAsync(Guid userId);
    Task AddAsync(MedicalRecord medicalRecord);
    Task UpdateAsync(MedicalRecord medicalRecord);
    Task DeleteAsync(MedicalRecord medicalRecord);
    Task SaveChangesAsync();
}