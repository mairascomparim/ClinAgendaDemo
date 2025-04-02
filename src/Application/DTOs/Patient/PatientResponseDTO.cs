using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Patient
{
    public class PatientResponseDTO
    {
        public int Total { get; set; }
        public List<PatientListReturnDTO> Items { get; set; }
    }
}