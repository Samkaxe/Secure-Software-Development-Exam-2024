using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/medicalRecord")]
public class MedicalRecordController(IMedicalRecordService medicalRecordService) : Controller
{
        // Endpoint: Get all medical records for a specific patient
        [Authorize(Policy = "DoctorPolicy")]
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetMedicalRecords(Guid patientId)
        {
            var records = await medicalRecordService.GetAllByUserIdAsync(patientId);
            if (!records.Any())
                return NotFound("No records found for this patient.");

            return Ok(records);
        }

        // Endpoint: Patient can view their own medical records
        [Authorize(Policy = "PatientPolicy")]
        [HttpGet("my-records")]
        public async Task<IActionResult> GetMyMedicalRecords()
        {
            var userId = Guid.Parse(User.FindFirst("sub")?.Value);
            var records = await medicalRecordService.GetAllByUserIdAsync(userId);

            if (!records.Any())
                return NotFound("No records found for your account.");

            return Ok(records);
        }

        // Endpoint: Create a new medical record (Doctor only)
        [Authorize(Policy = "DoctorPolicy")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateMedicalRecord([FromBody] CreateMedicalRecordDTO record)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdRecord = await medicalRecordService.AddAsync(record);

            return CreatedAtAction(nameof(GetMedicalRecords), new { patientId = record.UserId }, createdRecord);
        }

        // Endpoint: Edit an existing medical record (Doctor only)
        [Authorize(Policy = "DoctorPolicy")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateMedicalRecord(Guid id, [FromBody] UpdateMedicalRecordDTO updatedRecord)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await medicalRecordService.UpdateAsync(id, updatedRecord);

            /*if (!result)
                return NotFound("Medical record not found.");*/

            return NoContent();
        }

        // Endpoint: Delete a medical record (Doctor only)
        [Authorize(Policy = "DoctorPolicy")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMedicalRecord(Guid id)
        {
            await medicalRecordService.DeleteAsync(id);
            return NoContent();
        }

        // Emergency Access: View all patient records (Emergency responder only)
        [Authorize(Policy = "EmergencyResponderPolicy")]
        [HttpGet("emergency-access/{patientId}")]
        public async Task<IActionResult> EmergencyAccess(Guid patientId)
        {
            var records = await medicalRecordService.GetAllByUserIdAsync(patientId);

            if (!records.Any())
                return NotFound("No records found for this patient.");

            // Add logging for emergency access
            Console.WriteLine($"Emergency Access granted to user {User.Identity?.Name} for patient {patientId}");

            return Ok(records);
        }
}