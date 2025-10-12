using DAL;
using DAL.Controler;
using DAL.Funciones;
using DAL.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication.Class;
using WebApplication.ViewModels;

namespace WebApplication
{
    public partial class Menu : Page
    {
        public MenuViewModels Models { get; private set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int IdVenta = 0;
                var vendedor = JsonConvert.DeserializeObject<Vendedor>(Session["Vendedor"].ToString());
                List<V_Cuentas> cuentas = new List<V_Cuentas>();
                cuentas = V_CuentasControler.Lista_IdVendedor(vendedor.id);
                if (cuentas.Count == 0)
                {
                    //en esta parte creamos una nueva cuenta para el vendedor
                    IdVenta = TablaVentas_f.NuevaVenta();
                    if (IdVenta > 0)
                    {
                        //ahora relacionamos el id del vendedor con el id de la venta
                        if (!R_VentaVendedor_f.Relacionar_Vendedor_Venta(IdVenta, vendedor.id))
                        {
                            AlertModerno.Error(this, "¡Error!", "No fue posible crear la relación del vendedor con la venta.", true);
                        }
                    }
                    else
                    {
                        AlertModerno.Error(this, "¡Error!", "No fue posible crear una nueva cuenta.", true);
                    }
                    //ahora si cargamos las cuentas
                    cuentas = new List<V_Cuentas>();
                    cuentas = V_CuentasControler.Lista_IdVendedor(vendedor.id);
                }
                else
                {
                    IdVenta = cuentas.FirstOrDefault().id;
                }
                var zonas = ZonasControler.Lista();
                int idzonaactiva = zonas.FirstOrDefault().id;
                var categorias = V_CategoriaControler.lista();
                int idcate = categorias.FirstOrDefault().id;
                Session["zonaactiva"] = idzonaactiva;
                Models = new MenuViewModels
                {
                    IdCuentaActiva = IdVenta,
                    IdZonaActiva = idzonaactiva,
                    IdMesaActiva = 0,
                    IdCategoriaActiva = idcate,
                    cuentas = cuentas,
                    zonas = zonas,
                    Mesas = MesasControler.Lista(idzonaactiva),
                    categorias = categorias,
                    productos = v_productoVentaControler.Lista(),
                    venta = V_TablaVentasControler.Consultar_Id(IdVenta),
                    detalleCaja = V_DetalleCajaControler.Lista_IdVenta(IdVenta),
                };

                // Usar ListaProductos según lo indicaste
                if (Models != null && Models.productos != null)
                {
                    rpProductos.DataSource = Models.productos;
                    rpProductos.DataBind();
                }

                Session["Models"] = Models;
                DataBind();
            }
        }

        protected void rpServicios_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AbrirServicio")
            {
                int idStr = Convert.ToInt32(e.CommandArgument);

                Models = new MenuViewModels();
                Models = Session["Models"] as MenuViewModels;

                Models.venta = V_TablaVentasControler.Consultar_Id(idStr);
                Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(idStr);

                Models.IdCuentaActiva = idStr;
                Session["Models"] = Models;

                //AlertModerno.Success(this,"ok",$"servicio seleccionado {idStr}",true);
                if (Models != null && Models.productos != null)
                {
                    rpProductos.DataSource = Models.productos;
                    rpProductos.DataBind();
                }
                DataBind();
            }
        }

        protected void btnNuevoServicio_Click(object sender, EventArgs e)
        {
            AlertModerno.Success(this, "ok", $"boton seleccionado {e.ToString()}", true);
        }

        protected void btnEliminarServicio_Clik(object sender, EventArgs e)
        {
            var btn = (System.Web.UI.HtmlControls.HtmlButton)sender;
            AlertModerno.Success(this, "ok", $"boton seleccionado {btn.ID}", true);
        }

        protected void rpZonas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "CambiarZona")
            {
                int idZona = Convert.ToInt32(e.CommandArgument);
                Session["zonaactiva"] = idZona;
                Models = new MenuViewModels();
                Models = Session["Models"] as MenuViewModels;
                Models.IdZonaActiva = idZona;
                Models.Mesas = MesasControler.Lista(idZona);
                if (Models != null && Models.productos != null)
                {
                    rpProductos.DataSource = Models.productos;
                    rpProductos.DataBind();
                }
                Session["Models"] = Models;
                DataBind();
            }
        }

        protected void rpMesas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Models = new MenuViewModels();
            Models = Session["Models"] as MenuViewModels;

            if (e.CommandName == "AbrirMesa")
            {
                int idMesa = Convert.ToInt32(e.CommandArgument);
                var mesa = MesasControler.Consultar_id(idMesa);
                if (mesa != null)
                {
                    //ahora consultamos si esta mesa esta en una de las cuentas activa
                    var cuentas = V_CuentasControler.Lista_Mesa(mesa.nombreMesa);
                    if (cuentas.Count != 0)
                    {
                        //cargamos los datos de la mesa
                        Models = new MenuViewModels();
                        Models = Session["Models"] as MenuViewModels;
                        Models.IdCuentaActiva = cuentas.FirstOrDefault().id;
                        Models.IdMesaActiva = idMesa;
                        Models.Mesas = MesasControler.Lista(Models.IdZonaActiva);
                        Session["Models"] = Models;
                        DataBind();
                    }
                    else
                    {
                        // Construimos los callbacks: hacer “click” en los botones ocultos
                        string jsConfirm = $"document.getElementById('{btnMesaNuevaCuenta.ClientID}').click();";

                        // AHORA: abrir modal (sin postback), seteando mesa y título
                        string jsDeny = $@"
abrirModalServicios('{System.Web.HttpUtility.JavaScriptStringEncode(mesa.nombreMesa ?? $"MESA {idMesa}")}', '{idMesa}');
";
                        // ==================== / FIN CAMBIO ÚNICO ============================

                        AlertModerno.ConfirmDual(
                            this,
                            $"Mesa - {mesa.nombreMesa}",
                            "¿Qué deseas hacer con esta mesa?",
                            jsConfirm,
                            jsDeny,
                            textoConfirm: "Nuevo servicio",
                            textoDeny: "Amarrar a existente",
                            textoCancel: "Cancelar"
                        );
                        //enviamos la pregunta

                        Models.IdMesaActiva = idMesa;
                        Models.Mesas = MesasControler.Lista(Models.IdZonaActiva);
                        if (Models != null && Models.productos != null)
                        {
                            rpProductos.DataSource = Models.productos;
                            rpProductos.DataBind();
                        }
                        Session["Models"] = Models;
                        DataBind();
                    }
                }


            }
        }
        protected void MesaNuevaCuenta(object sender, EventArgs e)
        {
            Models = new MenuViewModels();
            Models = Session["Models"] as MenuViewModels;

            var mesa = MesasControler.Consultar_id(Models.IdMesaActiva);
            if (mesa != null)
            {
                //creamos la nueva venta
                int idVenta = TablaVentas_f.NuevaVenta();

                if (idVenta > 0)
                {
                    Models.IdCuentaActiva = idVenta;
                    //amaramos la venta con la mesa
                    var rvm = R_VentaMesa_f.Relacionar_Venta_Mesa(idVenta, Models.IdMesaActiva);
                    if (rvm)
                    {
                        //cambiamos el estado de la mesa
                        mesa.estadoMesa = 1;
                        bool resp = MesasControler.CRUD(mesa, 1);
                        //amarramos al venta con el vendedor
                        var rvv = R_VentaVendedor_f.Relacionar_Vendedor_Venta(idVenta, Convert.ToInt32(Session["idvendedor"]));
                        if (rvv)
                        {
                            AlertModerno.Success(this, "Listo", $"Servicio #{idVenta} creado para la mesa {mesa.nombreMesa}", true, 2000);
                        }
                        else
                        {
                            AlertModerno.Error(this, "Error", $"Servicio #{idVenta} creado con exíto. pero no se amarro la mesa {mesa.nombreMesa}", true, 2000);
                        }
                    }
                    else
                    {
                        AlertModerno.Error(this, "Error", $"Servicio #{idVenta} creado con exíto. pero no se amarro la mesa {mesa.nombreMesa}", true, 2000);
                    }
                }
                else
                {
                    AlertModerno.Error(this, "Error", $"No se creo el servicio", true, 2000);
                }
            }
            else
            {
                AlertModerno.Error(this, "Error", $"No se encontro la mesa.", true, 2000);
            }
            Models.Mesas = MesasControler.Lista(Models.IdZonaActiva);
            Models.cuentas = V_CuentasControler.Lista_IdVendedor(Convert.ToInt32(Session["idvendedor"]));
            Models.venta = V_TablaVentasControler.Consultar_Id(Models.IdCuentaActiva);
            Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva);
            if (Models != null && Models.productos != null)
            {
                rpProductos.DataSource = Models.productos;
                rpProductos.DataBind();
            }
            Session["Models"] = Models;
            DataBind();
        }

        protected void MesaAmarar(object sender, EventArgs e)
        {
            int idMesa = int.Parse(hfMesaId.Value);
            int idServicio = int.Parse(hfServicioId.Value);

            Models = new MenuViewModels();
            Models = Session["Models"] as MenuViewModels;

            var mesa = MesasControler.Consultar_id(idMesa);
            mesa.estadoMesa = 1;
            bool respCrud = MesasControler.CRUD(mesa, 1);

            bool resp = R_VentaMesa_f.Relacionar_Venta_Mesa(idServicio, idMesa);
            if (resp)
            {
                AlertModerno.Success(this, "Amarrada",
    $"Mesa {mesa.nombreMesa} amarrada al servicio #{idServicio}.", true, 1200);
            }
            else
            {
                AlertModerno.Error(this, "Error",
    $"Mesa {mesa.nombreMesa} no fue amarrada al servicio #{idServicio}.", true, 1200);
            }

            Models.IdCuentaActiva = idServicio;
            Models.IdMesaActiva = idMesa;
            Models.cuentas = V_CuentasControler.Lista_IdVendedor(Convert.ToInt32(Session["idvendedor"]));
            Models.Mesas = MesasControler.Lista(Models.IdZonaActiva);
            Models.venta = V_TablaVentasControler.Consultar_Id(idServicio);
            Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(idServicio);
            if (Models != null && Models.productos != null)
            {
                rpProductos.DataSource = Models.productos;
                rpProductos.DataBind();
            }
            Session["Models"] = Models;
            DataBind();
        }

        //protected void rpCategorias_ItemCommand(object source, RepeaterCommandEventArgs e)
        //{
        //    if (e.CommandName == "SeleccionarCategoria")
        //    {
        //        int idCategoria = Convert.ToInt32(e.CommandArgument);

        //        Models = new MenuViewModels();
        //        Models = Session["Models"] as MenuViewModels;

        //        // Guardamos la categoría activa
        //        Models.IdCategoriaActiva = idCategoria;

        //        // Aquí puedes llamar la acción o recargar los productos de esa categoría
        //        Models.productos = v_productoVentaControler.Lista_IdCategoria(idCategoria);

        //        DataBind();
        //    }
        //}


        protected void rpProductos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Models = new MenuViewModels();
            Models = Session["Models"] as MenuViewModels;


            // DEBUG rápido
            System.Diagnostics.Debug.WriteLine("rpProductos_ItemCommand fired: " + e.CommandName);

            if (e.CommandName == "AgregarAlCarrito")
            {
                int idPresentacion;
                if (!int.TryParse(Convert.ToString(e.CommandArgument), out idPresentacion))
                {
                    // CommandArgument inválido
                    return;
                }

                // Obtener TextBox de la misma fila
                var txt = e.Item.FindControl("txtCantidad") as TextBox;
                int cantidad = 0;
                if (txt != null) int.TryParse(txt.Text, out cantidad);

                /* validamos la cantidad */
                if (cantidad <= 0)
                {
                    /* como la cantidad es 0 o menor a cero solamente hacemos en return sin enviar alert */
                    return;
                }

                // en esta parte llamamos la función que se encarga de recibir la cantidad y el id de la presentación.
                //retorna una respuesta true o false
                var resp = DetalleVenta_f.AgregarProducto(idPresentacion, cantidad, Models.IdCuentaActiva);
                if (resp.estado)
                {
                    //enviamos alert ok
                    AlertModerno.Success(this, "¡OK!", $"{resp.mensaje}", true);
                }
                else
                {
                    //enviamos alert error
                    AlertModerno.Error(this, "¡Error!", $"{resp.mensaje}", true);
                }

                // reiniciamos el cuadro de texto de cantidad
                if (txt != null) txt.Text = "0";

                Models.venta = V_TablaVentasControler.Consultar_Id(Models.IdCuentaActiva);
                Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva);
                // Usar ListaProductos según lo indicaste
                if (Models != null && Models.productos != null)
                {
                    rpProductos.DataSource = Models.productos;
                    rpProductos.DataBind();
                }
                Session["Models"] = Models;
                DataBind();
            }
        }

        protected void btnCuentaGeneral_Click(object sender, EventArgs e)
        {
            Models = new MenuViewModels();
            Models = Session["Models"] as MenuViewModels;
            // Usar ListaProductos según lo indicaste
            if (Models != null && Models.productos != null)
            {
                rpProductos.DataSource = Models.productos;
                rpProductos.DataBind();
            }
            Session["Models"] = Models;
            DataBind();
        }
    }
}