using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Appointment;
using ClinAgenda.src.Application.DTOs.Doctor;
using ClinAgenda.src.Application.DTOs.Patient;
using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Core.Interfaces;

namespace ClinAgenda.src.Application.UseCases
{
    public class AppointmentUseCase
    {
        private readonly IAppointmentRepository _appointmentRepository;
        public AppointmentUseCase(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }
        public async Task<AppointmentResponseDTO> GetAppointmentsAsync(string? patientName, string? doctorName, int? specialtyId, int itemsPerPage, int page)
        {
            var (total, rawData) = await _appointmentRepository.GetAppointmentsAsync(patientName, doctorName, specialtyId, itemsPerPage, page);
            var appointmentMap = new Dictionary<int, AppointmentListReturnDTO>();

            foreach (var item in rawData)
            {
                if (!appointmentMap.ContainsKey(item.Id))
                {
                    appointmentMap[item.Id] = new AppointmentListReturnDTO
                    {
                        Id = item.Id,
                        Patient = new PatientReturnAppointmentDTO
                        {
                            Name = item.PatientName,
                            documentNumber = item.PatientDocument
                        },
                        Doctor = new DoctorReturnAppointmentDTO
                        {
                            Name = item.DoctorName
                        },
                        Specialty = new SpecialtyDTO
                        {
                            Id = item.SpecialtyId,
                            Name = item.SpecialtyName
                        },
                        AppointmentDate = item.AppointmentDate,
                        Observation = item.Observation,
                    };
                }
            }

            return new AppointmentResponseDTO
            {
                Total = total,
                Items = appointmentMap.Values.ToList()
            };
        }

        public async Task<int> CreateAppointmentAsync(AppointmentDTO appointmentDTO)
        {
            var newAppointmentId = await _appointmentRepository.InsertAppointmentAsync(appointmentDTO);

            return newAppointmentId;
        }
        public async Task<AppointmentDTO?> GetAppointmentByIdAsync(int id)
        {
            return await _appointmentRepository.GetByIdAsync(id);
        }
        public async Task<bool> UpdateAppointmentAsync(int appointmentId, AppointmentDTO appointmentDTO)
        {

            var updatedPatient = new AppointmentInsertDTO
            {
                Id = appointmentId,
                PatientId = appointmentDTO.PatientId,
                DoctorId = appointmentDTO.DoctorId,
                SpecialtyId = appointmentDTO.SpecialtyId,
                AppointmentDate = appointmentDTO.AppointmentDate,
                Observation = appointmentDTO.Observation
            };

            var isUpdated = await _appointmentRepository.UpdateAsync(updatedPatient);

            return isUpdated;
        }
        public async Task<bool> DeleteAppointmentByIdAsync(int id)
        {
            var rowsAffected = await _appointmentRepository.DeleteAsync(id);

            return rowsAffected > 0;
        }

    }
}