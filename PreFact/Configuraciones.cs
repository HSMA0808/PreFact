using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using PreFact.Clases;
using System.Drawing.Printing;

namespace PreFact
{
    public partial class Configuraciones : Form
    {
        private static String idventa = string.Empty;
        public Configuraciones()
        {
            InitializeComponent();
            var conexion = new ConxionDB();
            var DS = new DataSet();
            DS = conexion.ExtraerDataSet(new SqlCommand("exec Mostrar_Info_General"));
            txtNombreNegocio.Text = DS.Tables[0].Rows[0]["Nombre_Negocio"].ToString();
            lblITBIS_Valor.Text = DS.Tables[0].Rows[0]["ITBIS_Fijo"].ToString();
            txtITBIS_Valor.Text = (double.Parse(DS.Tables[0].Rows[0]["ITBIS_Fijo"].ToString()) * 100).ToString();
            txtColetilla.Text = DS.Tables[0].Rows[0]["ColetillaFactura"].ToString();
            txtDireccion.Text = DS.Tables[0].Rows[0]["Direccion"].ToString();
            txtTelefonoNegocio.Text = DS.Tables[0].Rows[0]["Telefono"].ToString();
            DS.Clear();
            DS = conexion.ExtraerDataSet(new SqlCommand("EXEC MOSTRARVENTAS"));
            for (int i = 0; i < DS.Tables[0].Rows.Count; i++)
            {
                dataGridView1.Rows.Add(OperacionesComunes.IDVentaFormateado(DS.Tables[0].Rows[i]["No. Factura"].ToString()), DS.Tables[0].Rows[i]["Cliente"].ToString(),
                    DS.Tables[0].Rows[i]["Telefono"].ToString(), DS.Tables[0].Rows[i]["Dirección"].ToString(), DS.Tables[0].Rows[i]["Condiciones"].ToString(),
                    DS.Tables[0].Rows[i]["Fecha"].ToString(), OperacionesComunes.MostrarDecimales(DS.Tables[0].Rows[i]["ITBIS"].ToString()), OperacionesComunes.MostrarDecimales(DS.Tables[0].Rows[i]["Sub-Total"].ToString()),
                    OperacionesComunes.MostrarDecimales(DS.Tables[0].Rows[i]["Total"].ToString()));
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (txtITBIS_Valor.Text.Trim() == string.Empty)
            {
                lblITBIS_Valor.Text = "0.00";
            }
            else if (double.TryParse(txtITBIS_Valor.Text, out _))
            {
                if (double.Parse(txtITBIS_Valor.Text) > 100 || double.Parse(txtITBIS_Valor.Text) <= 0)
                {
                    MessageBox.Show("El valor insertado en el campo '" + lblITBIS.Text + "' debe ser mayor que 0 y menor o igual a 100");
                    txtITBIS_Valor.Focus();
                }
                else
                {
                    lblITBIS_Valor.Text = (double.Parse(txtITBIS_Valor.Text) / 100).ToString();
                }
            }
            else
            {
                MessageBox.Show("Para establecer el ITBIS a aplicar verifique el campo '"+lblITBIS.Text+"' contenga un valor numerico");
                txtITBIS_Valor.Focus();
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            var Principal = new Form1();
            this.Hide();
            Principal.Show();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            OperacionesComunes.DesactivarControles(true, txtNombreNegocio, txtTelefonoNegocio, txtITBIS_Valor, txtDireccion, txtColetilla, btnCancelar, btnGuardar);
            btnEditar.Visible = false;
            txtNombreNegocio.Focus();
        }

        private async void btnNuevo_Click(object sender, EventArgs e)
        {
            if (!OperacionesComunes.ValidarCampos(txtNombreNegocio, txtTelefonoNegocio, txtITBIS_Valor, txtDireccion, txtColetilla))
            {
                MessageBox.Show("Para realizar la operacion debe de completar todos los campos listados a continuacion: " + lblNombreNegocio.Text + ", " + lblTelefonoNegocio.Text + ", " +
                    "" + lblITBIS.Text + ", " + lblDireccion.Text + ", " + lblColetilla.Text + "");
            }
            else
            {
                var InfoEmpresa = new Info_Empresa()
                {
                    Nombre_Negocio = txtNombreNegocio.Text,
                    Telefono_Negocio = txtTelefonoNegocio.Text,
                    ITBIS_Fijo = double.Parse(lblITBIS_Valor.Text),
                    ColetillaFactura = txtColetilla.Text,
                    Direccion = txtDireccion.Text
                };
                await Task.Run(() => {
                    InfoEmpresa.Actualizar();
                });
                OperacionesComunes.DesactivarControles(false, txtNombreNegocio, txtTelefonoNegocio, txtITBIS_Valor, txtDireccion, txtColetilla, btnCancelar, btnGuardar);
                btnEditar.Visible = true;
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            OperacionesComunes.DesactivarControles(false, txtNombreNegocio, txtTelefonoNegocio, txtITBIS_Valor, txtDireccion, txtColetilla, btnGuardar, btnCancelar);
            btnEditar.Visible = true;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            idventa = OperacionesComunes.DesFormatearIDVenta(dataGridView1.CurrentRow.Cells[0].Value.ToString());
        }

        private void Imprime(object sender, PrintPageEventArgs e)
        {
            Font fuente = new Font("Consolas", 8, FontStyle.Regular);
            var Conexion = new Clases.ConxionDB();
            var DS = new DataSet();
            int linea = 0;
            DS = Conexion.ExtraerDataSet(new SqlCommand("EXEC FACTURAHEADER"));
            e.Graphics.DrawString(DS.Tables[0].Rows[0]["Negocio"].ToString(), fuente, Brushes.Black, new RectangleF(0, linea, 700, 200));
            e.Graphics.DrawString(DS.Tables[0].Rows[0]["Direccion"].ToString(), fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            e.Graphics.DrawString(DS.Tables[0].Rows[0]["Telefono"].ToString(), fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            e.Graphics.DrawString(".........................................", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            string coletilla = DS.Tables[0].Rows[0]["Coletilla"].ToString();
            DS.Clear();
            DS = Conexion.ExtraerDataSet(new SqlCommand("EXEC FACTURA " + idventa + ""));
            var SubTotal = Double.Parse(DS.Tables[0].Rows[0]["Sub-Total"].ToString()).ToString("C").Substring(1);
            var ITBIS = Double.Parse(DS.Tables[0].Rows[0]["ITBIS"].ToString()).ToString("C").Substring(1);
            var Total = Double.Parse(DS.Tables[0].Rows[0]["Total"].ToString()).ToString("C").Substring(1);
            var dtp = new DateTimePicker();
            dtp.Value = DateTime.Parse(DS.Tables[0].Rows[0]["Fecha"].ToString());
            e.Graphics.DrawString("Fecha: " + dtp.Value.ToShortDateString() + "", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            e.Graphics.DrawString("No. Factura: " + OperacionesComunes.IDVentaFormateado(DS.Tables[0].Rows[0]["No. Factura"].ToString()) + "", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            e.Graphics.DrawString("Condicion: " + DS.Tables[0].Rows[0]["Condiciones"].ToString() + "", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            e.Graphics.DrawString("Cliente: " + DS.Tables[0].Rows[0]["Cliente"].ToString() + "", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            e.Graphics.DrawString("Telefono: " + DS.Tables[0].Rows[0]["Telefono"].ToString() + "", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            e.Graphics.DrawString("..........................................", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            e.Graphics.DrawString("Articulo", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            e.Graphics.DrawString("Cant.", fuente, Brushes.Black, new RectangleF(210, linea, 700, 200));
            e.Graphics.DrawString("Precio", fuente, Brushes.Black, new RectangleF(318, linea, 700, 200));
            e.Graphics.DrawString("..........................................", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            DS.Clear();
            DS = Conexion.ExtraerDataSet(new SqlCommand("EXEC FACTURADETAIL " + idventa + ""));
            for (var i = 0; i < DS.Tables[0].Rows.Count; i++)
            {
                e.Graphics.DrawString(DS.Tables[0].Rows[i]["Descripción"].ToString(), fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
                e.Graphics.DrawString(DS.Tables[0].Rows[i]["Cantidad"].ToString(), fuente, Brushes.Black, new RectangleF(210, linea, 700, 200));
                e.Graphics.DrawString(Double.Parse(DS.Tables[0].Rows[i]["Precio Unitario"].ToString()).ToString("C").Substring(1), fuente, Brushes.Black, new RectangleF(318, linea, 700, 200));
            }
            DS.Clear();
            e.Graphics.DrawString("..........................................", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            e.Graphics.DrawString("Sub-Total:", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            e.Graphics.DrawString(SubTotal, fuente, Brushes.Black, new RectangleF(318, linea, 700, 200));

            e.Graphics.DrawString("ITBIS:", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            e.Graphics.DrawString(ITBIS, fuente, Brushes.Black, new RectangleF(318, linea, 700, 200));

            e.Graphics.DrawString("Total Ventas:", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            e.Graphics.DrawString(Total, fuente, Brushes.Black, new RectangleF(318, linea, 700, 200));

            e.Graphics.DrawString("Vendedor:__________________________", fuente, Brushes.Black, new RectangleF(0, linea += 40, 700, 200));
            e.Graphics.DrawString("Despachado:________________________", fuente, Brushes.Black, new RectangleF(0, linea += 30, 700, 200));

            e.Graphics.DrawString("..........................................", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            e.Graphics.DrawString(coletilla, fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
        }

        private void btnImprimir_Click_1(object sender, EventArgs e)
        {
            if (idventa == string.Empty)
            {
                MessageBox.Show("Por favor seleccione un registro con el mouse antes de presionar el boton [Imprimir]", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                printDocument1 = new PrintDocument();
                var PS = new PrinterSettings();
                printDocument1.PrinterSettings = PS;
                printDocument1.PrintPage += Imprime;
                printDocument1.Print();
            }
        }
    }
}
