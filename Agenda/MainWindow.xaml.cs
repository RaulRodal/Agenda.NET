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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Agenda
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConexionDB mConexion;
        public MainWindow()
        {
            mConexion = new ConexionDB();
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string sqlInsertContactos = "INSERT INTO dbo.Contactos (Nombre, Apellidos, Comentario) VALUES (@Nombre, @Apellidos, @Comentario)";
            string sqlInsertTelefono = "INSERT INTO dbo.Telefonos (ID_Contacto, Telefono) VALUES (@ID_Contacto, @Telefono)";
            string sqlInsertCorreo = "INSERT INTO dbo.Correos (ID_Contacto, Correo) VALUES (@ID_Contacto, @Correo)";

            using (SqlCommand command = new SqlCommand(sqlInsertContactos, mConexion.getConexion()))
            {
                command.Parameters.AddWithValue("@Nombre", txtnombre.Text);
                command.Parameters.AddWithValue("@Apellidos", txtapellidos.Text);
                command.Parameters.AddWithValue("@Comentario", txtcomentario.Text);

                command.ExecuteNonQuery();
            }

            string sql = "SELECT MAX(Id) FROM dbo.Contactos";

            using (SqlCommand commandID = new SqlCommand(sql, mConexion.getConexion()))
            {
                int ultimoId = (int)commandID.ExecuteScalar();

                using (SqlCommand command = new SqlCommand(sqlInsertTelefono, mConexion.getConexion()))
                {
                    command.Parameters.AddWithValue("@ID_Contacto", ultimoId);
                    command.Parameters.AddWithValue("@Telefono", txttelefono.Text);

                    command.ExecuteNonQuery();
                }

                using (SqlCommand command = new SqlCommand(sqlInsertCorreo, mConexion.getConexion()))
                {
                    command.Parameters.AddWithValue("@ID_Contacto", ultimoId);
                    command.Parameters.AddWithValue("@Correo", txtemail.Text);

                    command.ExecuteNonQuery();
                }

            }





        }
    }
}
