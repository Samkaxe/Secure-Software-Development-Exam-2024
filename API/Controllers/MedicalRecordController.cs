using System.Security.Claims;
using System.Text;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/medicalRecord")]
public class MedicalRecordController(IMedicalRecordService medicalRecordService, IUserEncyrptionKeyService userEncyrptionKeyService) : Controller
{
        // Endpoint: Get all medical records for a specific patient
        [Authorize(Policy = "DoctorPolicy")]
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetMedicalRecords(Guid patientId)
        {
            Byte[] uek = await userEncyrptionKeyService.GetUserEncryptionKeyAsync(patientId);
            var records = await medicalRecordService.GetAllByUserIdAsync(patientId, uek);
            if (!records.Any())
                return NotFound("No records found for this patient.");

            return Ok(records);
        }

        // Endpoint: Patient can view their own medical records
        [Authorize(Policy = "PatientPolicy")]
        [HttpGet("my-records")]
        public async Task<IActionResult> GetMyMedicalRecords()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("user id");
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("User ID not found in token.");
            }

            var userEncryptionKey = HttpContext.Session.Get("uek");
            Console.WriteLine("user encryption key");
            Console.WriteLine(BitConverter.ToString(userEncryptionKey));
            if (userEncryptionKey == null)
            {
                return Unauthorized("User session is invalid");
            }

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return BadRequest("Invalid user ID format in token.");
            }

            var records = await medicalRecordService.GetAllByUserIdAsync(userId, userEncryptionKey);

            if (!records.Any())
            {
                return NotFound("No records found for your account.");
            }


            return Ok(records);
        }

        // Endpoint: Create a new medical record (Doctor only)
        [Authorize(Policy = "DoctorPolicy")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateMedicalRecord([FromBody] CreateMedicalRecordDTO record)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Byte[] uek = await userEncyrptionKeyService.GetUserEncryptionKeyAsync(record.UserId);
            var createdRecord = await medicalRecordService.AddAsync(record, uek);

            return CreatedAtAction(nameof(GetMedicalRecords), new { patientId = record.UserId }, createdRecord);
        }

        // Endpoint: Edit an existing medical record (Doctor only)
        [Authorize(Policy = "DoctorPolicy")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateMedicalRecord(Guid id, [FromBody] UpdateMedicalRecordDTO updatedRecord)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Byte[] uek = await userEncyrptionKeyService.GetUserEncryptionKeyAsync(id);
            var result = await medicalRecordService.UpdateAsync(id, updatedRecord, uek);

            /*if (!result)
                return NotFound("Medical record not found.");*/

            return NoContent();
        }
        
        
        [Authorize(Policy = "PatientPolicy")] // Only the patient can delete their own record
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMedicalRecordvia(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized("Invalid or missing user ID in token.");
            }
            
            var encryptionKey = HttpContext.Session.Get("uek");
            if (encryptionKey == null)
            {
                return Unauthorized("User session is invalid or encryption key missing.");
            }
            
            var record = await medicalRecordService.GetByIdAsync(id, encryptionKey);
            if (record == null)
            {
                return NotFound("Medical record not found.");
            }
            
            if (record.UserId != userId)
            {
                return Forbid("You are not authorized to delete this record.");
            }
            
            await medicalRecordService.DeleteAsync(id, encryptionKey);
            return NoContent();
        }

        // Emergency Access: View all patient records (Emergency responder only)
        [Authorize(Policy = "EmergencyResponderPolicy")]
        [HttpGet("emergency-access/{patientId}")]
        public async Task<IActionResult> EmergencyAccess(Guid patientId)
        {
            Byte[] uek = await userEncyrptionKeyService.GetUserEncryptionKeyAsync(patientId);
            var records = await medicalRecordService.GetAllByUserIdAsync(patientId, uek);

            if (!records.Any())
                return NotFound("No records found for this patient.");

            // Add logging for emergency access
            Console.WriteLine($"Emergency Access granted to user {User.Identity?.Name} for patient {patientId}");

            return Ok(records);
        }
}