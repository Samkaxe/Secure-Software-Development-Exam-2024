using Core.Entites;
using Infrastructure.DataAccessInterfaces;
using Infrastructure.helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccessServices;

public class MedicalRecordRepository : IMedicalRecordRepository
{
    private readonly ApplicationDbContext _context;

    public MedicalRecordRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MedicalRecord> GetByIdAsync(Guid id)
    {
        return await _context.MedicalRecords
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<MedicalRecord>> GetAllByUserIdAsync(Guid userId)
    {
        return await _context.MedicalRecords
            .Where(m => m.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(MedicalRecord medicalRecord)
    {
        ValidateMedicalRecord(medicalRecord);
        await _context.MedicalRecords.AddAsync(medicalRecord);
    }

    public async Task UpdateAsync(MedicalRecord medicalRecord)
    {
        ValidateMedicalRecord(medicalRecord);
        _context.MedicalRecords.Update(medicalRecord);
    }

    public async Task DeleteAsync(MedicalRecord medicalRecord)
    {
        
        _context.MedicalRecords.Remove(medicalRecord);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private void ValidateMedicalRecord(MedicalRecord medicalRecord)
    {
        if (!ValidatorHelper.IsSqlInjectionSafe(medicalRecord.RecordData))
        {
            throw new ArgumentException("Invalid record data detected. Potential SQL injection.");
        }

        if (!ValidatorHelper.IsSqlInjectionSafe(medicalRecord.EncryptionKey))
        {
            throw new ArgumentException("Invalid encryption key detected. Potential SQL injection.");
        }
    }
}