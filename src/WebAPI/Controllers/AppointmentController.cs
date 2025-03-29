using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Appointment;
using ClinAgenda.src.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace ClinAgenda.src.WebAPI.Controllers
{
    [ApiController]
    [Route("api/appointment")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentUseCase _appointmentUseCase;
        private readonly DoctorUseCase _doctorUseCase;
        private readonly PatientUseCase _patientUseCase;
        private readonly SpecialtyUseCase _specialtyUseCase;

        public AppointmentController(AppointmentUseCase service, DoctorUseCase doctorUseCase, PatientUseCase patientUseCase, SpecialtyUseCase specialtyUseCase)
        {
            _appointmentUseCase = service;
            _doctorUseCase = doctorUseCase;
            _patientUseCase = patientUseCase;
            _specialtyUseCase = specialtyUseCase;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetAppointmentsAsync([FromQuery] string? patientName, [FromQuery] string? doctorName, [FromQuery] int? specialtyId, [FromQuery] int itemsPerPage = 10, [FromQuery] int page = 1)
        {
            try
            {
                var result = await _appointmentUseCase.GetAppointmentsAsync(patientName, doctorName, specialtyId, itemsPerPage, page);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
        [HttpPost("insert")]
        public async Task<IActionResult> CreateAppointmentAsync([FromBody] AppointmentDTO appointment)
        {
            try
            {
                var hasDoctor = await _doctorUseCase.GetDoctorByIdAsync(appointment.DoctorId);
                if (hasDoctor == null)
                    return BadRequest($"O Doctor com ID {appointment.DoctorId} não existe.");

                var hasPatient = await _patientUseCase.GetPatientByIdAsync(appointment.PatientId);
                if (hasPatient == null)
                    return BadRequest($"O Paciente com ID {appointment.PatientId} não existe.");

                var specialties = await _specialtyUseCase.GetSpecialtyByIdAsync(appointment.SpecialtyId);


                if (specialties == null)
                {
                    return BadRequest($"A especialidade com o ID {appointment.SpecialtyId} não existe.");
                }


                var createdAppointment = await _appointmentUseCase.CreateAppointmentAsync(appointment);

                var infosAppointmentCreated = await _appointmentUseCase.GetAppointmentByIdAsync(createdAppointment);
                return Ok(infosAppointmentCreated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
        [HttpGet("listById/{id}")]
        public async Task<IActionResult> GetAppointmentByIdAsync(int id)
        {
            try
            {
                var appointment = await _appointmentUseCase.GetAppointmentByIdAsync(id);
                if (appointment == null) return NotFound();
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAppointmentAsync(int id, [FromBody] AppointmentDTO appointment)
        {
            try
            {
                if (appointment == null) return BadRequest();

                 var hasDoctor = await _doctorUseCase.GetDoctorByIdAsync(appointment.DoctorId);
                if (hasDoctor == null)
                    return BadRequest($"O Doctor com ID {appointment.DoctorId} não existe.");

                var hasPatient = await _patientUseCase.GetPatientByIdAsync(appointment.PatientId);
                if (hasPatient == null)
                    return BadRequest($"O Paciente com ID {appointment.PatientId} não existe.");

                var specialties = await _specialtyUseCase.GetSpecialtyByIdAsync(appointment.SpecialtyId);


                if (specialties == null)
                {
                    return BadRequest($"A especialidade com o ID {appointment.SpecialtyId} não existe.");
                }

                bool updated = await _appointmentUseCase.UpdateAppointmentAsync(id, appointment);
                if (!updated) return NotFound("Paciente não encontrado.");

                var infosDoctorUpdate = await _appointmentUseCase.GetAppointmentByIdAsync(id);
                return Ok(infosDoctorUpdate);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }

        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAppointmentAsync(int id)
        {
            try
            {
                var success = await _appointmentUseCase.DeleteAppointmentByIdAsync(id);

                if (!success)
                {
                    return NotFound($"Agendamento com ID {id} não encontrado.");
                }

                return Ok("Agendamento deletado com sucesso");
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }
    }
}