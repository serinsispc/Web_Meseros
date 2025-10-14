using DAL;
using DAL.Controler;
using DAL.Funciones;
using DAL.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication.Class;
using WebApplication.ViewModels;

namespace WebApplication
{
    public partial class Menu : Page
    {
        #region Constantes de sesión / claves
        private const string SessionVendedorKey = "Vendedor";
        private const string SessionZonaActivaKey = "zonaactiva";
        private const string SessionModelsKey = "Models";
        private const string SessionIdVendedorKey = "idvendedor";
        #endregion

        public MenuViewModels Models { get; private set; }

        // variable pública para usar en el .aspx
        public string CuentasJson { get; set; }
       

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Models = new MenuViewModels();
                Models = Session[SessionModelsKey] as MenuViewModels;

                if (!IsPostBack)
                {
                    InicializarPagina();
                }
                else
                {
                    ProcesarPostBack();
                }

                // Filtrar cuentas de la venta activa
                var cuentasActivas = Models.v_CuentaClientes
                .Where(x => x.idVenta == Models.IdCuentaActiva)
                .Select(x => new
                {
                    id = x.id,
                    nombre = x.nombreCuenta ?? x.nombreCuenta,
                    total = x.totalVenta ?? 0
                })
                .ToList();

                var serializer = new JavaScriptSerializer();

                CuentasJson = serializer.Serialize(cuentasActivas);
            }
            catch (Exception ex)
            {
                // Error general en Page_Load
                System.Diagnostics.Debug.WriteLine("Page_Load error: " + ex);
                AlertModerno.Error(this, "¡Error!", "Ocurrió un error inesperado al cargar la página.", true);
            }
        }

        #region Inicialización (primer load)

        private void InicializarPagina()
        {
            var vendedor = ObtenerVendedorDesdeSession();
            if (vendedor == null)
            {
                AlertModerno.Error(this, "¡Error!", "No se encontró la sesión del vendedor.", true);
                return;
            }

            // Obtener cuentas del vendedor
            var cuentas = V_CuentasControler.Lista_IdVendedor(vendedor.id) ?? new List<V_Cuentas>();
            int idVenta;

            if (!cuentas.Any())
            {
                idVenta = TablaVentas_f.NuevaVenta();
                if (idVenta <= 0)
                {
                    AlertModerno.Error(this, "¡Error!", "No fue posible crear una nueva cuenta.", true);
                    return;
                }

                var relacionado = R_VentaVendedor_f.Relacionar_Vendedor_Venta(idVenta, vendedor.id);
                if (!relacionado)
                {
                    AlertModerno.Error(this, "¡Error!", "No fue posible crear la relación del vendedor con la venta.", true);
                    // aún así intentamos recargar cuentas para que la UI no quede rota
                }

                // recargar cuentas
                cuentas = V_CuentasControler.Lista_IdVendedor(vendedor.id) ?? new List<V_Cuentas>();
            }
            else
            {
                idVenta = cuentas.First().id;
            }

            // Cargar colecciones base
            var zonas = ZonasControler.Lista() ?? new List<Zonas>();
            var categorias = V_CategoriaControler.lista() ?? new List<V_Categoria>();
            var mesas = MesasControler.Lista() ?? new List<Mesas>();
            var productos = v_productoVentaControler.Lista() ?? new List<v_productoVenta>();

            int idZonaActiva = zonas.FirstOrDefault()?.id ?? 0;
            int idCategoriaActiva = categorias.FirstOrDefault()?.id ?? 0;

            // Guardar zona activa en sesión (si otros módulos la usan)
            Session[SessionZonaActivaKey] = idZonaActiva;
            var listacc = V_CuentaClienteCotroler.Lista(false);

            // Construir ViewModel
            Models = new MenuViewModels
            {
                IdCuentaActiva = idVenta,
                IdZonaActiva = idZonaActiva,
                IdMesaActiva = 0,
                IdCategoriaActiva = idCategoriaActiva,
                IdCuenteClienteActiva = listacc.FirstOrDefault().id,
                cuentas = cuentas,
                zonas = zonas,
                Mesas = mesas,
                categorias = categorias,
                productos = productos,
                venta = V_TablaVentasControler.Consultar_Id(idVenta),
                detalleCaja = V_DetalleCajaControler.Lista_IdVenta(idVenta),
                v_CuentaClientes =listacc
            };

            GuardarModelsEnSesion();
            BindProductos();
            DataBind();
        }

        private Vendedor ObtenerVendedorDesdeSession()
        {
            try
            {
                if (Session[SessionVendedorKey] == null) return null;
                var json = Session[SessionVendedorKey].ToString();
                if (string.IsNullOrWhiteSpace(json)) return null;
                return JsonConvert.DeserializeObject<Vendedor>(json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ObtenerVendedorDesdeSession error: " + ex);
                return null;
            }
        }

        #endregion

        #region PostBack y eventos globales

        private void ProcesarPostBack()
        {
            var eventTarget = Request["__EVENTTARGET"];
            var eventArgument = Request["__EVENTARGUMENT"];

            System.Diagnostics.Debug.WriteLine("EventTarget: " + eventTarget);
            System.Diagnostics.Debug.WriteLine("EventArgument: " + eventArgument);

            if (string.IsNullOrEmpty(eventTarget)) return;

            switch (eventTarget)
            {
                case "ActualizarCantidad":
                    ProcesarActualizarCantidad(eventArgument);
                    break;

                case "EliminarDetalle":
                    ProcesarEliminarDetalle(eventArgument);
                    break;

                // en el switch donde procesas event targets:
                case "GuardarCuenta":
                    ProcesarGuardarCuenta(eventArgument);
                    break;

                case "AnclarDetalle":
                    ProcesarAnclarDetalle(eventArgument);
                    break;

                case "DividirDetalle":
                    ProcesarDividirDetalle(eventArgument);
                    break;

                default:
                    // otros eventos por nombre...
                    break;
            }
        }
        private void ProcesarDividirDetalle(string eventArgument)
        {
            if (string.IsNullOrWhiteSpace(eventArgument)) return;

            var parts = eventArgument.Split('|');
            if (parts.Length == 3)
            {
                if (int.TryParse(parts[0], out int detalleId) &&
                    int.TryParse(parts[1], out int cantidadActual) &&
                    int.TryParse(parts[2], out int cantidadDividir))
                {
                    var respuestadal = DetalleVenta_f.Dividir(detalleId, cantidadActual, cantidadDividir,Models.IdCuentaActiva);
                    string titulo;
                    if (respuestadal.estado)
                    {
                        titulo = "ok";
                        AlertModerno.Success(this, titulo, respuestadal.mensaje, true, 1000);
                    }
                    else
                    {
                        titulo = "Error";
                        AlertModerno.Error(this, titulo, respuestadal.mensaje, true, 1000);
                    }

                    Models.v_CuentaClientes = V_CuentaClienteCotroler.Lista(false);
                    Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva);

                    GuardarModelsEnSesion();
                    BindProductos();
                    DataBind();
                }
            }

        }
        private void ProcesarAnclarDetalle(string eventArgument)
        {
            if (string.IsNullOrWhiteSpace(eventArgument)) return;

            var parts = eventArgument.Split('|');
            if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], out int detalleId) &&
                    int.TryParse(parts[1], out int cuentaId))
                {
                    // Aquí llamas tu método que ancla el detalle a la cuenta
                    System.Diagnostics.Debug.WriteLine($"detalle ID {detalleId} con cuenta id {cuentaId}");

                    

                    var respuestadal = R_CuentaCliente_DetalleVenta_f.Insert(cuentaId,detalleId);
                    string titulo;
                    if (respuestadal.estado) 
                    { 
                        titulo = "ok";
                        AlertModerno.Success(this, titulo, respuestadal.mensaje, true, 1000);
                    } else 
                    { 
                        titulo = "Error";
                        AlertModerno.Error(this, titulo, respuestadal.mensaje, true, 1000);
                    }

                    Models.v_CuentaClientes = V_CuentaClienteCotroler.Lista(false);
                    Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva);

                    GuardarModelsEnSesion();
                    BindProductos();
                    DataBind();
                }
            }

        }
        private void ProcesarActualizarCantidad(string eventArgument)
        {
            if (string.IsNullOrWhiteSpace(eventArgument)) return;

            var partes = eventArgument.Split('|');
            if (partes.Length < 2) return;

            if (!int.TryParse(partes[0], out int id)) return;
            if (!int.TryParse(partes[1], out int cantidad)) return;

            System.Diagnostics.Debug.WriteLine($"Actualizar ID {id} con cantidad {cantidad}");
            ActualizarCantidadEnBaseDeDatos(id, cantidad);
        }

        private void ProcesarEliminarDetalle(string eventArgument)
        {
            if (string.IsNullOrWhiteSpace(eventArgument)) return;

            var partes = eventArgument.Split('|');
            if (partes.Length < 2) return;

            if (!int.TryParse(partes[0], out int id)) return;
            var nota = partes[1];

            EliminarDetalle(id, nota);
        }

        // ------------------ método a añadir en tu clase ------------------
        private void ProcesarGuardarCuenta(string eventArgument)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(eventArgument)) return;

                // formato: mode|id|nombre
                var partes = eventArgument.Split(new char[] { '|' }, 3);
                if (partes.Length < 3) return;

                var mode = partes[0]; // "crear" o "editar"
                var idPart = partes[1];
                var nombre = partes[2]?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(nombre) || nombre.Length < 2)
                {
                    // puedes mostrar alerta al usuario
                    AlertModerno.Error(this, "Error", "Nombre inválido.", true);
                    return;
                }

                if (mode == "crear")
                {
                    CrearCuenta(nombre);
                }
                else if (mode == "editar")
                {
                    if (!int.TryParse(idPart, out int idCuenta))
                    {
                        AlertModerno.Error(this, "Error", "ID de cuenta inválido.", true);
                        return;
                    }
                    EditarCuenta(idCuenta, nombre);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ProcesarGuardarCuenta error: " + ex);
                AlertModerno.Error(this, "Error", "Ocurrió un error al guardar la cuenta.", true);
            }
        }

        #endregion

        #region Helpers: carga/bind/sesión

        private void CargarModelsDesdeSesion()
        {
            try
            {
                Models = Session[SessionModelsKey] as MenuViewModels;
                if (Models == null)
                {
                    // Reconstruir mínimamente si no hay sesión
                    ReconstruirModelsBasico();
                }

                // Re-bind productos si existen
                BindProductos();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CargarModelsDesdeSesion error: " + ex);
            }
        }

        private void ReconstruirModelsBasico()
        {
            // Intento de reconstrucción conservadora para evitar nulls
            var vendedor = ObtenerVendedorDesdeSession();
            int vendedorId = vendedor?.id ?? (Session[SessionIdVendedorKey] != null ? Convert.ToInt32(Session[SessionIdVendedorKey]) : 0);
            var cuentas = V_CuentasControler.Lista_IdVendedor(vendedorId) ?? new List<V_Cuentas>();
            int idCuenta = cuentas.FirstOrDefault()?.id ?? 0;

            Models = new MenuViewModels
            {
                IdCuentaActiva = idCuenta,
                cuentas = cuentas,
                zonas = ZonasControler.Lista() ?? new List<Zonas>(),
                Mesas = MesasControler.Lista() ?? new List<Mesas>(),
                categorias = V_CategoriaControler.lista() ?? new List<V_Categoria>(),
                productos = v_productoVentaControler.Lista() ?? new List<v_productoVenta>(),
                venta = V_TablaVentasControler.Consultar_Id(idCuenta),
                detalleCaja = V_DetalleCajaControler.Lista_IdVenta(idCuenta)
            };

            GuardarModelsEnSesion();
        }

        private void BindProductos()
        {
            try
            {
                if (Models?.productos != null && Models.productos.Any())
                {
                    rpProductos.DataSource = Models.productos;
                }
                else
                {
                    rpProductos.DataSource = null;
                }
                rpProductos.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BindProductos error: " + ex);
            }
        }

        private void GuardarModelsEnSesion()
        {
            Session[SessionModelsKey] = Models;
        }

        #endregion

        #region Comandos de repeaters / botones

        protected void rpServicios_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "AbrirServicio") return;

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int idServicio)) return;

            CargarModelsDesdeSesion();
            Models.venta = V_TablaVentasControler.Consultar_Id(idServicio);
            Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(idServicio);

            Models.IdCuentaActiva = idServicio;
            GuardarModelsEnSesion();

            BindProductos();
            DataBind();
        }

        protected void rpZonas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "CambiarZona") return;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int idZona)) return;

            Session[SessionZonaActivaKey] = idZona;
            CargarModelsDesdeSesion();
            Models.IdZonaActiva = idZona;
            GuardarModelsEnSesion();

            BindProductos();
            DataBind();
        }

        protected void rpMesas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            CargarModelsDesdeSesion();

            if (e.CommandName != "AbrirMesa")
            {
                // solo manejamos AbrirMesa en este handler
                return;
            }

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int idMesa)) return;

            var mesa = MesasControler.Consultar_id(idMesa);
            if (mesa == null)
            {
                AlertModerno.Error(this, "Error", "No se encontró la mesa.", true);
                return;
            }

            var cuentasMesa = V_CuentasControler.Lista_Mesa(mesa.nombreMesa) ?? new List<V_Cuentas>();
            if (cuentasMesa.Any())
            {
                // La mesa ya está asociada a una cuenta
                Models.IdCuentaActiva = cuentasMesa.First().id;
                Models.IdMesaActiva = idMesa;
                Models.Mesas = MesasControler.Lista();
                GuardarModelsEnSesion();
                DataBind();
            }
            else
            {
                // Preguntar acción: nuevo servicio o amarrar a existente (usa AlertModerno con callbacks JS)
                string jsConfirm = $"document.getElementById('{btnMesaNuevaCuenta.ClientID}').click();";
                string jsDeny = $@"abrirModalServicios('{System.Web.HttpUtility.JavaScriptStringEncode(mesa.nombreMesa ?? $"MESA {idMesa}")}', '{idMesa}');";

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

                Models.IdMesaActiva = idMesa;
                Models.Mesas = MesasControler.Lista();
                GuardarModelsEnSesion();
            }

            BindProductos();
            DataBind();
        }

        protected void MesaNuevaCuenta(object sender, EventArgs e)
        {
            CargarModelsDesdeSesion();

            var mesa = MesasControler.Consultar_id(Models.IdMesaActiva);
            if (mesa == null)
            {
                AlertModerno.Error(this, "Error", "No se encontró la mesa.", true, 2000);
                return;
            }

            int idVenta = TablaVentas_f.NuevaVenta();
            if (idVenta <= 0)
            {
                AlertModerno.Error(this, "Error", "No se creó el servicio.", true, 2000);
                return;
            }

            Models.IdCuentaActiva = idVenta;

            // Relacionar venta - mesa
            var rvm = R_VentaMesa_f.Relacionar_Venta_Mesa(idVenta, Models.IdMesaActiva);
            if (!rvm)
            {
                AlertModerno.Error(this, "Error", $"Servicio #{idVenta} creado pero no se amarró la mesa {mesa.nombreMesa}", true, 2000);
            }
            else
            {
                mesa.estadoMesa = 1;
                MesasControler.CRUD(mesa, 1);

                // amarro venta con vendedor (uso session idvendedor si existe)
                var rvv = R_VentaVendedor_f.Relacionar_Vendedor_Venta(idVenta, ObtenerIdVendedorSeguro());
                if (rvv)
                {
                    AlertModerno.Success(this, "Listo", $"Servicio #{idVenta} creado para la mesa {mesa.nombreMesa}", true, 2000);
                }
                else
                {
                    AlertModerno.Error(this, "Error", $"Servicio #{idVenta} creado con éxito, pero no se amarró al vendedor.", true, 2000);
                }
            }

            // Actualizar modelos y UI
            Models.Mesas = MesasControler.Lista();
            Models.cuentas = V_CuentasControler.Lista_IdVendedor(ObtenerIdVendedorSeguro());
            Models.venta = V_TablaVentasControler.Consultar_Id(Models.IdCuentaActiva);
            Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva);

            GuardarModelsEnSesion();
            BindProductos();
            DataBind();
        }

        protected void MesaAmarar(object sender, EventArgs e)
        {
            if (!int.TryParse(hfMesaId?.Value, out int idMesa)) return;
            if (!int.TryParse(hfServicioId?.Value, out int idServicio)) return;

            CargarModelsDesdeSesion();

            var mesa = MesasControler.Consultar_id(idMesa);
            if (mesa == null)
            {
                AlertModerno.Error(this, "Error", "No se encontró la mesa.", true);
                return;
            }

            mesa.estadoMesa = 1;
            MesasControler.CRUD(mesa, 1);

            bool resp = R_VentaMesa_f.Relacionar_Venta_Mesa(idServicio, idMesa);
            if (resp)
            {
                AlertModerno.Success(this, "Amarrada", $"Mesa {mesa.nombreMesa} amarrada al servicio #{idServicio}.", true, 1200);
            }
            else
            {
                AlertModerno.Error(this, "Error", $"Mesa {mesa.nombreMesa} no fue amarrada al servicio #{idServicio}.", true, 1200);
            }

            Models.IdCuentaActiva = idServicio;
            Models.IdMesaActiva = idMesa;
            Models.cuentas = V_CuentasControler.Lista_IdVendedor(ObtenerIdVendedorSeguro());
            Models.Mesas = MesasControler.Lista();
            Models.venta = V_TablaVentasControler.Consultar_Id(idServicio);
            Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(idServicio);

            GuardarModelsEnSesion();
            BindProductos();
            DataBind();
        }

        protected void rpProductos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

            if (e.CommandName != "AgregarAlCarrito") return;

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int idPresentacion))
            {
                // CommandArgument inválido
                return;
            }

            var txt = e.Item.FindControl("txtCantidad") as TextBox;
            int cantidad = 0;
            if (txt != null) int.TryParse(txt.Text, out cantidad);

            if (cantidad <= 0)
            {
                AlertModerno.Error(this, "¡Error!", $"la cantidad reportada es ({cantidad})", true);
                BindProductos();
                DataBind();
                return;
            }

            var resp = DetalleVenta_f.AgregarProducto(idPresentacion, cantidad, Models.IdCuentaActiva);
            if (resp.estado)
            {
                AlertModerno.Success(this, "¡OK!", $"{resp.mensaje}", true, 800);
            }
            else
            {
                AlertModerno.Error(this, "¡Error!", $"{resp.mensaje}", true);
            }

            if (txt != null) txt.Text = "0";

            Models.venta = V_TablaVentasControler.Consultar_Id(Models.IdCuentaActiva);
            Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva);

            GuardarModelsEnSesion();
            BindProductos();
            DataBind();
        }

        protected void btnCuentaGeneral_Click(object sender, EventArgs e)
        {
            CargarModelsDesdeSesion();
            BindProductos();
            GuardarModelsEnSesion();
            DataBind();
        }

        #endregion

        #region Operaciones sobre detalle (cantidad / eliminar)

        private void ActualizarCantidadEnBaseDeDatos(int id, int cantidad)
        {
            try
            {
                CargarModelsDesdeSesion();

                var respdal = DetalleVenta_f.ActualizarCantidadDetalle(id, cantidad);
                if (respdal.estado)
                {
                    AlertModerno.Success(this, "Ok", respdal.mensaje, true, 500);
                }
                else
                {
                    AlertModerno.Error(this, "Error", respdal.mensaje, true, 500);
                }

                Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva);
                Models.venta = V_TablaVentasControler.Consultar_Id(Models.IdCuentaActiva);

                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ActualizarCantidadEnBaseDeDatos error: " + ex);
                AlertModerno.Error(this, "Error", "No se pudo actualizar la cantidad.", true);
            }
        }

        private void EliminarDetalle(int id, string nota)
        {
            try
            {
                CargarModelsDesdeSesion();

                var respdal = DetalleVenta_f.Eliminar(id, nota);
                if (respdal.estado)
                {
                    AlertModerno.Success(this, "Ok", respdal.mensaje, true, 500);
                }
                else
                {
                    AlertModerno.Error(this, "Error", respdal.mensaje, true, 500);
                }

                Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva);
                Models.venta = V_TablaVentasControler.Consultar_Id(Models.IdCuentaActiva);

                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("EliminarDetalle error: " + ex);
                AlertModerno.Error(this, "Error", "No se pudo eliminar el detalle.", true);
            }
        }

        private void CrearCuenta(string nombreCuenta)
        {
            int idventa= Models.IdCuentaActiva;
            // TODO: reemplaza estas llamadas por tus funciones DAL reales
            // Ejemplo genérico: crear una nueva venta/tabla de cuentas y asignarle nombre
            int nuevoId = CuentaCliente_f.Crear(idventa,nombreCuenta); // si ese es el flujo para crear 'cuenta'
            if (nuevoId <= 0)
            {
                AlertModerno.Error(this, "Error", "No fue posible crear la cuenta.", true);
                return;
            }

            // Si tienes un método para asignar nombre a la cuenta, usarlo:
            // bool ok = V_CuentasControler.AsignarNombreCuenta(nuevoId, nombreCuenta);
            // Si no, haz la llamada DAL que corresponda. Aquí dejo un placeholder:
            bool ok = true; // reemplaza

            if (ok)
            {
                // refrescar Models para que se vea en UI
                CargarModelsDesdeSesion(); // o reconstruir Models
                                           // Forzar recarga de cuentas desde BD
                Models.v_CuentaClientes = V_CuentaClienteCotroler.Lista(false);
                GuardarModelsEnSesion();
                BindProductos();
                AlertModerno.Success(this, "Creado", $"Cuenta creada con nombre '{nombreCuenta}'.", true, 1500);
            }
            else
            {
                AlertModerno.Error(this, "Error", "No fue posible asignar nombre a la cuenta.", true);
            }
        }

        private void EditarCuenta(int idCuenta, string nuevoNombre)
        {
            // TODO: reemplaza por la llamada real que actualice campo nombre
            // Ej: bool ok = V_CuentasControler.EditarNombre(idCuenta, nuevoNombre);
            bool ok = CuentaCliente_f.Editar(idCuenta,nuevoNombre); ; // placeholder

            if (ok)
            {
                // refrescar datos
                Models.v_CuentaClientes = V_CuentaClienteCotroler.Lista(false);
                GuardarModelsEnSesion();
                BindProductos();
                AlertModerno.Success(this, "Actualizado", $"Nombre de cuenta actualizado.", true, 1200);
            }
            else
            {
                AlertModerno.Error(this, "Error", "No fue posible actualizar la cuenta.", true);
            }
        }

        #endregion

        #region Utilidades

        private int ObtenerIdVendedorSeguro()
        {
            try
            {
                var vendedor = ObtenerVendedorDesdeSession();
                if (vendedor != null) return vendedor.id;
                if (Session[SessionIdVendedorKey] != null) return Convert.ToInt32(Session[SessionIdVendedorKey]);
            }
            catch { /* ignorar */ }
            return 0;
        }

        #endregion

        protected void btnEliminarServicio_ServerClick(object sender, EventArgs e)
        {

        }
    }
}
