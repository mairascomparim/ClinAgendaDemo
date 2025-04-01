using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Patient;
using ClinAgenda.src.Application.UseCases;
using ClinAgendaAPI.StatusUseCase;
using Microsoft.AspNetCore.Mvc;

namespace ClinAgenda.src.WebAPI.Controllers
{
    [ApiController]
    [Route("api/patient")]
    public class PatientController : ControllerBase
    {
        private readonly PatientUseCase _patientUseCase;
        private readonly StatusUseCase _statusUseCase;
        private readonly AppointmentUseCase _appointmentUseCase;

        public PatientController(PatientUseCase patientService, StatusUseCase statusUseCase, AppointmentUseCase appointmentUseCase)
        {
            _patientUseCase = patientService;
            _statusUseCase = statusUseCase;
            _appointmentUseCase = appointmentUseCase;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetPatientsAsync([FromQuery] string? name, [FromQuery] string? documentNumber, [FromQuery] int? statusId, [FromQuery] int itemsPerPage = 10, [FromQuery] int page = 1)
        {
            try
            {
                var result = await _patientUseCase.GetPatientsAsync(name, documentNumber, statusId, itemsPerPage, page);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }
        [HttpPost("insert")]
        public async Task<IActionResult> CreatePatientAsync([FromBody] PatientInsertDTO patient)
        {
            try
            {
                var hasStatus = await _statusUseCase.GetStatusByIdAsync(patient.StatusId);
                if (hasStatus == null)
                    return BadRequest($"O status ID {patient.StatusId} não existe");

                var createdPatientId = await _patientUseCase.CreatePatientAsync(patient);

                if (!(createdPatientId > 0))
                {
                    return StatusCode(500, "Erro ao criar a Paciente.");
                }
                var infosPatientCreated = await _patientUseCase.GetPatientByIdAsync(createdPatientId);

                return Ok(infosPatientCreated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"={ex.Message}");
            }
        }
        [HttpGet("listById/{id}")]
        public async Task<IActionResult> GetPatientByIdAsync(int id)
        {
            try
            {
                var patient = await _patientUseCase.GetPatientByIdAsync(id);
                if (patient == null) return NotFound();
                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePatientAsync(int id, [FromBody] PatientInsertDTO patient)
        {
            try
            {
                if (patient == null) return BadRequest();

                var hasStatus = await _statusUseCase.GetStatusByIdAsync(patient.StatusId);
                if (hasStatus == null)
                    return BadRequest($"O status ID {patient.StatusId} não existe");

                bool updated = await _patientUseCase.UpdatePatientAsync(id, patient);
                if (!updated) return NotFound("Paciente não encontrado.");

                var infosPatientUpdate = await _patientUseCase.GetPatientByIdAsync(id);
                return Ok(infosPatientUpdate);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }
        [HttpGet("autocomplete")]
        public async Task<IActionResult> AutoComplete([FromQuery] string? name)
        {
            try
            {
                var result = await _patientUseCase.AutoComplete(name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePatientAsync(int id)
        {
            try
            {
                var patientInfo = await _patientUseCase.GetPatientByIdAsync(id);

                var appointment = await _appointmentUseCase.GetAppointmentsAsync(patientName: patientInfo.Name, null, null, 1, 1);

                if (appointment.Total > 0)
                    return NotFound($"Erro ao deletar, Paciente com agendamento marcado");

                var success = await _patientUseCase.DeletPatientByIdAsync(id);

                if (!success)
                {
                    return NotFound($"Paciente com ID {id} não encontrado.");
                }

                return Ok("Paciente deletado com sucesso");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}