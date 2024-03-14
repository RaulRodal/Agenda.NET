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
using static MaterialDesignThemes.Wpf.Theme;
using System.Data;
using System.IO;
using System.Reflection.Metadata;
using Microsoft.Win32;

using iText.Kernel.Pdf;
using iText.Html2pdf;

namespace Agenda
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        private ConexionDB mConexion;
        private List<ContactoModel> listaContactos;
        private List<ContactoModel> listaFiltrada;
        private bool favorito = false;

        String consultaTelefonos = "select * from dbo.Telefonos where ID_Contacto = ";
        String consultaCorreos = "select * from dbo.Correos where ID_Contacto = ";

        String consultaNoFav = "select * from dbo.Contactos";
        String consultaFav = "select * from dbo.Contactos where Favorito = 1";

        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            listaContactos = new List<ContactoModel>();
            mConexion = new ConexionDB();
            InitializeComponent();

            // Suscribir el evento SelectionChanged del DataGridView
            DG.SelectionChanged += DG_SelectionChanged;
            Refresh();


            Timer.Tick += new EventHandler(Timer_Click);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        private void Timer_Click(object sender, EventArgs e)
        {
            DateTime d;
            d = DateTime.Now;
            hora.Text = d.Hour.ToString("D2") + ":" + d.Minute.ToString("D2") + ":" + d.Second.ToString("D2");
        }

        public void Refresh()
        {
            listaContactos.Clear();
            SqlDataReader sqlDataReader = null;

            if (mConexion.getConexion() != null)
            {
                String consulta = "";
                if (favToggle.IsChecked == true )
                {
                    consulta = consultaFav;
                }
                else 
                {
                    consulta = consultaNoFav;
                }
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
                listaFiltrada = listaContactos;
                sqlDataReader.Close();
            }

        }

        private void searchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var filtered = listaContactos.Where(contacto => contacto.Nombre.ToLower().Contains(searchBox.Text.ToLower()) || contacto.Apellidos.ToLower().Contains(searchBox.Text.ToLower()) || contacto.Comentario.ToLower().Contains(searchBox.Text.ToLower()));
            listaFiltrada = filtered.ToList();
            DG.ItemsSource = filtered;
        }


        private void Button_Informe(object sender, RoutedEventArgs e)
        {

            // Parte HTML do Contacto 
            const String taboaContacto =
                "<div>Nombre: @Nombre</div>" +
                "<div>Apellidos: @Apellidos</div>" +
                "<div>Comentario: @Comentario</div>" +
                "<table border=1 style=\"width: 100%\">" +
                "<tr style = \"background-color:lightgrey\">" +
                "<th style = \"width:50%\" align = \"center\" >Telefonos</th> " +
                "<th style = \"width:50%\" align = \"center\" >Correos</th> " +
                "</tr>";

            // Parte HTML do TelefonoyCorreo
            const String taboaTelefonoyCorreo =
                "<tr border=1>" +
                "<td style=\"width:20%\" align=\"center\">@Telefono</td>" +
                "<td style=\"width:50%\" align=\"center\">@Correo</td>" +
                "</tr>";

             
            String tableContacto = "";
            String tableTelefonoyCorreo = "";

            foreach (ContactoModel contacto in listaFiltrada)
            {

                List<TelefonoModel> listaTelefonos = new List<TelefonoModel>();
                List<CorreoModel> listaCorreos = new List<CorreoModel>();

                if (tableContacto.Length > 0)
                {
                    //cerramos a taboa de TelefonoyCorreos do Contacto anterior
                    tableContacto += "</table><br/><br/><br/>";
                }
                tableContacto += taboaContacto;
                tableContacto = tableContacto.Replace("@Nombre", contacto.Nombre);
                tableContacto = tableContacto.Replace("@Apellidos", contacto.Apellidos);
                tableContacto = tableContacto.Replace("@Comentario", contacto.Comentario);

                // Obtener lista de telefonos de un contacto
                listaTelefonos = GetTelefonosContacto(contacto);


                // Obtener lista de correos de un contacto
                listaCorreos = GetCorreosContacto(contacto);



                // Agregar todos los telefonos y correos
                // Determine the length of the longer list
                int maxLength = Math.Max(listaTelefonos?.Count ?? 0, listaCorreos?.Count ?? 0);
                // Loop through the lists simultaneously
                for (int i = 0; i < maxLength; i++)
                {
                    tableTelefonoyCorreo = "";
                    tableTelefonoyCorreo += taboaTelefonoyCorreo;
                    if (i < listaTelefonos.Count)
                    {
                        tableTelefonoyCorreo = tableTelefonoyCorreo.Replace("@Telefono", listaTelefonos[i].Telefono.ToString());
                    }
                    else
                    {
                        tableTelefonoyCorreo = tableTelefonoyCorreo.Replace("@Telefono", "");
                    }
                    if (i < listaCorreos.Count)
                    {
                        tableTelefonoyCorreo = tableTelefonoyCorreo.Replace("@Correo", listaCorreos[i].Correo.ToString());
                    }
                    else
                    {
                        tableTelefonoyCorreo = tableTelefonoyCorreo.Replace("@Correo", "");
                    }
                    tableContacto += tableTelefonoyCorreo;
                }
            }

            //Creamos obxeto para gardar o pdf
            SaveFileDialog gardarPDF = new SaveFileDialog();
            gardarPDF.FileName = "Contactos-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".pdf";

            //Leemos a plantilla que tiñamos e remplazamos donde toca
            String paxinahtml = new StreamReader("../../../Models/plantilla-contactos.html").ReadToEnd();
            paxinahtml = paxinahtml.Replace("@DATOS_CONTACTO", tableContacto);

            if (gardarPDF.ShowDialog() == true)
            {
                PdfWriter writer = new PdfWriter(gardarPDF.FileName);
                //convertimos o HTML resultante nun pdf
                HtmlConverter.ConvertToPdf(paxinahtml, writer);
            }
        }

        private List<TelefonoModel> GetTelefonosContacto(ContactoModel c)
        {
            List<TelefonoModel> listaTelefonos = new List<TelefonoModel>();
            //Creacion lista telefonos
            if (mConexion.getConexion() != null)
            {
                SqlCommand sqlCommand = new SqlCommand(consultaTelefonos + c.IdContacto);
                sqlCommand.Connection = mConexion.getConexion();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    TelefonoModel telefono = new TelefonoModel();
                    telefono.IdTelefono = sqlDataReader.GetInt32(0);
                    telefono.IdContacto = sqlDataReader.GetInt32(1);
                    telefono.Telefono = sqlDataReader.GetString(2);
                    MessageBox.Show(telefono.Telefono);
                    listaTelefonos.Add(telefono);
                }
                sqlDataReader.Close();
                return listaTelefonos;
            }
            return null;
        }

        public List<CorreoModel> GetCorreosContacto(ContactoModel c)
        {
            List<CorreoModel> listaCorreos = new List<CorreoModel>();
            //Creacion lista correos
            if (mConexion.getConexion() != null)
            {
                SqlCommand sqlCommand = new SqlCommand(consultaCorreos + c.IdContacto);
                sqlCommand.Connection = mConexion.getConexion();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    CorreoModel correo = new CorreoModel();
                    correo.IdCorreo = sqlDataReader.GetInt32(0);
                    correo.IdContacto = sqlDataReader.GetInt32(1);
                    correo.Correo = sqlDataReader.GetString(2);
                    listaCorreos.Add(correo);
                }
                sqlDataReader.Close();
                return listaCorreos;
            }
            return null;
        }


        private void DG_SelectionChanged(object sender, EventArgs e)
        {
            // Verificar si hay al menos una fila seleccionada
            if (DG.SelectedItems.Count > 0)
            {
                // Cambiar el texto del botón y habilitarlo
                if (DG.SelectedItems.Count > 1)
                {
                    txtbEliminar.Text = "Eliminar contactos";
                }
                else
                {

                    txtbEliminar.Text = "Eliminar contacto";
                }

                btnEliminar.Width = 180;
                btnEliminar.IsEnabled = true;
            }
            else
            {
                txtbEliminar.Text = "Seleciona uno o mas contactos";
                btnEliminar.Width = 250;
                // Si no hay ninguna fila seleccionada, deshabilitar el botón
                btnEliminar.IsEnabled = false;
            }
        }

        private void Button_Nuevo(object sender, RoutedEventArgs e)
        {
            Formulario pFormulario = new Formulario();
            pFormulario.ShowDialog();
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
        private void Button_Duplicar(object sender, RoutedEventArgs e)
        {

            string sqlConsultaUno = "select * from dbo.Contactos where ID = @IdContacto";
            string sqlInsertContactos = "INSERT INTO dbo.Contactos (Nombre, Apellidos, Comentario, Favorito) VALUES (@Nombre, @Apellidos, @Comentario, @Favorito)";

            int Id = (int)((System.Windows.Controls.Button)sender).CommandParameter;

            ContactoModel contacto = new ContactoModel();

            if (mConexion.getConexion() != null)
            {
                SqlCommand sqlCommand = new SqlCommand(sqlConsultaUno);
                sqlCommand.Parameters.AddWithValue("@IdContacto", Id);
                sqlCommand.Connection = mConexion.getConexion();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();


                while (sqlDataReader.Read())
                {
                    contacto.Nombre = sqlDataReader.GetString(1);
                    contacto.Apellidos = sqlDataReader.GetString(2);
                    contacto.Comentario = sqlDataReader.GetString(3);
                    contacto.Favorito = sqlDataReader.GetBoolean(4);
                }
                sqlDataReader.Close();
            }
            using (SqlCommand command = new SqlCommand(sqlInsertContactos, mConexion.getConexion()))
            {
                command.Parameters.AddWithValue("@Nombre", contacto.Nombre);
                command.Parameters.AddWithValue("@Apellidos", contacto.Apellidos);
                command.Parameters.AddWithValue("@Comentario", contacto.Comentario);
                command.Parameters.AddWithValue("@Favorito", contacto.Favorito);
                command.ExecuteNonQuery();
            }
            Refresh();
        }

        private void Button_Editar(object sender, RoutedEventArgs e)
        {
            int Id = (int)((System.Windows.Controls.Button)sender).CommandParameter;

            Formulario pFormulario = new Formulario(Id);
            pFormulario.ShowDialog();
        }
        

        private void Button_Refrescar(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
        private void Button_Correos(object sender, RoutedEventArgs e)
        {
            int Id = (int)((System.Windows.Controls.Button)sender).CommandParameter;

            Correo ventanaCorreos = new Correo(Id);
            ventanaCorreos.ShowDialog();
            ventanaCorreos.Owner = this;
        }
        private void Button_Telefono(object sender, RoutedEventArgs e)
        {
            int Id = (int)((System.Windows.Controls.Button)sender).CommandParameter;

            Telefono ventanaTelefonos = new Telefono(Id);
            ventanaTelefonos.ShowDialog();
            ventanaTelefonos.Owner = this;
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        private void Button_Ayuda(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

    }
}
