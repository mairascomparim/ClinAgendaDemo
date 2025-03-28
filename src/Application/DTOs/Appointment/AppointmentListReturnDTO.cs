using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;
using ClinAgenda.src.Application.DTOs.Patient;
using ClinAgenda.src.Application.DTOs.Specialty;

namespace ClinAgenda.src.Application.DTOs.Appointment
{
    public class AppointmentListReturnDTO
    {
         public int Id { get; set; }
        public required PatientReturnAppointmentDTO Patient { get; set; }
        public required DoctorReturnAppointmentDTO Doctor { get; set; }
        public required SpecialtyDTO Specialty { get; set; }
        public required string AppointmentDate { get; set; }
    }
}