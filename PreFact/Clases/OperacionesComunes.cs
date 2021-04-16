using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace PreFact.Clases
{
    static class OperacionesComunes
    {
        public const string ValorCero = "0.00";
        public static void DesactivarControles(bool Decision, params Control[] Controles)
        {
            foreach (var control in Controles)
            {
                control.Enabled = Decision;
            }
        }

        public static void Limpiar(params TextBoxBase[] TextBoxes)
        {
            foreach (var TextBox in TextBoxes)
            {
                TextBox.Clear();
            }
        }

        public static void Limpiar(params DataGridView[] DGVS)
        {
            foreach (var DGV in DGVS)
            {
                DGV.Rows.Clear();
            }
        }

        public static void Limpiar(params Label[] Labels)
        {
            for (int i = 0; i<Labels.Count(); i++)
            {
                Labels[i].Text = "0.00";
            }
        }

        public static bool ValidarCampos(params TextBoxBase[] TextBoxes)
        {
            bool Completos = true;
            foreach (var txt in TextBoxes)
            {
                if (txt.Text.Trim() == string.Empty)
                {
                    Completos = false;
                }
            }
            return Completos;
        }

        public static bool ValidarCampos(DataGridView DGV)
        {
            bool Completos = true;
            if (DGV.Rows.Count < 2)
            {
                Completos = false;
            }
            return Completos;
        }

        public static void ErrorAbriendoConexion(Exception Descripcion)
        {
            MessageBox.Show("Ha ocurrido un error al tratar de conectar la base de datos: " + Descripcion.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ErrorEjecutandoQuery(Exception Descripcion)
        {
            MessageBox.Show("Ha ocurrido un error realizando la operacion en la base de datos: " + Descripcion.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void OperacionRealizada()
        {
            MessageBox.Show("Se realizo la operacion satisfactoriamente", "Completado", MessageBoxButtons.OK, MessageBoxIcon.Information);
         
        }

        public static string MostrarDecimales(string valor)
        {
            return double.Parse(valor).ToString("C").Substring(1);
        }

        public static string IDVentaFormateado(string idventa)
        {
            string ceros = "00000000";
            int caracteres = idventa.Length;
            ceros = ceros.Substring(caracteres) + idventa.ToString();
            return ceros;
        }

        //utilizar este metodo en la pantalla "Configuraciones.cs" para quitar los "0" al idventa antes de consultar
        public static string DesFormatearIDVenta(string idventa)
        {
            int indice = 0;
            foreach (char letra in idventa)
            {
                if (letra == '0')
                {
                    indice++;
                }
                else
                {
                    break;
                }
            }
            return idventa = idventa.Substring(indice);
        }
    }
}
