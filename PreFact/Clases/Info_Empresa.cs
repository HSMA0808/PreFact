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
    class Info_Empresa : Clases.ConxionDB
    {
        public string Nombre_Negocio { get; set; }
        public string Telefono_Negocio { get; set; }
        public double ITBIS_Fijo { get; set; }
        public string Direccion { get; set; }
        public string ColetillaFactura { get; set; }

        public void Actualizar()
        {
            var Comando = new SqlCommand("EXEC ACTUALIZAR_INFO_GENERAL @Nombre_Negocio, @ITBIS_Fijo, @Telefono, @ColetillaFactura, @Direccion");
            Comando.Parameters.Add(new SqlParameter("@Nombre_Negocio", Nombre_Negocio));
            Comando.Parameters.Add(new SqlParameter("@ITBIS_Fijo", ITBIS_Fijo));
            Comando.Parameters.Add(new SqlParameter("@Telefono", Telefono_Negocio));
            Comando.Parameters.Add(new SqlParameter("@ColetillaFactura", ColetillaFactura));
            Comando.Parameters.Add(new SqlParameter("@Direccion", Direccion));
            EjecutarQuery(Comando);
        }

        public DataSet InfoParametrizable()
        {
            var comando = new SqlCommand("EXEC MOSTRAR_INFO_GENERAL");
            return ExtraerDataSet(comando);
        }
    }
}
