using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrdenanzasJudiciales.Dominio.Entidades;

namespace OrdenanzasJudiciales.Dominio.Interfaces
{
    public interface IArchivoRepositorio
    {
        Task GuardarArchivoAsync(cargaArchivo archivo);

    }
}
