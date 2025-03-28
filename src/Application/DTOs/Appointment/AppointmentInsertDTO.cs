using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Appointment
{
    public class AppointmentInsertDTO
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int SpecialtyId { get; set; }
        public required string AppointmentDate { get; set; }
        public required string Observation { get; set; }
    }
}