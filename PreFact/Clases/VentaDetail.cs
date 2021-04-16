using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PreFact.Clases;

namespace PreFact.Clases
{
    class VentaDetail : Clases.ConxionDB
    {
        public int IdVentaDetail { get; set; }
        public int IdVenta { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public double Precio_Unitario { get; set; }
        public double ITBIS_Articulo { get; set; }
        public double Monto { get; set; }

        public void InsertarVentaDetail()
        {
            var comando = new SqlCommand("EXEC INSERTAR_VENTADETAIL @IdVenta, @Descripcion, @Cantidad, @Precio_Unitario, " +
            "@Itbis_Articulo, @Monto");
            comando.Parameters.Add(new SqlParameter("@IdVenta", IdVenta));
            comando.Parameters.Add(new SqlParameter("@Descripcion", Descripcion));
            comando.Parameters.Add(new SqlParameter("@Cantidad", Cantidad));
            comando.Parameters.Add(new SqlParameter("@Precio_Unitario", Precio_Unitario));
            comando.Parameters.Add(new SqlParameter("@Itbis_Articulo", ITBIS_Articulo));
            comando.Parameters.Add(new SqlParameter("@Monto", Monto));
            EjecutarQuery(comando, true);
        }

        //La variable Tipo_Binario solamente funciona con los valores 1 y 0
        // 1: Consultara unico articulo por el Id del detalle, 2: Consultara todos lso articulos de la venta
        public DataSet VentaDetailParametrizable(int id, int Tipo_Binario)
        {
            var comando = new SqlCommand("EXEC MOSTRAR_VentaDetail_ID @Tipo, @valor");
            comando.Parameters.Add(new SqlParameter("@valor", id));
            comando.Parameters.Add(new SqlParameter("@Tipo", Tipo_Binario));
            return ExtraerDataSet(comando);
        }
    }
}
