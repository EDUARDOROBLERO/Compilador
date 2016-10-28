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
    class lexico
    {
        public int[,] MTri;
        public string text, lexema, error,tipo;
        public int toquen=0,linea=1,puntero=0,edo=0,col=0,tok;
        public char caracter;
        public CrearNodo P, Q, cabeza;
        public DataGridView griderror,gridtoken;        

        public lexico(string texto)
        {
            text = texto;            
            #region Matriz de Transición

            MTri = new int[,]
            {
            //       0       1       2       3        4      5       6       7        8      9       10      11      12      13      14      15       16      17      18      19      20     21      22      23      24      25       26      27*/
           /*Q\W  // L		 D		 +		 -		  *		 /		 >		 <		  =		 \		  .		  ,		  ;		 :		  (		  )	      {       }       !		  "		  '	     &&      eb	     tab 	 nl		 eol	 eof	  OC
            /*0*/{  1   ,   2   ,   5   ,   6   ,   105 ,   7   ,   8   ,   9   ,   109 ,   10  ,   118 ,   119 ,   120 ,   11  ,   122 ,   123 ,   124 ,   125 ,   12  ,   13  ,   14  ,   16  ,   0   ,   0   ,   0   ,   0   ,   0   ,   500 },
            /*1*/{   1   ,   1   ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 ,   100 },
            /*2*/{   101 ,   2   ,   101 ,   101 ,   101 ,   101 ,   101 ,   101 ,   101 ,   101 ,   3   ,   101 ,   101 ,   101 ,   101 ,   101 ,   101 ,   101 ,   101 ,   101 ,   101 ,   101 ,   101 ,   101 ,   101 ,   101 ,   101 ,   101 },
            /*3*/{   500 ,   4   ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 ,   500 },
            /*4*/{   102 ,   4   ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 ,   102 },
            /*5*/{   103 ,   103 ,   113 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 ,   103 },
            /*6*/{   104 ,   104 ,   104 ,   114 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 ,   104 },
            /*7*/{   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   115 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 ,   106 },
            /*8*/{   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   111 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 ,   107 },
            /*9*/{   108 ,   108 ,   108 ,   108 ,   108 ,   108 ,   129 ,   108 ,   110 ,   108 ,   108 ,   108 ,   108 ,   108 ,   108 ,   108 ,   108 ,   108 ,   108 ,   108 ,   108 ,   108 ,   108 ,   108 ,   108 ,   108 ,   108 ,   108 },
           /*10*/{   117 ,   117 ,   117 ,   117 ,   117 ,   116 ,   117 ,   117 ,   112 ,   117 ,   117 ,   117 ,   117 ,   117 ,   117 ,   117 ,   117 ,   117 ,   117 ,   117 ,   117 ,   117 ,   117 ,   117 ,   117 ,   117 ,   117 ,   117 },
           /*11*/{   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   126 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 ,   121 },
           /*12*/{   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  ,   12  },
           /*13*/{   13  ,   13  ,   13  ,   13  ,   13  ,   13  ,   13  ,   13  ,   13  ,   13  ,   13  ,   13  ,   13  ,   13  ,   13  ,   13  ,   13  ,   13  ,   13  ,   125 ,   13  ,   13  ,   13  ,   501 ,   501 ,   501 ,   501 ,   13  },
           /*14*/{   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   502 ,   503 ,   503 ,   503 ,   503 ,   503 ,   503 ,   15  },
           /*15*/{   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   126 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 ,   504 },
           /*16*/{   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 ,   128 ,   524 ,   524 ,   524 ,   524 ,   524 ,   524 }
            };

            #endregion
        }

        public int obtener_col(char xcaracter)
        {
            char s = (char)39;//comilla simple
            char i = (char)92;//diagonal invertida
            char b = (char)32;//espacio en blanco                       
            
            switch (xcaracter)
            {
                case '+': col = 2; break;
                case '-': col = 3; break;
                case '*': col = 4; break;
                case '/': col = 5; break;
                case '>': col = 6; break;
                case '<': col = 7; break;                
                case '=': col = 8; break;

                case '.': col = 10;break;
                case ',': col = 11; break;
                case ';': col = 12; break;
                case ':': col = 13; break;
                case '(': col = 14; break;
                case ')': col = 15; break;
                case '{': col = 16;break;
                case '}': col = 17;break;                
                case '!': col = 18; break;
                case '"': col = 19; break;

                case '&': col = 21;break;

                case '\t': col = 23; break;
                case '\n':col = 24;linea = linea + 1; break;
                case '\r': col = 25;break;
                default: col = 27; break;
            }
            //diagonal invertida
            if (xcaracter == i) col = 9;
            //comilla simple       
            if (xcaracter == s) col = 20;
            //espacio en blanco
            if (xcaracter == b) col = 22;                        
            if (char.IsLetter(caracter)) col = 0;
            if (char.IsDigit(caracter)) col = 1;           
            return col;
        }

        public void agregarnodo()
        {
            if (P == null)
            {
                P = new CrearNodo(lexema, toquen, linea, puntero);                
                P.siguiente = null;
                cabeza = P;
            }
            else
            {
                Q = new CrearNodo(lexema, toquen, linea, puntero);             
                Q.siguiente = null;
                P.siguiente = Q;
                P = Q;
            }
        }

        Errores busca_error = new Errores();
        public void analizador()
        {            
            do
            {
                if (caracter == '\n' || caracter == '\r')
                {
                    lexema = "";
                    edo = 0;
                }
                if (text == "") break;               
                caracter = text[puntero];                
                obtener_col(caracter);

                edo = MTri[edo, col];

                #region Codigo Lexico               
                if (edo >=100 && edo < 500)
                {
                    toquen = edo;

                    //cuando es <= || >= || \= || ++ || -- || /\ || \/ || := || "" || '' || && ||<>
                    if (edo >= 110&&edo<=116||edo>=124&&edo<=126||edo==128||edo==129)
                    {
                        lexema += caracter;
                        agregarnodo();
                        tipo = TipoPalabra(toquen);
                        gridtoken.Rows.Add(lexema, tipo, toquen, linea, puntero);
                        edo = 0;
                        lexema = "";
                        puntero += 1;
                        continue;
                    }                                                            
                    if (edo == 100)
                    {
                        toquen = PalabraRecervada(lexema);
                    }
                    if (lexema != ""&&lexema!=null)
                    {
                        agregarnodo();

                        tipo = TipoPalabra(toquen);
                        gridtoken.Rows.Add(lexema, tipo, toquen, linea, puntero);                        
                    }
                    
                    edo = 0;                    
                    if (toquen >= 100)
                    {
                        obtener_col(caracter);
                        edo = MTri[edo, col];
                        if (edo >= 100 && edo < 500)
                        {
                            lexema = caracter.ToString();
                            toquen = edo;
                            if (edo == 100)
                            {
                                toquen = PalabraRecervada(lexema);
                            }

                            agregarnodo();

                            tipo = TipoPalabra(toquen);
                            gridtoken.Rows.Add(lexema, tipo, toquen, linea, puntero);                            
                            edo = 0;
                            string nada = " ";
                            caracter = Convert.ToChar(nada);
                        }
                    }
                    lexema = "";                    
                }
                if (edo >= 500)
                {
                    if (caracter == '\n' || caracter == '\r' || caracter == 32)
                    {

                    }
                    else
                    {
                        lexema = Convert.ToString(caracter);
                    }                    
                    toquen = edo;
                    error = busca_error.ERROR(toquen);
                    griderror.Rows.Add(toquen, "Se encontro '" + lexema + "' y provoco un error, " + error, linea, puntero);
                    edo = 0;
                    lexema = "";                    
                    puntero += 1;
                    continue;
                }
                if (caracter == '\n' || caracter == '\r'||caracter==32)
                {
                    puntero += 1;
                    continue;
                }
                lexema += caracter;
                puntero += 1;
                if (puntero == text.Length)
                {
                    if (edo == 7)
                    {
                        puntero += 1;
                    }
                    else
                    {
                        toquen = edo;
                        if (toquen == 100 || toquen == 1)
                        {
                            toquen = PalabraRecervada(lexema);
                        }
                        if (toquen < 500)
                        {
                            if (toquen == 13)
                            {
                                toquen = 502;
                                error = busca_error.ERROR(toquen);
                                griderror.Rows.Add(toquen, "Se encontro '" + lexema + "' y provoco un error, " + error, linea, puntero);
                                lexema = "";
                                edo = 0;
                                puntero += 1;
                                break;                                 
                            }
                            if (toquen == 15)
                            {
                                toquen = 503;
                                error = busca_error.ERROR(toquen);
                                griderror.Rows.Add(toquen, "Se encontro '" + lexema + "' y provoco un error, " + error, linea, puntero);
                                lexema = "";
                                edo = 0;
                                puntero += 1;
                                break;
                            }
                            if (toquen == 14)
                            {
                                toquen = 504;
                                error = busca_error.ERROR(toquen);
                                griderror.Rows.Add(toquen, "Se encontro '" + lexema + "' y provoco un error, " + error, linea, puntero);
                                lexema = "";
                                edo = 0;
                                puntero += 1;
                                break;
                            }
                            agregarnodo();

                            tipo = TipoPalabra(toquen);
                            gridtoken.Rows.Add(lexema, tipo, toquen, linea, puntero);                                                                                                                                        
                            edo = 0;
                            lexema = "";
                            puntero += 1;
                        }
                        else
                        {
                            error = busca_error.ERROR(toquen);
                            griderror.Rows.Add(toquen, "Se encontro '"+lexema+"' y provoco un error, " + error, linea, puntero);
                            edo = 0;
                            lexema = "";
                            puntero += 1;
                        }
                    }                                        
                }
                if (puntero > text.Length) break;               
                #endregion                               

            } while (text.Length > puntero);
        }

        public int PalabraRecervada(string lexem)
        {            
            switch (lexem)
            {
                //casos de Let
                case "let":tok = 200;  break;
                case "Let": tok = 200; break;
                case "LET":tok = 200;break;                
                //casos de begin
                case "begin":tok = 201;  break;
                case "Begin": tok = 201; break;
                case "BEGIN": tok = 201;break;
                //casos de do
                case "do":tok = 202;  break;
                case "Do": tok = 202; break;
                case "DO": tok = 202;break;
                //casos de while
                case "while":tok = 203;  break;
                case "While": tok = 203;break;
                case "WHILE":tok = 203;break;                
                //casos de else
                case "else":tok = 204;  break;
                case "Else":tok = 204;break;
                case "ELSE":tok = 204;break;                
                //casos de end
                case "end":tok = 205;  break;
                case"End": tok = 205; break;
                case "END":tok= 205; break;
                //casos de if
                case "if":tok = 206;  break;
                case "If":tok = 206;break;
                case "IF":tok = 206;break;
                //casos de in
                case "in":tok=207;  break;
                case "In":tok = 207;break;
                case "IN":tok = 207;break;
                //casos de then
                case "then": tok = 208;  break;
                case "Then":tok = 208;break;
                case "THEN":tok = 208;break;                
                //casos de var
                case "var": tok = 209;  break;
                case "Var":tok = 209;break;
                case "VAR":tok = 209;break;
                //casos de true
                case "true": tok = 210;  break;
                case "True":tok = 210;break;
                case "TRUE":tok = 210;break;
                //casos de false
                case "false": tok = 211;  break;
                case "False":tok = 211;break;
                case "FALSE":tok = 211;break;
                //casos de put
                case "put": tok = 212;  break;
                case "Put":tok = 212;break;
                case "PUT":tok = 212;break;
                //casos de get
                case "get": tok = 213;  break;
                case "Get":tok = 213;break;
                case "GET":tok = 213;break;
                //casos de integer
                case "integer": tok = 214;  break;
                case "Integer":tok = 214;break;
                case "INTEGER":tok = 214;break;
                //casos de char
                case "char": tok = 215;  break;
                case "Char":tok = 215;break;
                case "CHAR":tok = 215;break;
                //casos de string
                case "string": tok = 216;  break;
                case "String":tok = 216;break;
                case "STRING":tok = 216;break;

                //casos de for
                case "for": tok = 217; break;
                case "For": tok = 217; break;
                case "FOR": tok = 217; break;

                //casos de double
                case "double":tok = 218;break;
                case "Double": tok = 218; break;
                case "DOUBLE": tok = 218; break;

                //casos de boolean
                case "boolean": tok = 219; break;
                case "Boolean": tok = 219; break;
                case "BOOLEAN": tok = 219; break;

                default: tok = toquen;  break;
            }
            return tok;
        }
        
        public string TipoPalabra(int token2)
        {
            if (token2 == 1||token2==100)
            {
                tipo = "Identificador";
                
            }
            else if (token2 == 2||token2==101)
            {
                tipo = "Numero Entero";
                
            }
            else if (token2 == 4 || token2 == 102)
            {
                tipo = "Numero Decimal";

            }
            else if ((token2 >=103&& token2<=106)||token2>=3&&token2<=7)
            {
                tipo = "Operador Aritmetico";
                
            }
            else if (token2==8||token2==9||(token2>=107&&token2<=114))
            {
                tipo = "Operador Relacional";
                
            }
            else if ((token2>=115&&token2<=117)||token2==10||token2==128||token2==129)
            {
                tipo = "Operador Logico";
                
            }
            else if ((token2>=118&&token2<=121)||token2==11)
            {
                tipo = "Simbolo de Puntuacion";
                
            }
            else if (token2==122||token2==123)
            {
                tipo = "Simbolo de Agrupacion";
                
            }
            else if (token2==13||token2==125)
            {
                tipo = "Cadena";
                
            }
            else if (token2>=200&&token2<=219)
            {
                tipo = "Palabra Recervada";
                
            }else if (token2 == 14 || token2 == 126)
            {
                tipo = "Simbolo de asignacion";
            }
            return tipo;
        }        
    }
}
