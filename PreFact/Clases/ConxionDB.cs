using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using PreFact.Clases;


namespace PreFact.Clases
{
    class ConxionDB
    {
        private SqlConnection Conexion { get; set; }
        private SqlCommand Comando { get; set; }
        private SqlDataAdapter DA { get; set; }
        private DataSet DS { get; set; }
        private string ConexionLocal = ConfigurationManager.ConnectionStrings["ConexionLocal"].ToString();

        public SqlConnection Conectar()
        {
            var con = new SqlConnection(ConexionLocal);
            try
            {
                con.Open();
                return con;
            }
            catch (Exception Error_Descripcion)
            {
                OperacionesComunes.ErrorAbriendoConexion(Error_Descripcion);
            }
            return new SqlConnection();
        }

        public void EjecutarQuery(SqlCommand comando, bool Iteracion = false)
        {
            Conexion = Conectar();
            var Transaccion = Conexion.BeginTransaction();
            comando.Connection = Conexion;
            comando.Transaction = Transaccion;
            try
            {
                comando.ExecuteNonQuery();
                comando.Transaction.Commit();
                if (Iteracion == false)
                {
                    OperacionesComunes.OperacionRealizada();
                }
            }
            catch (Exception Error_Descripcion)
            {
                comando.Transaction.Rollback();
                OperacionesComunes.ErrorEjecutandoQuery(Error_Descripcion);
            }
        }

        public DataSet ExtraerDataSet(SqlCommand comando)
        {
            Conexion = Conectar();
            DS = new DataSet();
            try
            {
                DA = new SqlDataAdapter(comando.CommandText, Conexion);
                DA.Fill(DS);
                return DS;
            }
            catch (Exception Error_Descripcion)
            {
                OperacionesComunes.ErrorEjecutandoQuery(Error_Descripcion);
            }
            return new DataSet();
        }
    }
}
