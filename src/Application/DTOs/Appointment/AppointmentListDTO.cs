using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Appointment
{
    public class AppointmentListDTO
    {
        public int Id { get; set; }
        public required string PatientName { get; set; }
        public required string PatientDocument { get; set; }
        public required string DoctorName { get; set; }
        public required int SpecialtyId { get; set; }
        public required string SpecialtyName { get; set; }
        public required string AppointmentDate { get; set; }
        public int ScheduleDuration { get; set; }
        public required string Observation { get; set; }

    }
}