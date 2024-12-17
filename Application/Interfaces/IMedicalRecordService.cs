using Application.DTOs;

namespace Application.Interfaces;

public interface IMedicalRecordService
{
    Task<MedicalRecordDTO> GetByIdAsync(Guid id);
    Task<IEnumerable<MedicalRecordDTO>> GetAllByUserIdAsync(Guid userId);
    Task<MedicalRecordDTO> AddAsync(CreateMedicalRecordDTO medicalRecordDto);
    Task<MedicalRecordDTO> UpdateAsync(Guid id, UpdateMedicalRecordDTO medicalRecordDto);
    Task DeleteAsync(Guid id);
}