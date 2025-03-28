using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Patient
{
    public class PatientReturnAppointmentDTO
    {
        public required string Name { get; set; }
        public required string documentNumber { get; set; }
    }
}