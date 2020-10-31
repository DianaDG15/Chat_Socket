using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using Transitions;


namespace Chat_ClienteServidor
{
    public partial class Form1 : Form
    {
        //Hace la  conexion con el servidor 
        static private NetworkStream stream;
        static private StreamWriter escribir;
        static private StreamReader leer;
        //permite enviar datos entre varios clientes
        static private TcpClient cliente = new TcpClient();
        //identifica que usuario esta utilizando el chat
        static private string nomusuario = "Desconocido";
        //DaddItem permite mostrar la informcion que se pasa en el servidor 
        private delegate void DaddItem(String s);
        private void AddItem(String s)
        {//añade los datos al listbox como el usuario y comentario
            listBox1.Items.Add(s);
        }

        public Form1()
        {
            InitializeComponent();
        }

        void Esc_Con()
        {//Realiza la conexion 
            while (cliente.Connected)
            {
                try
                {//añade la informacion del metodo convertido en informacion delectura 
                    this.Invoke(new DaddItem(AddItem), leer.ReadLine());
                }
                catch
                {/*Si no se tiene abierto el servidor mostrara este mensaje
                  * y cierra la aplicacion */
                    MessageBox.Show("No se puede conectar al servidor");
                    Application.Exit();
                }
            }
        }

        void Conectar()
        {
            try
            {//Verifica y realiza la conexion estable con el servidor y puerto predeterminado
                cliente.Connect("localhost", 8080);
                if (cliente.Connected)
                {//permite ver al inforamcion de la lista
                    Thread Td = new Thread(Esc_Con);

                    stream = cliente.GetStream();
                    escribir = new StreamWriter(stream);
                    leer = new StreamReader(stream);
                    //muestra el nombre de usuario
                    escribir.WriteLine(nomusuario);
                    //limpia
                    escribir.Flush();
                    //se inicia la conexion
                    Td.Start();
                }
                else
                {
                    MessageBox.Show("Servidor no Disponible");
                }
            }
            catch(Exception ex)
            {//Si no se puede acceder al servidor muestra el mensaje y cierra la ventana de chat
                MessageBox.Show("Servidor no Disponible");
                Application.Exit();
            }
        }

        //Diseño de transicion y conexion de los componentes
        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Location = new Point(-329, 250);
            textBox2.Location = new Point(-329,250);
            listBox1.Location = new Point(-329,23);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {//Boton conectar
            nomusuario = textBox1.Text;
            Conectar();
            //tiempo de transicion de componentes en la interfaz
            Transition t = new Transition(new TransitionType_EaseInEaseOut(900));
            t.add(label1, "Left", 555);
            t.add(textBox1, "Left", 555);
            t.add(button1, "Left", 555);
            t.add(listBox1, "Left", 26);
            t.add(textBox2, "Left", 26);
            t.add(button2, "Left", 283);
            t.run();

        }

        private void button2_Click(object sender, EventArgs e)
        {//Boton enviar mensaje
            escribir.WriteLine(textBox2.Text);
            escribir.Flush();
            textBox2.Clear();
        }
    }
}
