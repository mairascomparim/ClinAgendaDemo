using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;
using ClinAgenda.src.Application.UseCases;
using ClinAgendaAPI.StatusUseCase;
using Microsoft.AspNetCore.Mvc;

namespace ClinAgenda.src.WebAPI.Controllers
{
    [ApiController]
    [Route("api/doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly DoctorUseCase _doctorUseCase;
        private readonly StatusUseCase _statusUseCase;
        private readonly SpecialtyUseCase _specialtyUseCase;
        private readonly AppointmentUseCase _appointmentUseCase;

        public DoctorController(DoctorUseCase doctorUseCase, StatusUseCase statusUseCase, SpecialtyUseCase specialtyUseCase, AppointmentUseCase appointmentUseCase)
        {
            _doctorUseCase = doctorUseCase;
            _statusUseCase = statusUseCase;
            _specialtyUseCase = specialtyUseCase;
            _appointmentUseCase = appointmentUseCase;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetDoctors([FromQuery] string? name, [FromQuery] int? specialtyId, [FromQuery] int? statusId, [FromQuery] int itemsPerPage = 10, [FromQuery] int page = 1)
        {
            try
            {
                var result = await _doctorUseCase.GetDoctorsAsync(name, specialtyId, statusId, itemsPerPage, page);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
        [HttpPost("insert")]
        public async Task<IActionResult> CreateDoctorAsync([FromBody] DoctorInsertDTO doctor)
        {
            try
            {
                var hasStatus = await _statusUseCase.GetStatusByIdAsync(doctor.StatusId);
                if (hasStatus == null)
                    return BadRequest($"O status com ID {doctor.StatusId} não existe.");

                var specialties = await _specialtyUseCase.GetSpecialtiesByIds(doctor.Specialty);

                var notFoundSpecialties = doctor.Specialty.Except(specialties.Select(s => s.Id)).ToList();

                if (notFoundSpecialties.Any())
                {
                    return BadRequest(notFoundSpecialties.Count > 1 ? $"As especialidades com os IDs {string.Join(", ", notFoundSpecialties)} não existem." : $"A especialidade com o ID {notFoundSpecialties.First().ToString()} não existe.");
                }

                var createdDoctorId = await _doctorUseCase.CreateDoctorAsync(doctor);

                var ifosDoctor = await _doctorUseCase.GetDoctorByIdAsync(createdDoctorId);

                return Ok(ifosDoctor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateDoctorAsync(int id, [FromBody] DoctorInsertDTO doctor)
        {
            if (doctor == null) return BadRequest();

            var hasStatus = await _statusUseCase.GetStatusByIdAsync(doctor.StatusId);
            if (hasStatus == null)
                return BadRequest($"O status com ID {doctor.StatusId} não existe.");

            var specialties = await _specialtyUseCase.GetSpecialtiesByIds(doctor.Specialty);

            var notFoundSpecialties = doctor.Specialty.Except(specialties.Select(s => s.Id)).ToList();

            if (notFoundSpecialties.Any())
            {
                return BadRequest(notFoundSpecialties.Count > 1 ? $"As especialidades com os IDs {string.Join(", ", notFoundSpecialties)} não existem." : $"A especialidade com o ID {notFoundSpecialties.First().ToString()} não existe.");
            }

            bool updated = await _doctorUseCase.UpdateDoctorAsync(id, doctor);

            if (!updated) return NotFound("Doutor não encontrado.");

            var infosDoctorUpdate = await _doctorUseCase.GetDoctorByIdAsync(id);
            return Ok(infosDoctorUpdate);

        }
        [HttpGet("listById/{id}")]
        public async Task<IActionResult> GetDoctorByIdAsync(int id)
        {
            var doctor = await _doctorUseCase.GetDoctorByIdAsync(id);
            if (doctor == null) return NotFound();
            return Ok(doctor);
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDoctorAsync(int id)
        {
            try
            {
                var doctorInfo = await _doctorUseCase.GetDoctorByIdAsync(id);

                var appointment = await _appointmentUseCase.GetAppointmentsAsync(null, doctorName: doctorInfo.Name, null, 1, 1);

                if (appointment.Total > 0)
                    return NotFound($"Erro ao deletar, Doutor com agendamento marcado");

                var success = await _doctorUseCase.DeleteDoctorByIdAsync(id);

                if (!success)
                {
                    return NotFound($"Doutor com ID {id} não encontrado.");
                }

                return Ok("Doutor deletado com sucesso");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}