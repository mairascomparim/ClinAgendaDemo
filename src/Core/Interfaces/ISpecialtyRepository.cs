using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Specialty;

namespace ClinAgenda.src.Core.Interfaces
{
    public interface ISpecialtyRepository
    {
        Task<(int total, IEnumerable<SpecialtyDTO> specialtys)> GetAllAsync(string? name, int? itemsPerPage, int? page);
        Task<int> InsertSpecialtyAsync(SpecialtyInsertDTO specialtyInsertDTO);
        Task<SpecialtyDTO> GetByIdAsync(int id);
        Task<int> DeleteSpecialtyAsync(int id);
        Task<IEnumerable<SpecialtyDTO>> GetSpecialtiesByIds(List<int> specialtiesId);
    }
}