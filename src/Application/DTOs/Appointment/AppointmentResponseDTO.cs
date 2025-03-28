using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Appointment
{
    public class AppointmentResponseDTO
    {
        public int Total { get; set; }
        public List<AppointmentListReturnDTO> Items { get; set; }
    }
}