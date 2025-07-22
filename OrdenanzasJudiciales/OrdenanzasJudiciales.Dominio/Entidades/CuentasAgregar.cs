using DocumentFormat.OpenXml.Office.CoverPageProps;
using System.ComponentModel.DataAnnotations;

namespace OrdenanzasJudiciales.Dominio.Entidades
{
    public class CuentasAgregar
    {
        [Required(ErrorMessage = "El nombre del acreedor es obligatorio.")]
        [StringLength(200, ErrorMessage = "El nombre no debe superar los 200 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten letras sin tildes ni ñ.")]
        public string numeroCuenta { get; set; }
        public int idEntidadBancaria { get; set; }
        public int idAcreedor { get; set; }
        public string tipoCuenta { get; set; }
    }
}