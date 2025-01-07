using System.Security.Cryptography;
using System.Text;
using Application.DTOs;
using Application.Interfaces;
using Core.Entites;
using Infrastructure.DataAccessInterfaces;

namespace Application.Services;

public class MedicalRecordService(IMedicalRecordRepository medicalRecordRepository, EncryptionHelper encryptionHelper)
    : IMedicalRecordService
{
    public async Task<MedicalRecordDTO> GetByIdAsync(Guid id, byte[] encryptionKey)
    {
        try
        {
            var record = await medicalRecordRepository.GetByIdAsync(id);
            if (record == null) return null;

            byte[] decryptedDataBytes = encryptionHelper.DecryptWithSpecificKey(record.RecordData, encryptionKey);
            string decryptedData = Encoding.UTF8.GetString(decryptedDataBytes);

            return new MedicalRecordDTO
            {
                Id = record.Id,
                UserId = record.UserId,
                RecordData = decryptedData,
                CreatedAt = record.CreatedAt,
                UpdatedAt = record.UpdatedAt
            };
        }
        catch (CryptographicException ex)
        {
            Console.WriteLine(ex.Message);
            return null; // Or throw a custom exception if appropriate
        }
    }

    public async Task<IEnumerable<MedicalRecordDTO>> GetAllByUserIdAsync(Guid userId, byte[] encryptionKey)
    {
        try
        {
            var records = await medicalRecordRepository.GetAllByUserIdAsync(userId);

            return records.Select(record =>
            {
                try
                {
                    byte[] decryptedDataBytes = encryptionHelper.DecryptWithSpecificKey(record.RecordData, encryptionKey);
                    string decryptedData = Encoding.UTF8.GetString(decryptedDataBytes);
                    return new MedicalRecordDTO
                    {
                        Id = record.Id,
                        UserId = record.UserId,
                        RecordData = decryptedData,
                        CreatedAt = record.CreatedAt,
                        UpdatedAt = record.UpdatedAt
                    };
                }
                catch (CryptographicException ex)
                {
                    Console.WriteLine(ex.Message);
                    return null; // Or handle the error as needed
                }
            }).Where(dto => dto != null); // Filter out null DTOs
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null; // Or handle the error as needed
        }
    }

    public async Task<MedicalRecordDTO> AddAsync(CreateMedicalRecordDTO medicalRecordDto, byte[] encryptionKey)
    {
        var encryptedData = encryptionHelper.EncryptWithSpecificKey(
            Encoding.UTF8.GetBytes(medicalRecordDto.RecordData), encryptionKey    
        );

        var newRecord = new MedicalRecord
        {
            UserId = medicalRecordDto.UserId,
            RecordData = encryptedData,
            CreatedAt = DateTime.UtcNow
        };

        var savedRecord = await medicalRecordRepository.AddAsync(newRecord);
        await medicalRecordRepository.SaveChangesAsync();
        
        return new MedicalRecordDTO
        {
            Id = savedRecord.Id,
            UserId = savedRecord.UserId,
            RecordData = medicalRecordDto.RecordData,
            CreatedAt = savedRecord.CreatedAt,
            UpdatedAt = savedRecord.UpdatedAt
        };
    }

    public async Task<MedicalRecordDTO> UpdateAsync(Guid id, UpdateMedicalRecordDTO medicalRecordDto, byte[] encryptionKey)
    {
        try
        {
            var record = await medicalRecordRepository.GetByIdAsync(id);
            if (record == null) throw new KeyNotFoundException("Medical record not found.");

            record.RecordData = encryptionHelper.EncryptWithSpecificKey(
                Encoding.UTF8.GetBytes(medicalRecordDto.RecordData),
                encryptionKey
            );
            record.UpdatedAt = DateTime.UtcNow;

            var savedRecord = await medicalRecordRepository.UpdateAsync(record);
            await medicalRecordRepository.SaveChangesAsync();

            byte[] decryptedDataBytes = encryptionHelper.DecryptWithSpecificKey(savedRecord.RecordData, encryptionKey);
            string decryptedRecordData = Encoding.UTF8.GetString(decryptedDataBytes);

            return new MedicalRecordDTO
            {
                Id = savedRecord.Id,
                UserId = savedRecord.UserId,
                RecordData = decryptedRecordData,
                CreatedAt = savedRecord.CreatedAt,
                UpdatedAt = savedRecord.UpdatedAt
            };
        }
        catch (CryptographicException ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

   
    public async Task DeleteAsync(Guid id, byte[] encryptionKey)
    {
        var record = await medicalRecordRepository.GetByIdAsync(id);
        if (record == null)
        {
            throw new KeyNotFoundException("Medical record not found.");
        }

        
        try
        {
            encryptionHelper.DecryptWithSpecificKey(record.RecordData, encryptionKey);
        }
        catch (CryptographicException)
        {
            throw new UnauthorizedAccessException("Invalid encryption key for the record.");
        }
        
        await medicalRecordRepository.DeleteAsync(record);
        await medicalRecordRepository.SaveChangesAsync();
    }
}