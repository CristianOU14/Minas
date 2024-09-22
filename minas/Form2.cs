using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Net.Http;
using System.Threading.Tasks;

namespace minas
{
    public partial class Form2 : Form
    {
        private int FILAS;
        private int COLUMNAS;
        private int NUM_MINAS;
        private Button[,] botones;
        private bool[,] minas;
        private bool[,] descubiertas;
        Label tiempo = new Label();
        private bool time = false;
        private double chrono = 0;
        private Timer cronometro;

        string fondoaqui = "https://www.plaintextures.com/resources/thumbnails/textures/ground/soil/texture_soil_2069/texture_soil_2069_1_thumbnail.jpg";

        private void InicializarTablero()
        {
            for (int i = 0; i < FILAS; i++)
            {
                for (int j = 0; j < COLUMNAS; j++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(40, 40);
                    btn.Location = new Point(j * 40, i * 40);
                    btn.Tag = new Point(i, j);
                    btn.Click += Boton_Click;
                    btn.Font = new Font("Arial", 20);
                    btn.ForeColor = Color.Black;
                    btn.BackColor = Color.DarkGreen;
                    btn.MouseDown += ColocarBanderitas;
                    Controls.Add(btn);
                    botones[i, j] = btn;
                }
            }

            this.Size = new Size(COLUMNAS * 50 + 100, FILAS * 50 + 100);

            tiempo.Size = new Size(100, 100);
            tiempo.Text = "0";
            tiempo.Font = new Font("Arial", 30);
            tiempo.ForeColor = Color.White;
            tiempo.Location = new System.Drawing.Point(FILAS * 43, 0);
            Controls.Add(tiempo);
             cronometro = new Timer();
            cronometro.Interval = 1000; // Cada segundo
            cronometro.Tick += ActualizarCronometro; // Método que actualizará el cronómetro
            cronometro.Start();
            time = true;
        }
        private void ActualizarCronometro(object sender, EventArgs e)
        {
            if (time)
            {
                chrono++;
                tiempo.Text = chrono.ToString();
            }
            else
            {
                cronometro.Stop(); // Detener el cronómetro si el juego termina
            }
        }
        private async Task<Image> DescargarImagenDesdeURLAsync(string url)
        {
            Image imagen = await ObtenerImagenDesdeURL(url);

            if (imagen != null)
            {
                return RedimensionarImagen(imagen, 256);
            }

            return null;
        }

        private async Task<Image> ObtenerImagenDesdeURL(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    byte[] imageData = await client.GetByteArrayAsync(url);
                    using (MemoryStream stream = new MemoryStream(imageData))
                    {
                        Image imagenOriginal = Image.FromStream(stream);
                        Image imagenRedimensionada = RedimensionarImagen(imagenOriginal, 200);
                        return imagenRedimensionada;
                    }
                }
                catch (HttpRequestException e)
                {
                    MessageBox.Show($"Error al obtener la imagen: {e.Message}");
                    return null;
                }
            }
        }

        private Image RedimensionarImagen(Image imagen, int ancho)
        {
            double relacionAspecto = (double)imagen.Width / imagen.Height;
            int nuevoAncho = ancho;
            int nuevoAlto = (int)(ancho / relacionAspecto);

            return new Bitmap(imagen, nuevoAncho, nuevoAlto);
        }

        private void ColocarBanderitas(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;

            if (e.Button == MouseButtons.Right)
            {
                Point coordenadas = (Point)btn.Tag;
                int fila = coordenadas.X;
                int columna = coordenadas.Y;

                if (!descubiertas[fila, columna])
                {
                    btn.Text = btn.Text == "🚩" ? "" : "🚩";
                }
            }
        }

        private void ColocarMinas()
        {
            Random rnd = new Random();

            for (int i = 0; i < NUM_MINAS; i++)
            {
                int fila = rnd.Next(FILAS);
                int columna = rnd.Next(COLUMNAS);

                if (!minas[fila, columna])
                {
                    minas[fila, columna] = true;
                }
                else
                {
                    i--;
                }
            }
        }

        private void Boton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Point coordenadas = (Point)btn.Tag;
            int fila = coordenadas.X;
            int columna = coordenadas.Y;

            if (minas[fila, columna])
            {
                MostrarMinas();
                time = false;
                MessageBox.Show("¡Has perdido!");
                this.Close();
            }
            else
            {
                DescubrirCasilla(fila, columna);

                if (ComprobarVictoria())
                {
                    MessageBox.Show("¡Has ganado!");
                    MostrarMinas();
                    time = false;
                    Form3 Resultado = new Form3(FILAS, chrono);
                    Resultado.Show();
                }
            }
        }

        private void DescubrirCasilla(int fila, int columna)
        {
            if (fila < 0 || fila >= FILAS || columna < 0 || columna >= COLUMNAS || descubiertas[fila, columna])
                return;

            descubiertas[fila, columna] = true;
            int minasCercanas = ContarMinasCercanas(fila, columna);

            if (minasCercanas == 0)
            {
                botones[fila, columna].Enabled = false;
                botones[fila, columna].BackColor = Color.SandyBrown;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        DescubrirCasilla(fila + i, columna + j);
                    }
                }
            }
            else
            {
                botones[fila, columna].Text = minasCercanas.ToString();
                botones[fila, columna].Enabled = false;
                botones[fila, columna].BackColor = Color.SandyBrown;
            }
        }

        private int ContarMinasCercanas(int fila, int columna)
        {
            int contador = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int nuevaFila = fila + i;
                    int nuevaColumna = columna + j;

                    if (nuevaFila >= 0 && nuevaFila < FILAS && nuevaColumna >= 0 && nuevaColumna < COLUMNAS && minas[nuevaFila, nuevaColumna])
                    {
                        contador++;
                    }
                }
            }

            return contador;
        }

        private void MostrarMinas()
        {
            for (int i = 0; i < FILAS; i++)
            {
                for (int j = 0; j < COLUMNAS; j++)
                {
                    if (minas[i, j])
                    {
                        botones[i, j].Text = "💣";
                    }
                }
            }
        }

        private bool ComprobarVictoria()
        {
            for (int i = 0; i < FILAS; i++)
            {
                for (int j = 0; j < COLUMNAS; j++)
                {
                    if (!descubiertas[i, j] && !minas[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Form2(int FILAS, int COLUMNAS, int NUM_MINAS)
        {
            this.FILAS = FILAS;
            this.COLUMNAS = COLUMNAS;
            this.NUM_MINAS = NUM_MINAS;
            botones = new Button[FILAS, COLUMNAS];
            minas = new bool[FILAS, COLUMNAS];
            descubiertas = new bool[FILAS, COLUMNAS];
            InitializeComponent();
            InicializarTablero();
            ColocarMinas();
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                this.BackgroundImage = await DescargarImagenDesdeURLAsync(fondoaqui);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la imagen: {ex.Message}");
            }
        }
    }

    public class Resultados
    {
        public string Nombre { get; set; }
        public string Modo { get; set; }
        public double tiempo { get; set; }

        public Resultados(string nombre, string modo, double tiempo)
        {
            Nombre = nombre;
            Modo = modo;
            this.tiempo = tiempo;
        }
    }
}
