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
using System.Xml.Serialization;
using System.Net;
using static minas.Form3;
using System.Xml;
using System.Net.Http;
namespace minas
{
    public partial class Form3 : Form
    {
        List<Resultados> resultados = new List<Resultados>();
        public string Nombre;
        public int FILAS;
        public double chrono;

        string fondo2 = "https://img.freepik.com/fotos-premium/sapper-ejercito-limpiando-campo-minas-tonos-terrenales-pops-o-diseno-cartel-2d-a4-ideas-creativas_655090-1124136.jpg";
        public Form3(int FILAS, double chrono)
        {
            InitializeComponent();  
            this.FILAS = FILAS;
            this.chrono = chrono;
        }

        private async void Form3_Load(object sender, EventArgs e)
        
            {
                // Descargar la imagen desde Internet
                using (HttpClient client = new HttpClient())
                {
                    byte[] imageData = await client.GetByteArrayAsync(fondo2);
                    
                    using (MemoryStream stream = new MemoryStream(imageData))
                    {
                        // Redimensionar la imagen
                        Image imagenOriginal = Image.FromStream(stream);
                        Image imagenRedimensionada = Redimensionari(imagenOriginal, 400); // Redimensionar la imagen

                        // Establecer la imagen redimensionada como fondo del formulario
                        this.BackgroundImage = imagenRedimensionada;
                    }
                }
            }
            // metodo para redimensionar la imagen manteniendo la proporción
            private Image Redimensionari(Image imagen, int ancho)
            {
                // Calculamos la relación de aspecto de la imagen original
                double relacionAspecto = (double)imagen.Width / imagen.Height;

                // Calculamos el nuevo ancho basado en el ancho máximo y la relación de aspecto
                int nuevo = ancho;
                int nueva = (int)(ancho / relacionAspecto);

                // Creamos una nueva imagen redimensionada con las dimensiones calculadas
                return new Bitmap(imagen, nuevo, nueva);
            }
        

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Nombre = textBox1.Text;  //definimos el modo basando en la variablen FILAS que traemos desde el form 2
            string Modo = "";
            switch (FILAS)
            {
                case 10:
                    Modo = "Facíl";
                    break;
                case 15:
                    Modo = "Medio";
                    break;
                case 20:
                    Modo = "Difícil";
                    break;
            }

            resultados.Add(new Resultados(Nombre, Modo, chrono));  // agregamos el resultado conseguido a la lista de resultados definida al inicio de este form
            string ruta = "C:/Users/ocamp/Desktop/Minas/minas/minasBiblioteca_puntajes.xml";  
            //se define la ruta donde se va a guradar la lista (Debe cambiarla con la ruta que quede en su equipo a la hora de correr el archivo)
            XmlDocument documento = new XmlDocument(); //Se crea el archivo 
            if (File.Exists(ruta))  // se revisa si el archivo ya existe 
            {
                documento.Load(ruta);  // en caso de que exista se cargan los datos a la lista
            }
            else
            {
                // en caso de no existir se crea 
                var raiz = documento.CreateElement("Puntajes");
                documento.AppendChild(raiz);
            }
            XmlNode resultado = documento.CreateElement("Resultado");
            XmlNode nombre = documento.CreateElement("Nombre");
            XmlNode modo = documento.CreateElement("Modo");
            XmlNode Tiempo = documento.CreateElement("Tiempo");
            // Leer y deserializar el archivo existente
            foreach (var item in resultados)//Aca agregamos cada uno de los resultados que estan en la lista
            {
                nombre.InnerText = item.Nombre;
                modo.InnerText = item.Modo;
                Tiempo.InnerText = item.tiempo+"";
                resultado.AppendChild(nombre);
                resultado.AppendChild(modo);
                resultado.AppendChild(Tiempo);
                documento.FirstChild.AppendChild(resultado);
                documento.Save(ruta);
            }
            this.Close();      
            }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
    }

