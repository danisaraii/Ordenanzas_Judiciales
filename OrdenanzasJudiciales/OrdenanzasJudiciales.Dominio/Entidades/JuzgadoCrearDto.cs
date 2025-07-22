using System.ComponentModel.DataAnnotations;

namespace OrdenanzasJudiciales.Dominio.Entidades
{
    public class JuzgadoCrearDto
    {
        [Required(ErrorMessage = "El nombre del juzgado es obligatorio.")]
        [StringLength(200, ErrorMessage = "El nombre no debe superar los 200 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten letras sin tildes ni ñ.")]
        public string juzgadoNombre { get; set; }
        [Required(ErrorMessage = "El campo Alias es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre alias no debe superar los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten letras sin tildes ni ñ.")]
        public string nombreAlias { get; set; }
        [Required(ErrorMessage = "La direccion es obligatoria.")]
        [StringLength(200, ErrorMessage = "La direccion no debe superar los 200 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten letras sin tildes ni ñ.")]
        public string direccionJuzgado { get; set; }
        [Required(ErrorMessage = "El nombre de la ciudad es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no debe superar los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten letras sin tildes ni ñ.")]
        public string ciudadJuzgado { get; set; }
        [Required(ErrorMessage = "El nombre del Juez/Funcionario es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no debe superar los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten letras sin tildes ni ñ.")]
        public string funcionarioJuez { get; set; }
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Debe ingresar un correo válido.")]
        public string email { get; set; }
    }
}
