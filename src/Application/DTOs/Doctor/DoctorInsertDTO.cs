using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    public class DoctorInsertDTO
    {
        [Required(ErrorMessage = "O Nome do Doutor é obrigatório", AllowEmptyStrings = false)]
        public required string Name { get; set; }
        [Required(ErrorMessage = "A Especialidade do Doutor é obrigatório", AllowEmptyStrings = false)]
        public required List<int> Specialty { get; set; }
        [Required(ErrorMessage = "O Status do Doutor é obrigatório", AllowEmptyStrings = false)]
        public int StatusId { get; set; }
    }
}