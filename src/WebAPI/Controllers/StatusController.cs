using ClinAgenda.src.Application.DTOs.Status;
using ClinAgenda.src.Application.UseCases;
using ClinAgendaAPI;
using ClinAgendaAPI.StatusUseCase;
using Microsoft.AspNetCore.Mvc;

namespace ClinAgenda.src.WebAPI.Controllers
{
    [ApiController]
    [Route("api/status")]
    public class StatusController : ControllerBase
    {
        private readonly StatusUseCase _statusUseCase;
        private readonly PatientUseCase _patientUseCase;
        private readonly DoctorUseCase _doctorUseCase;

        public StatusController(StatusUseCase service, PatientUseCase patientUseCase, DoctorUseCase doctorUseCase)
        {
            _statusUseCase = service;
            _doctorUseCase = doctorUseCase;
            _patientUseCase = patientUseCase;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetStatusAsync([FromQuery] int itemsPerPage = 10, [FromQuery] int page = 1)
        {
            try
            {
                var specialty = await _statusUseCase.GetStatusAsync(itemsPerPage, page);
                return Ok(specialty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }

        [HttpGet("listById/{id}")]
        public async Task<IActionResult> GetStatusByIdAsync(int id)
        {
            try
            {
                var specialty = await _statusUseCase.GetStatusByIdAsync(id);

                if (specialty == null)
                {
                    return NotFound($"Status com ID {id} não encontrado.");
                }

                return Ok(specialty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }

        [HttpPost("insert")]
        public async Task<IActionResult> CreateStatusAsync([FromBody] StatusInsertDTO status)
        {
            try
            {
                if (status == null || string.IsNullOrWhiteSpace(status.Name))
                {
                    return BadRequest("Os dados do status são inválidos.");
                }

                var createdStatus = await _statusUseCase.CreateStatusAsync(status);
                var infosStatusCreated = await _statusUseCase.GetStatusByIdAsync(createdStatus);

                return Ok(infosStatusCreated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStatusAsync(int id)
        {
            var hasPatients = await _patientUseCase.GetPatientsAsync(null, null, statusId: id, 1, 1);
            var hasDoctor = await _doctorUseCase.GetDoctorsAsync(null, null, statusId: id, 1, 1);

            if (hasPatients.Total > 0 || hasDoctor.Total > 0)
                return StatusCode(500,$"O status está associado a um ou mais pacientes ou médicos.");

            var success = await _statusUseCase.DeleteStatusByIdAsync(id);
            
            if (!success)
            {
                return NotFound($"Status com ID {id} não encontrada.");
            }
            return Ok();
        }

    }

}