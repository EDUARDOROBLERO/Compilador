using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorTriangulo
{
    public class CrearNodo
    {
        public string lexema;
        public int toquen, linea, apuntador;
        public CrearNodo siguiente,cabeza;
        public CrearNodo(string lexe, int toque, int lin, int apu)
        {
            this.lexema = lexe;
            this.toquen = toque;
            this.linea = lin;
            this.apuntador = apu;
            this.siguiente = null;                       
        }
    }
}
