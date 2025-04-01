using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface IDoctorRepository
    {
        Task<(int total, IEnumerable<DoctorListDTO> doctors)> GetDoctorsAsync(string? name, int? specialtyId, int? statusId, int offset, int itemsPerPage); Task<IEnumerable<SpecialtyDoctorDTO>> GetDoctorSpecialtiesAsync(int[] doctorIds);
        Task<int> InsertDoctorAsync(DoctorInsertDTO doctor);
        Task<IEnumerable<DoctorListDTO>> GetByIdAsync(int id);
        Task<bool> UpdateAsync(DoctorDTO doctor);
        Task<int> DeleteByDoctorIdAsync(int id);
    }
}