using Application.DTOs;

namespace Application.Interfaces;

public interface IMedicalRecordService
{
    Task<MedicalRecordDTO> GetByIdAsync(Guid id);
    Task<IEnumerable<MedicalRecordDTO>> GetAllByUserIdAsync(Guid userId);
    Task AddAsync(CreateMedicalRecordDTO medicalRecordDto);
    Task UpdateAsync(Guid id, UpdateMedicalRecordDTO medicalRecordDto);
    Task DeleteAsync(Guid id);
}