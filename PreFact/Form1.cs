using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PreFact.Clases;

namespace PreFact
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DgvData.Rows.Clear();
            var conexion = new ConxionDB();
            var comando = new SqlCommand("SELECT ITBIS_Fijo FROM Info_General");
            ITBIS_FIJO = double.Parse(conexion.ExtraerDataSet(comando).Tables[0].Rows[0][0].ToString());
            Precio_Base = 1 + ITBIS_FIJO;
        }

        public static Double ITBIS_FIJO = 0.00;
        public static Double Precio_Base = 0.00;
        public static int Decimales = 2;
        public string IDVENTA { get; set; }

        private void Form1_Load(object sender, EventArgs e)
        {
            OperacionesComunes.DesactivarControles(false, txtCliente, txtTelefono, txtDireccion, txtCondiciones,
            txtDescripcion, txtCantidad, txtPrecio, btnImprimir, btnLimpiar, btnAgregar, dateTimePicker1);
            OperacionesComunes.DesactivarControles(true, btnNuevo);
        }

        private async void btnImprimir_Click(object sender, EventArgs e)
        {
            btnImprimir.Enabled = false;
            if (!OperacionesComunes.ValidarCampos(txtCliente, txtTelefono, txtDireccion, txtCondiciones))
            {
                MessageBox.Show("Debe de completar cada uno de los siguientes campos para guardar la información: '" + lblCliente.Text + "', " +
                "'" + lblTelefono.Text + "', '" + lblDireccion.Text + "', '" + lblCondiciones.Text + "'", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                btnImprimir.Enabled = true;
            }
            else if (!OperacionesComunes.ValidarCampos(DgvData))
            {
                MessageBox.Show("Debe de al menos agregar un producto a la tabla para guardar la información", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                btnImprimir.Enabled = true;
            }
            else
            {
                var VentaDetail = new VentaDetail();
                var Venta = new Venta()
                {
                    Cliente = txtCliente.Text.ToUpper(),
                    Telefono = txtTelefono.Text.ToUpper(),
                    Direccion = txtDireccion.Text.ToUpper(),
                    Condiciones = txtCondiciones.Text.ToUpper(),
                    SubTotal = double.Parse(lblSubTotal_Valor.Text.ToUpper()),
                    ITBIS = double.Parse(lblITBIS_Valor.Text.ToUpper()),
                    Total = double.Parse(lblTotalVentas_Valor.Text.ToUpper()),
                    Fecha = dateTimePicker1.Value
                };
                await Task.Run(() =>
                {
                    IDVENTA = Venta.InsertarVenta();
                    var dgv = new List<DataGridViewRow>(DgvData.Rows.Cast<DataGridViewRow>());
                    //Se elimina el registro en blanco del Datagrdiview
                    dgv.RemoveAt((dgv.Count - 1));
                    foreach (DataGridViewRow Fila in dgv)
                    {
                        VentaDetail.IdVenta = int.Parse(IDVENTA);
                        VentaDetail.Descripcion = Fila.Cells[0].Value.ToString();
                        VentaDetail.Cantidad = int.Parse(Fila.Cells[1].Value.ToString());
                        VentaDetail.Precio_Unitario = double.Parse(Fila.Cells[2].Value.ToString());
                        VentaDetail.Monto = double.Parse(Fila.Cells[3].Value.ToString());
                        VentaDetail.ITBIS_Articulo = double.Parse(Fila.Cells[4].Value.ToString());
                        VentaDetail.InsertarVentaDetail();
                    }
                    OperacionesComunes.OperacionRealizada();
                });
                txtNoFactura.Text = OperacionesComunes.IDVentaFormateado(IDVENTA.ToString());
                OperacionesComunes.DesactivarControles(false, txtCliente, txtTelefono, txtDireccion, txtCondiciones,
                txtDescripcion, txtCantidad, txtPrecio, btnImprimir, btnLimpiar, btnAgregar, dateTimePicker1);
                OperacionesComunes.DesactivarControles(true, btnNuevo);
                dateTimePicker1.Value = DateTime.Now;
                printDocument1 = new PrintDocument();
                var PS = new PrinterSettings();
                printDocument1.PrinterSettings = PS;
                printDocument1.PrintPage += Imprime;
                printDocument1.Print();
            }
        }

        private void Imprime(object sender, PrintPageEventArgs e)
        {
            Font fuente = new Font("Consolas", 8, FontStyle.Regular);
            var Conexion = new Clases.ConxionDB();
            var DS = new DataSet();
            int linea = 0;
            DS = Conexion.ExtraerDataSet(new SqlCommand("EXEC FACTURAHEADER"));
            e.Graphics.DrawString(DS.Tables[0].Rows[0]["Negocio"].ToString(), fuente, Brushes.Black, new RectangleF(0, linea, 700, 200));
            e.Graphics.DrawString(DS.Tables[0].Rows[0]["Direccion"].ToString(), fuente, Brushes.Black, new RectangleF(0, linea+=20, 700, 200));
            e.Graphics.DrawString(DS.Tables[0].Rows[0]["Telefono"].ToString(), fuente, Brushes.Black, new RectangleF(0, linea+=20, 700, 200));
            e.Graphics.DrawString(".........................................", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            string coletilla = DS.Tables[0].Rows[0]["Coletilla"].ToString();
            DS.Clear();
            DS = Conexion.ExtraerDataSet(new SqlCommand("EXEC FACTURA " + IDVENTA + ""));
            var SubTotal = Double.Parse(DS.Tables[0].Rows[0]["Sub-Total"].ToString()).ToString("C").Substring(1);
            var ITBIS = Double.Parse(DS.Tables[0].Rows[0]["ITBIS"].ToString()).ToString("C").Substring(1);
            var Total = Double.Parse(DS.Tables[0].Rows[0]["Total"].ToString()).ToString("C").Substring(1);
            var dtp = new DateTimePicker();
            dtp.Value = DateTime.Parse(DS.Tables[0].Rows[0]["Fecha"].ToString());
            e.Graphics.DrawString("Fecha: " + dtp.Value.ToShortDateString() +"", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
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
            DS = Conexion.ExtraerDataSet(new SqlCommand("EXEC FACTURADETAIL " + IDVENTA + ""));
            for (var i = 0; i<DS.Tables[0].Rows.Count;i++)
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

            e.Graphics.DrawString("Vendedor:__________________________", fuente, Brushes.Black, new RectangleF(0, linea+=40, 700, 200));
            e.Graphics.DrawString("Despachado:________________________", fuente, Brushes.Black, new RectangleF(0, linea += 30, 700, 200));

            e.Graphics.DrawString("..........................................", fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
            e.Graphics.DrawString(coletilla, fuente, Brushes.Black, new RectangleF(0, linea += 20, 700, 200));
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            OperacionesComunes.Limpiar(txtCliente, txtTelefono, txtDireccion, txtCondiciones, txtDescripcion,
            txtCantidad, txtPrecio, txtNoFactura);
            OperacionesComunes.DesactivarControles(true, txtCliente, txtTelefono, txtDireccion, txtCondiciones,
            txtDescripcion, txtCantidad, txtPrecio, btnImprimir, btnLimpiar, btnAgregar, dateTimePicker1);
            OperacionesComunes.Limpiar(DgvData);
            OperacionesComunes.DesactivarControles(false, btnNuevo);
            lblITBIS_Valor.Text = OperacionesComunes.MostrarDecimales(0.ToString());
            lblSubTotal_Valor.Text = OperacionesComunes.MostrarDecimales(0.ToString());
            lblTotalVentas_Valor.Text = OperacionesComunes.MostrarDecimales(0.ToString());
            dateTimePicker1.Value = DateTime.Now;
            txtCliente.Focus();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            OperacionesComunes.Limpiar(txtCliente, txtTelefono, txtDireccion, txtCondiciones, txtDescripcion,
            txtCantidad, txtPrecio, txtNoFactura);
            lblTotalVentas_Valor.Text = OperacionesComunes.ValorCero;
            lblITBIS_Valor.Text = OperacionesComunes.ValorCero;
            lblSubTotal_Valor.Text = OperacionesComunes.ValorCero;
            OperacionesComunes.Limpiar(DgvData);
            dateTimePicker1.Value = DateTime.Now;
            txtCliente.Focus();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (txtDescripcion.Text.Trim() == string.Empty || txtPrecio.Text.Trim() == string.Empty || txtCantidad.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Para agregar un registro verifique que los siguientes campos tenga informacion: " + lblDescripcion.Text + ", " + lblCantidad.Text + ", " + lblPrecio.Text + "", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (double.TryParse(txtCantidad.Text, out _) == false || double.TryParse(txtPrecio.Text, out _) == false)
            {
                MessageBox.Show("Para agregar un registro verifique que los siguientes campos contengan valores numericos: " + lblCantidad.Text + ", " + lblPrecio.Text + "", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (double.Parse(txtCantidad.Text) <= 0 || double.Parse(txtPrecio.Text) <= 0)
            {
                MessageBox.Show("Se deben de insertar valores mayores que 0 en los campos "+lblCantidad.Text+" y "+lblPrecio.Text+"", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                DgvData.Rows.Add(txtDescripcion.Text.ToUpper(), txtCantidad.Text, OperacionesComunes.MostrarDecimales(txtPrecio.Text), Math.Round((double.Parse(txtPrecio.Text) * double.Parse(txtCantidad.Text)), Decimales).ToString("C").Substring(1), Math.Round((((double.Parse(txtPrecio.Text) * double.Parse(txtCantidad.Text)) / Precio_Base) * ITBIS_FIJO), Decimales).ToString("C").Substring(1));
                lblTotalVentas_Valor.Text = SumatoriaColumna(3).Substring(1);
                lblSubTotal_Valor.Text = Math.Round(double.Parse(lblTotalVentas_Valor.Text) / Precio_Base, Decimales).ToString("C").Substring(1);
                lblITBIS_Valor.Text = Math.Round(double.Parse(lblSubTotal_Valor.Text) * ITBIS_FIJO, Decimales).ToString("C").Substring(1);
                OperacionesComunes.Limpiar(txtDescripcion, txtCantidad, txtPrecio);
                txtDescripcion.Focus();
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Desea cerrar PreFact?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private string SumatoriaColumna(int Columna)
        {
            double sumatoria = 0.00;
            if (DgvData.Rows.Count > 1)
            {
                for (int Fila = 0; Fila < (DgvData.Rows.Count - 1); Fila++)
                {
                    sumatoria = sumatoria + double.Parse(DgvData.Rows[Fila].Cells[Columna].Value.ToString());
                }
            }
            else
            {
                return DgvData.Rows[0].Cells[0].Value.ToString();
            }
            return sumatoria.ToString("C");
        }

        private void DgvData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DgvData.CurrentRow.Index < (DgvData.Rows.Count - 1))
            {
                if (MessageBox.Show("¿Desea eliminar el registro seleccionado de la tabla?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    lblTotalVentas_Valor.Text = (double.Parse(lblTotalVentas_Valor.Text) - double.Parse(DgvData.CurrentRow.Cells["Monto"].Value.ToString())).ToString("C").Substring(1);
                    lblSubTotal_Valor.Text = (double.Parse(lblSubTotal_Valor.Text) - (double.Parse(DgvData.CurrentRow.Cells["Monto"].Value.ToString()) - double.Parse(DgvData.CurrentRow.Cells["ITBIS"].Value.ToString()))).ToString("C").Substring(1);
                    lblITBIS_Valor.Text = (double.Parse(lblITBIS_Valor.Text) - double.Parse(DgvData.CurrentRow.Cells["ITBIS"].Value.ToString())).ToString("C").Substring(1);
                    if (lblTotalVentas_Valor.Text == "0" && lblITBIS_Valor.Text == "0" && lblSubTotal_Valor.Text == "0")
                    {
                        lblTotalVentas_Valor.Text = OperacionesComunes.MostrarDecimales(0.ToString());
                        lblITBIS_Valor.Text = OperacionesComunes.MostrarDecimales(0.ToString());
                        lblSubTotal_Valor.Text = OperacionesComunes.MostrarDecimales(0.ToString());
                    }
                    DgvData.Rows.RemoveAt(DgvData.CurrentRow.Index);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var configuraciones = new Configuraciones();
            this.Hide();
            configuraciones.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
  
    }
}