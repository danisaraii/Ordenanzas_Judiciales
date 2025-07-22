using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdenanzasJudiciales.Dominio.Entidades
{
    public class FuncionarioCrearDto
    {
        [Required]
        public int JuzgadoId { get; set; }
        [Required(ErrorMessage = "El nombre del Juez/Funcionario es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no debe superar los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten letras sin tildes ni ñ.")]
        public string funcionarioNombre { get; set; }
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Debe ingresar un correo válido.")]
        public string emailFuncionario { get; set; }
    }
}
