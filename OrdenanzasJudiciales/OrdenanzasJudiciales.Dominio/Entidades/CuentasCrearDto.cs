using System.ComponentModel.DataAnnotations;

namespace OrdenanzasJudiciales.Dominio.Entidades
{
    public class CuentasCrearDto
    {
        [Required(ErrorMessage = "El nombre del acreedor es obligatorio.")]
        [StringLength(200, ErrorMessage = "El nombre no debe superar los 200 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten letras sin tildes ni ñ.")]
        public string nombreAcreedor { get; set; }
        [Required(ErrorMessage = "La cédula o RUC es obligatoria.")]
        [RegularExpression(@"^(?:\d{10}|\d{10}001)$", ErrorMessage = "Debe tener 10 dígitos o 13 dígitos terminados en 001.")]
        public string rucCedula { get; set; }
        public string tipoIdentificacion { get; set; }
    }
}