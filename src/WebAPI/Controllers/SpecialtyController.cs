using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace ClinAgenda.src.WebAPI.Controllers
{

    [ApiController]
    [Route("api/specialty")]
    public class SpecialtyController : ControllerBase
    {
        private readonly SpecialtyUseCase _specialtyUsecase;
        private readonly DoctorUseCase _doctorUseCase;
        public SpecialtyController(SpecialtyUseCase service, DoctorUseCase doctorUseCase)
        {
            _specialtyUsecase = service;
            _doctorUseCase = doctorUseCase;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetSpecialtyAsync([FromQuery] string? name, [FromQuery] int itemsPerPage = 10, [FromQuery] int page = 1)
        {
            try
            {
                var specialty = await _specialtyUsecase.GetSpecialtyAsync(name, itemsPerPage, page);
                return Ok(specialty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }
        [HttpPost("insert")]
        public async Task<IActionResult> CreateSpecialtyAsync([FromBody] SpecialtyInsertDTO specialty)
        {
            try
            {
                if (specialty == null)
                {
                    return BadRequest("Dados inválidos para criação de especialidade.");
                }

                var createdSpecialtyId = await _specialtyUsecase.CreateSpecialtyAsync(specialty);

                if (!(createdSpecialtyId > 0))
                {
                    return StatusCode(500, "Erro ao criar a especialidade.");
                }

                var infosSpecialtyCreated = await _specialtyUsecase.GetSpecialtyByIdAsync(createdSpecialtyId);
                return Ok(infosSpecialtyCreated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }
        [HttpGet("listById/{id}")]
        public async Task<IActionResult> GetSpecialtyByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("ID inválido.");
                }

                var specialty = await _specialtyUsecase.GetSpecialtyByIdAsync(id);
                if (specialty == null)
                {
                    return NotFound($"Especialidade com ID {id} não encontrada.");
                }

                return Ok(specialty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSpecialtyAsync(int id)
        {
            try
            {
                var hasDoctor = await _doctorUseCase.GetDoctorsAsync(null, specialtyId: id, null, 1, 1);

                if (hasDoctor.Total > 0)
                    return StatusCode(500, $"A especialidade está associado a um ou mais médicos.");

                var success = await _specialtyUsecase.DeleteSpecialtyByIdAsync(id);

                if (!success)
                {
                    return NotFound($"Especialidade com ID {id} não encontrada.");
                }

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}