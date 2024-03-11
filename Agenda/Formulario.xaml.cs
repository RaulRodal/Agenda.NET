using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
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
    /// Lógica de interacción para Formulario.xaml
    /// </summary>
    public partial class Formulario : Window
    {
        private ConexionDB mConexion;
        public int Id = 0;
        bool update = false;
        string sqlConsultaTodos = "select * from dbo.Contactos";
        string sqlConsultaUno = "select * from dbo.Contactos where ID = @IdContacto";
        string sqlInsertContactos = "INSERT INTO dbo.Contactos (Nombre, Apellidos, Comentario) VALUES (@Nombre, @Apellidos, @Comentario)";
        string sqlUpdate = "UPDATE dbo.Contactos SET Nombre= @Nombre, Apellidos = @Apellidos, Comentario = @Comentario WHERE ID = @IdContacto";

        public Formulario(int Id = 0)
        {
            mConexion = new ConexionDB();
            InitializeComponent();

            this.Id = Id;
            if (this.Id != 0)
            {
                update = true;
                btn.Content = "Modificar contacto";
                SqlDataReader sqlDataReader = null;

                if (mConexion.getConexion() != null)
                {
                    SqlCommand sqlCommand = new SqlCommand(sqlConsultaUno);
                    sqlCommand.Parameters.AddWithValue("@IdContacto", Id);
                    sqlCommand.Connection = mConexion.getConexion();
                    sqlDataReader = sqlCommand.ExecuteReader();


                    while (sqlDataReader.Read())
                    {
                        txtnombre.Text = sqlDataReader.GetString(1);
                        txtapellidos.Text = sqlDataReader.GetString(2);
                        txtcomentario.Text = sqlDataReader.GetString(3);

                    }
                    
                    sqlDataReader.Close();
                }
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (update)
            {
                using (SqlCommand command = new SqlCommand(sqlUpdate, mConexion.getConexion()))
                {
                    command.Parameters.AddWithValue("@Nombre", txtnombre.Text);
                    command.Parameters.AddWithValue("@Apellidos", txtapellidos.Text);
                    command.Parameters.AddWithValue("@Comentario", txtcomentario.Text);
                    command.Parameters.AddWithValue("@IdContacto", Id);

                    command.ExecuteNonQuery();
                }
            }
            else
            {
                using (SqlCommand command = new SqlCommand(sqlInsertContactos, mConexion.getConexion()))
                {
                    command.Parameters.AddWithValue("@Nombre", txtnombre.Text);
                    command.Parameters.AddWithValue("@Apellidos", txtapellidos.Text);
                    command.Parameters.AddWithValue("@Comentario", txtcomentario.Text);

                    command.ExecuteNonQuery();
                }
            }
            Volver();
        }
        private void Cerrar(object sender, CancelEventArgs e)
        {
            Volver();
        }

        private void Volver()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide();
        }

    }
}
