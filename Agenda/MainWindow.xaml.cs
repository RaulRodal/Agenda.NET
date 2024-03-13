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
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            listaContactos = new List<ContactoModel>();
            mConexion = new ConexionDB();
            InitializeComponent();

            // Suscribir el evento SelectionChanged del DataGridView
            DG.SelectionChanged += DG_SelectionChanged;
            Refresh();
        }

        private void Refresh()
        {
            listaContactos.Clear();
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
                DG.ItemsSource = null;
                DG.ItemsSource = listaContactos;
                sqlDataReader.Close();
            }

        }

        private void DG_SelectionChanged(object sender, EventArgs e)
        {
            // Verificar si hay al menos una fila seleccionada
            if (DG.SelectedItems.Count > 0)
            {
                // Cambiar el texto del botón y habilitarlo
                if(DG.SelectedItems.Count > 1)
                {
                    btnEliminar.Content = "Eliminar contactos";
                }
                else
                {

                    btnEliminar.Content = "Eliminar contacto";
                }

                btnEliminar.Width = 150;
                btnEliminar.IsEnabled = true;
            }
            else
            {
                btnEliminar.Content = "Seleciona uno o mas contactos";
                btnEliminar.Width = 200;
                // Si no hay ninguna fila seleccionada, deshabilitar el botón
                btnEliminar.IsEnabled = false;
            }
        }

        private void Button_Nuevo(object sender, RoutedEventArgs e)
        {
            Formulario pFormulario = new Formulario();
            pFormulario.Show();

            this.Close();
        }
        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            string sqlDelete = "delete from dbo.Contactos where ID = @IdContacto";
            //int Id = (int)((Button)sender).CommandParameter;

            foreach (ContactoModel contacto in DG.SelectedItems)
            {
                if (mConexion.getConexion() != null)
                {
                    SqlCommand sqlCommand = new SqlCommand(sqlDelete);
                    sqlCommand.Parameters.AddWithValue("@IdContacto", contacto.IdContacto);
                    sqlCommand.Connection = mConexion.getConexion();
                    sqlCommand.ExecuteNonQuery();
                }
            }
            Refresh();
        }


        private void Button_Editar(object sender, RoutedEventArgs e)
        {
            int Id = (int)((Button)sender).CommandParameter;

            Formulario pFormulario = new Formulario(Id);
            pFormulario.ShowDialog();

            this.Close();
        }

        private void Button_Refrescar(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
        private void Button_Correos(object sender, RoutedEventArgs e)
        {
            int Id = (int)((Button)sender).CommandParameter;

            Correo ventanaCorreos = new Correo(Id);
            ventanaCorreos.ShowDialog();
            ventanaCorreos.Owner = this;
        }
        private void Button_Telefono(object sender, RoutedEventArgs e)
        {
            int Id = (int)((Button)sender).CommandParameter;

            Telefono ventanaTelefonos = new Telefono(Id);
            ventanaTelefonos.ShowDialog();
            ventanaTelefonos.Owner = this;
        }
    }
}
