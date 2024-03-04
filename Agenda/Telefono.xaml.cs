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

            if (mConexion.getConexion() != null)
            {
                SqlCommand sqlCommand = new SqlCommand(consulta);
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

        private void Button_Guardar(object sender, RoutedEventArgs e)
        {

            using (SqlCommand command = new SqlCommand(sqlInsertTelefono, mConexion.getConexion()))
            {
                command.Parameters.AddWithValue("@ID_Contacto", Id);
                command.Parameters.AddWithValue("@Telefono", txtcorreo.Text);

                command.ExecuteNonQuery();
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


        private void Volver()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
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

