﻿using System.Text;
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

    public async Task<MedicalRecord> AddAsync(MedicalRecord medicalRecord)
    {
        ValidateMedicalRecord(medicalRecord);
        await _context.MedicalRecords.AddAsync(medicalRecord);
        return medicalRecord;
    }

    public async Task<MedicalRecord> UpdateAsync(MedicalRecord medicalRecord)
    {
        ValidateMedicalRecord(medicalRecord);
        _context.MedicalRecords.Update(medicalRecord);
        return medicalRecord;
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
        if (!ValidatorHelper.IsSqlInjectionSafe(Encoding.UTF8.GetString(medicalRecord.RecordData)))
        {
            throw new ArgumentException("Invalid record data detected. Potential SQL injection.");
        }
    }
}