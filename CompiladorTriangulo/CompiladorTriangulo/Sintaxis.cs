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

        public string error;
        /*   verificar errores     agregar variable   encuentra variable   analisis completo    doble declarada     variable declarada una ves*/
        public Boolean band = true,  agregar = true,   esta = false,       correcto = true,     doble = true,       simple = true;
        private CrearNodo cabeza;        
        public DataGridView grierror,declarados,errores;
 

        public struct CrearNodo2
        {
            public string lexe;
            public string tipo;
            public string dato;            
        }

        public static List<CrearNodo2> lista_declarados = new List<CrearNodo2>();
        

        public Sintaxis(CrearNodo cabeza)
        {
            this.cabeza = cabeza;
        }

        Errores busca_error = new Errores();

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
                esta=false;
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
                        errores.Rows.Add("Variable " + cabeza.lexema + " Declarada Anteriormente ", cabeza.linea);
                        doble = true;
                    }
                    cabeza = cabeza.siguiente;                    
                    //si es :
                    if (cabeza.toquen == 11 || cabeza.toquen == 121)
                    {
                        cabeza = cabeza.siguiente;
                        //      integer                     char                  string                double
                        if (cabeza.toquen == 214 || cabeza.toquen == 215 || cabeza.toquen == 216 || cabeza.toquen == 218)
                        {
                            //para poner el tipo de dato el la lista
                            li.tipo = cabeza.lexema;

                            //para agregar al datagried el contenido
                            lista_declarados.Add(li);
                            declarados.Rows.Add(li.tipo,li.lexe,li.dato);
                            
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
                                grierror.Rows.Add(error,511,cabeza.lexema,cabeza.linea);
                                band = false;
                                break;
                            }
                        }
                        else
                        {
                            error = busca_error.ERROR(521);
                            grierror.Rows.Add(error, 521, cabeza.lexema, cabeza.linea);
                            band = false;
                            break;
                        }
                    }
                    else
                    {
                        error = busca_error.ERROR(519);
                        grierror.Rows.Add(error,519,cabeza.lexema,cabeza.linea);
                        band = false;
                        break;
                    }                    
                }
                else
                {
                    error = busca_error.ERROR(515);
                    grierror.Rows.Add(error,515,cabeza.lexema,cabeza.linea);
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
                    errores.Rows.Add("Variable " + cabeza.lexema + " Inexistente ", cabeza.linea);
                    simple = true;
                }
                cabeza = cabeza.siguiente;
            }
            //si es un numero
            else if (cabeza.toquen == 101)
            {
                
                cabeza = cabeza.siguiente;
                               
            }
            //si es un decimal
            else if (cabeza.toquen == 102)
            {
                cabeza = cabeza.siguiente;
            }
            //si es operador
            else if (cabeza.toquen >= 103 && cabeza.toquen <= 117)
            {
                cabeza = cabeza.siguiente;                
                cabeza = PrimaryExpresion(cabeza);
            }
            //si es true
            else if (cabeza.toquen == 210)
            {
                cabeza = cabeza.siguiente;
            }
            //si es false
            else if (cabeza.toquen == 211)
            {
                cabeza = cabeza.siguiente;
            }
            //si es (
            else if(cabeza.toquen == 122)
            {
                cabeza=cabeza.siguiente;
                cabeza = Expresion(cabeza);
            }
            return cabeza;
        }

        //metodo para verificar las segundas expresiones2
        public CrearNodo SecondExpresion2(CrearNodo cabeza)
        {
            //si es un operador
            if (cabeza.toquen >= 103 && cabeza.toquen <= 117)
            {
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
                            grierror.Rows.Add(error, 520, cabeza.lexema, cabeza.linea);
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
                                                        grierror.Rows.Add(error, 513, cabeza.lexema, cabeza.linea);
                                                        band = false;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    error = busca_error.ERROR(512);
                                                    grierror.Rows.Add(error, 512, cabeza.lexema, cabeza.linea);
                                                    band = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            error = busca_error.ERROR(513);
                                            grierror.Rows.Add(error, 513, cabeza.lexema, cabeza.linea);
                                            band = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        error = busca_error.ERROR(508);
                                        grierror.Rows.Add(error, 508, cabeza.lexema, cabeza.linea);
                                        band = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    error = busca_error.ERROR(512);
                                    grierror.Rows.Add(error, 512, cabeza.lexema, cabeza.linea);
                                    band = false;
                                    break;
                                }
                            }
                            else
                            {
                                error = busca_error.ERROR(516);
                                grierror.Rows.Add(error, 516, cabeza.lexema, cabeza.linea);
                                band = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        error = busca_error.ERROR(514);
                        grierror.Rows.Add(error, 514, cabeza.lexema, cabeza.linea);
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
                        grierror.Rows.Add(error, 508, cabeza.lexema, cabeza.linea);
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
                #region Identificador
                //si es un identificador            
                if (cabeza.toquen == 100)
                {                  
                    variables(cabeza.lexema);
                                      
                    if (simple == false)
                    {
                        errores.Rows.Add("Variable " + cabeza.lexema + " Inexistente ", cabeza.linea);
                        simple = true;
                    }

                    if (cabeza.siguiente.toquen == 126)
                    {
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
                                grierror.Rows.Add(error, 520, cabeza.lexema, cabeza.linea);
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
                                //si es ;
                                if (cabeza.toquen == 120)
                                {
                                    cabeza = cabeza.siguiente;
                                    break;
                                }
                                else
                                {
                                    error = busca_error.ERROR(511);
                                    grierror.Rows.Add(error, 511, cabeza.lexema, cabeza.linea);
                                    band = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            error = busca_error.ERROR(510);
                            grierror.Rows.Add(error, 510, cabeza.lexema, cabeza.linea);
                            band = false;
                            break;
                        }
                        #endregion
                    }
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
                                grierror.Rows.Add(error, 511, cabeza.lexema, cabeza.linea);
                                band = false;
                                break;
                            }
                        }
                        else
                        {
                            error = busca_error.ERROR(520);
                            grierror.Rows.Add(error, 520, cabeza.lexema, cabeza.linea);
                            band = false;
                            break;
                        }
                        #endregion
                    }
                    else
                    {
                        error = busca_error.ERROR(521);
                        grierror.Rows.Add(error, 521, cabeza.lexema, cabeza.linea);
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
                            grierror.Rows.Add(error, 520, cabeza.lexema, cabeza.linea);
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
                                                        grierror.Rows.Add(error, 513, cabeza.lexema, cabeza.linea);
                                                        band = false;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    error = busca_error.ERROR(512);
                                                    grierror.Rows.Add(error, 512, cabeza.lexema, cabeza.linea);
                                                    band = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            error = busca_error.ERROR(513);
                                            grierror.Rows.Add(error, 513, cabeza.lexema, cabeza.linea);
                                            band = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        error = busca_error.ERROR(508);
                                        grierror.Rows.Add(error, 508, cabeza.lexema, cabeza.linea);
                                        band = false;
                                        break;
                                    }                                    
                                }
                                else
                                {
                                    error = busca_error.ERROR(512);
                                    grierror.Rows.Add(error, 512, cabeza.lexema, cabeza.linea);
                                    band = false;
                                    break;
                                }
                            }
                            else
                            {
                                error = busca_error.ERROR(516);
                                grierror.Rows.Add(error, 516, cabeza.lexema, cabeza.linea);
                                band = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        error = busca_error.ERROR(514);
                        grierror.Rows.Add(error, 514, cabeza.lexema, cabeza.linea);
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
                            grierror.Rows.Add(error, 520, cabeza.lexema, cabeza.linea);
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
                                        grierror.Rows.Add(error, 513, cabeza.lexema, cabeza.linea);
                                        band = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    error = busca_error.ERROR(512);
                                    grierror.Rows.Add(error, 512, cabeza.lexema, cabeza.linea);
                                    band = false;
                                    break;
                                }
                            }
                            else
                            {
                                error = busca_error.ERROR(516);
                                grierror.Rows.Add(error, 516, cabeza.lexema, cabeza.linea);
                                band = false;
                                break;
                            }                            
                        }
                    }
                    else
                    {
                        error = busca_error.ERROR(514);
                        grierror.Rows.Add(error, 514, cabeza.lexema, cabeza.linea);
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
                                errores.Rows.Add("La variable " + cabeza.lexema + " Inexistente ", cabeza.linea);
                                simple = true;
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
                                    grierror.Rows.Add(error, 511, cabeza.lexema, cabeza.linea);
                                    band = false;
                                    break;
                                }
                            }
                            else
                            {
                                error = busca_error.ERROR(516);
                                grierror.Rows.Add(error, 516, cabeza.lexema, cabeza.linea);
                                band = false;
                                break;
                            }
                        }
                        else
                        {
                            error = busca_error.ERROR(515);
                            grierror.Rows.Add(error, 515, cabeza.lexema, cabeza.linea);
                            band = false;
                            break;
                        }
                    }
                    else
                    {
                        error = busca_error.ERROR(514);
                        grierror.Rows.Add(error, 514, cabeza.lexema, cabeza.linea);
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
                                        errores.Rows.Add("La variable " + cabeza.lexema + " Inexistente ", cabeza.linea);
                                        simple = true;
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
                                            grierror.Rows.Add(error, 511, cabeza.lexema, cabeza.linea);
                                            band = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        error = busca_error.ERROR(516);
                                        grierror.Rows.Add(error, 516, cabeza.lexema, cabeza.linea);
                                        band = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    error = busca_error.ERROR(515);
                                    grierror.Rows.Add(error, 515, cabeza.lexema, cabeza.linea);
                                    band = false;
                                    break;
                                }
                            }
                            else
                            {
                                error = busca_error.ERROR(518);
                                grierror.Rows.Add(error, 518, cabeza.lexema, cabeza.linea);
                                band = false;
                                break;
                            }
                        }
                        else
                        {
                            error = busca_error.ERROR(517);
                            grierror.Rows.Add(error, 517, cabeza.lexema, cabeza.linea);
                            band = false;
                            break;
                        }
                    }
                    else
                    {
                        error = busca_error.ERROR(514);
                        grierror.Rows.Add(error, 514, cabeza.lexema, cabeza.linea);
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
                            grierror.Rows.Add(error, 520, cabeza.lexema, cabeza.linea);
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
                                    grierror.Rows.Add(error, 520, cabeza.lexema, cabeza.linea);
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
                                                    grierror.Rows.Add(error, 513, cabeza.lexema, cabeza.linea);
                                                    band = false;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                error = busca_error.ERROR(508);
                                                grierror.Rows.Add(error, 508, cabeza.lexema, cabeza.linea);
                                                band = false;
                                                break;
                                            }
                                            
                                        }
                                        else
                                        {
                                            error = busca_error.ERROR(512);
                                            grierror.Rows.Add(error, 512, cabeza.lexema, cabeza.linea);
                                            band = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        error = busca_error.ERROR(516);
                                        grierror.Rows.Add(error, 516, cabeza.lexema, cabeza.linea);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                error = busca_error.ERROR(511);
                                grierror.Rows.Add(error, 511, cabeza.lexema, cabeza.linea);
                                break;
                            }
                        }

                    }
                    else
                    {
                        error = busca_error.ERROR(515);
                        grierror.Rows.Add(error, 515, cabeza.lexema, cabeza.linea);
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
                                    if (correcto == false)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Analisis Sintactico Exitoso");
                                        lista_declarados.Clear();
                                        break;
                                    }                                    
                                }
                                else
                                {
                                    error = busca_error.ERROR(509);
                                    grierror.Rows.Add(error, 509, cabeza.lexema, cabeza.linea);
                                    band = false;
                                    break;
                                }
                            }
                            else
                            {
                                error = busca_error.ERROR(508);
                                grierror.Rows.Add(error, 508, cabeza.lexema, cabeza.linea);
                                band = false;
                                break;
                            }

                        }
                        else
                        {
                            error = busca_error.ERROR(507);
                            grierror.Rows.Add(error, 507, cabeza.lexema, cabeza.linea);
                            band = false;
                            break;
                        }
                    }
                    else
                    {
                        error = busca_error.ERROR(506);
                        grierror.Rows.Add(error, 506, cabeza.lexema, cabeza.linea);
                        band = false;
                        break;
                    }
                }
                else
                {
                    error = busca_error.ERROR(505);
                    grierror.Rows.Add(error, 505, cabeza.lexema, cabeza.linea);
                    band = false;
                    break;
                }
                #endregion
            }
        }
    }
}
