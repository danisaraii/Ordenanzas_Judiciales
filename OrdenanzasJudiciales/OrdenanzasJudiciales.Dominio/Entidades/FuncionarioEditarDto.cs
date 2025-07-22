using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdenanzasJudiciales.Dominio.Entidades
{
    public class FuncionarioEditarDto
    {
        public int JuzgadoId { get; set; }
        public int FuncionarioId { get; set; }
        [Required(ErrorMessage = "El nombre del juzgado es obligatorio.")]
        [StringLength(200, ErrorMessage = "El nombre no debe superar los 200 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten letras sin tildes ni ñ.")]
        public string juzgadoNombre { get; set; }
        [Required(ErrorMessage = "El campo Alias es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre alias no debe superar los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten letras sin tildes ni ñ.")]
        public string Alias { get; set; }
        [Required(ErrorMessage = "El nombre del Juez/Funcionario es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no debe superar los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten letras sin tildes ni ñ.")]
        public string NombreFuncionario { get; set; }
        [Required(ErrorMessage = "La direccion es obligatoria.")]
        [StringLength(200, ErrorMessage = "La direccion no debe superar los 200 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten letras sin tildes ni ñ.")]
        public string DireccionJuzgado { get; set; }
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Debe ingresar un correo válido.")]
        public string Correo { get; set; }
    }
}
