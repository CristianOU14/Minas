using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Net.Http;
using System.Threading.Tasks;
namespace minas
{

    public partial class Form2 : Form
    {//inicialización de las variables 
        private int FILAS;
        private int COLUMNAS;
        private int NUM_MINAS;
        private Button[,] botones; // creacion del tablero 
        private bool[,] minas; //matriz booleana true y false donde se muestra la ubicación de las minas
        private bool[,] descubiertas;//matriz booleana true y false donde se muestra si se ha descubierto la casilla o no
        Label tiempo = new Label(); // label donde se mostrara el cronometro

        private bool time = false;// controlar el tiempo en el juego
        private double chrono = 0;

        // string fondoaqui = "https://www.plaintextures.com/resources/thumbnails/textures/ground/soil/texture_soil_2069/texture_soil_2069_1_thumbnail.jpg"; //url del la imagen que utilizamos para el fondo y label de tiempo
        string fondoaqui = "C:/Users/ocamp/Desktop/Minas/texttierra.jpg"; // Aqui hay que poner la direccion de la imagen en la carpeta minas que se llama texttierra

        // metodo para inicializar el tablero 
        private void InicializarTablero()
        {
            for (int i = 0; i < FILAS; i++)
            {
                for (int j = 0; j < COLUMNAS; j++)
                {
                    //creacion de un nuevo boton con su tamaño ubicación fuente funcion (boton_click) color letra color fondo, 
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
                    this.Size = new Size(FILAS * 50, COLUMNAS * 50);


                    //condiguración del label tiempo inicialñizada en 0  con fuente, tamaño, color fuente, imagen fondo, ubicación y se inicializa el tiempo por estar en verdadero
                    tiempo.Size = new Size(100, 100);
                    tiempo.Text = "0";
                    tiempo.Font = new Font("Arial", 30);
                    tiempo.ForeColor = Color.White;
                    ////tiempo.BackgroundImage = DescargarImagenDesdeURL(fondoaqui); // Cambiamos la imagen de fondo
                    tiempo.BackgroundImage = Image.FromFile(fondoaqui);
                    tiempo.Location = new System.Drawing.Point(FILAS * 43, 0);
                    Controls.Add(tiempo);
                    time = true;

                }
            }
        }

        // Método para descargar una imagen desde una URL y devolverla como un objeto Image, redimensionada
        private async Task<Image> DescargarImagenDesdeURLAsync(string url)
        {
            // Obtenemos la imagen desde la URL de manera asíncrona
            Image imagen = await ObtenerImagenDesdeURL(url);
            
            if (imagen != null)
            {
                // Redimensionamos la imagen descargada
                return Redimensionari(imagen, 256);
            }
            
            return null; // Devuelve null si no se pudo obtener la imagen
        }

        // Método para obtener una imagen desde una URL
        private async Task<Image> ObtenerImagenDesdeURL(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    byte[] imageData = await client.GetByteArrayAsync(url);
                    using (MemoryStream stream = new MemoryStream(imageData))
                    {
                        return Image.FromStream(stream);
                    }
                }
                catch (HttpRequestException e)
                {
                    // Manejar errores de la solicitud
                    MessageBox.Show($"Error al obtener la imagen: {e.Message}");
                    return null; // O puedes lanzar una excepción, según lo que prefieras
                }
            }
        }

        // Método para redimensionar la imagen manteniendo la proporción
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






        // Método para colocar banderitas en el tablero al hacer clic derecho
        private void ColocarBanderitas(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;

            // Verificar si se hizo clic derecho
            if (e.Button == MouseButtons.Right)
            {
                // Obtener las coordenadas del botón
                Point coordenadas = (Point)btn.Tag;
                int fila = coordenadas.X;
                int columna = coordenadas.Y;

                // Verificar si la casilla ya ha sido descubierta
                if (!descubiertas[fila, columna])
                {
                    // Si no hay banderita, colocarla; si ya la hay, quitarla
                    if (btn.Text != "🚩")
                    {
                        btn.Text = "🚩";
                        
                    }
                    else
                    {
                        btn.Text = "";
                    }
                }
            }
        }





        //metodo para colocar las minas en el tablero  de forma aleatoria 

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
                    i--; // Intentar de nuevo colocar una mina en una posición aleatoria si ya hay una mina en esa posición
                }
            }
        }

        //medodo para cuando se presiona un bonton 
        private void Boton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Point coordenadas = (Point)btn.Tag;
            int fila = coordenadas.X;
            int columna = coordenadas.Y;
            // si tocamos una mina aparezca mensaje has perdido se pareel tiempo y se cierre 
            if (minas[fila, columna])
            {
                MostrarMinas();
                time = false;
                MessageBox.Show("¡Has perdido!");
                this.Close();
            }
            //si no tocamos mina llamamos al metodo descubrir casilla 
            else
            {
                DescubrirCasilla(fila, columna);

                // llamamos al metodo comprobar victoruia para saber si ya ganamos, mostrar mensaje y parar el tiempo
                if (ComprobarVictoria())
                {
                    MessageBox.Show("¡Has ganado!");
                    MostrarMinas();
                    time = false;
                    Form3 Resultado = new Form3(FILAS, Double.Parse(tiempo.Text));
                    Resultado.Show();
                }
            }

        }


        // metodo para mostrar las minas decubiertas  
        private void DescubrirCasilla(int fila, int columna)
        {
            if (fila < 0 || fila >= FILAS || columna < 0 || columna >= COLUMNAS || descubiertas[fila, columna])
                return;

            descubiertas[fila, columna] = true; // marcar fila como descubierta
            int minasCercanas = ContarMinasCercanas(fila, columna); // contar mina cercana llamando a un metodo

            if (minasCercanas == 0)
            {
                botones[fila, columna].Enabled = false;// desabilitar boton
                botones[fila, columna].BackColor= Color.SandyBrown; // cambiar el color a las minas descubiertas
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        DescubrirCasilla(fila + i, columna + j); // aqui usamos  recursividad  para encontar si las casillas cercanas no tienen una mina es decir casillas vacias
                    }
                }
            }
            else
            {
                botones[fila, columna].Text = minasCercanas.ToString(); // mostramos el numero de casillas cercanas
                botones[fila, columna].Enabled = false; // desabilitar boton
                botones[fila, columna].BackColor = Color.SandyBrown; // cambiar color a cadfe de donde aparecen los numeros 
            }
        }


        // metodo para contar minas cercanas 
        private int ContarMinasCercanas(int fila, int columna)
        {
            int contador = 0; // contador de minas
            // recorrer posiciones
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int nuevaFila = fila + i; // calcular nueva fila
                    int nuevaColumna = columna + j;// calcular nueva columna 
                    // Verificar si la posición contiene una mina y si la hay pues la suma
                    if (nuevaFila >= 0 && nuevaFila < FILAS && nuevaColumna >= 0 && nuevaColumna < COLUMNAS && minas[nuevaFila, nuevaColumna])
                    {
                        contador++;
                    }
                }
            }

            return contador; // devuelve el numero de minas cercanas
        }

        // Método para mostrar todas las minas en el tablero
        private void MostrarMinas()
        {
            // recorrer tablero
            for (int i = 0; i < FILAS; i++)
            {
                for (int j = 0; j < COLUMNAS; j++)
                {
                    //si hay minas las dibuja
                    if (minas[i, j])
                    {
                        botones[i, j].Text = "💣"; // mostrar el dibujito de una mina
                    }
                }
            }
        }

        // metodo para comprobar la victoria  
        private bool ComprobarVictoria()
        {
            // recorrer tablero
            for (int i = 0; i < FILAS; i++)
            {
                for (int j = 0; j < COLUMNAS; j++)
                {
                    //  si hay una casilla no descubierta y sin mina
                    if (!descubiertas[i, j] && !minas[i, j])
                    {
                        return false; // aun falta por descubrir 
                    }
                }
            }

            return true; // sino si gano

        }


        public Form2(int FILAS,int COLUMNAS,int NUM_MINAS)
        {
            // Inicializar tamaño del tablero y matrices
            this.FILAS = FILAS;
            this.COLUMNAS = COLUMNAS;
            this.NUM_MINAS = NUM_MINAS;
            botones = new Button[FILAS, COLUMNAS];
            minas = new bool[FILAS, COLUMNAS];
            descubiertas = new bool[FILAS, COLUMNAS];
            InitializeComponent();
            {
                InitializeComponent();
                InicializarTablero();
                ColocarMinas();
            }
            // Iniciar hilo para llevar el tiempo de juego
            Thread hilo1 = new Thread(() =>
            {
                while (time)
                {
                    chrono++; // incrementar tiempo en el label de tiempo
                    this.Invoke((MethodInvoker)delegate
                    {
                        tiempo.Text = Convert.ToString(chrono);
                    }
                    );
                    Thread.Sleep(1000);// esperar 1 segundo
                }
            });
            hilo1.Start(); // inicializar hilo

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // this.BackgroundImage = DescargarImagenDesdeURL(fondoaqui); // poner el fondo de este formulario
            this.BackgroundImage = Image.FromFile(fondoaqui);
        }
    }
    public class Resultados
    {
        public string Nombre { set; get; }
        public string Modo { set; get; }
        public double tiempo { set; get; }

        public Resultados(string nombre, string modo, double tiempo)
        {
            Nombre = nombre;
            Modo = modo;
            this.tiempo = tiempo;
        }
    }
}
