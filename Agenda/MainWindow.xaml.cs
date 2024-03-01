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

using Agenda.Models;

namespace Agenda
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConexionDB mConexion;
        private List<ContactoModel> listaContactos;
        public MainWindow()
        {

            listaContactos = new List<ContactoModel>();
            mConexion = new ConexionDB();
            InitializeComponent();

            Refresh();
        }

        private void Refresh()
        {

            SqlDataReader sqlDataReader = null;
            String consulta = "select * from dbo.Contactos";

            if (mConexion.getConexion() != null)
            {
                SqlCommand sqlCommand = new SqlCommand(consulta);
                sqlCommand.Connection = mConexion.getConexion();
                sqlDataReader = sqlCommand.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    ContactoModel contacto = new ContactoModel();
                    contacto.IdContacto = sqlDataReader.GetInt32(0);
                    contacto.Nombre = sqlDataReader.GetString(1);
                    contacto.Apellidos = sqlDataReader.GetString(2);
                    contacto.Comentario = sqlDataReader.GetString(3);

                    listaContactos.Add(contacto);  
                }
                DG.ItemsSource = listaContactos;
                sqlDataReader.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Formulario pFormulario = new Formulario();
            pFormulario.Show();
            pFormulario.Owner = this;

            this.Hide();
        }
        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            string sqlDelete = "delete from dbo.Contactos where ID = @IdContacto";
            int Id = (int)((Button)sender).CommandParameter;

            if (mConexion.getConexion() != null)
            {
                SqlCommand sqlCommand = new SqlCommand(sqlDelete);
                sqlCommand.Parameters.AddWithValue("@IdContacto", Id);
                sqlCommand.Connection = mConexion.getConexion();
                sqlCommand.ExecuteNonQuery();
            }

            Refresh();

        }

        private void Button_Editar(object sender, RoutedEventArgs e)
        {
            int Id = (int)((Button)sender).CommandParameter;

            Formulario pFormulario = new Formulario(Id);
            pFormulario.Show();
            pFormulario.Owner = this;

            this.Hide();

        }

        private void Button_Refrescar(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}
