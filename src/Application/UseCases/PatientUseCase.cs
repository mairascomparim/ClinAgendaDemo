using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Patient;
using ClinAgenda.src.Application.DTOs.Status;
using ClinAgenda.src.Core.Interfaces;

namespace ClinAgenda.src.Application.UseCases
{
    public class PatientUseCase
    {
        private readonly IPatientRepository _patientRepository;
        public PatientUseCase(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }
        public async Task<PatientResponseDTO> GetPatientsAsync(string? name, string? documentNumber, int? statusId, int itemsPerPage, int page)
        {
            var (total, rawData) = await _patientRepository.GetPatientsAsync(name, documentNumber, statusId, itemsPerPage, page);

            var patients = rawData
                .Select(p => new PatientListReturnDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    PhoneNumber = p.PhoneNumber,
                    DocumentNumber = p.DocumentNumber,
                    BirthDate = p.BirthDate,
                    Status = new StatusDTO
                    {
                        Id = p.StatusId,
                        Name = p.StatusName
                    }
                })
                .ToList();

            return new PatientResponseDTO
            {
                Total = total,
                Items = patients
            };
        }
        public async Task<int> CreatePatientAsync(PatientInsertDTO patientDTO)
        {
            var newPatientId = await _patientRepository.InsertPatientAsync(patientDTO);
            return newPatientId;
        }
        public async Task<PatientDTO?> GetPatientByIdAsync(int id)
        {
            return await _patientRepository.GetByIdAsync(id);
        }
        public async Task<bool> UpdatePatientAsync(int patientId, PatientInsertDTO patientDTO)
        {
            var existingPatient = await _patientRepository.GetByIdAsync(patientId) ?? throw new KeyNotFoundException("Paciente não encontrado.");

            existingPatient.Name = patientDTO.Name;
            existingPatient.PhoneNumber = patientDTO.PhoneNumber;
            existingPatient.DocumentNumber = patientDTO.DocumentNumber;
            existingPatient.StatusId = patientDTO.StatusId;
            existingPatient.BirthDate = patientDTO.BirthDate;

            var isUpdated = await _patientRepository.UpdateAsync(existingPatient);

            return isUpdated;
        }
        public async Task<object?> AutoComplete(string name)
        {
            var rawData = await _patientRepository.AutoComplete(name);

            var patients = rawData
                          .Select(p => new PatientListReturnDTO
                          {
                              Id = p.Id,
                              Name = p.Name,
                              PhoneNumber = p.PhoneNumber,
                              DocumentNumber = p.DocumentNumber,
                              BirthDate = p.BirthDate,
                              Status = new StatusDTO
                              {
                                  Id = p.StatusId,
                                  Name = p.StatusName
                              }
                          })
                          .ToList();

            return patients;
        }
        public async Task<bool> DeletPatientByIdAsync(int id)
        {
            var rowsAffected = await _patientRepository.DeleteByPatientIdAsync(id);
            return rowsAffected > 0;
        }
    }
}