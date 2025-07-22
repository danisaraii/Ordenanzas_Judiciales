using System.ComponentModel.DataAnnotations;

namespace OrdenanzasJudiciales.Dominio.Entidades
{
    public class CuentasEntidadCrearDto
    {
        [Required(ErrorMessage = "El nombre de la Entidad Bancaria es obligatorio.")]
        [StringLength(200, ErrorMessage = "El nombre no debe superar los 200 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten letras sin tildes ni ñ.")]
        public string nombreEntidad { get; set; }
    }
}