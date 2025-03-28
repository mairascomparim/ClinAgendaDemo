using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Appointment
{
    public class AppointmentDTO
    {
        [Required(ErrorMessage = "O Id do Paciente é obrigatório", AllowEmptyStrings = false)]
        public int PatientId { get; set; }
        [Required(ErrorMessage = "O Id do Doutor é obrigatório", AllowEmptyStrings = false)]
        public int DoctorId { get; set; }
        [Required(ErrorMessage = "O Id da especialidade é obrigatório", AllowEmptyStrings = false)]
        public int SpecialtyId { get; set; }
        [Required(ErrorMessage = "A data é obrigatória", AllowEmptyStrings = false)]
        public required string AppointmentDate { get; set; }
        public required string Observation { get; set; }
    }
}