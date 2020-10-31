using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.ComponentModel;

namespace Socket_Servidor
{
    class Servidor
    {
        //TcpListener espera la conexion del cliente
        private TcpListener servidor;
        //TcpClient proporciona conexion entre servidor/cliente 
        private TcpClient cliente = new TcpClient();
        /*IpEndPoint y IPAdress toma la ip y puerto
         * de la computadora en la que se estan ejecutando*/
        private IPEndPoint Endp = new IPEndPoint(IPAddress.Any, 8080);
        //Mostara la informacion de conexion
        private List<Conexion> lista = new List<Conexion>();

        Conexion con;

        private struct Conexion
        {
            //Informacion que se mostrara al conectar un usuario al servidor
            public NetworkStream stream;
            public StreamWriter escribe;
            public StreamReader lee;
            //Variable para identificar el usuario
            public string nomusuario;
        }

        public Servidor()
        {
            Iniciar();
        }

        //Se ejecuta el sistema
        public void Iniciar()
        {
            Console.WriteLine("Conectado al Servidor");
            servidor = new TcpListener(Endp);
            servidor.Start();

            while (true)
            {
                cliente = servidor.AcceptTcpClient();

                con = new Conexion();
                //hace la conexion con el cliente
                con.stream = cliente.GetStream();
                //lee lo que se esta enviando 
                con.lee = new StreamReader(con.stream);
                //permite escribir en el chat 
                con.escribe = new StreamWriter(con.stream);
                //muestra el nombre del usuario
                con.nomusuario = con.lee.ReadLine();
                // y se agrega en la lista de conexion 
                lista.Add(con);
                //muestra nombre del usuario que se a conectado
                Console.WriteLine(con.nomusuario + " se a conectado");
                //Mantiene la conexion 
                Thread Td = new Thread(Escucha_conexion);
                Td.Start();
            }
        }

        private void Escucha_conexion()
        {//accede a la conexion 
            Conexion conn = con;

            do
            {
                try
                {/*condicion para verificar si el cliente-usuario
                 * se a podido conectar con el servidor*/
                    string tmp = conn.lee.ReadLine();
                    //muestra usuario y mensaje enviado
                    Console.WriteLine(conn.nomusuario + ": " + tmp);
                    foreach (Conexion c in lista)
                    {//mostrara el listado de datos leidos
                        try
                        {
                            c.escribe.WriteLine(conn.nomusuario + ": " + tmp);
                            c.escribe.Flush();
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {/*si el usuario se desconecta no se guardara la informacion se removera
                  * y mostrara que el usuario se a desconectad*/
                    lista.Remove(con);
                    Console.WriteLine(conn.nomusuario + " se ha desconectado");
                    break;//sale usuario del servidor 
                }
            } while (true);
        }
    }
}
