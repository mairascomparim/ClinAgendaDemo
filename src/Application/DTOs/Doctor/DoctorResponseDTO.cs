using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    public class DoctorResponseDTO
    {
         public int Total { get; set; }
        public List<DoctorListReturnDTO> Items { get; set; }
    }
}