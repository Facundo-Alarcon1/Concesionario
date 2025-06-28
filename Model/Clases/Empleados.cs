using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Clases
{
    public class Empleados
    {  // atributos de Empleados que estan en la base de datos
        public int ID_empleado { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string turno { get; set; }
        public string Puesto { get; set; }
        
    }
}
