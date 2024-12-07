using Application.DTOs;
using Application.Interfaces;
using Core.Entites;
using Infrastructure.DataAccessInterfaces;

namespace Application.Services;

public class MedicalRecordService : IMedicalRecordService
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly EncryptionHelper _encryptionHelper;

    public MedicalRecordService(IMedicalRecordRepository medicalRecordRepository, EncryptionHelper encryptionHelper)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _encryptionHelper = encryptionHelper;
    }

    public async Task<MedicalRecordDTO> GetByIdAsync(Guid id)
    {
        var record = await _medicalRecordRepository.GetByIdAsync(id);
        if (record == null) return null;

        return new MedicalRecordDTO
        {
            Id = record.Id,
            UserId = record.UserId,
            RecordData = _encryptionHelper.Decrypt(record.RecordData),
            CreatedAt = record.CreatedAt,
            UpdatedAt = record.UpdatedAt
        };
    }

    public async Task<IEnumerable<MedicalRecordDTO>> GetAllByUserIdAsync(Guid userId)
    {
        var records = await _medicalRecordRepository.GetAllByUserIdAsync(userId);

        return records.Select(record => new MedicalRecordDTO
        {
            Id = record.Id,
            UserId = record.UserId,
            RecordData = _encryptionHelper.Decrypt(record.RecordData),
            CreatedAt = record.CreatedAt,
            UpdatedAt = record.UpdatedAt
        });
    }

    public async Task AddAsync(CreateMedicalRecordDTO medicalRecordDto)
    {
        var encryptedData = _encryptionHelper.Encrypt(medicalRecordDto.RecordData);

        var newRecord = new MedicalRecord
        {
            UserId = medicalRecordDto.UserId,
            RecordData = encryptedData,
            EncryptionKey = medicalRecordDto.EncryptionKey,
            CreatedAt = DateTime.UtcNow
        };

        await _medicalRecordRepository.AddAsync(newRecord);
        await _medicalRecordRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, UpdateMedicalRecordDTO medicalRecordDto)
    {
        var record = await _medicalRecordRepository.GetByIdAsync(id);
        if (record == null) throw new KeyNotFoundException("Medical record not found.");

        record.RecordData = _encryptionHelper.Encrypt(medicalRecordDto.RecordData);
        record.EncryptionKey = medicalRecordDto.EncryptionKey;
        record.UpdatedAt = DateTime.UtcNow;

        await _medicalRecordRepository.UpdateAsync(record);
        await _medicalRecordRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var record = await _medicalRecordRepository.GetByIdAsync(id);
        if (record == null) throw new KeyNotFoundException("i love single moms :) .");

        await _medicalRecordRepository.DeleteAsync(record);
        await _medicalRecordRepository.SaveChangesAsync();
    }
}