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
    /// Lógica de interacción para Correo.xaml
    /// </summary>
    public partial class Correo : Window
    {

        public int Id = 0;
        private ConexionDB mConexion;
        private List<CorreoModel> listaCorreos;
        string sqlInsertTelefono = "INSERT INTO dbo.Telefonos (ID_Contacto, Telefono) VALUES (@ID_Contacto, @Telefono)";
        string sqlInsertCorreo = "INSERT INTO dbo.Correos (ID_Contacto, Correo) VALUES (@ID_Contacto, @Correo)";
        string sqlDeleteCorreo = "delete from dbo.Correos where ID = @IdCorreo";

        public Correo(int Id)
        {
            InitializeComponent();
            listaCorreos = new List<CorreoModel>();
            mConexion = new ConexionDB();
            this.Id = Id;

            Refresh();

        }


        private void Refresh()
        {
            listaCorreos.Clear();
            SqlDataReader sqlDataReader = null;
            String consulta = "select * from dbo.Correos where ID_Contacto = " + Id;

            if (mConexion.getConexion() != null)
            {
                SqlCommand sqlCommand = new SqlCommand(consulta);
                sqlCommand.Connection = mConexion.getConexion();
                sqlDataReader = sqlCommand.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    CorreoModel contacto = new CorreoModel();
                    contacto.IdCorreo = sqlDataReader.GetInt32(0);
                    contacto.IdContacto = sqlDataReader.GetInt32(1);
                    contacto.Correo = sqlDataReader.GetString(2);

                    listaCorreos.Add(contacto);
                }
                DG.ItemsSource = null;
                DG.ItemsSource = listaCorreos;
                sqlDataReader.Close();
            }
        }

        private void Button_Guardar(object sender, RoutedEventArgs e)
        {

            using (SqlCommand command = new SqlCommand(sqlInsertCorreo, mConexion.getConexion()))
            {
                command.Parameters.AddWithValue("@ID_Contacto", Id);
                command.Parameters.AddWithValue("@Correo", txtcorreo.Text);

                command.ExecuteNonQuery();
            }
            Refresh();
        }

        private void Button_Eliminar(object sender, RoutedEventArgs e)
        {
            int Id = (int)((Button)sender).CommandParameter;

            if (mConexion.getConexion() != null)
            {
                SqlCommand sqlCommand = new SqlCommand(sqlDeleteCorreo);
                sqlCommand.Parameters.AddWithValue("@IdCorreo", Id);
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
