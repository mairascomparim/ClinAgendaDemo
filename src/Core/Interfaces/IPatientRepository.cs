using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Patient;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface IPatientRepository
    {
        Task<(int total, IEnumerable<PatientListDTO> patient)> GetPatientsAsync(string? name, string? documentNumber, int? statusId, int itemsPerPage, int page);
        Task<int> InsertPatientAsync(PatientInsertDTO patient);
        Task<PatientListDTO?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(PatientDTO patient);
        Task<int> DeleteByPatientIdAsync(int patientId);
        Task<IEnumerable<PatientListDTO>> AutoComplete(string name);
    }
}