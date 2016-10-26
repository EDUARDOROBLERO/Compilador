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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string nomarchivox;        

        public void abrirarchivo()
        {
            try
            {
                OpenFileDialog dialogotxt = new OpenFileDialog();
                dialogotxt.Filter = "Archivos Triangulo|*.try";
                //directorio inicial
                dialogotxt.InitialDirectory = "C:/";
                dialogotxt.ShowDialog();
                if (File.Exists(dialogotxt.FileName))
                {
                    using (Stream stream = dialogotxt.OpenFile())
                    {
                        leerarchivo(dialogotxt.FileName);
                        nomarchivox = dialogotxt.FileName;
                        textBox2.Text = dialogotxt.FileName;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("El archivo no se abrio correctamente");
            }
        }

        public void leerarchivo(string nomarchivo)
        {
            StreamReader sr = new StreamReader(nomarchivo, System.Text.Encoding.Default);
            string texto;
            texto = sr.ReadToEnd();
            sr.Close();
            textBox1.Text = texto;
        }

        public void guardaArchivo()
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Archivos Triangulo|*.try";
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(saveFile.FileName))
                {                                         
                    //------------------ para sobrescribir el texto ...................
                    StreamWriter codigonuevo = File.CreateText(saveFile.FileName);
                    codigonuevo.Write(textBox1.Text);
                    codigonuevo.Flush();
                    codigonuevo.Close();
                    nomarchivox = saveFile.FileName;
                    textBox2.Text = saveFile.FileName;
                }
                else
                {
                    // el archivo no extiste
                    StreamWriter codigonuevo = File.CreateText(saveFile.FileName);                                        
                    codigonuevo.Write(textBox1.Text);
                    codigonuevo.Flush();
                    codigonuevo.Close();
                    nomarchivox = saveFile.FileName;
                    textBox2.Text = saveFile.FileName;
                }
            }
        }

        public void guardaArchivo2(string nomarchivo)
        {
            try
            {
                if (nomarchivo == null)
                {
                    guardaArchivo();
                }
                else
                {
                    // el archivo nuevo
                    StreamWriter codigonuevo = File.CreateText(nomarchivo);
                    codigonuevo.Write(textBox1.Text);
                    codigonuevo.Flush();
                    codigonuevo.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("error al guardar");
            }        
        }

        public bool revisasiarchivoexiste(string nomarchivo)
        {
            bool existe;
            if (File.Exists(nomarchivo))
            {
                // el archivo existe
                existe = true;
            }
            else
            {
                // el archivo no extiste
                existe = false;
            }
            return existe;
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abrirarchivo();
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guardaArchivo2(nomarchivox);
            MessageBox.Show("Archivo Modificado","Informacion",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guardaArchivo();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            NotifyIcon notificacion = new NotifyIcon();

            notificacion.Text = "Compilador Triangulo";
            notificacion.BalloonTipTitle = "Hola";
            notificacion.BalloonTipText = "Bienvenido al Compilador";
            notificacion.BalloonTipIcon = ToolTipIcon.Info;                    
            notificacion.ShowBalloonTip(5000);           
        }

        private void lexicoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1Lexico.Rows.Clear();            
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();

            Tabla_De_Errores.Rows.Clear();
            string text = textBox1.Text;

            //analisis lexico
            lexico form = new lexico(text);
            form.gridtoken = dataGridView1Lexico;
            form.griderror = Tabla_De_Errores;
            form.analizador();

            if (Tabla_De_Errores.Rows.Count == 0)
            {
                MessageBox.Show("Analisis lexico finalizado con exito");
               

            }
            else
            {
                MessageBox.Show("Analisis lexico finalizado con errores");
            }
        }

        private void sintaxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1Lexico.Rows.Clear();            
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            Tabla_De_Errores.Rows.Clear();
            string text = textBox1.Text;            


            //analisis lexico
            lexico form = new lexico(text);
            form.gridtoken = dataGridView1Lexico;

            form.griderror = Tabla_De_Errores;            
            form.analizador();

            if (Tabla_De_Errores.Rows.Count == 0)
            {
                //analisis sintactico
                Sintaxis form2 = new Sintaxis(form.cabeza);
                form2.grierror = Tabla_De_Errores;
                form2.declarados = dataGridView2;

                form2.errores = Tabla_De_Errores;
                form2.analisador();
            }
            else
            {
                MessageBox.Show("Analisis lexico finalizado con errores");
            }
        }

        private void todoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1Lexico.Rows.Clear();           
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            string text = textBox1.Text;

            //analisis lexico
            lexico form = new lexico(text);
            form.gridtoken = dataGridView1Lexico;
            form.griderror = Tabla_De_Errores;
            form.analizador();

            //analisis sintactico
            Sintaxis form2 = new Sintaxis(form.cabeza);
            form2.grierror = Tabla_De_Errores;
            form2.analisador();

            if (Tabla_De_Errores.Rows.Count == 0)
            {
                                               
            }
            else
            {
                MessageBox.Show("Analisis finalizado con errores");
            }
        }

        private void limpiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1Lexico.Rows.Clear();            
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            Tabla_De_Errores.Rows.Clear();
            textBox1.Clear();            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            /*if (textBox1.Text == "")
            {

            }
            if (textBox1.Text != "")
            {
                dataGridView1Lexico.Rows.Clear();
                dataGridView2errores.Rows.Clear();
                dataGridView1.Rows.Clear();
                string text = textBox1.Text;

                //analisis lexico
                lexico form = new lexico(text);
                form.gridtoken = dataGridView1Lexico;
                form.griderror = dataGridView2errores;
                form.analizador();

                //analisis sintactico
                Sintaxis form2 = new Sintaxis(form.cabeza);
                form2.grierror = dataGridView1;
                form2.analisador();
                
            }*/
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F7)
            {
                dataGridView1Lexico.Rows.Clear();
                dataGridView2.Rows.Clear();
                dataGridView3.Rows.Clear();
                Tabla_De_Errores.Rows.Clear();
                string text = textBox1.Text;


                //analisis lexico
                lexico form = new lexico(text);
                form.gridtoken = dataGridView1Lexico;

                form.griderror = Tabla_De_Errores;
                form.analizador();

                if (Tabla_De_Errores.Rows.Count == 0)
                {
                    //analisis sintactico
                    Sintaxis form2 = new Sintaxis(form.cabeza);
                    form2.grierror = Tabla_De_Errores;
                    form2.declarados = dataGridView2;

                    form2.errores = Tabla_De_Errores;
                    form2.analisador();
                }
                else
                {
                    MessageBox.Show("Analisis lexico finalizado con errores");
                }
            }
        }
    }

}
