using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorTriangulo
{
    class Errores
    {        
        public string error;
        
        public string ERROR(int tk)
        {
            switch (tk)
            {
                case 500:
                    error = "simbolo no valido";
                    break;
                case 501:
                    error = "se esperaba un punto";
                    break;
                case 502:
                    error = "se esperaba una comilla";
                    break;
                case 503:
                    error = "Se esperaba una comilla simple";
                    break;
                case 504:
                    error = "Se esperaba un caracter";
                    break;
                case 524:
                    error = "Se esperaba otra &";
                    break;

                //errores sintacticos
                case 505:
                    error = "se esperava un let";
                    break;
                case 506:
                    error = "se esperaba una declaracion";
                    break;
                case 507:
                    error = "se esperaba in";
                    break;
                case 508:
                    error = "se esperaba un comando";
                    break;
                case 509:
                    error = "se esperaba end";
                    break;
                case 510:
                    error = "se esperaba :=";
                    break;
                case 511:
                    error = "se esperaba un ;";
                    break;
                case 512:
                    //cambiar then por {
                    error = "se esperaba un {";
                    break;
                case 513:
                    //quitar el do ya que funcionara con el 512
                    // y cambiarlo por " } "
                    error = "se esperaba un }";
                    break;
                case 514:
                    error = "se esperaba (";
                    break;
                case 515:
                    error = "se esperaba un identificador";
                    break;
                case 516:
                    error = "se esperaba un )";
                    break;
                case 517:
                    error = "se esperaba una cadena";
                    break;
                case 518:
                    error = "se esperaba una commilla";
                    break;
                case 519:
                    error = "se esperaba :";
                    break;
                case 520:
                    error = "se esperaba una exprecion";
                    break;
                case 521:
                    error = "se esperaba un tipo de asignacion";
                    break;

            }
            return error;
        }
    }
}
