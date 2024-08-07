﻿using System;
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
using System.Text.RegularExpressions;

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
        string sqlInsertCorreo = "INSERT INTO dbo.Correos (ID_Contacto, Correo) VALUES (@ID_Contacto, @Correo)";
        string sqlDeleteCorreo = "delete from dbo.Correos where ID = @IdCorreo";

        public Correo(int Id)
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
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

                lblNombre.Text = "Correo/s de " + nombreContacto;
                sqlDataReader.Close();

                //obtener telefonos
                sqlCommand = new SqlCommand(consulta);
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
        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                guardar();
            }
        }
        public bool IsValidEmailAddress(string s)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(s);
        }

        private void Button_Guardar(object sender, RoutedEventArgs e)
        {
            guardar();
        }

        private void guardar()
        {

            if (IsValidEmailAddress(emailTextBox.Text))
            {
                using (SqlCommand command = new SqlCommand(sqlInsertCorreo, mConexion.getConexion()))
                {
                    command.Parameters.AddWithValue("@ID_Contacto", Id);
                    command.Parameters.AddWithValue("@Correo", emailTextBox.Text);

                    command.ExecuteNonQuery();
                }
                emailTextBox.Text = "";
            }
            else
            {
                MessageBox.Show("Correo introducido incorrecto");
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
