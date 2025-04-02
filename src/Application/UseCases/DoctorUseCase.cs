using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;
using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Application.DTOs.Status;
using ClinAgenda.src.Core.Interfaces;

namespace ClinAgenda.src.Application.UseCases
{
    public class DoctorUseCase
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDoctorSpecialtyRepository _doctorSpecialtyRepository;
        private readonly ISpecialtyRepository _specialtyRepository;
        public DoctorUseCase(IDoctorRepository doctorRepository, IDoctorSpecialtyRepository doctorspecialtyRepository, ISpecialtyRepository specialtyRepository)
        {
            _doctorRepository = doctorRepository;
            _doctorSpecialtyRepository = doctorspecialtyRepository;
            _specialtyRepository = specialtyRepository;
        }

        public async Task<DoctorResponseDTO> GetDoctorsAsync(string? name, int? specialtyId, int? statusId, int itemsPerPage, int page)
        {
            int offset = (page - 1) * itemsPerPage;

            var rawData = await _doctorRepository.GetDoctorsAsync(name, specialtyId, statusId, offset, itemsPerPage);

            if (!rawData.doctors.Any())
                return new DoctorResponseDTO { Total = 0, Items = new List<DoctorListReturnDTO>() };

            var doctorIds = rawData.doctors.Select(d => d.Id).ToArray();
            var specialties = (await _doctorRepository.GetDoctorSpecialtiesAsync(doctorIds)).ToList();

            var result = rawData.doctors.Select(d => new DoctorListReturnDTO
            {
                Id = d.Id,
                Name = d.Name,
                Status = new StatusDTO
                {
                    Id = d.StatusId,
                    Name = d.StatusName
                },
                Specialty = specialties.Where(s => s.DoctorId == d.Id)
                    .Select(s => new SpecialtyDTO
                    {
                        Id = s.SpecialtyId,
                        Name = s.SpecialtyName,
                        ScheduleDuration = s.ScheduleDuration
                    }
                    ).ToList()
            });

            return new DoctorResponseDTO
            {
                Total = rawData.total,
                Items = result.ToList()
            };
        }
        public async Task<int> CreateDoctorAsync(DoctorInsertDTO doctorDto)
        {
            var newDoctorId = await _doctorRepository.InsertDoctorAsync(doctorDto);

            var doctor_specialities = new DoctorSpecialtyDTO
            {
                DoctorId = newDoctorId,
                SpecialtyId = doctorDto.Specialty
            };

            await _doctorSpecialtyRepository.InsertAsync(doctor_specialities);

            return newDoctorId;
        }
        public async Task<DoctorListReturnDTO> GetDoctorByIdAsync(int id)
        {
            var rawData = await _doctorRepository.GetByIdAsync(id);

            List<DoctorListReturnDTO> infoDoctor = new List<DoctorListReturnDTO>();

            foreach (var group in rawData.GroupBy(item => item.Id))
            {
                DoctorListReturnDTO doctor = new DoctorListReturnDTO
                {
                    Id = group.Key,
                    Name = group.First().Name,
                    Specialty = group.Select(s => new SpecialtyDTO
                    {
                        Id = s.SpecialtyId,
                        Name = s.SpecialtyName
                    }).ToList(),
                    Status = new StatusDTO
                    {
                        Id = group.First().StatusId,
                        Name = group.First().StatusName
                    }
                };

                infoDoctor.Add(doctor);
            }

            return infoDoctor.First();
        }

        public async Task<bool> UpdateDoctorAsync(int id, DoctorInsertDTO doctorDto)
        {
            var doctorToUpdate = new DoctorDTO
            {
                Id = id,
                Name = doctorDto.Name,
                StatusId = doctorDto.StatusId
            };

            await _doctorRepository.UpdateAsync(doctorToUpdate);

            await _doctorSpecialtyRepository.DeleteByDoctorIdAsync(id);

            var doctorSpecialties = new DoctorSpecialtyDTO
            {
                DoctorId = id,
                SpecialtyId = doctorDto.Specialty
            };

            await _doctorSpecialtyRepository.InsertAsync(doctorSpecialties);

            return true;
        }
        public async Task<bool> DeleteDoctorByIdAsync(int id)
        {
            var rowsAffected = await _doctorRepository.DeleteByDoctorIdAsync(id);
            return rowsAffected > 0;
        }
    }

}