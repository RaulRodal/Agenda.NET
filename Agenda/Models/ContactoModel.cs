using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Models
{
    public class ContactoModel
    {
        public int IdContacto { get; set; }
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string Comentario { get; set; }

    }
}
