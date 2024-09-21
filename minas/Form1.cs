using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;

namespace minas
{
    public partial class Form1 : Form
    {
        string fondo2 = "https://img.freepik.com/fotos-premium/sapper-ejercito-limpiando-campo-minas-tonos-terrenales-pops-o-diseno-cartel-2d-a4-ideas-creativas_655090-1124136.jpg";
        // utilizamos imagenes de internet para que ualquie usuario con acceso a la red pueda usar el juego
        Form2 agregar;
        Form4 tabla;

        
        public Form1()
        {
            InitializeComponent();
        }
        private async Task CargarImagenAsync(string fondo2)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // Descargar los datos de la imagen
                    byte[] imageData = await client.GetByteArrayAsync(fondo2);
                    using (MemoryStream stream = new MemoryStream(imageData))
                    {
                        // Redimensionar la imagen
                        Image imagenOriginal = Image.FromStream(stream);
                        Image imagenRedimensionada = Redimensionari(imagenOriginal, 400); // el 400 es el tamaño de la imagen para que la memoria sea suficiente

                        // Establecer la imagen redimensionada como fondo del formulario
                        this.BackgroundImage = imagenRedimensionada;
                    }
                }
                catch (HttpRequestException e)
                {
                    // Manejar errores de la solicitud
                    MessageBox.Show($"Error al cargar la imagen: {e.Message}");
                }
            }
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            // Descargar la imagen desde Internet
            await CargarImagenAsync(fondo2);
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


        private void bajaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            agregar = new Form2(10, 10, 10);
            agregar.Show();
        }

        private void mediaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            agregar = new Form2(15, 15, 30);
            agregar.Show();
        }

        private void altaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            agregar = new Form2(20, 20, 50);
            agregar.Show();
        }

        private void resulatdosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabla = new Form4();
            tabla.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {
  
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
