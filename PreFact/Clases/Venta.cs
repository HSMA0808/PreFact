using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PreFact.Clases;
using System.Data;
using System.Data.SqlClient;

namespace PreFact.Clases
{
    class Venta : Clases.ConxionDB
    {
        public int IdVenta { get; set; }
        public string Cliente { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Condiciones { get; set; }
        public string Despachado { get; set; }
        public string Vendedor { get; set; }
        public double SubTotal { get; set; }
        public double ITBIS { get; set; }
        public double Total { get; set; }
        public DateTime Fecha { get; set; }

        public string InsertarVenta()
        {
            var IdVenta = string.Empty;
            var comando = new SqlCommand("EXEC INSERTAR_VENTA @Cliente, @Telefono, @Direccion, " +
            "@Condiciones, @SubTotal, @Itbis, @Total, @Fecha");
            comando.Parameters.Add(new SqlParameter("@Cliente", Cliente));
            comando.Parameters.Add(new SqlParameter("@Telefono", Telefono));
            comando.Parameters.Add(new SqlParameter("@Direccion", Direccion));
            comando.Parameters.Add(new SqlParameter("@Condiciones", Condiciones));
            comando.Parameters.Add(new SqlParameter("@SubTotal", SubTotal));
            comando.Parameters.Add(new SqlParameter("@Itbis", ITBIS));
            comando.Parameters.Add(new SqlParameter("@Total", Total));
            comando.Parameters.Add(new SqlParameter("@Fecha", Fecha));
            EjecutarQuery(comando, true);
            //retorna el id de la venta insertada
            return ExtraerDataSet(new SqlCommand("select MAX(IdVenta) from Ventas")).Tables[0].Rows[0][0].ToString();
        }

        public DataSet VentaParametrizable(string id)
        {
            var comando = new SqlCommand("EXEC MOSTRAR_VENTA_ID @IdVenta");
            comando.Parameters.Add(new SqlParameter("@IdVenta", id));
            return ExtraerDataSet(comando);
        }
    }
}
