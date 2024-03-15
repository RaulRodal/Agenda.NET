using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Agenda.Models;
using System.ComponentModel;
using System.Windows.Controls;

namespace Agenda
{
    /// <summary>
    /// Lógica de interacción para Telefono.xaml
    /// </summary>
    public partial class Telefono : Window
    {
        public int Id = 0;
        private ConexionDB mConexion;
        private List<TelefonoModel> listaTelefonos;
        string sqlInsertTelefono = "INSERT INTO dbo.Telefonos (ID_Contacto, Telefono) VALUES (@ID_Contacto, @Telefono)";
        string sqlDeleteTelefono = "delete from dbo.Telefonos where ID = @IdTelefono";

        public Telefono(int Id)
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            listaTelefonos = new List<TelefonoModel>();
            mConexion = new ConexionDB();
            this.Id = Id;

            Refresh();

        }

        private void Refresh()
        {
            listaTelefonos.Clear();
            SqlDataReader sqlDataReader = null;
            String consulta = "select * from dbo.Telefonos where ID_Contacto = " + Id;
            String consultaNombre = "select Nombre from dbo.Contactos where ID = " + Id;
            String nombreContacto = "";

            if (mConexion.getConexion() != null)
            {
                //obtener nombre de contacto 
                SqlCommand sqlCommand = new SqlCommand(consultaNombre);
                sqlCommand.Connection = mConexion.getConexion();
                sqlDataReader = sqlCommand.ExecuteReader();

                while (sqlDataReader.Read())
                { 
                    nombreContacto = sqlDataReader.GetString(0);
                }

                lblNombre.Text = "Telefono/s de " + nombreContacto;
                sqlDataReader.Close();

                //obtener telefonos
                sqlCommand = new SqlCommand(consulta);
                sqlCommand.Connection = mConexion.getConexion();
                sqlDataReader = sqlCommand.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    TelefonoModel contacto = new TelefonoModel();
                    contacto.IdTelefono = sqlDataReader.GetInt32(0);
                    contacto.IdContacto = sqlDataReader.GetInt32(1);
                    contacto.Telefono = sqlDataReader.GetString(2);

                    listaTelefonos.Add(contacto);
                }
                DG.ItemsSource = null;
                DG.ItemsSource = listaTelefonos;
                sqlDataReader.Close();
            }
        }

        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                guardar();
            }
        }

        private bool VerificarTextBox(TextBox textBox)
        {
            bool ret = true;

            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                ret = false;
            }
            else if (textBox.Text.Trim().Length == 0)
            {
                ret = false;
            }
            return ret;

        }

        private void Button_Guardar(object sender, RoutedEventArgs e)
        {
            guardar();
        }

        private void guardar()
        {
            if (VerificarTextBox(textBox))
            {
                using (SqlCommand command = new SqlCommand(sqlInsertTelefono, mConexion.getConexion()))
                {
                    command.Parameters.AddWithValue("@ID_Contacto", Id);
                    command.Parameters.AddWithValue("@Telefono", textBox.Text);

                    command.ExecuteNonQuery();
                }
                textBox.Text = "";
            }
            Refresh();
        }
        private void Button_Eliminar(object sender, RoutedEventArgs e)
        {
            int Id = (int)((Button)sender).CommandParameter;

            if (mConexion.getConexion() != null)
            {
                SqlCommand sqlCommand = new SqlCommand(sqlDeleteTelefono);
                sqlCommand.Parameters.AddWithValue("@IdTelefono", Id);
                sqlCommand.Connection = mConexion.getConexion();
                sqlCommand.ExecuteNonQuery();
            }
            Refresh();
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Permitir solo números y el signo '+'
            if (!char.IsDigit(e.Text, 0) && e.Text != "+")
            {
                e.Handled = true;
            }

            // Permitir el signo '+' solo si es el primer carácter
            TextBox textBox = sender as TextBox;
            if (e.Text == "+" && textBox.Text.Length != 0)
            {
                e.Handled = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Verificar si el texto supera los 12 caracteres
            if (textBox.Text.Length > 12)
            {
                MessageBox.Show("El numero no puede superar los 12 caracteres.");
                textBox.Text = textBox.Text.Substring(0, 12);
                textBox.Select(textBox.Text.Length, 0); // Colocar el cursor al final
            }
        }

        private void Volver()
        {
            this.Hide();
        }

        private void Button_Volver(object sender, RoutedEventArgs e)
        {
            Volver();
        }
        private void Cerrar(object sender, CancelEventArgs e)
        {
            Volver();
        }
    }
}

