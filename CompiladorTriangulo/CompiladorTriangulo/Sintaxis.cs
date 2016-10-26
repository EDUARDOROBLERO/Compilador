using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CompiladorTriangulo
{
    class Sintaxis
    {
        public Sintaxis(CrearNodo cabeza)
        {
            this.cabeza = cabeza;
        }
        #region Variables
        public string error, valor_de_variable;
        /*   verificar errores     agregar variable   encuentra variable   analisis completo    doble declarada     variable declarada una ves*/
        public Boolean band = true,  agregar = true,   esta = false,       correcto = true,     doble = true,       simple = true;
        private CrearNodo cabeza;        
        public DataGridView grierror,declarados,errores;
        public Boolean valor=true;

        #endregion

        #region Listas para guardar informacion

        //guarda lexema y tipo
        public struct CrearNodo2
        {
            public string lexe;
            public string tipo;                        
        }
        //guarda lexema y valor
        public struct CrearNodo3
        {
            public string lexm;
            public string valor;
        }

        //para crear una lista para checar incompativilidad
        public struct CrearNodo4
        {
            public string tipo_variable_principal;
            public string variable_principal;
            public string informacion;
        }

        //lista 1
        public static List<CrearNodo2> lista_declarados = new List<CrearNodo2>();
        //lista 2
        public static List<CrearNodo3> valor_variable = new List<CrearNodo3>();
        //lista 3
        public static List<CrearNodo4> incompatibilidad = new List<CrearNodo4>();
        #endregion

        #region Clase para Errores

        Errores busca_error = new Errores();

        #endregion        

        #region Corrida Sintactica

        //metodo para checar las declaraciones
        public CrearNodo declaration(CrearNodo cabeza)
        {
            var li = new CrearNodo2();
            //si el resultado es igual a var
            while (cabeza.toquen == 209)
            {                
                cabeza = cabeza.siguiente;
                //si es un identificar
                if (cabeza.toquen == 100)
                {
                    //para poner el tipo de identificador de la lista
                    li.lexe = cabeza.lexema;
                    variablesdeclaradas(cabeza.lexema);
                    if (doble == false)
                    {
                        errores.Rows.Add(cabeza.toquen,"Variable '" + cabeza.lexema + "' Declarada Anteriormente ", cabeza.linea,"");
                        doble = true;
                    }
                    cabeza = cabeza.siguiente;                    
                    //si es :
                    if (cabeza.toquen == 11 || cabeza.toquen == 121)
                    {
                        cabeza = cabeza.siguiente;
                        //      integer                     char                  string                double                  Boolean
                        if (cabeza.toquen == 214 || cabeza.toquen == 215 || cabeza.toquen == 216 || cabeza.toquen == 218 || cabeza.toquen==219)
                        {
                            //para poner el tipo de dato el la lista
                            li.tipo = cabeza.lexema;

                            //para agregar al datagried el contenido
                            lista_declarados.Add(li);                                                                                   
                            cabeza = cabeza.siguiente;
                            //si es ;
                            if (cabeza.toquen == 120)
                            {
                                cabeza = cabeza.siguiente;
                                agregar = true;
                                break;                                
                            }
                            else
                            {
                                error = busca_error.ERROR(511);
                                grierror.Rows.Add(511, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                band = false;
                                break;
                            }
                        }
                        else
                        {
                            error = busca_error.ERROR(521);
                            grierror.Rows.Add(521, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                            band = false;
                            break;
                        }
                    }
                    else
                    {
                        error = busca_error.ERROR(519);
                        grierror.Rows.Add(519, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                        band = false;
                        break;
                    }                    
                }
                else
                {
                    error = busca_error.ERROR(515);
                    grierror.Rows.Add(515, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                    band = false;
                    break;
                }                                
            }
            return cabeza;            
        }
        //metodo para verificar expresiones primarias
        public CrearNodo PrimaryExpresion(CrearNodo cabeza)
        {
            //si es integer
            if (cabeza.toquen == 214)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                inserta_cola(cabeza.toquen);
                cabeza = cabeza.siguiente;
            }
            //si es char
            else if (cabeza.toquen == 215)
            {
                cabeza = cabeza.siguiente;
            }
            //si es string
            else if (cabeza.toquen == 216)
            {
                cabeza = cabeza.siguiente;
            }
            //si es double
            else if (cabeza.toquen == 218)
            {
                cabeza = cabeza.siguiente;
            }
            //si es identoificador
            else if (cabeza.toquen == 100)
            {
                variables(cabeza.lexema);                
                if (simple == false)
                {
                    errores.Rows.Add(cabeza.toquen,"Variable '" + cabeza.lexema + "' Inexistente ", cabeza.linea,"");
                    simple = true;
                }

                Buscar_var(cabeza.lexema,cabeza.toquen,cabeza.linea);
                valor_de_variable = valor_de_variable + cabeza.lexema;
                inserta_cola(cabeza.toquen);
                cabeza = cabeza.siguiente;
            }
            //si es un numero
            else if (cabeza.toquen == 101)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                inserta_cola(cabeza.toquen);
                cabeza = cabeza.siguiente;                               
            }
            //si es un decimal
            else if (cabeza.toquen == 102)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                inserta_cola(cabeza.toquen);
                cabeza = cabeza.siguiente;
            }
            //si es una cadena
            else if (cabeza.toquen==125)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                inserta_cola(cabeza.toquen);
                cabeza = cabeza.siguiente;
            }
            //si es operador
            else if (cabeza.toquen >= 103 && cabeza.toquen <= 117)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                inserta_cola(cabeza.toquen);
                cabeza = cabeza.siguiente;                
                cabeza = PrimaryExpresion(cabeza);
            }
            //si es true
            else if (cabeza.toquen == 210)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                inserta_cola(cabeza.toquen);
                cabeza = cabeza.siguiente;
            }
            //si es false
            else if (cabeza.toquen == 211)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                inserta_cola(cabeza.toquen);
                cabeza = cabeza.siguiente;
            }
            //si es (
            else if(cabeza.toquen == 122)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                inserta_cola(cabeza.toquen);
                cabeza =cabeza.siguiente;
                cabeza = Expresion(cabeza);
                //si es )
                if (cabeza.toquen == 123)
                {
                    valor_de_variable = valor_de_variable + cabeza.lexema;
                    inserta_cola(cabeza.toquen);
                    cabeza = cabeza.siguiente;
                }
                else
                {
                    error = busca_error.ERROR(516);
                    grierror.Rows.Add(516, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                    band = false;
                }             
            }           
            return cabeza;
        }
        //metodo para verificar las segundas expresiones2
        public CrearNodo SecondExpresion2(CrearNodo cabeza)
        {
            //si es un operador
            if (cabeza.toquen >= 103 && cabeza.toquen <= 117)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                inserta_cola(cabeza.toquen);
                cabeza = cabeza.siguiente;
                cabeza = PrimaryExpresion(cabeza);
                //cabeza = cabeza.siguiente;
                cabeza = SecondExpresion2(cabeza);
            }
            else
            {
                //no se hace nada
                //cabeza = cabeza.siguiente;               
            }
            return cabeza;
        }
        //metodo para verificar segundas expresiones
        public CrearNodo SecondExpresion(CrearNodo cabeza)
        {
            cabeza = PrimaryExpresion(cabeza);            
            cabeza = SecondExpresion2(cabeza);
            return cabeza;
        }
        //metodo para checar las expreciones
        public CrearNodo Expresion(CrearNodo cabeza)
        {
            cabeza = SecondExpresion(cabeza);                      
            return cabeza;
        }
        //metodo de else_estatement
        public CrearNodo else_estatement(CrearNodo cabeza)
        {
            while (cabeza.toquen == 100 || cabeza.toquen == 206 || cabeza.toquen == 203 || cabeza.toquen == 212 || cabeza.toquen == 213)
            {
                #region IF
                //si es un if
                if (cabeza.toquen == 206)
                {
                    cabeza = cabeza.siguiente;
                    //si es (
                    if (cabeza.toquen == 122)
                    {
                        cabeza = cabeza.siguiente;
                        //si no se genera ninguna exprexion
                        if (cabeza.toquen == 208)
                        {
                            error = busca_error.ERROR(520);
                            grierror.Rows.Add(520, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                            band = false;
                            break;
                        }
                        else
                        {
                            cabeza = Expresion(cabeza);
                            if (band == false)
                            {
                                break;
                            }
                            //si es )
                            if (cabeza.toquen == 123)
                            {
                                cabeza = cabeza.siguiente;
                                //si es {
                                if (cabeza.toquen == 124)
                                {
                                    cabeza = cabeza.siguiente;
                                    //si se genera un comando
                                    if (cabeza.toquen == 100 || cabeza.toquen == 206 || cabeza.toquen == 203 || cabeza.toquen == 212 || cabeza.toquen == 213)
                                    {
                                        while (cabeza.toquen == 100 || cabeza.toquen == 206 || cabeza.toquen == 203 || cabeza.toquen == 212 || cabeza.toquen == 213)
                                        {
                                            cabeza = command(cabeza);
                                            if (band == false)
                                            {
                                                break;
                                            }
                                        }
                                        if (band == false)
                                        {
                                            break;
                                        }
                                        //si es }
                                        if (cabeza.toquen == 125)
                                        {
                                            cabeza = cabeza.siguiente;
                                            //si ocurre un else
                                            if (cabeza.toquen == 204)
                                            {
                                                cabeza = cabeza.siguiente;
                                                //si es {
                                                if (cabeza.toquen == 124)
                                                {
                                                    cabeza = cabeza.siguiente;
                                                    cabeza = else_estatement(cabeza);
                                                    if (band == false)
                                                    {
                                                        break;
                                                    }
                                                    //si es }                                                                                    
                                                    if (cabeza.toquen == 125)
                                                    {
                                                        cabeza = cabeza.siguiente;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        error = busca_error.ERROR(513);
                                                        grierror.Rows.Add(513, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                                        band = false;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    error = busca_error.ERROR(512);
                                                    grierror.Rows.Add(512, "Se encontro  '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                                    band = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            error = busca_error.ERROR(513);
                                            grierror.Rows.Add(513, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                            band = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        error = busca_error.ERROR(508);
                                        grierror.Rows.Add(508, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                        band = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    error = busca_error.ERROR(512);
                                    grierror.Rows.Add(512, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                    band = false;
                                    break;
                                }
                            }
                            else
                            {
                                error = busca_error.ERROR(516);
                                grierror.Rows.Add(516, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                band = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        error = busca_error.ERROR(514);
                        grierror.Rows.Add(514, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                        band = false;
                        break;
                    }
                }
                #endregion
                else
                {
                    //si se genera un comando
                    if (cabeza.toquen == 100 || cabeza.toquen == 206 || cabeza.toquen == 203 || cabeza.toquen == 212 || cabeza.toquen == 213)
                    {
                        while (cabeza.toquen == 100 || cabeza.toquen == 206 || cabeza.toquen == 203 || cabeza.toquen == 212 || cabeza.toquen == 213)
                        {
                            cabeza = command(cabeza);
                            if (band == false)
                            {
                                break;
                            }
                        }
                        if (band == false)
                        {
                            break;
                        }                        
                    }
                    else
                    {
                        error = busca_error.ERROR(508);
                        grierror.Rows.Add(508, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                        band = false;
                        break;
                    }                    
                }
            }
                        
            return cabeza;
        }
        //metodo para checar los comandos
        public CrearNodo command(CrearNodo cabeza)
        {            
            while (cabeza.toquen==100||cabeza.toquen==206||cabeza.toquen==203||cabeza.toquen==212 || cabeza.toquen == 213 || cabeza.toquen == 217)
            {
                var li = new CrearNodo3();
                var li2 = new CrearNodo4();
                #region Identificador
                //si es un identificador            
                if (cabeza.toquen == 100)
                {                  
                    variables(cabeza.lexema);
                    //para checar el tipo
                    li2.tipo_variable_principal = tipo_variable(cabeza.lexema);
                    li2.variable_principal = cabeza.lexema;
                                      
                    if (simple == false)
                    {
                        errores.Rows.Add(cabeza.toquen,"Variable '" + cabeza.lexema + "' Inexistente ", cabeza.linea,"");
                        simple = true;
                    }
                    //si es una declaracion
                    if (cabeza.siguiente.toquen == 126)
                    {
                        //para agregar a la lista de valores de variables
                        valor_de_variable = "";
                        li.lexm = cabeza.lexema;
                        cabeza = cabeza.siguiente;
                        //si es := 
                        #region declaracion
                        if (cabeza.toquen == 126)
                        {
                            //para agregar a la cola
                            FRENTE = 0;
                            FINAL = 0;
                            MAX = 10000;
                            //.............
                            cabeza = cabeza.siguiente;
                            //si no se genera una expresion
                            if (cabeza.toquen == 120)
                            {
                                error = busca_error.ERROR(520);
                                grierror.Rows.Add(520, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                band = false;
                                break;
                            }
                            else
                            {                                
                                cabeza = Expresion(cabeza);

                                //para agregar a la lista
                                li.valor = valor_de_variable;
                                li2.informacion = valor_de_variable;                                
                                valor_variable.Add(li);
                                incompatibilidad.Add(li2);
                                checar_incopativilidad(li2.variable_principal);                                
                                //---------------------------

                                if (band == false)
                                {
                                    break;
                                }                                
                                //si es ;
                                else if (cabeza.toquen == 120)
                                {
                                    cabeza = cabeza.siguiente;
                                    break;
                                }
                                else
                                {
                                    error = busca_error.ERROR(511);
                                    grierror.Rows.Add(511, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                    band = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            error = busca_error.ERROR(510);
                            grierror.Rows.Add(510, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                            band = false;
                            break;
                        }
                        #endregion
                    }
                    //si es una expresion
                    if (cabeza.siguiente.toquen >= 103 && cabeza.siguiente.toquen <= 117)
                    {
                        cabeza = cabeza.siguiente;
                        #region exprecion                
                        if (cabeza.toquen >= 103 && cabeza.toquen <= 117)
                        {
                            cabeza = cabeza.siguiente;
                            cabeza = Expresion(cabeza);
                            //si es ;
                            if (cabeza.toquen == 120)
                            {
                                cabeza = cabeza.siguiente;
                                break;
                            }
                            else
                            {
                                error = busca_error.ERROR(511);
                                grierror.Rows.Add(511, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                band = false;
                                break;
                            }
                        }
                        else
                        {
                            error = busca_error.ERROR(520);
                            grierror.Rows.Add(520, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                            band = false;
                            break;
                        }
                        #endregion
                    }
                    else
                    {
                        error = busca_error.ERROR(521);
                        grierror.Rows.Add(521, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                        band = false;
                        break;
                    }

                }
                #endregion

                #region IF
                //si es un if
                if (cabeza.toquen == 206)
                {
                    cabeza = cabeza.siguiente;
                    //si es (
                    if (cabeza.toquen == 122)
                    {
                        cabeza = cabeza.siguiente;
                        //si no se genera ninguna exprexion
                        if (cabeza.toquen == 123)
                        {
                            error = busca_error.ERROR(520);
                            grierror.Rows.Add(520, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                            band = false;
                            break;
                        }
                        else
                        {
                            cabeza = Expresion(cabeza);
                            if (band == false)
                            {
                                break;
                            }
                            //si es )
                            if (cabeza.toquen == 123)
                            {
                                cabeza = cabeza.siguiente;
                                //si es {
                                if (cabeza.toquen == 124)
                                {
                                    cabeza = cabeza.siguiente;
                                    //si se genera un comando
                                    if (cabeza.toquen == 100 || cabeza.toquen == 206 || cabeza.toquen == 203 || cabeza.toquen == 212 || cabeza.toquen == 213)
                                    {
                                        while (cabeza.toquen == 100 || cabeza.toquen == 206 || cabeza.toquen == 203 || cabeza.toquen == 212 || cabeza.toquen == 213)
                                        {
                                            cabeza = command(cabeza);
                                            if (band == false)
                                            {
                                                break;
                                            }
                                        }
                                        if (band == false)
                                        {
                                            break;
                                        }
                                        //si es }
                                        if (cabeza.toquen == 125)
                                        {
                                            cabeza = cabeza.siguiente;
                                            //si ocurre un else
                                            if (cabeza.toquen == 204)
                                            {
                                                cabeza = cabeza.siguiente;
                                                //si es {
                                                if (cabeza.toquen == 124)
                                                {
                                                    cabeza = cabeza.siguiente;
                                                    cabeza = else_estatement(cabeza);
                                                    if (band == false)
                                                    {
                                                        break;
                                                    }
                                                    //si es }                                                                                    
                                                    if (cabeza.toquen == 125)
                                                    {
                                                        cabeza = cabeza.siguiente;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        error = busca_error.ERROR(513);
                                                        grierror.Rows.Add(513, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                                        band = false;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    error = busca_error.ERROR(512);
                                                    grierror.Rows.Add(512, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                                    band = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            error = busca_error.ERROR(513);
                                            grierror.Rows.Add(513, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                            band = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        error = busca_error.ERROR(508);
                                        grierror.Rows.Add(508, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                        band = false;
                                        break;
                                    }                                    
                                }
                                else
                                {
                                    error = busca_error.ERROR(512);
                                    grierror.Rows.Add(512, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                    band = false;
                                    break;
                                }
                            }
                            else
                            {
                                error = busca_error.ERROR(516);
                                grierror.Rows.Add(516, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                band = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        error = busca_error.ERROR(514);
                        grierror.Rows.Add(514, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                        band = false;
                        break;
                    }                    
                    
                }
                #endregion

                #region While
                //si es un while
                if (cabeza.toquen == 203)
                {
                    cabeza = cabeza.siguiente;
                    //si es (
                    if (cabeza.toquen == 122)
                    {
                        cabeza = cabeza.siguiente;
                        //si no se genera una expresion
                        if (cabeza.toquen == 123)
                        {
                            error = busca_error.ERROR(520);
                            grierror.Rows.Add(520, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                            band = false;
                            break;
                        }
                        else
                        {
                            cabeza = Expresion(cabeza);
                            //cabeza = cabeza.siguiente;
                            //si es )
                            if (cabeza.toquen == 123)
                            {
                                cabeza = cabeza.siguiente;
                                //si es un {
                                if (cabeza.toquen == 124)
                                {
                                    //checar en esta parte por que no regresa el {
                                    cabeza = cabeza.siguiente;
                                    cabeza = command(cabeza);
                                    if (band == false)
                                    {
                                        break;
                                    }
                                    //si es }                                                                                    
                                    if (cabeza.toquen == 125)
                                    {
                                        cabeza = cabeza.siguiente;
                                        break;
                                    }
                                    else
                                    {
                                        error = busca_error.ERROR(513);
                                        grierror.Rows.Add(513, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                        band = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    error = busca_error.ERROR(512);
                                    grierror.Rows.Add(512, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                    band = false;
                                    break;
                                }
                            }
                            else
                            {
                                error = busca_error.ERROR(516);
                                grierror.Rows.Add(516, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                band = false;
                                break;
                            }                            
                        }
                    }
                    else
                    {
                        error = busca_error.ERROR(514);
                        grierror.Rows.Add(514, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                        band = false;
                        break;
                    }                                       
                }
                #endregion

                #region Get
                //si es un get
                if (cabeza.toquen == 213)
                {
                    cabeza = cabeza.siguiente;
                    //si es (
                    if (cabeza.toquen == 122)
                    {
                        cabeza = cabeza.siguiente;
                        //si es identificador
                        if (cabeza.toquen == 100)
                        {
                            variables(cabeza.lexema);
                            if (simple == false)
                            {
                                errores.Rows.Add(cabeza.toquen,"Variable '" + cabeza.lexema + "' Inexistente ", cabeza.linea,"");
                                simple = true;
                            }

                            Buscar_var(cabeza.lexema,cabeza.toquen,cabeza.linea);

                            cabeza = cabeza.siguiente;
                            //si es un )
                            if (cabeza.toquen == 123)
                            {
                                cabeza = cabeza.siguiente;
                                //si es un ;
                                if (cabeza.toquen == 120)
                                {
                                    cabeza = cabeza.siguiente;
                                    break;
                                }
                                else
                                {
                                    error = busca_error.ERROR(511);
                                    grierror.Rows.Add(511, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                    band = false;
                                    break;
                                }
                            }
                            else
                            {
                                error = busca_error.ERROR(516);
                                grierror.Rows.Add(516, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                band = false;
                                break;
                            }
                        }
                        else
                        {
                            error = busca_error.ERROR(515);
                            grierror.Rows.Add(515, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                            band = false;
                            break;
                        }
                    }
                    else
                    {
                        error = busca_error.ERROR(514);
                        grierror.Rows.Add(514, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                        band = false;
                        break;
                    }
                }
                #endregion

                #region Put
                //si es un put
                if (cabeza.toquen == 212)
                {
                    cabeza = cabeza.siguiente;
                    //si es (
                    if (cabeza.toquen == 122)
                    {
                        cabeza = cabeza.siguiente;
                        //si es una cadena
                        if (cabeza.toquen == 125)
                        {
                            cabeza = cabeza.siguiente;
                            // si es una comilla
                            if (cabeza.toquen == 119)
                            {
                                cabeza = cabeza.siguiente;
                                //si es un identificador
                                if (cabeza.toquen == 100)
                                {
                                    variables(cabeza.lexema);
                                    if (simple == false)
                                    {
                                        errores.Rows.Add(cabeza.toquen,"Variable '" + cabeza.lexema + "' Inexistente ", cabeza.linea,"");
                                        simple = true;
                                    }
                                    Buscar_var(cabeza.lexema,cabeza.toquen,cabeza.linea);
                                    cabeza = cabeza.siguiente;
                                    //si es )
                                    if (cabeza.toquen == 123)
                                    {
                                        cabeza = cabeza.siguiente;
                                        //si es ;
                                        if (cabeza.toquen == 120)
                                        {
                                            cabeza = cabeza.siguiente;
                                            break;
                                        }
                                        else
                                        {
                                            error = busca_error.ERROR(511);
                                            grierror.Rows.Add(511, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                            band = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        error = busca_error.ERROR(516);
                                        grierror.Rows.Add(516, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                        band = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    error = busca_error.ERROR(515);
                                    grierror.Rows.Add(515, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                    band = false;
                                    break;
                                }
                            }
                            else
                            {
                                error = busca_error.ERROR(518);
                                grierror.Rows.Add(518, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                band = false;
                                break;
                            }
                        }
                        else
                        {
                            error = busca_error.ERROR(517);
                            grierror.Rows.Add(517, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                            band = false;
                            break;
                        }
                    }
                    else
                    {
                        error = busca_error.ERROR(514);
                        grierror.Rows.Add(514, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                        band = false;
                        break;
                    }
                }
                #endregion

                #region For
                if (cabeza.toquen==217)
                {
                    cabeza = cabeza.siguiente;
                    //si es (
                    if (cabeza.toquen == 122)
                    {
                        cabeza = cabeza.siguiente;
                        //declaracion for
                        cabeza=declaration(cabeza);
                        if (cabeza.toquen == 123)
                        {
                            error = busca_error.ERROR(520);
                            grierror.Rows.Add(520, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                            band = false;
                        }
                        else
                        {
                            cabeza = Expresion(cabeza);

                            if (cabeza.toquen == 120)
                            {
                                //si no se genera ninguna exprexion
                                cabeza = cabeza.siguiente;
                                if (cabeza.toquen == 123)
                                {
                                    error = busca_error.ERROR(520);
                                    grierror.Rows.Add(520, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                    band = false;
                                }
                                else
                                {
                                    cabeza = Expresion(cabeza);
                                    //cabeza = cabeza.siguiente;
                                    //si se genera un comando
                                    //si es )
                                    if (cabeza.toquen == 123)
                                    {
                                        cabeza = cabeza.siguiente;
                                        //si es un {
                                        if (cabeza.toquen == 124)
                                        {                                           
                                            cabeza = cabeza.siguiente;
                                            //si se genera un comando
                                            if (cabeza.toquen == 100 || cabeza.toquen == 206 || cabeza.toquen == 203 || cabeza.toquen == 212 || cabeza.toquen == 213 || cabeza.toquen == 217)
                                            {
                                                while (cabeza.toquen == 100 || cabeza.toquen == 206 || cabeza.toquen == 203 || cabeza.toquen == 212 || cabeza.toquen == 213 || cabeza.toquen == 217)
                                                {
                                                    cabeza = command(cabeza);
                                                    if (band == false)
                                                    {
                                                        break;
                                                    }
                                                }
                                                if (band == false)
                                                {
                                                    break;
                                                }
                                                //si es }                                                                                    
                                                if (cabeza.toquen == 125)
                                                {
                                                    cabeza = cabeza.siguiente;
                                                    break;
                                                }
                                                else
                                                {
                                                    error = busca_error.ERROR(513);
                                                    grierror.Rows.Add(513, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                                    band = false;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                error = busca_error.ERROR(508);
                                                grierror.Rows.Add(508, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                                band = false;
                                                break;
                                            }
                                            
                                        }
                                        else
                                        {
                                            error = busca_error.ERROR(512);
                                            grierror.Rows.Add(512, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                            band = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        error = busca_error.ERROR(516);
                                        grierror.Rows.Add(516, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                error = busca_error.ERROR(511);
                                grierror.Rows.Add(511, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                break;
                            }
                        }

                    }
                    else
                    {
                        error = busca_error.ERROR(515);
                        grierror.Rows.Add(515, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                        band = false;
                        break;                    
                    }
                }
                
                #endregion

                else
                {
                    break;
                }
            }
            return cabeza;
        }              
        public void analisador()
        {
            lista_declarados.Clear();
            valor_variable.Clear();
            while (cabeza!=null)
            {
                #region analizador
                //si el resutado es igual a let
                if (cabeza.toquen == 200)   
                {
                    cabeza = cabeza.siguiente;
                    //area de declaraciones
                   if (cabeza.toquen == 209)
                    {
                        //      primer metodo       //
                        while (cabeza.toquen == 209)
                        {
                            cabeza = declaration(cabeza);
                            if (band == false)
                            {
                                break;
                            }
                        }
                        if (band == false)
                        {
                            break;
                        }
                        //si el resultado es in
                        if (cabeza.toquen == 207)
                        {

                            cabeza = cabeza.siguiente;
                            //si se genera un comando
                            if (cabeza.toquen == 100 || cabeza.toquen == 206 || cabeza.toquen == 203 || cabeza.toquen == 212 || cabeza.toquen == 213 ||cabeza.toquen==217)
                            {
                                while (cabeza.toquen == 100 || cabeza.toquen == 206 || cabeza.toquen == 203 || cabeza.toquen == 212 || cabeza.toquen == 213 || cabeza.toquen == 217)
                                {
                                    cabeza = command(cabeza);
                                    if (band == false)
                                    {
                                        break;
                                    }
                                }
                                if (band == false)
                                {
                                    break;
                                }

                                //si es end
                                if (cabeza.toquen == 205)
                                {
                                    //para buscar variables
                                    //Buscar_Var(cabeza.lexema); 
                                    Llenar_tabla();
                                    lista_declarados.Clear();
                                    valor_variable.Clear();
                                    if (correcto == false)
                                    {
                                        break;
                                    }
                                    else
                                    {                                        
                                        
                                        MessageBox.Show("Analisis Sintactico Exitoso");
                                        break;
                                    }                                    
                                }
                                else
                                {
                                    error = busca_error.ERROR(509);
                                    grierror.Rows.Add(509, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                    band = false;
                                    break;
                                }
                            }
                            else
                            {
                                error = busca_error.ERROR(508);
                                grierror.Rows.Add(508, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                                band = false;
                                break;
                            }

                        }
                        else
                        {
                            error = busca_error.ERROR(507);
                            grierror.Rows.Add(507, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                            band = false;
                            break;
                        }
                    }
                    else
                    {
                        error = busca_error.ERROR(506);
                        grierror.Rows.Add(506, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                        band = false;
                        break;
                    }
                }
                else
                {
                    error = busca_error.ERROR(505);
                    grierror.Rows.Add(505,"Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                    band = false;
                    break;
                }
                #endregion
            }
        }

        #endregion

        #region manipulacion de variables
        public string aux;
        //para llenar la tabla de variables con sus valores
        public void Llenar_tabla()
        {
            int a = 0;          
            string dat, tip, lex, lex1;
            for (int i = a; i < lista_declarados.Count; i++)
            {
                lex = lista_declarados[i].lexe;

                for (int j = 0; j < valor_variable.Count; j++)
                {
                    lex1 = valor_variable[j].lexm;
                    if (lex == lex1)
                    {
                        aux = valor_variable[j].valor; ;
                        valor = true;
                    }
                    else
                    {
                        valor = false;
                    }
                }
                if (valor == true)
                {
                    lex = lista_declarados[i].lexe;
                    tip = lista_declarados[i].tipo;

                    declarados.Rows.Add(tip, lex, aux);
                    aux = "";
                }
                else if (valor == false)
                {
                    if (aux == "")
                    {
                        valor = true;
                        lex = lista_declarados[i].lexe;
                        tip = lista_declarados[i].tipo;
                        dat = "";
                        declarados.Rows.Add(tip, lex, dat);
                    }
                    else
                    {
                        valor = true;
                        lex = lista_declarados[i].lexe;
                        tip = lista_declarados[i].tipo;
                        dat = aux;
                        declarados.Rows.Add(tip, lex, dat);
                        aux = "";
                    }
                }
            }
        }
        //para buscar si esta incicializada
        public void Buscar_var(string lexema, int toquen, int linea)
        {
            int val = 0;
            for (int j = 0; j < valor_variable.Count; j++, val = j)
            {
                if (valor_variable[j].lexm == lexema)
                {
                    if (valor_variable[j].valor == "")
                    {
                        errores.Rows.Add(toquen, "Variable '" + lexema + "' No Inicializada ", linea, "");
                        valor = false;
                        break;
                    }
                    esta = true;
                    break;
                }
                else
                {
                    esta = false;
                }
            }
            if (valor_variable.Count == 0 || esta == false)
            {
                errores.Rows.Add(toquen, "Variable '" + lexema + "' No Inicializada ", linea, "");
                correcto = false;
            }
        }
        //metodos para burcar si esta o no la variable declarada
        private void variables(string Plexema)
        {
            for (int i = 0; i < lista_declarados.Count; i++)
            {
                if (lista_declarados[i].lexe == Plexema)
                {
                    esta = true;
                    break;
                }
            }
            if (esta == true)
            {
                esta = false;
            }
            else
            {
                correcto = false;
                simple = false;
                esta = false;
            }

        }
        public void variablesdeclaradas(string Plexema)
        {
            for (int i = 0; i < lista_declarados.Count; i++)
            {
                if (lista_declarados[i].lexe == Plexema)
                {
                    esta = true;
                    break;
                }
                else
                {
                    esta = false;
                }
            }
            if (esta == true)
            {
                esta = false;
                agregar = false;
                correcto = false;
                doble = false;
            }
            else
            {
                esta = false;
                agregar = true;
            }
        }
        #endregion

        #region incompativilidad
        //para buscar el tipo y checar compativilidad
        public string tipo;
        public string tipo_variable(string Plexema)
        {            
            for (int i = 0; i < lista_declarados.Count; i++)
            {
                if (lista_declarados[i].lexe == Plexema)
                {
                    tipo = lista_declarados[i].tipo;
                    break;
                }                
            }
            return tipo;
        }

        public string incompatible;
        public string checar_incopativilidad(string variable)
        {
            string tipo="";            
            string informacion="";
            for (int i = 0; i < incompatibilidad.Count; i++)
            {
                if (incompatibilidad[i].variable_principal == variable)
                {                    
                    tipo = incompatibilidad[i].tipo_variable_principal;
                    //informacion = incompatibilidad[i].informacion;
                    break;
                }                
            }
            //checar de que tipo es
            if (tipo == "integer"|| tipo == "Integer"|| tipo == "INTEGER")
            {
                incompatible = CASOS_INTEGER(informacion);
            }
            else if(tipo=="string"|| tipo == "String"|| tipo == "STRING")
            {
                incompatible = CASOS_STRING(informacion);
            }
            else if(tipo == "double"||tipo == "Double"|| tipo == "DOUBLE")
            {
                incompatible = CASOS_DOUBLE(informacion);
            }
            else if(tipo == "boolean"|| tipo == "Boolean"|| tipo == "BOOLEAN")
            {
                incompatible = CASOS_BOOLEAN(informacion);
            }

            return incompatible;
        }

        public int FRENTE, FINAL,DATO1;
        /*variables que vamos a usar del metodo de pila que esta declarado abajo

        */
        public int[] COLA = new int[100000];
        public void inserta_cola(int DATO)
        {
            if (FINAL < MAX)
            {
                FINAL = FINAL + 1;
                COLA[FINAL] = DATO;
                if (FINAL == 1)
                {
                    FRENTE = 1;
                }
                else
                {
                    //mensaje de cola llena
                }
            }
        }

        public void elimina_cola()
        {
            if (FRENTE != 0)
            {
                DATO1 = COLA[FRENTE];
                if (FRENTE == FINAL)
                {
                    FRENTE = 0;
                    FINAL = 0;
                }
                else
                {
                    FRENTE = FRENTE + 1;
                }
            }
            else
            {
                //mensaje de cola vacia
            }
        }


        #region INTEGER
        public string CASOS_INTEGER(string info)
        {
            elimina_cola();
            return incompatible;
        }
        #endregion

        #region STRING
        public string CASOS_STRING(string info)
        {            
            return incompatible;            
        }
        #endregion

        #region DOUBLE
        public string CASOS_DOUBLE(string info)
        {           
            return incompatible;            
        }
        #endregion

        #region BOOLEAN
        public string CASOS_BOOLEAN(string info)
        {
            elimina_cola();
            return incompatible;            
        }
        #endregion

        #endregion

        #region Infix To Postfix
        public Boolean BAND;
        public string[] PILA = new string[100000];
        public string texto, DATO, EPOS, aux1, aux2,postfijo;
        public int TOPE, MAX, puntero, prioridad, prioridad_pila;
        public char caracter;

        public void postfix(string infix)
        {            
            postfijo="";
            texto = "";
            aux1 = "";
            aux2 = "";
            EPOS = "";
            MAX = 0;
            TOPE = 0;
            puntero = 0;
            prioridad = 0;
            prioridad_pila = 0;
            texto = infix;
            MAX = texto.Length;

            if (texto == "")
            {
                MessageBox.Show("No has introducido nada para calcular", "ALERTA", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
            else
            {
                do
                {
                    caracter = texto[puntero];
                    //si es un numero o letra
                    if (char.IsLetterOrDigit(caracter))
                    {
                        if (char.IsLetter(caracter))
                        {
                            aux1 = aux1 + caracter;
                            puntero++;
                        }
                        if (char.IsDigit(caracter))
                        {
                            aux2 = aux2 + caracter;
                            puntero++;
                        }
                    }
                    else
                    {
                        if (aux != "")
                        {
                            EPOS = EPOS + aux1;
                            EPOS = EPOS + " ";
                            aux1 = "";
                        }
                        if (aux2 != "")
                        {
                            EPOS = EPOS + aux2;
                            EPOS = EPOS + " ";
                            aux2 = "";
                        }
                        checar_prioridad(caracter);

                        //espacio en blanco
                        if (caracter == 32)
                        {
                            puntero++;
                        }
                        //si es (                
                        else if (prioridad == -1)
                        {
                            prioridad_pila = 0;
                            puntero++;

                        }
                        //si  es ) vaciar la pila
                        else if (prioridad == -2)
                        {
                            while (PILA != null)
                            {
                                quita();
                                if (TOPE == -1)
                                {
                                    TOPE = 0;
                                    break;
                                }
                                EPOS = EPOS + DATO;
                                EPOS = EPOS + " ";
                            }
                            puntero++;

                        }

                        //igual procedencia
                        else if (prioridad == prioridad_pila)
                        {
                            quita();
                            EPOS = EPOS + DATO;
                            EPOS = EPOS + " ";
                            DATO = caracter.ToString();
                            pone();
                            puntero++;

                        }
                        //mayor procedencia
                        else if (prioridad > prioridad_pila)
                        {
                            DATO = caracter.ToString();
                            pone();
                            puntero++;

                        }

                        //menor procedencia
                        else if (prioridad < prioridad_pila)
                        {
                            while (PILA != null)
                            {
                                quita();
                                if (TOPE == -1)
                                {
                                    TOPE = 0;
                                    break;
                                }

                                EPOS = EPOS + DATO;
                                EPOS = EPOS + " ";
                            }
                            DATO = caracter.ToString();
                            pone();
                            puntero++;

                        }
                    }
                    if (texto.Length == puntero)
                    {
                        if (aux1 != "")
                        {
                            EPOS = EPOS + aux1;
                            EPOS = EPOS + " ";
                            aux1 = "";
                        }
                        if (aux2 != "")
                        {
                            EPOS = EPOS + aux2;
                            EPOS = EPOS + " ";
                            aux2 = "";
                        }
                        while (PILA != null)
                        {
                            quita();
                            if (TOPE == -1)
                            {
                                TOPE = 0;
                                break;
                            }
                            EPOS = EPOS + DATO;
                            EPOS = EPOS + " ";
                        }
                    }
                } while (texto.Length > puntero);
            }
            postfijo = EPOS;
        }
        public void checar_prioridad(char xcaracter)
        {
            /*clasificacion de caracteres             
             * 4.-  [ ] { }             
             * 3.-  Exponencial Raiz             
             * 2.-  * /             
             * 1.-  + -            
             *-1.-  (
             *-2.-  )                       
            */
            char s = (char)94;//Exponencial
            char l = (char)251;//Raiz            
            switch (xcaracter)
            {
                case '(': prioridad = -1; break;
                case ')': prioridad = -2; break;

                case '[': prioridad = 4; break;
                case ']': prioridad = 4; break;
                case '{': prioridad = 4; break;
                case '}': prioridad = 4; break;


                case '*': prioridad = 2; break;
                case '/': prioridad = 2; break;
                case '+': prioridad = 1; break;
                case '-': prioridad = 1; break;
            }
            if (xcaracter == s) prioridad = 3;
            if (xcaracter == l) prioridad = 3;
        }
        public void pila_vacia()
        {
            if (TOPE == 0)
            {
                BAND = true;
                TOPE = -1;
                prioridad_pila = 0;
                prioridad = 0;
            }
            else
            {
                BAND = false;
            }
        }
        public void pila_llena()
        {
            if (TOPE == MAX)
            {
                BAND = true;
            }
            else
            {
                BAND = false;
            }
        }
        public void pone()
        {
            pila_llena();
            if (BAND == true)
            {
                //mensaje de lleno
            }
            else
            {
                TOPE = TOPE + 1;
                PILA[TOPE] = DATO;
            }
            prioridad_pila = prioridad;
        }
        public void quita()
        {
            pila_vacia();
            if (BAND == true)
            {
                //mensaje de vacio
            }
            else
            {
                DATO = PILA[TOPE];
                TOPE = TOPE - 1;
            }
        }
        #endregion
    }
}