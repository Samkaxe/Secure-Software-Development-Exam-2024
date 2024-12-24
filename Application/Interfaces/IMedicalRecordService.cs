using Application.DTOs;

namespace Application.Interfaces;

public interface IMedicalRecordService
{
    Task<MedicalRecordDTO> GetByIdAsync(Guid id, byte[] encyrptionKey);
    Task<IEnumerable<MedicalRecordDTO>> GetAllByUserIdAsync(Guid userId, byte[] encryptionKey);
    Task<MedicalRecordDTO> AddAsync(CreateMedicalRecordDTO medicalRecordDto, byte[] encryptionKey);
    Task<MedicalRecordDTO> UpdateAsync(Guid id, UpdateMedicalRecordDTO medicalRecordDto, byte[] encryptionKey);
    Task DeleteAsync(Guid id, byte[] encryptionKey);
}