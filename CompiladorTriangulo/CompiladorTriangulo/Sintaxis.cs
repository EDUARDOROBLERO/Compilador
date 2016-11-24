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
        //para la incompatibilidad
        public string tipo_variable_principal;
        public string variable_principal;
        //..........

        /*   verificar errores     agregar variable   encuentra variable   analisis completo    doble declarada     variable declarada una ves*/
        public Boolean band = true,  agregar = true,   esta = false,       correcto = true,     doble = true,       simple = true, inicia=false , esta1=true;
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

        //lista 1
        public static List<CrearNodo2> lista_declarados = new List<CrearNodo2>();
        //lista 2
        public static List<CrearNodo3> valor_variable = new List<CrearNodo3>();

        #endregion

        #region compilador
        public string var1, var2;
        public CrearNodo COMPILADOR1(CrearNodo cabeza)
        {

            // si es identificador
            if (cabeza.toquen == 100)
            {
                //si es ++ ó --
                if (cabeza.siguiente.toquen == 113 || cabeza.siguiente.toquen == 114)
                {
                    
                    cabeza = cabeza.siguiente;
                    if (cabeza.toquen == 113)
                    {
                        au = "\n\tMOV AX, " + Buscar_var3(variable_principal) + "\n\tI_ASIGNAR " + variable_principal + ", AX\n\tSUMAR " + variable_principal + ", 1,$1\n\tMOV AX, $1\n\tI_ASIGNAR "+variable_principal+", AX\n";
                        complemento += au;
                        cabeza = cabeza.siguiente;
                    }
                    if (cabeza.toquen == 114)
                    {
                        de = "\n\tMOV AX, " + Buscar_var3(variable_principal) + "\n\tI_ASIGNAR " + variable_principal + ", AX\n\tRESTA " + variable_principal + ", 1,$1\n\tMOV AX, $1\n\tI_ASIGNAR " + variable_principal + ", AX\n";
                        complemento += au;
                        cabeza = cabeza.siguiente;
                    }
                }
                while(cabeza.siguiente.toquen >= 103 && cabeza.siguiente.toquen <= 117)
                {
                    var1 = cabeza.lexema;
                    
                    //para avanzar el operador
                    cabeza =cabeza.siguiente;
                    //si es una suma
                    if (cabeza.toquen == 103)
                    {
                        cabeza = cabeza.siguiente;
                        //si es un numero identificador decimal
                        if (cabeza.toquen == 101 || cabeza.toquen == 102 || cabeza.toquen == 100)
                        {
                            var2 = cabeza.lexema;
                            complemento += "\n\tSUMAR " + var1 + ", " + var2 + ",$1\n";
                            if (cabeza.siguiente.toquen >= 103 && cabeza.siguiente.toquen <= 117)
                            {
                                cabeza = cabeza.siguiente;
                                cabeza = COMPILADOR1(cabeza);
                            }
                        }                       
                    }
                    //si es una resta                   
                    if (cabeza.toquen == 104)
                    {
                        cabeza = cabeza.siguiente;
                        //si es un numero identificador decimal
                        if (cabeza.toquen == 101 || cabeza.toquen == 102 || cabeza.toquen == 100)
                        {
                            var2 = cabeza.lexema;
                            complemento += "\n\tRESTA " + var1 + ", " + var2 + ",$1\n";
                            if (cabeza.siguiente.toquen >= 103 && cabeza.siguiente.toquen <= 117)
                            {
                                cabeza = cabeza.siguiente;
                                cabeza = COMPILADOR1(cabeza);
                            }
                        }
                    }
                    //si es una multiplicacion                   
                    if (cabeza.toquen == 105)
                    {
                        cabeza = cabeza.siguiente;
                        //si es un numero identificador decimal
                        if (cabeza.toquen == 101 || cabeza.toquen == 102 || cabeza.toquen == 100)
                        {
                            var2 = cabeza.lexema;
                            complemento += "\n\tMULTI " + var1 + ", " + var2 + ",$1\n";
                            if (cabeza.siguiente.toquen >= 103 && cabeza.siguiente.toquen <= 117)
                            {
                                cabeza = cabeza.siguiente;
                                cabeza = COMPILADOR1(cabeza);
                            }
                        }
                    }
                    //si es una divicion                   
                    if (cabeza.toquen == 105)
                    {
                        cabeza = cabeza.siguiente;
                        //si es un numero identificador decimal
                        if (cabeza.toquen == 101 || cabeza.toquen == 102 || cabeza.toquen == 100)
                        {
                            var2 = cabeza.lexema;
                            complemento += "\n\tDIVIDE " + var1 + ", " + var2 + ",$1\n";
                            if (cabeza.siguiente.toquen >= 103 && cabeza.siguiente.toquen <= 117)
                            {
                                cabeza = cabeza.siguiente;
                                cabeza = COMPILADOR1(cabeza);
                            }
                        }
                    }
                }

            }
            //si es un numero o decimal
            else if (cabeza.toquen == 101|| cabeza.toquen == 102)
            {
                asig = "\n\tMOV AX, " + cabeza.lexema + " \n\tI_ASIGNAR " + variable_principal + ", AX\n";                
                complemento += asig;
                cabeza = cabeza.siguiente;
            }           
            //si es una cadena
            else if (cabeza.toquen == 127)
            {                
                asig = "\n\tS_ASIGNAR "+ variable_principal + " $TmpASM_" + conteo1 + "\n";
                complemento += asig;
                conteo1++;
            }
            //si es un caracter
            else if (cabeza.toquen == 128)
            {
                
            }
            //si es operador
            else if (cabeza.toquen >= 103 && cabeza.toquen <= 117)
            {
                //si estos no estan en vacio quiere decir que se esta asiendo varias operaciones
                if (var1 != "" || var2 != "")
                {
                    //si es una resta 
                    if (cabeza.toquen == 104)
                    {
                        cabeza = cabeza.siguiente;
                        //si es un numero identificador decimal
                        if (cabeza.toquen == 101|| cabeza.toquen == 102|| cabeza.toquen == 100)
                        {
                            complemento += "\n\tRESTA $1, " + cabeza.lexema + ",$1\n";
                            cabeza = cabeza.siguiente;
                            cabeza = COMPILADOR1(cabeza);
                        }                        
                    }
                    //si es una suma
                    if (cabeza.toquen == 103)
                    {
                        cabeza = cabeza.siguiente;
                        //si es un numero identificador decimal
                        if (cabeza.toquen == 101 || cabeza.toquen == 102 || cabeza.toquen == 100)
                        {
                            complemento += "\n\tSUMAR $1, " + cabeza.lexema + ",$1\n";
                            cabeza = cabeza.siguiente;
                            cabeza = COMPILADOR1(cabeza);
                        }
                    }
                    //si es una multiplicacion
                    if (cabeza.toquen == 105)
                    {
                        cabeza = cabeza.siguiente;
                        //si es un numero identificador decimal
                        if (cabeza.toquen == 101 || cabeza.toquen == 102 || cabeza.toquen == 100)
                        {
                            complemento += "\n\tMULTI $1, " + cabeza.lexema + ",$1\n";
                            cabeza = cabeza.siguiente;
                            cabeza = COMPILADOR1(cabeza);
                        }
                    }
                    //si es una divicion
                    if (cabeza.toquen == 106)
                    {
                        cabeza = cabeza.siguiente;
                        //si es un numero identificador decimal
                        if (cabeza.toquen == 101 || cabeza.toquen == 102 || cabeza.toquen == 100)
                        {
                            complemento += "\n\tDIVIDE $1, " + cabeza.lexema + ",$1\n";
                            cabeza = cabeza.siguiente;
                            cabeza = COMPILADOR1(cabeza);
                        }
                    }
                }               
                
            }
            //si es true o false
            else if (cabeza.toquen == 210 || cabeza.toquen == 211)
            {
                
            }
            //si es (
            else if (cabeza.toquen == 122)
            {
                cabeza = cabeza.siguiente;
                cabeza = COMPILADOR(cabeza);
                //si es )
                if (cabeza.toquen == 123)
                {
                    cabeza = cabeza.siguiente;
                }
            }
            return cabeza;
        }
        //metodo para verificar las segundas expresiones2
        public CrearNodo COMPILADOR2(CrearNodo cabeza)
        {
            //si es un operador
            if (cabeza.toquen >= 103 && cabeza.toquen <= 117 || cabeza.toquen == 129 || cabeza.toquen == 130)
            {
                cabeza = cabeza.siguiente;
                cabeza = COMPILADOR1(cabeza);
                //cabeza = cabeza.siguiente;
                cabeza = COMPILADOR2(cabeza);
            }
            else
            {
                //no se hace nada
                //cabeza = cabeza.siguiente;               
            }
            return cabeza;
        }
        //metodo para verificar segundas expresiones
        public CrearNodo COMPILADOR(CrearNodo cabeza)
        {
            cabeza = COMPILADOR1(cabeza);
            cabeza = COMPILADOR2(cabeza);
            return cabeza;
        }
        //metodo para checar las expreciones
        public CrearNodo COMP(CrearNodo cabeza)
        {
            cabeza = COMPILADOR(cabeza);
            return cabeza;
        }
        #endregion

        #region Clase para Errores

        Errores busca_error = new Errores();

        #endregion

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
                else
                {
                    esta = true;
                }                
            }
            return tipo;
        }

        #region sentencia if y while 2


        public CrearNodo INCOMPATIBILIDAD1IF2(CrearNodo cabeza)
        {
            if (cabeza.toquen == 100)
            {
                string tip;
                string caracter ;

                //verificar el tipo de variable
                tip = tipo_variable(cabeza.lexema);
                //nos posicionamos en el caracter
                cabeza = cabeza.siguiente; 
                //si es < > <= >= \= <>               
                if(cabeza.toquen>=107 && cabeza.toquen <= 112)
                {
                    
                    caracter = cabeza.lexema;
                    errores.Rows.Add(cabeza.toquen,"Error con '"+caracter+"' ,Solo las expresiones de asignacion, incremento o decremento se pueden usar como instruccion", cabeza.linea, "");
                }
                //si es ++ --
                else if (cabeza.toquen == 112 || cabeza.toquen == 113)
                {
                    caracter = cabeza.lexema;
                    if (tip == "STRING" || tip == "BOOLEAN")
                    {
                        errores.Rows.Add(cabeza.toquen, "El operador '" + caracter + "' no se puede aplicar al operando del tipo '"+tip+"'", cabeza.linea, "");
                    }                       
                }
                cabeza = cabeza.siguiente;
            }
            return cabeza;
        }
        //metodo para verificar las segundas expresiones2
        public CrearNodo INCOMPATIBILIDAD2IF2(CrearNodo cabeza)
        {
            //si es &&           
            if (cabeza.toquen == 128)
            {
                cabeza = cabeza.siguiente;
                cabeza = INCOMPATIBILIDAD1IF2(cabeza);
                cabeza = INCOMPATIBILIDAD2IF2(cabeza);
            }
            else
            {
                //no se hace nada
                //cabeza = cabeza.siguiente;               
            }
            return cabeza;
        }
        //metodo para verificar segundas expresiones
        public CrearNodo INCOMPATIBILIDADIF2(CrearNodo cabeza)
        {
            cabeza = INCOMPATIBILIDAD1IF2(cabeza);
            cabeza = INCOMPATIBILIDAD2IF2(cabeza);
            return cabeza;
        }
        //metodo para checar las expreciones
        public CrearNodo INCOMPATIBLEIF2(CrearNodo cabeza)
        {
            cabeza = INCOMPATIBILIDADIF2(cabeza);
            return cabeza;
        }
        #endregion*/

        #region sentencia if y while
        public CrearNodo INCOMPATIBILIDAD1IF(CrearNodo cabeza)
        {
            string tip1,tip2,lex1,lex2;
            int operador;
            string caracter;
            //por si es un operador entre tipos            
            if (cabeza.siguiente.toquen >= 107 && cabeza.siguiente.toquen<=112||cabeza.siguiente.toquen==129)
            {
                tip1 = tipo_variable(cabeza.lexema);
                lex1 = cabeza.lexema;
                cabeza = cabeza.siguiente;
                operador = cabeza.toquen;
                caracter = cabeza.lexema;
                cabeza = cabeza.siguiente;
                tip2 = tipo_variable(cabeza.lexema);
                lex2 = cabeza.lexema;


                //no se permite string y boolean en < > <= >=
                if (operador >= 107 && operador<109 ||operador>=110 && operador<=111)
                {
                    if ((tip1 == "STRING"||tip1=="BOOLEAN")&&(tip2 == "STRING" || tip2 == "BOOLEAN"))
                    {
                        errores.Rows.Add(cabeza.toquen, "El operador '" + caracter + "' no se puede aplicar en operandos de tipo " + tip1 + " y " + tip2, cabeza.linea, "");
                    }

                    if((tip1=="INTEGER"||tip1=="DOUBLE") && (tip2 == "STRING" || tip2 == "BOOLEAN"))
                    {
                        errores.Rows.Add(cabeza.toquen, "El operador '" + caracter + "' no se puede aplicar en operandos de tipo " + tip1 + " y " + tip2, cabeza.linea, "");                        
                    }
                }

                //si el operador es \= del mismo tipo
                if (operador ==112|| operador == 129||operador==109)
                {
                    if ((tip1 == "STRING" || tip1 == "BOOLEAN") && (tip2 != tip1))
                    {
                        errores.Rows.Add(cabeza.toquen, "El operador '" + caracter + "' no se puede aplicar en operandos de tipo " + tip1 + " y " + tip2, cabeza.linea, "");
                    }

                    if ((tip1 == "INTEGER" || tip1 == "DOUBLE") && (tip2 == "STRING" || tip2 == "BOOLEAN"))
                    {
                        errores.Rows.Add(cabeza.toquen, "El operador '" + caracter + "' no se puede aplicar en operandos de tipo " + tip1 + " y " + tip2, cabeza.linea, "");
                    }
                }
                cabeza = cabeza.siguiente;
            }
            return cabeza;
        }
        //metodo para verificar las segundas expresiones2
        public CrearNodo INCOMPATIBILIDAD2IF(CrearNodo cabeza)
        {
            //si es &&           
            if (cabeza.toquen == 128)
            {
                cabeza = cabeza.siguiente;
                cabeza = INCOMPATIBILIDAD1IF(cabeza);
                cabeza = INCOMPATIBILIDAD2IF(cabeza);
            }
            else
            {
                //no se hace nada
                //cabeza = cabeza.siguiente;               
            }
            return cabeza;
        }
        //metodo para verificar segundas expresiones
        public CrearNodo INCOMPATIBILIDADIF(CrearNodo cabeza)
        {
            cabeza = INCOMPATIBILIDAD1IF(cabeza);
            cabeza = INCOMPATIBILIDAD2IF(cabeza);
            return cabeza;
        }
        //metodo para checar las expreciones
        public CrearNodo INCOMPATIBLEIF(CrearNodo cabeza)
        {
            cabeza = INCOMPATIBILIDADIF(cabeza);
            return cabeza;
        }
        #endregion*/

        #region asignaciones
        public CrearNodo INCOMPATIBILIDAD1(CrearNodo cabeza)
        {            
            //si es integer
            if (cabeza.toquen == 214)
            {                
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
                string tip;

                //por si es un operador entre tipos
                //verificar el tipo de variable
                tip = tipo_variable(cabeza.lexema);
                //INTEGER
                if (tipo_variable_principal == "INTEGER")
                {
                    //INTEGER NO PERMITE
                    //      DOUBLE               STRING              BOOLEAN
                    if (tip != "INTEGER")
                    {
                        errores.Rows.Add(cabeza.toquen, "Error con '" + cabeza.lexema + "' " + "No se puede convertir implicitamente el tipo '" + tipo_variable_principal + "' en '" + tip + "' ", cabeza.linea, "");
                    }
                }

                //DOUBLE            
                if (tipo_variable_principal == "DOUBLE")
                {
                    //NO PERMITE            STRING                                                     BOOLEAN 
                    if (tip == "BOOLEAN")
                    {
                        errores.Rows.Add(cabeza.toquen, "Error con '" + cabeza.lexema + "' " + "No se puede convertir implicitamente el tipo '" + tipo_variable_principal + "' en '" + tip + "' ", cabeza.linea, "");
                    }
                }

                //STRING
                if (tipo_variable_principal == "STRING")
                {
                    //STRING NO PERMITE
                    //      INTEGER               DOUBLE              BOOLEAN
                    if (tip != "STRING")
                    {
                        errores.Rows.Add(cabeza.toquen, "Error con '" + cabeza.lexema + "' " + "No se puede convertir implicitamente el tipo '" + tipo_variable_principal + "' en '" + tip + "' ", cabeza.linea, "");
                    }
                }

                //BOOLEAN
                if (tipo_variable_principal == "BOOLEAN")
                {
                    //BOOLEAN NO PERMITE
                    //      INTEGER               DOUBLE              STRING
                    if (tip != "BOOLEAN")
                    {
                        errores.Rows.Add(cabeza.toquen, "Error con '" + cabeza.lexema + "' " + "No se puede convertir implicitamente el tipo '" + tipo_variable_principal + "' en '" + tip + "' ", cabeza.linea, "");
                    }
                }
                cabeza = cabeza.siguiente;
            }
            //si es un numero
            else if (cabeza.toquen == 101)
            {               
                if (tipo_variable_principal == "STRING")
                {
                    errores.Rows.Add(cabeza.toquen, "Error con '" + cabeza.lexema + "' " + "No se puede convertir implicitamente el tipo '" + "INTEGER" + "' en '" + tipo_variable_principal + "' ", cabeza.linea, "");
                }

                if (tipo_variable_principal == "BOOLEAN")
                {
                    errores.Rows.Add(cabeza.toquen, "Error con '" + cabeza.lexema + "' " + "No se puede convertir implicitamente el tipo '" + "INTEGER" + "' en '" + tipo_variable_principal + "' ", cabeza.linea, "");
                }
                if (tipo_variable_principal == "CHAR")
                {
                    errores.Rows.Add(cabeza.toquen, "Error con '" + cabeza.lexema + "' " + "No se puede convertir implicitamente el tipo '" + "INTEGER" + "' en '" + tipo_variable_principal + "' ", cabeza.linea, "");
                }
                cabeza = cabeza.siguiente;
            }
            //si es un decimal
            else if (cabeza.toquen == 102)
            {
                //INTEGER
                if (tipo_variable_principal == "INTEGER")
                {
                    errores.Rows.Add(cabeza.toquen, "Error con '" + cabeza.lexema + "' " + "Posible perdida de presicion, No se puede convertir implicitamente el tipo '" + "DOUBLE" + "' en '" + tipo_variable_principal + "' ", cabeza.linea, "");
                }                

                //STRING
                if (tipo_variable_principal == "STRING")
                {
                    errores.Rows.Add(cabeza.toquen, "Error con '" + cabeza.lexema + "' " + "No se puede convertir implicitamente el tipo '" + "DOUBLE" + "' en '" + tipo_variable_principal + "' ", cabeza.linea, "");
                }

                //BOOLEAN
                if (tipo_variable_principal == "BOOLEAN")
                {
                    errores.Rows.Add(cabeza.toquen, "Error con '" + cabeza.lexema + "' " + "No se puede convertir implicitamente el tipo '" + "DOUBLE" + "' en '" + tipo_variable_principal + "' ", cabeza.linea, "");
                }
                //CAHR
                if (tipo_variable_principal == "CHAR")
                {
                    errores.Rows.Add(cabeza.toquen, "Error con '" + cabeza.lexema + "' " + "No se puede convertir implicitamente el tipo '" + "DOUBLE" + "' en '" + tipo_variable_principal + "' ", cabeza.linea, "");
                }
                cabeza = cabeza.siguiente;
            }
            //si es una cadena
            else if (cabeza.toquen == 125)
            {
                //STRING
                if (tipo_variable_principal != "STRING")
                {
                    errores.Rows.Add(cabeza.toquen, "Error con '" + cabeza.lexema + "' " + "No se puede convertir implicitamente el tipo '" + "STRING" + "' en '" + tipo_variable_principal + "' ", cabeza.linea, "");
                }
                cabeza = cabeza.siguiente;
            }
            //si es un caracter
            else if (cabeza.toquen == 128)
            {
                //STRING
                if (tipo_variable_principal != "CHAR")
                {
                    errores.Rows.Add(cabeza.toquen, "Error con '" + cabeza.lexema + "' " + "No se puede convertir implicitamente el tipo '" + "STRING" + "' en '" + tipo_variable_principal + "' ", cabeza.linea, "");
                }
                cabeza = cabeza.siguiente;
            }
            //si es operador
            else if (cabeza.toquen >= 103 && cabeza.toquen <= 117)
            {
                cabeza = cabeza.siguiente;
                cabeza = INCOMPATIBILIDAD1(cabeza);
            }
            //si es true o false
            else if (cabeza.toquen == 210|| cabeza.toquen == 211)
            {
                //BOOLEAN
                if (tipo_variable_principal != "BOOLEAN")
                {
                    errores.Rows.Add(cabeza.toquen, "Error con '" + cabeza.lexema + "' " + "No se puede convertir implicitamente el tipo '" + "BOOLEAN" + "' en '" + tipo_variable_principal + "' ", cabeza.linea, "");
                }
                cabeza = cabeza.siguiente;
            }            
            //si es (
            else if (cabeza.toquen == 122)
            {
                cabeza = cabeza.siguiente;
                cabeza = INCOMPATIBLE(cabeza);
                //si es )
                if (cabeza.toquen == 123)
                {                    
                    cabeza = cabeza.siguiente;
                }                
            }
            return cabeza;
        }       
        //metodo para verificar las segundas expresiones2
        public CrearNodo INCOMPATIBILIDAD2(CrearNodo cabeza)
        {
            //si es un operador
            if (cabeza.toquen >= 103 && cabeza.toquen <= 117 || cabeza.toquen == 129 || cabeza.toquen == 130)
            {                
                cabeza = cabeza.siguiente;
                cabeza = INCOMPATIBILIDAD1(cabeza);
                //cabeza = cabeza.siguiente;
                cabeza = INCOMPATIBILIDAD2(cabeza);
            }            
            else
            {
                //no se hace nada
                //cabeza = cabeza.siguiente;               
            }
            return cabeza;
        }
        //metodo para verificar segundas expresiones
        public CrearNodo INCOMPATIBILIDAD(CrearNodo cabeza)
        {
            cabeza = INCOMPATIBILIDAD1(cabeza);
            cabeza = INCOMPATIBILIDAD2(cabeza);
            return cabeza;
        }
        //metodo para checar las expreciones
        public CrearNodo INCOMPATIBLE(CrearNodo cabeza)
        {
            cabeza = INCOMPATIBILIDAD(cabeza);
            return cabeza;
        }
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
                            //declaraciones en emsamblador
                            if (li.tipo == "INTEGER" || li.tipo == "DOUBLE" || li.tipo == "BOOLEAN")
                            {

                                VARIABLES +=li.lexe+ "	 DW 	 ?\n";
                            }                          
                            if (li.tipo == "STRING")
                            {
                                VARIABLES += li.lexe + "	 DB 	 255 	 DUP('$')\n";                               
                            }
                            //declaraciones en emsamblador

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
                //inserta_cola(cabeza.toquen);
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
            //si es identificador
            else if (cabeza.toquen == 100)
            {
                
                variables(cabeza.lexema);
                                
                if (simple == false)
                {
                    errores.Rows.Add(cabeza.toquen,"Variable '" + cabeza.lexema + "' Inexistente ", cabeza.linea,"");
                    simple = true;
                    esta1 = false;
                }                

                //verificar si es asignacion :=
                if (cabeza.siguiente.toquen == 126)
                {
                    //cabeza=cabeza.siguiente;
                    var li = new CrearNodo3();
                    //para checar el tipo                    
                    tipo_variable_principal = tipo_variable(cabeza.lexema);
                    variable_principal = cabeza.lexema;
                    //para agregar a la lista de valores de variables
                    valor_de_variable = "";
                    li.lexm = cabeza.lexema;
                    //aqui esta en :=
                    cabeza = cabeza.siguiente;
                    //avanzamos
                    cabeza = cabeza.siguiente;
                    //si no se genera una expresion
                    if (cabeza.toquen == 120)
                    {
                        error = busca_error.ERROR(520);
                        grierror.Rows.Add(520, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                        band = false;
                    }
                    else
                    {
                        if (esta1 == true)
                        {
                            INCOMPATIBLE(cabeza);
                        }
                        else if (esta1 == false)
                        {
                            esta1 = true;
                        }
                        cabeza = Expresion(cabeza);

                        //para agregar a la lista
                        li.valor = valor_de_variable;
                        valor_variable.Add(li);
                    }
                }                
                else
                {
                    if (inicia == true)
                    {
                        Buscar_var(cabeza.lexema, cabeza.toquen, cabeza.linea);
                        inicia = false;
                    }
                    
                    valor_de_variable = valor_de_variable + cabeza.lexema;
                    cabeza = cabeza.siguiente;
                }
                
            }
            //si es un numero
            else if (cabeza.toquen == 101)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                //inserta_cola(cabeza.toquen);
                cabeza = cabeza.siguiente;                               
            }
            //si es un decimal
            else if (cabeza.toquen == 102)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                //inserta_cola(cabeza.toquen);
                cabeza = cabeza.siguiente;
            }
            //si es una cadena
            else if (cabeza.toquen==127)
            {
                valor_de_variable = valor_de_variable + QUITA_COMILLAS(cabeza.lexema); 
                //inserta_cola(cabeza.toquen);
                cabeza = cabeza.siguiente;
            }
            //si es caracter
            else if (cabeza.toquen == 128)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                cabeza = cabeza.siguiente;
            }
            //si es operador
            else if (cabeza.toquen >= 103 && cabeza.toquen <= 117)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                //inserta_cola(cabeza.toquen);
                cabeza = cabeza.siguiente;                
                cabeza = PrimaryExpresion(cabeza);
            }
            //si es true
            else if (cabeza.toquen == 210)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                //inserta_cola(cabeza.toquen);
                cabeza = cabeza.siguiente;
            }
            //si es false
            else if (cabeza.toquen == 211)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                //inserta_cola(cabeza.toquen);
                cabeza = cabeza.siguiente;
            }
            //si es (
            else if(cabeza.toquen == 122)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;
                //inserta_cola(cabeza.toquen);
                cabeza =cabeza.siguiente;
                cabeza = Expresion(cabeza);
                //si es )
                if (cabeza.toquen == 123)
                {
                    valor_de_variable = valor_de_variable + cabeza.lexema;
                    //inserta_cola(cabeza.toquen);
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
            if (cabeza.toquen >= 103 && cabeza.toquen <= 117||cabeza.toquen==129)
            {
                valor_de_variable = valor_de_variable + cabeza.lexema;                
                cabeza = cabeza.siguiente;
                cabeza = PrimaryExpresion(cabeza);               
                cabeza = SecondExpresion2(cabeza);
            }
            else if (cabeza.toquen == 130)
            {
                cabeza = cabeza.siguiente;
                cabeza = PrimaryExpresion(cabeza);
                cabeza = SecondExpresion2(cabeza);
            }            
            else
            {
                //no se hace nada                               
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
            while (cabeza.toquen==100||cabeza.toquen==206||cabeza.toquen==203||cabeza.toquen==212 || cabeza.toquen == 213 || cabeza.toquen == 217|| cabeza.toquen == 300|| cabeza.toquen == 301)
            {
                var li = new CrearNodo3();                
                #region Identificador
                //si es un identificador            
                if (cabeza.toquen == 100)
                {                  
                    variables(cabeza.lexema);
                    
                                      
                    if (simple == false)
                    {
                        errores.Rows.Add(cabeza.toquen,"Variable '" + cabeza.lexema + "' Inexistente ", cabeza.linea,"");
                        simple = true;
                        esta1 = false;
                    }
                    //para checar el tipo                    
                    tipo_variable_principal = tipo_variable(cabeza.lexema);
                    
                    ////para lo de ensamblador en asignaciones aumento y decremento
                    //variable = cabeza.lexema;
                    ////..................................
              
                    variable_principal = cabeza.lexema;

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
                                
                                if (esta1 == true)
                                {
                                    COMP(cabeza);
                                    if (var1 != "" || var2 != "")
                                    {
                                        complemento += "\n\tMOV AX, $1\n\tI_ASIGNAR " + variable_principal + ", AX\n";
                                        var1 = "";
                                        var2 = "";
                                    }
                                    INCOMPATIBLE(cabeza);                                                                       
                                }
                                else if (esta1 == false)
                                {
                                    esta1 = true;
                                }
                                cabeza = Expresion(cabeza);

                                //para agregar a la lista
                                li.valor = valor_de_variable;
                                valor_variable.Add(li);

                                
                                //checar_incopativilidad();                                
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
                    if (cabeza.siguiente.toquen >= 103 && cabeza.siguiente.toquen<109||cabeza.siguiente.toquen>109 && cabeza.siguiente.toquen <= 117)
                    {

                        if (cabeza.siguiente.toquen==113||cabeza.siguiente.toquen==114)
                        {
                            COMP(cabeza);
                            INCOMPATIBLEIF2(cabeza);
                        }
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
                    //si llega =
                    if (cabeza.siguiente.toquen == 109)
                    {
                        cabeza = cabeza.siguiente;
                        error = busca_error.ERROR(521);
                        grierror.Rows.Add(521, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                        band = false;
                        break;
                    }
                    else
                    {
                        error = busca_error.ERROR(522);
                        grierror.Rows.Add(522, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
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
                            //si es identificador y viene con < > <=  <>  >= para checar incompatibilidades que sean iguales
                            if ((cabeza.toquen == 100||cabeza.toquen==210||cabeza.toquen==211) && (cabeza.siguiente.toquen >= 107 && cabeza.siguiente.toquen <= 112 || cabeza.siguiente.toquen == 129))
                            {
                                INCOMPATIBLEIF(cabeza);
                            }
                            cabeza = Expresion(cabeza);                            
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

                            if (inicia == true)
                            {
                                Buscar_var(cabeza.lexema, cabeza.toquen, cabeza.linea);
                                inicia = false;
                            }

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
                                    if (inicia == true)
                                    {
                                        Buscar_var(cabeza.lexema, cabeza.toquen, cabeza.linea);
                                        inicia = false;
                                    }
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

                #region CHECAR
                
                #region escribir
                if (cabeza.toquen == 300)
                {
                    cabeza = cabeza.siguiente;
                    if (cabeza.toquen == 122)
                    {
                        cabeza = cabeza.siguiente;
                        while (cabeza.toquen == 100||cabeza.toquen==103||cabeza.toquen==127||cabeza.toquen==101||cabeza.toquen==102)
                        {
                            if (cabeza.toquen == 100)
                            {
                                variables(cabeza.lexema);

                                if (simple == false)
                                {
                                    errores.Rows.Add(cabeza.toquen, "Variable '" + cabeza.lexema + "' Inexistente ", cabeza.linea, "");
                                    simple = true;
                                    esta1 = false;
                                }
                                //para checar el tipo                    
                                tipo_variable_principal = tipo_variable(cabeza.lexema);                                
                                if (tipo_variable_principal == "STRING")
                                {
                                    escribir = "\n\tS_COUT " + Buscar_var3(cabeza.lexema)+"\n";
                                    cabeza = cabeza.siguiente;
                                    complemento += escribir;
                                }
                                if(tipo_variable_principal == "INTEGER"|| tipo_variable_principal == "DOUBLE")
                                {
                                    escribir = "\n\tI_COUT " + Buscar_var3(cabeza.lexema) + "\n";
                                    cabeza = cabeza.siguiente;
                                    complemento += escribir;
                                }                                                              

                            }
                            if (cabeza.toquen == 127)
                            {
                                while (cabeza.toquen == 127)
                                {
                                    escribir = "\n\tS_COUT " + cabeza.lexema + "";
                                    //si viene una suma
                                    if (cabeza.siguiente.toquen == 103)
                                    {
                                        cabeza = cabeza.siguiente;
                                        while (cabeza.toquen == 103)
                                        {
                                            cabeza = cabeza.siguiente;
                                            //cabeza = cabeza.siguiente;
                                            //si viene una cadena
                                            if (cabeza.toquen == 127)
                                            {
                                                escribir += " " + cabeza.lexema;
                                                cabeza = cabeza.siguiente;
                                            }
                                            else
                                            {                                            
                                                break;
                                            }

                                        }
                                        if (cabeza.toquen != 103)
                                        {
                                            escribir += "\n";
                                        }
                                    }
                                    else
                                    {
                                        escribir += "\n";
                                        cabeza = cabeza.siguiente;
                                        break;
                                    }
                                }
                                complemento += escribir;                                
                            }
                            if (cabeza.toquen == 101 || cabeza.toquen == 102)
                            {
                                escribir = "\n\tI_COUT " + cabeza.lexema + "\n";
                                cabeza = cabeza.siguiente;
                                complemento += escribir;
                            }
                                                        
                        }
                        if (cabeza.toquen ==123)
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
                        error = busca_error.ERROR(514);
                        grierror.Rows.Add(514, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                        band = false;
                        break;
                    }
                }
                #endregion

                #region leer
                if (cabeza.toquen == 301)
                {
                    cabeza = cabeza.siguiente;
                    if (cabeza.toquen == 122)
                    {
                        cabeza = cabeza.siguiente;
                        while (cabeza.toquen == 100)
                        {
                            leer += cabeza.lexema;
                            cabeza = cabeza.siguiente;

                        }
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
                        error = busca_error.ERROR(514);
                        grierror.Rows.Add(514, "Se encontro '" + cabeza.lexema + "' y provoco un error, " + error, cabeza.linea, "");
                        band = false;
                        break;
                    }
                }
                #endregion

                
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
                            //si es identificador y viene con < > <=  <>  >= para checar incompatibilidades que sean iguales
                            if ((cabeza.toquen==100)&& (cabeza.siguiente.toquen >= 107 && cabeza.siguiente.toquen <= 112 || cabeza.siguiente.toquen == 129))
                            {
                                INCOMPATIBLEIF(cabeza);
                            }                                                      
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
                                    //si es identificador o numero y viene con < > <=  <>  >= no lo deve de aseptar
                                    if ((cabeza.toquen >=100 && cabeza.toquen<=102) && (cabeza.siguiente.toquen >= 107 && cabeza.siguiente.toquen <= 114 || cabeza.siguiente.toquen == 129))
                                    {
                                        COMP(cabeza);
                                        INCOMPATIBLEIF2(cabeza);
                                    }
                                    cabeza = Expresion(cabeza);                                   
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
            conteo = 0;
            conteo1 = 0;
            conteo01 =0;
            var1 = "";
            var2 = "";
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
                            if (cabeza.toquen == 100 || cabeza.toquen == 206 || cabeza.toquen == 203 || cabeza.toquen == 212 || cabeza.toquen == 213 ||cabeza.toquen==217|| cabeza.toquen == 300|| cabeza.toquen == 301)
                            {
                                while (cabeza.toquen == 100 || cabeza.toquen == 206 || cabeza.toquen == 203 || cabeza.toquen == 212 || cabeza.toquen == 213 || cabeza.toquen == 217|| cabeza.toquen == 300 || cabeza.toquen == 301)
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
                                    if (errores.Rows.Count == 0)
                                    {
                                        DECLARACIONES_STRING();
                                        GENERAR_ENSAMBLADOR();
                                    }
                                    Llenar_tabla();
                                    lista_declarados.Clear();
                                    valor_variable.Clear();
                                    
                                    break;                                    
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

        #region Ensamblador
        //variables

        //variables nuevas a usar
        public string VARIABLES, ASIGNACIONES_STRING;

        public string escribir,leer, header, codigofinal, final1,final2,complemento,asig,au,de;
        public int conteo = 0,conteo1=0, conteo01=0;
        public void GENERAR_ENSAMBLADOR()
        {
            
            header = "INCLUDE macros.mac DOSSEG .MODEL SMALL STACK 100h .DATA $BLANCOS DB '$' \n$MENOS DB '-$' $COUNT DW 0 \n$NEGATIVO DB  0 \n$BUFFER DB  8   DUP('$') \n$BUFFERTEMP DB 8   DUP('$') \n$LISTAPAR LABEL BYTE \n$LONGMAX DB 255 $TOTCAR DB ?  \n$INTRODUCIDOS DB 255 DUP('$') \n$S_TEMP DB  255 DUP('$'); STRING TEMPORAL  \n$I_TEMP DW  0000H; INT TEMPORAL  \n$CONCAT DB  255 DUP('$') \n$1     DW  0000H \n$2     DW  0000H \n$3     DW  0000H \n\n\n";
            final1 = ".CODE \n.386 \nBEGIN: \n\tMOV AX, @DATA \n\tMOV DS, AX \n\tCALL COMPI \n\tMOV AX, 4C00H \n\tINT 21H \n\tCOMPI  PROC \n";
            final2 = "\n\tRET COMPI  \n\tENDP INCLUDE subs.sub \nEND BEGIN" + "\n";

            VARIABLES += "\n\n";
            ASIGNACIONES_STRING += "\n\n";            
            complemento += "\n\n";
            //concat += "\n\n";
            codigofinal = header+VARIABLES+ASIGNACIONES_STRING+final1+complemento+final2;

            //codigofinal =header+declaraciones+declaracionesstring+ /*concat + */ impresionesstring+final1+complemento+final2;
            //guardar el codigo generado
            System.IO.StreamWriter file = new System.IO.StreamWriter("C:/Compilador/CompiladorTriangulo/CompiladorTriangulo/ensamblador.txt");
            file.WriteLine(codigofinal);
            file.Close();            
        }

        #endregion

        #region manipulacion de variables


        //PARA BUSCAR DECLARACIONES EN STRING
        public void DECLARACIONES_STRING()
        {
            int a = 0;
            string tip, lex, lex1;
            for (int i = a; i < lista_declarados.Count; i++)
            {
                lex = lista_declarados[i].lexe;
                tip = lista_declarados[i].tipo;

                for (int j = 0; j < valor_variable.Count; j++)
                {
                    lex1 = valor_variable[j].lexm;
                    if (lex == lex1 && tip=="STRING")
                    {                       
                        ASIGNACIONES_STRING += "@TmpASM_" + conteo + " DB '" + valor_variable[j].valor + "','$' \n";
                        conteo++;
                    }                    
                }                
            }            
        }
        //metodo para quitar las commillas en los string
        public string QUITA_COMILLAS(string texto)
        {            
            int t = texto.Length;
            t = t - 2;            
            texto = texto.Substring(1, t);
            return texto;
        }

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
                        aux = valor_variable[j].valor;
                        
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

        public string cadena;
        public string Buscar_var3(string lexema)
        {                      
            for (int i = 0; i < valor_variable.Count; i++)
            {
                if (valor_variable[i].lexm == lexema)
                {
                    if (valor_variable[i].valor != "")
                    {
                        cadena = valor_variable[i].valor;                       
                    }
                }
            }
            return cadena;
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
                else
                {
                    esta = false;
                }
            }
            if (esta == true)
            {
                esta = false;
                inicia = true;
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

        #region Infix To Postfix
        public Boolean BAND;
        public string[] PILA = new string[1000];
        public string texto, DATO, EPOS, aux1, aux2,postfijo;
        public int TOPE, MAX=100, puntero, prioridad, prioridad_pila;
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