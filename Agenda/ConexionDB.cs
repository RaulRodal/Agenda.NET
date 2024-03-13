using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace Agenda
{
    internal class ConexionDB
    {
        private SqlConnection conexion;
        private String database = "Agenda";
        private String servidor = "localhost";
        private String usuario = "sa";
        private String password = "abc123.";
        private String cadeaConexion;

        public ConexionDB()
        {
            cadeaConexion = "Persist Security Info = False;" +
                "; User id = " + usuario + ";" +
                "; Password = " + password + ";" +
                "Initial Catalog = " + database + ";" +
                "Server = " + servidor + ";";
            cadeaConexion = "Data Source=RODAL\\SQLEXPRESS;Initial Catalog=Agenda;Integrated Security=True";
        }

        public SqlConnection getConexion()
        {
            if (conexion == null)
            {
                conexion = new SqlConnection(cadeaConexion);
                conexion.Open();
            }
            return conexion;
        } 

    }
}
