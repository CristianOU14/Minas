using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace minas
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
                // Llamar al método para cargar los datos del archivo XML en el DataGridView
                CargarDatosDesdeXML();
            }
            private void CargarDatosDesdeXML()
            {
                try
                {
                    // Crear un nuevo DataSet
                    DataSet dataSet = new DataSet();

                    // Cargar datos desde el archivo XML
                    dataSet.ReadXml("C:/Users/salas/source/repos/minas/minas/minas/minasBiblioteca_puntajes.xml");

                    // Verificar si hay al menos una tabla en el DataSet
                    if (dataSet.Tables.Count > 0)
                    {
                        // Asignar la primera tabla del DataSet al DataGridView
                        dataGridView1.DataSource = dataSet.Tables[0];
                    }
                    else
                    {
                        MessageBox.Show("No se encontraron datos en el archivo XML.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los datos desde el archivo XML: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
    }

