using DAL;
using DAL.Controler;
using DAL.Funciones;
using DAL.Model;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication.Class;
using WebApplication.ViewModels;
using static System.Net.Mime.MediaTypeNames;

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
                if(Models == null)
                {
                    Models.v_CuentaClientes = new List<V_CuentaCliente>();
                } 

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

        public void InicializarPagina()
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
                idVenta = TablaVentas_f.NuevaVenta((int)Session["porpropina"]);
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
                estadopropina = (bool)Session["estadopropina"],
                porpropina = (int)Session["porpropina"],
                IdMesero = Convert.ToInt32(Session["idvendedor"].ToString()),
                NombreMesero = Session["NombreMesero"].ToString(),
                IdCuentaActiva = idVenta,
                IdZonaActiva = idZonaActiva,
                IdMesaActiva = 0,
                IdCategoriaActiva = idCategoriaActiva,
                IdCuenteClienteActiva = 0,
                cuentas = cuentas,
                zonas = zonas,
                Mesas = mesas,
                categorias = categorias,
                productos = productos,
                venta = V_TablaVentasControler.Consultar_Id(idVenta),
                ventaCuenta = V_CuentaClienteCotroler.Consultar(0),
                detalleCaja = V_DetalleCajaControler.Lista_IdVenta(idVenta, 0),
                v_CuentaClientes = listacc,
                adiciones = V_CatagoriaAdicionControler.Lista()
            };

            GuardarModelsEnSesion();
            BindProductos();
            DataBind();
        }

        public Vendedor ObtenerVendedorDesdeSession()
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

                case "NotasDetalle":
                    ProcesarNotasDetalle(eventArgument);
                    break;

                case "btnNuevoServicio":
                    BTN_NuevoServicio();
                    break;

                case "btnEliminarServicio":
                    btnEliminarServicio();
                    break;

                case "btnLiberarMesa":
                    btnLiberarMesa(eventArgument);
                    break;

                case "btnMesa":
                    rpMesas_ItemCommand(eventArgument);
                    break;

                case "btnCuentaCliente":
                    btnCuentaCliente(eventArgument);
                    break;

                case "btnEditarPropina":
                    var json = Request.Form["hdnEditarPropina"];
                    btnEditarPropina(json);
                    break;

                case "btnCuscarProducto":
                    btnBuscarProducto(eventArgument);
                    break;

                default:
                    // otros eventos por nombre...
                    break;
            }
        }
        private void btnBuscarProducto(string eventArgument)
        {
            // el valor que vino desde el input del buscador
            string textoBuscado = eventArgument?.Trim() ?? string.Empty;
            // consultamos el id de la presentasion en la lista de productos
            var producto = Models.productos.Where(x => x.codigoProducto == textoBuscado).FirstOrDefault();
            if (producto == null) 
            {
                AlertModerno.Error(this, "¡Error!", $"el código {textoBuscado} no se encontro", true);
                Models.venta = V_TablaVentasControler.Consultar_Id(Models.IdCuentaActiva);
                Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva, Models.IdCuenteClienteActiva);
                Models.v_CuentaClientes = V_CuentaClienteCotroler.Lista(false);
                Models.ventaCuenta = V_CuentaClienteCotroler.Consultar(Models.IdCuenteClienteActiva);
                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
            }
            else
            {
                var resp = DetalleVenta_f.AgregarProducto(producto.idPresentacion, 1, Models.IdCuentaActiva);
                if (resp.estado)
                {
                    //como si se creo el produto ahora verificamos si esta activa una cuenta de cleinte
                    if (Models.IdCuenteClienteActiva > 0)
                    {
                        var ralacion = new R_CuentaCliente_DetalleVenta
                        {
                            id = 0,
                            fecha = DateTime.Now,
                            idCuentaCliente = Models.IdCuenteClienteActiva,
                            idDetalleVenta = (int)resp.data,
                            eliminada = false
                        };
                        var crudrelacion = R_CuentaCliente_DetalleVentaControler.CRUD(ralacion, 0);
                    }
                    AlertModerno.Success(this, "¡OK!", $"{resp.mensaje}", true, 800);
                }
                else
                {
                    AlertModerno.Error(this, "¡Error!", $"{resp.mensaje}", true);
                }

                Models.venta = V_TablaVentasControler.Consultar_Id(Models.IdCuentaActiva);
                Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva, Models.IdCuenteClienteActiva);
                Models.v_CuentaClientes = V_CuentaClienteCotroler.Lista(false);
                Models.ventaCuenta = V_CuentaClienteCotroler.Consultar(Models.IdCuenteClienteActiva);
                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
            }

        }
        private void btnCuentaCliente (string eventArgument)
        {
            if (string.IsNullOrEmpty(eventArgument))
            {
                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
                return;
            }

            // 1️⃣ Separar por "|"
            var parts = eventArgument.Split('|');

            // 2️⃣ Validar que haya al menos dos partes
            if (parts.Length < 1)
            {
                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
                return;
            }

            // 3️⃣ Convertir la primera parte a int
            if (!int.TryParse(parts[0], out int idCuenta))
            {
                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
                return;
            }


            Models.IdCuenteClienteActiva = idCuenta;
            Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva,idCuenta);
            Models.ventaCuenta = V_CuentaClienteCotroler.Consultar(idCuenta);
            GuardarModelsEnSesion();
            BindProductos();
            DataBind();

        }
        private void btnLiberarMesa(string eventArgument)
        {
            if (string.IsNullOrEmpty(eventArgument))
            {
                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
                return;
            }


            // 1️⃣ Separar por "|"
            var parts = eventArgument.Split('|');

            // 2️⃣ Validar que haya al menos dos partes
            if (parts.Length < 2)
            {
                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
                return;
            }

            // 3️⃣ Convertir la primera parte a int
            if (!int.TryParse(parts[0], out int idMesa))
            {
                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
                return;
            }

            // 4️⃣ Tomar la segunda parte (nombre)
            string nombreMesa = parts[1];

            //primero consultamos el id de la mesa
            var mesa = MesasControler.Consultar_id(idMesa);
            if(mesa==null)
            {
                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
                return;
            }

            mesa.estadoMesa = 0;
            var crud = MesasControler.CRUD(mesa,1);
            if (!crud)
            {
                AlertModerno.Error(this,"Error",$"no se pudo modificar el estado de la mesa {nombreMesa}");
                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
                return;
            }

            //ahora eliminos la relacion de la mesa con el servicio
            var relacion = R_VentaMesaControler.Consultar_relacion(Models.IdCuentaActiva,idMesa);
            if (relacion == null) 
            {
                AlertModerno.Error(this, "Error", $"no se pudo modificar el estado de la mesa {nombreMesa}");
                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
            }

            var crud_r = R_VentaMesaControler.CRUD(relacion, 2);
            if (!crud_r)
            {
                AlertModerno.Error(this, "Error", $"no se pudo modificar el estado de la mesa {nombreMesa}");
                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
            }

            AlertModerno.Success(this, "Ok", $"mesa {nombreMesa} liberada");
            Models.Mesas = MesasControler.Lista();
            Models.cuentas = V_CuentasControler.Lista_IdVendedor(Models.IdMesero);
            GuardarModelsEnSesion();
            BindProductos();
            DataBind();

        }
        private void ProcesarNotasDetalle(string eventArgument)
        {
            if (string.IsNullOrWhiteSpace(eventArgument)) return;

            var parts = eventArgument.Split('|');
            if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], out int detalleId))
                {
                    string notaDetalle = parts[1];

                    var respuestadal = DetalleVenta_f.NotasDetalle(detalleId, notaDetalle);
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
                    Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva, Models.IdCuenteClienteActiva);

                    GuardarModelsEnSesion();
                    BindProductos();
                    DataBind();
                }
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
                    Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva, Models.IdCuenteClienteActiva);
                    Models.ventaCuenta = V_CuentaClienteCotroler.Consultar(Models.IdCuenteClienteActiva);
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
                    Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva, Models.IdCuenteClienteActiva);
                    Models.ventaCuenta = V_CuentaClienteCotroler.Consultar(Models.IdCuenteClienteActiva);
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
                detalleCaja = V_DetalleCajaControler.Lista_IdVenta(idCuenta, Models.IdCuenteClienteActiva)
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
            Models.IdCuenteClienteActiva = 0;
            Models.venta = V_TablaVentasControler.Consultar_Id(idServicio);
            Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(idServicio, Models.IdCuenteClienteActiva);
            Models.v_CuentaClientes = V_CuentaClienteCotroler.Lista(false);
            Models.IdCuentaActiva = idServicio;
            Models.ventaCuenta = V_CuentaClienteCotroler.Consultar(Models.IdCuenteClienteActiva);
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

        private void rpMesas_ItemCommand(string eventArgument)
        {
            CargarModelsDesdeSesion();

            if (string.IsNullOrWhiteSpace(eventArgument)) return;

            var partes = eventArgument.Split('|');

            if (!int.TryParse(partes[0], out int idMesa)) return;

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
                if (cuentasMesa.FirstOrDefault().idVendedor != Models.IdMesero)
                {
                    AlertModerno.Error(this, "Error", $"la mesa {mesa.nombreMesa} pertenece a otro mesero.",true);
                    GuardarModelsEnSesion();
                    BindProductos();
                    DataBind();
                    return; 
                }
                Models.v_CuentaClientes = V_CuentaClienteCotroler.Lista(false);
                Models.IdCuentaActiva = cuentasMesa.First().id;
                Models.IdCuenteClienteActiva = 0;
                Models.IdMesaActiva = idMesa;
                Models.Mesas = MesasControler.Lista();
                Models.venta = V_TablaVentasControler.Consultar_Id(Models.IdCuentaActiva);
                Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva,0);
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
                Models.v_CuentaClientes = V_CuentaClienteCotroler.Lista(false);
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

            int idVenta = TablaVentas_f.NuevaVenta((int)Session["porpropina"]);
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
            Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva, Models.IdCuenteClienteActiva);

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
            Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(idServicio, Models.IdCuenteClienteActiva);

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
                //como si se creo el produto ahora verificamos si esta activa una cuenta de cleinte
                if (Models.IdCuenteClienteActiva > 0)
                {
                    var ralacion = new R_CuentaCliente_DetalleVenta { 
                        id=0,
                     fecha=DateTime.Now,
                     idCuentaCliente=Models.IdCuenteClienteActiva,
                     idDetalleVenta=(int)resp.data,
                     eliminada=false};
                    var crudrelacion = R_CuentaCliente_DetalleVentaControler.CRUD(ralacion,0);
                }
                AlertModerno.Success(this, "¡OK!", $"{resp.mensaje}", true, 800);
            }
            else
            {
                AlertModerno.Error(this, "¡Error!", $"{resp.mensaje}", true);
            }

            if (txt != null) txt.Text = "0";

            Models.venta = V_TablaVentasControler.Consultar_Id(Models.IdCuentaActiva);
            Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva, Models.IdCuenteClienteActiva);
            Models.v_CuentaClientes = V_CuentaClienteCotroler.Lista(false);
            Models.ventaCuenta = V_CuentaClienteCotroler.Consultar(Models.IdCuenteClienteActiva);
            GuardarModelsEnSesion();
            BindProductos();
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

                Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva, Models.IdCuenteClienteActiva);
                Models.venta = V_TablaVentasControler.Consultar_Id(Models.IdCuentaActiva);
                Models.v_CuentaClientes = V_CuentaClienteCotroler.Lista(false);
                Models.ventaCuenta = V_CuentaClienteCotroler.Consultar(Models.IdCuenteClienteActiva);
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

                Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva, Models.IdCuenteClienteActiva);
                Models.venta = V_TablaVentasControler.Consultar_Id(Models.IdCuentaActiva);
                Models.v_CuentaClientes = V_CuentaClienteCotroler.Lista(false);
                Models.ventaCuenta = V_CuentaClienteCotroler.Consultar(Models.IdCuenteClienteActiva);
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
            int nuevoId = CuentaCliente_f.Crear(idventa, nombreCuenta, (int)Session["porpropina"]); // si ese es el flujo para crear 'cuenta'
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
        private void BTN_NuevoServicio()
        {
            int idVenta = TablaVentas_f.NuevaVenta((int)Session["porpropina"]);
            if (idVenta <= 0)
            {
                AlertModerno.Error(this, "Error", "No se creó el servicio.", true, 2000);
                return;
            }
            else
            {
                // procedemos a modificar el alias
                var resp = TablaVentasControler.Consultar_Id(idVenta);
                if (resp.estado)
                {
                    var venta = resp.data as TablaVentas;
                    venta.aliasVenta =Convert.ToString(idVenta);
                    var crud = TablaVentasControler.CRUD(venta,1);
                }
            }
            Models.IdCuentaActiva = idVenta;

            // amarro venta con vendedor (uso session idvendedor si existe)
            var rvv = R_VentaVendedor_f.Relacionar_Vendedor_Venta(idVenta, ObtenerIdVendedorSeguro());
            if (rvv)
            {
                AlertModerno.Success(this, "Listo", $"Servicio #{idVenta} creado con éxito.", true, 2000);
            }
            else
            {
                AlertModerno.Error(this, "Error", $"Servicio #{idVenta} creado con éxito, pero no se amarró al vendedor.", true, 2000);
            }


            // Actualizar modelos y UI
            Models.IdCuenteClienteActiva = 0;
            Models.cuentas = V_CuentasControler.Lista_IdVendedor(ObtenerIdVendedorSeguro());
            Models.venta = V_TablaVentasControler.Consultar_Id(Models.IdCuentaActiva);
            Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva,Models.IdCuenteClienteActiva);
            Models.v_CuentaClientes = V_CuentaClienteCotroler.Lista(false, Models.IdCuentaActiva);
            Models.ventaCuenta = V_CuentaClienteCotroler.Consultar(Models.IdCuenteClienteActiva);
            GuardarModelsEnSesion();
            BindProductos();
            DataBind();
        }

        #endregion

        private void btnEliminarServicio()
        {
            int idventa = Models.IdCuentaActiva;

            //consultamos si la venta tiene detalle cargados
            var detalles = V_DetalleCajaControler.Lista_IdVenta(idventa, Models.IdCuenteClienteActiva);
            if(detalles!=null && detalles.Count > 0)
            {
                AlertModerno.Error(this, "Error", $"El servicio #{idventa} aun tiene items cargados.",true);
                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
                return;
            }

            var rsp = TablaVentasControler.Consultar_Id(idventa);
            if (!rsp.estado)
            {
                AlertModerno.Error(this, "Error", $"El servicio #{idventa} no se pudo eliminar.", true);
                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
                return;
            }

            var venta = rsp.data as TablaVentas;
            venta.eliminada = true;
            var rsp_crud = TablaVentasControler.CRUD(venta,1);
            if (!rsp_crud.estado)
            {
                AlertModerno.Error(this, "Error", $"El servicio #{idventa} no se pudo eliminar.", true);
                GuardarModelsEnSesion();
                BindProductos();
                DataBind();
                return;
            }

            AlertModerno.Success(this,"OK",$"Servicio #{idventa} eliminado con éxito.");
            InicializarPagina();
        }

        protected void btnGuardarAlias_Click(object sender, EventArgs e)
        {
            try
            {
                var idCuenta = hfCuentaId.Value;
                var nuevoAlias = txtAlias.Text?.Trim() ?? "";
                var venta = new TablaVentas();
                var resp = TablaVentasControler.Consultar_Id(Convert.ToInt32(idCuenta));
                if (!resp.estado)
                {
                    AlertModerno.Error(this,"Error",$"no se encontro la venta {idCuenta}");
                }
                else
                {
                    venta = resp.data as TablaVentas;
                }
                venta.aliasVenta = nuevoAlias;
                var crud = TablaVentasControler.CRUD(venta,1);
                if (!crud.estado)
                {
                    AlertModerno.Error(this, "Error", $"no se modifico el alias");
                }
                else
                {
                    AlertModerno.Success(this, "OK", $"alias modificado correctamente.",true);
                }
                Models.IdCuentaActiva = Convert.ToInt32(idCuenta);
                Models.cuentas = V_CuentasControler.Lista_IdVendedor(Convert.ToInt32(Session["idvendedor"]));
                // Recarga datos y rebind
                CargarModelsDesdeSesion(); // si usas este patrón
                DataBind();
            }
            catch (Exception ex)
            {
                AlertModerno.Error(this, "Error", ex.Message, true);
            }
        }

        protected void btnCuentaGeneral_Click(object sender, EventArgs e)
        {
            CargarModelsDesdeSesion();

            Models.IdCuenteClienteActiva = 0;
            Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva,0);
            Models.venta = V_TablaVentasControler.Consultar_Id(Models.IdCuentaActiva);

            BindProductos();
            GuardarModelsEnSesion();
            DataBind();
        }

        private class EditarPropinaDto
        {
            public decimal porcentaje { get; set; }
            public int propina { get; set; } // en COP enteros
            public int idventa { get; set; }
            public int idcuenta { get; set; }
        }
        private void btnEditarPropina(string json)
        {
            CargarModelsDesdeSesion();

            if (!string.IsNullOrWhiteSpace(json))
            {
                var dto = JsonConvert.DeserializeObject<EditarPropinaDto>(json);
                decimal por_propina = dto.porcentaje / 100;
                if (dto.idcuenta > 0)
                {
                    //hallamos la cuenta cliente con el id
                    var cc = CuentaClienteControler.CuentaCliente(dto.idcuenta);
                    if (cc != null)
                    {
                        cc.por_propina= por_propina;
                        cc.propina = dto.propina;
                        var respcc = CuentaClienteControler.CRUD(cc,1);
                    }
                }
                else
                {
                    var respuesta = TablaVentasControler.Consultar_Id(dto.idventa);
                    if(respuesta.estado)
                    {
                        var venta= respuesta.data as TablaVentas;
                        venta.porpropina = por_propina;
                        venta.propina = dto.propina;
                        var respventa = TablaVentasControler.CRUD(venta,1);
                    }
                }

                Models.IdCuenteClienteActiva = dto.idcuenta;
                Models.detalleCaja = V_DetalleCajaControler.Lista_IdVenta(Models.IdCuentaActiva, dto.idcuenta);
                Models.venta = V_TablaVentasControler.Consultar_Id(Models.IdCuentaActiva);
                Models.ventaCuenta = V_CuentaClienteCotroler.Consultar(dto.idcuenta);
                Models.v_CuentaClientes=V_CuentaClienteCotroler.Lista(false);
                BindProductos();
                GuardarModelsEnSesion();
                DataBind();
            }

            BindProductos();
            GuardarModelsEnSesion();
            DataBind();
        }

        protected void btnComandar_ServerClick(object sender, EventArgs e)
        {
            CargarModelsDesdeSesion();
            var comanda = new ImprecionComandaAdd
            {
                id = 0,
                idVenta = Models.IdCuentaActiva,
                idMesa = Convert.ToString(Models.IdMesaActiva),
                idMesero = Convert.ToString(Models.IdMesero),
                estado = 1
            };
            var resp = ImprecionComandaAddControler.CRUD(comanda,0);
            if (resp.estado)
            {
                AlertModerno.Success(this,"Ok","Comanda enviada correctamente.",true,1500);
            }
            else
            {
                AlertModerno.Error(this, "Error", "Comanda no enviada correctamente.", true, 1500);
            }

            GuardarModelsEnSesion();
            BindProductos();
            DataBind();
        }

        protected void btnCuenta_ServerClick(object sender, EventArgs e)
        {
            CargarModelsDesdeSesion();
            var cuenta = new ImprimirCuenta
            {
                id = 0,
                idventa = Models.IdCuentaActiva
            };
            var resp = ImprimirCuentaControler.CRUD(cuenta, 0);
            if (resp.estado)
            {
                AlertModerno.Success(this, "Ok", "Cuenta enviada correctamente.", true, 1500);
            }
            else
            {
                AlertModerno.Error(this, "Error", "Cuenta no enviada correctamente.", true, 1500);
            }

            GuardarModelsEnSesion();
            BindProductos();
            DataBind();
        }


        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            InicializarPagina(); // 🔁 Llama directamente tu método
        }

    }
}
