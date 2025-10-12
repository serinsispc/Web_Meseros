<%@ Page Title="Servicios" Language="C#" MasterPageFile="~/Menu.Master" AutoEventWireup="true" CodeBehind="Menu.aspx.cs" Inherits="WebApplication.Menu"
    MaintainScrollPositionOnPostback="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

        <style>
        /* Contenedor flotante */
        .scroll-buttons {
            position: fixed;
            bottom: 20px;
            right: 20px;
            display: flex;
            flex-direction: column;
            gap: 10px;
            z-index: 9999;
        }

        /* Estilo de cada botón */
        .scroll-btn {
            width: 50px;
            height: 50px;
            border-radius: 50%;
            border: none;
            background-color: #007bff;
            color: white;
            font-size: 24px;
            cursor: pointer;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.3);
            transition: background-color 0.3s, transform 0.3s;
        }

        .scroll-btn:hover {
            background-color: #0056b3;
            transform: scale(1.1);
        }
    </style>

    <script>
        // Función para subir al inicio
        function scrollToTop() {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        }

        // Función para bajar al final
        function scrollToBottom() {
            window.scrollTo({
                top: document.body.scrollHeight,
                behavior: 'smooth'
            });
        }
    </script>


    <%-- primero validamos el porcentaje de la propina --%>
    <%
        int porpropina = 0;
        int valorpropina = 0;
        int totalapagar = Convert.ToInt32(Models.venta.totalVenta);
        if (Models.venta.por_propina > 0)
        {
            var pp = Models.venta.por_propina;
            var pp2 = pp * 100;
            porpropina = Convert.ToInt32(pp2);
        }
        if (porpropina > 0)
        {
            if (Models.venta.propina == 0)
            {
                valorpropina = WebApplication.Class.ClassPropina.CalcularValoPropina(Models.venta.por_propina.ToString(), Models.venta.subtotalVenta.ToString());
            }
            else
            {
                valorpropina = Convert.ToInt32(Models.venta.subtotalVenta);
            }
        }
        totalapagar = totalapagar + valorpropina;
        Models.venta.total_A_Pagar = totalapagar;
        DataBind();
    %>



            <%-- Botones flotantes --%>
            <div class="scroll-buttons">
                <button type="button" class="scroll-btn" onclick="scrollToTop()">▲</button>
                <button type="button" class="scroll-btn" onclick="scrollToBottom()">▼</button>
            </div>
      

    <asp:HiddenField ID="hfMesaId" runat="server" />
    <asp:HiddenField ID="hfServicioId" runat="server" />

    <asp:Button ID="btnMesaNuevaCuenta" runat="server"
        OnClick="MesaNuevaCuenta" Style="display: none" UseSubmitBehavior="false" />

    <!-- Postback final cuando ya eligió servicio -->
    <asp:Button ID="btnMesaAmarrar" runat="server"
        OnClick="MesaAmarar" Style="display: none" UseSubmitBehavior="false" />

    <!-- Modal: seleccionar servicio existente -->
    <div class="modal fade" id="mdlServicios" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Seleccionar servicio para <span id="lblMesaSeleccionada" class="fw-semibold"></span>
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>
                <div class="modal-body">
                    <p class="text-muted mb-2">Servicios activos:</p>

                    <div id="listaServicios" class="list-group">
                        <asp:Repeater ID="rpServiciosActivos" runat="server" DataSource="<%# Models.cuentas %>">
                            <ItemTemplate>
                                <!-- Cada item es clickeable; lleva data-id del servicio -->
                                <button type="button"
                                    class="list-group-item list-group-item-action d-flex justify-content-between align-items-center servicio-item"
                                    data-id='<%# Eval("id") %>'>
                                    <span class="fw-semibold"><%# Eval("id") %></span>
                                    <small class="text-muted">#<%# Eval("id") %> · <%# Eval("mesa") %></small>
                                </button>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <div class="text-center text-secondary mt-3" runat="server" id="divSinServicios"
                        visible='<%# (Models.cuentas?.Count ?? 0) == 0 %>'>
                        No hay servicios activos para amarrar.
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cerrar</button>
                </div>
            </div>
        </div>
    </div>

    <div class="container-fluid menu-wrap py-3 py-lg-4">

        <!-- ====== fila: barra de servicios + acciones ====== -->
        <div class="row g-3 align-items-center mb-3">
            <div class="col-12 col-xl">
                <div class="d-flex flex-wrap gap-2">

                    <asp:Repeater runat="server" ID="rpCuentas"
                        DataSource="<%# Models.cuentas %>"
                        OnItemCommand="rpServicios_ItemCommand">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnServicio" runat="server"
                                CommandName="AbrirServicio"
                                CommandArgument='<%# Eval("id") %>'
                                CssClass='<%# "service-chip" + ((Eval("id").ToString() == Models.IdCuentaActiva.ToString()) ? " active" : "") %>'>
            <span class="chip-title"><%# Eval("id") %></span>
            <small class="text-muted d-block">
                <%# Eval("mesa") %>
            </small>
            <i class="bi bi-pencil-fill chip-edit"></i>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>




                </div>
            </div>

            <div class="col-12 col-xl-auto">
                <div class="d-flex gap-2 justify-content-start justify-content-xl-end">
                    <button runat="server" id="btnNuevoServicio"
                        onserverclick="btnNuevoServicio_Click"
                        class="btn btn-primary btn-sm">
                        <i class="bi bi-plus-circle me-1"></i>Nuevo servicio
                    </button>
                    <button runat="server" id="btnEliminarServicio"
                        type="button"
                        class="btn btn-warning btn-sm text-dark"
                        onserverclick="btnEliminarServicio_Clik">
                        <i class="bi bi-trash3 me-1"></i>Eliminar servicio
                    </button>
                    <button type="button" class="btn btn-outline-danger btn-sm">
                        <i class="bi bi-x-circle me-1"></i>Liberar mesa
                    </button>
                </div>
            </div>
        </div>

        <!-- ====== banner servicio activo ====== -->
        <div class="alert alert-primary-soft d-flex align-items-center justify-content-between px-3 py-2 mb-3">
            <div class="fw-semibold">Servicio Activo: <%# Models.IdCuentaActiva %> </div>
            <div class="small text-muted"></div>
        </div>

        <!-- ====== 3 columnas ====== -->
        <div class="row g-3">
            <!-- === Columna 1: Zonas / Mesas === -->
            <div class="col-12 col-lg-6 col-xl-4">
                <div class="card h-100">
                    <div class="card-body">
                        <!-- tabs de zona -->
                        <ul class="nav nav-pills mb-3 zone-tabs">


                            <asp:Repeater ID="rpZonas" runat="server"
                                DataSource="<%# Models.zonas %>"
                                OnItemCommand="rpZonas_ItemCommand">
                                <ItemTemplate>
                                    <li class="nav-item m-1">
                                        <asp:LinkButton ID="lnkZona" runat="server"
                                            CommandName="CambiarZona"
                                            CommandArgument='<%# Eval("id") %>'
                                            CssClass='<%# (Session["zonaactiva"]?.ToString() == Eval("id").ToString())
            ? "nav-link bg-primary text-white"
            : "nav-link" %>'>
  <%# Eval("nombreZona") %>
                                        </asp:LinkButton>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>




                        </ul>

                        <%--                        <div class="d-flex align-items-center gap-2 mb-2 small">
                            <span class="text-muted">Mesas en ZONA 1 :</span>
                            <span class="badge bg-secondary">10</span>
                            <span class="badge bg-success-subtle text-success">Libres: 7</span>
                            <span class="badge bg-danger-subtle text-danger">Ocupadas: 3</span>
                        </div>--%>

                        <!-- grilla de mesas -->
                        <div class="row g-2">


                            <asp:Repeater runat="server" ID="rpMesas"
                                DataSource="<%# Models.Mesas.Where(x=>x.idZona==Models.IdZonaActiva).ToList() %>"
                                OnItemCommand="rpMesas_ItemCommand">
                                <ItemTemplate>
                                    <div class="col-2 col-lg-6 col-xl-4" style="min-width:100px; max-width:110px">
                                        <asp:LinkButton ID="lnkMesa" runat="server"
                                            data-name='<%# Eval("nombreMesa") %>'
                                            CommandName="AbrirMesa"
                                            CommandArgument='<%# Eval("id") %>'
                                            CssClass='<%# (Convert.ToInt32(Eval("estadoMesa")) == 1)
                    ? "mesa-card ocupada d-block text-start"
                    : "mesa-card libre d-block text-start" %>'>
        <div class="mesa-titulo"><%# Eval("nombreMesa") %></div>
        <div class="mesa-sub">
          <%# (Convert.ToInt32(Eval("estadoMesa")) == 1) ? "Ocupada" : "Libre" %>
        </div>
                                        </asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>



                        </div>

                    </div>
                </div>
            </div>

            <!-- === Columna 2: Productos === -->
            <div class="col-12 col-lg-6 col-xl-4">
                <div class="card h-100">
                    <div class="card-body">
                        <!-- Buscador -->
                        <div class="input-group mb-3">
                            <span class="input-group-text bg-white"><i class="bi bi-search"></i></span>
                            <input type="text" id="buscador-productos" class="form-control" placeholder="Buscar producto por nombre..." />
                            <button type="button" id="limpiar-buscador" class="btn btn-outline-secondary"><i class="bi bi-x-lg"></i></button>
                        </div>


                        <!-- categorías -->
                        <div class="d-flex flex-wrap gap-2 mb-3">

                            <!-- Categorías -->
                            <div id="categorias-container">
                                <% foreach (var cat in Models.categorias)
                                    { %>
                                <a href="#"
                                    class="pill <%=(cat.id == Models.IdCategoriaActiva ? "active" : "") %>"
                                    data-id="<%= cat.id %>">
                                    <%= cat.nombreCategoria %>
                                </a>
                                <% }
                                %>
                            </div>




                        </div>

                        <!-- lista de productos -->
                        <div class="vstack gap-2">

                            <!-- Productos -->
                            <div id="productos-container">
                                <asp:Repeater ID="rpProductos" runat="server" OnItemCommand="rpProductos_ItemCommand">
                                    <ItemTemplate>
                                        <div class="producto-item" data-categoria='<%# Eval("idCategoria") %>'>
                                            <div class="prod-main">
                                                <div class="prod-info">
                                                    <div class="prod-name"><%# Eval("nombreProducto") %></div>
                                                    <div class="prod-meta">
                                                        $<%# string.Format("{0:N0}", Eval("precioVenta")) %>
                                                        <a href="#" class="link-primary small ms-2">Ver detalle</a>
                                                    </div>
                                                </div>

                                                <div class="product-actions d-flex flex-column align-items-stretch">
                                                    <div class="controls-inline d-flex align-items-center gap-2">
                                                        <button type="button" class="btn btn-light btn-sm minus" title="Disminuir">
                                                            <i class="bi bi-dash"></i>
                                                        </button>

                                                        <asp:TextBox ID="txtCantidad" runat="server"
                                                            CssClass="form-control qty-input text-center"
                                                            Text="0" />

                                                        <button type="button" class="btn btn-light btn-sm plus" title="Aumentar">
                                                            <i class="bi bi-plus"></i>
                                                        </button>

                                                        <asp:LinkButton ID="btnAgregarCarrito" runat="server"
                                                            CommandName="AgregarAlCarrito"
                                                            CommandArgument='<%# Eval("idPresentacion") %>'
                                                            CssClass="icon-btn cart-btn ms-2"
                                                            title="Agregar al carrito">
                                <i class="bi bi-cart2"></i>
                                                        </asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>




                        </div>

                    </div>
                </div>
            </div>

            <!-- === Columna 3: Pedido === -->
            <div class="col-12 col-xl-4">
                <div class="card h-100 order-card">
                    <div class="card-body">

                        <div class="d-flex align-items-center gap-2 mb-2">
                            <button class="btn btn-success-subtle btn-sm border-success text-success">
                                <i class="bi bi-plus-lg me-1"></i>Nueva cuenta
                            </button>
                            <div class="flex-grow-1">
                                <asp:LinkButton ID="btnCuentaGeneral" runat="server" CssClass="text-decoration-none w-100" OnClick="btnCuentaGeneral_Click">
    <div class="input-group input-group-sm bg-white border rounded">
        <span class="input-group-text bg-white"><i class="bi bi-person-badge"></i></span>
        <span class="form-control border-0 bg-white">Cuenta General</span>
        <span class="input-group-text bg-white fw-semibold">
            <%# "$" + string.Format("{0:N0}", Models.venta.total_A_Pagar ) %>
        </span>
    </div>
                                </asp:LinkButton>
                            </div>
                        </div>

                        <asp:Repeater runat="server" ID="rpDetalleCaja" DataSource="<%# Models.detalleCaja %>">
                            <ItemTemplate>
                                <!-- item pedido -->
                                <div class="pedido-item mb-2 p-2 border rounded" style="width: 100%; box-sizing: border-box;">

                                    <div class="d-flex flex-column">

                                        <!-- Nombre y precio pequeño -->
                                        <div class="d-flex justify-content-between align-items-start mb-2">
                                            <div class="nombre-producto fw-semibold lh-sm text-uppercase">
                                                <%# Eval("nombreProducto") %>
                                            </div>
                                            <div class="small text-muted precio-pequenho">
                                                <%# string.Format(new System.Globalization.CultureInfo("es-CO"), "{0:C0}", Eval("precioVenta")) %>
                                            </div>
                                        </div>

                                        <!-- Fila 1: Cantidad + carrito -->
<div class="d-flex justify-content-start align-items-center gap-2 flex-wrap mb-2">
    <div class="quantity-group btn-group btn-group-sm" role="group" aria-label="Cantidad">
        <button type="button" class="btn btn-light btn-qty btn-square btn-decrease">
            <i class="bi bi-dash"></i>
        </button>

        <input type="number" class="form-control quantity-input text-center" 
               value='<%# Convert.ToInt32(Eval("unidad")) %>' min="1" style="width:100px;" />

        <button type="button" class="btn btn-light btn-qty btn-square btn-increase">
            <i class="bi bi-plus"></i>
        </button>
    </div>

    <!-- Botón que usará __doPostBack -->
    <button type="button" class="icon-btn cart-btn ms-auto" 
            title="Guardar" 
            data-id='<%# Eval("id") %>'>
        <i class="bi bi-floppy"></i>
    </button>
</div>


                                        <!-- Fila 2: Iconos de acción -->
                                        <div class="d-flex flex-wrap gap-2 mb-2">

                                            <button type="button" class="icon-btn" title="Comentario" data-id='<%# Eval("id") %>'><i class="bi bi-chat"></i></button>
                                            <button type="button" class="icon-btn" title="Anclar" data-id='<%# Eval("id") %>'><i class="bi bi-link-45deg"></i></button>
                                            <button type="button" class="icon-btn danger" title="Eliminar" data-id='<%# Eval("id") %>'><i class="bi bi-trash"></i></button>
                                            <button type="button" class="icon-btn" title="Cortar / Promo" data-id='<%# Eval("id") %>'><i class="bi bi-scissors"></i></button>
                                        </div>

                                        <!-- Badge precio alineado a la derecha -->
                                        <div class="d-flex justify-content-end">
                                            <div class="price-badge">
                                                <%# string.Format(new System.Globalization.CultureInfo("es-CO"), "{0:C0}", Eval("totalDetalle")) %>
                                            </div>
                                        </div>

                                    </div>

                                </div>
                            </ItemTemplate>
                        </asp:Repeater>


                        <hr />

                        <!-- totales -->
                        <div class="d-flex justify-content-between small mb-1">
                            <span class="text-muted">SubTotal:</span>
                            <span><%# "$" + string.Format("{0:N0}", Models.venta.subtotalVenta) %></span>
                        </div>
                        <div class="d-flex justify-content-between small mb-2">
                            <span class="text-muted">Impuestos (8%)</span>
                            <span><%# "$" + string.Format("{0:N0}", Models.venta.ivaVenta) %></span>
                        </div>
                        <div class="d-flex justify-content-between fw-semibold mb-2">
                            <span>Total 1:</span>
                            <span><%# "$" + string.Format("{0:N0}", Models.venta.totalVenta) %></span>
                        </div>
                        <div class="d-flex justify-content-between align-items-center mb-3">
                            <span>Servicio (<%= porpropina %>%)</span>

                            <div>
                                <span class="badge bg-primary-subtle text-primary fw-semibold me-2">Editar</span>
                                <span><%= "$" + string.Format("{0:N0}", valorpropina) %></span>
                            </div>
                        </div>
                        <div class="d-flex justify-content-between fs-6 fw-bold mb-3">
                            <span>Total 2:</span>
                            <span><%= "$" + string.Format("{0:N0}", totalapagar) %></span>
                        </div>

                        <!-- acciones grandes -->
                        <div class="row g-3">
                            <div class="col-12 col-md-4" >
                                <button class="cta cta-orange w-100" style="min-height:80px; max-height:80px; height:80px;">
                                    <i class="bi bi-send me-2"></i>Comandar
                                </button>
                            </div>
                            <div class="col-12 col-md-4">
                                <button class="cta cta-purple w-100" style="min-height:80px; max-height:80px; height:80px;">
                                    <i class="bi bi-chat-left-text me-2"></i>Solicitar<br />
                                    Cuenta
                                </button>
                            </div>
                            <div class="col-12 col-md-4">
                                <button class="cta cta-green w-100" style="min-height:80px; max-height:80px; height:80px;">
                                    <i class="bi bi-cash-coin me-2"></i>Cobrar
                                </button>
                            </div>
                        </div>

                    </div>
                </div>
            </div>


        </div>
        <!-- /row 3 cols -->

    </div>
    <!-- /container-fluid -->

    <%-- ************************************************ --%>
    <%-- ************************************************ --%>
    <%-- ************************************************ --%>
    <%-- modales --%>
    <!-- Modal eliminar detalle -->
<div class="modal fade" id="modalEliminarDetalle" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content p-3">
            <div class="modal-header">
                <h5 class="modal-title">Eliminar detalle</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <label for="notaEliminar" class="form-label">Ingrese el motivo de eliminación:</label>
                <textarea id="notaEliminar" class="form-control" rows="3"></textarea>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                <button type="button" class="btn btn-danger" id="btnConfirmarEliminar">Eliminar</button>
            </div>
        </div>
    </div>
</div>

    <!-- Modal de alerta de sesión -->
<div class="modal fade" id="sessionModal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog modal-dialog-centered">
    <div class="modal-content border-warning">
      <div class="modal-header bg-warning text-dark">
        <h5 class="modal-title">Sesión a punto de expirar</h5>
      </div>
      <div class="modal-body">
        Tu sesión ha expirado o está por expirar. Serás redirigido automáticamente.
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-primary" id="btnGoDefault">Ir Ahora</button>
      </div>
    </div>
  </div>
</div>




    <script>
        // Debe estar fuera de DOMContentLoaded para que sea global
        function abrirModalServicios(nombreMesa, idMesa) {
            document.getElementById('lblMesaSeleccionada').textContent = nombreMesa || '';
            document.getElementById('<%= hfMesaId.ClientID %>').value = idMesa || '';

            var el = document.getElementById('mdlServicios');
            if (!el) { console.warn('#mdlServicios no existe'); return; }

            if (window.bootstrap && bootstrap.Modal) {
                var m = bootstrap.Modal.getInstance(el) || new bootstrap.Modal(el);
                m.show();
            } else if (window.jQuery && $.fn.modal) {
                $(el).modal('show');
            } else {
                console.error('Bootstrap 5/4 no está cargado. Revisa Site.Master.');
            }
        }
    </script>


    <script>
        document.addEventListener('DOMContentLoaded', function () {
            var contenedor = document.getElementById('listaServicios');
            if (!contenedor) return;

            // Evitar doble enlace si hay UpdatePanel
            if (!contenedor._wired) {
                contenedor._wired = true;

                contenedor.addEventListener('click', function (ev) {
                    var btn = ev.target.closest('.servicio-item');
                    if (!btn) return;

                    // Obtener ID del servicio seleccionado
                    var idServicio = btn.getAttribute('data-id');

                    // Setear hidden field para enviar al servidor
                    var hfServicio = document.getElementById('<%= hfServicioId.ClientID %>');
                    if (hfServicio) hfServicio.value = idServicio;

                    // Cerrar modal de manera segura con Bootstrap 5
                    var modalEl = document.getElementById('mdlServicios');
                    if (modalEl && window.bootstrap && bootstrap.Modal) {
                        var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                        modal.hide();

                        // Esperar 200ms para que termine animación antes de postback
                        setTimeout(function () {
                            // Ejecutar postback al botón oculto
                            __doPostBack('<%= btnMesaAmarrar.UniqueID %>', '');
                        }, 200);
                    } else {
                        // Fallback por si no hay Bootstrap 5
                        __doPostBack('<%= btnMesaAmarrar.UniqueID %>', '');
                    }
                });
            }
        });
    </script>


    <%-- al precionar enter dentro de la cantidad de la lista de productos --%>
    <script>
        (function () {
            function inicializarEventosProductos() {
                document.querySelectorAll('.qty-input').forEach(function (input) {
                    // cuando el usuario presione Enter
                    input.addEventListener('keydown', function (e) {
                        if (e.key === 'Enter' || e.keyCode === 13) {
                            e.preventDefault();

                            // buscamos el contenedor más cercano del producto
                            const contenedor = input.closest('.producto-item');
                            if (!contenedor) return;

                            // buscamos el botón de carrito dentro del mismo producto
                            const boton = contenedor.querySelector('.cart-btn');
                            if (boton) {
                                boton.click(); // simulamos clic
                            }
                        }
                    });
                });
            }

            // compatibilidad con UpdatePanel o cargas parciales
            if (window.Sys && Sys.Application) {
                Sys.Application.add_load(inicializarEventosProductos);
            } else {
                document.addEventListener('DOMContentLoaded', inicializarEventosProductos);
            }
        })();
    </script>




<script type="text/javascript">
    // Tiempo de sesión en milisegundos
    var sessionTimeoutMinutes = <%= Session.Timeout %>;
    var sessionTimeoutMs = sessionTimeoutMinutes * 60 * 1000;

    // Obtenemos el valor de Session["db"] desde el servidor
    var nombreDB = '<%= Session["db"] != null ? Session["db"].ToString() : "" %>';

    // Mostrar modal al expirar la sesión
    setTimeout(function () {
        var sessionModal = new bootstrap.Modal(document.getElementById('sessionModal'), {
            backdrop: 'static',
            keyboard: false
        });
        sessionModal.show();

        // Redirigir automáticamente después de 5 segundos
        setTimeout(function () {
            window.location.href = 'Default.aspx?db=' + encodeURIComponent(nombreDB);
        }, 5000);
    }, sessionTimeoutMs);

    // Botón para ir inmediatamente a Default.aspx
    document.getElementById('btnGoDefault').addEventListener('click', function () {
        window.location.href = 'Default.aspx?db=' + encodeURIComponent(nombreDB);
    });
</script>





    <%-- *************************************************** --%>
    <%-- *************************************************** --%>
    <%-- *************************************************** --%>
    <%-- *************************************************** --%>
    <%-- *************************************************** --%>

        <!-- ======= BLOQUE DE SCRIPTS OPTIMIZADO ======= -->
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            const categorias = document.querySelectorAll("#categorias-container .pill");
            const productos = document.querySelectorAll("#productos-container .producto-item");
            const buscador = document.getElementById("buscador-productos");
            const btnLimpiar = document.getElementById("limpiar-buscador");

            // ===== Función de filtrado de productos =====
            function filtrarProductos(texto = "") {
                const categoriaCount = {};

                productos.forEach(prod => {
                    const nombre = prod.querySelector(".prod-name").textContent.toLowerCase();
                    const catId = prod.dataset.categoria;
                    let visible = nombre.includes(texto.toLowerCase());
                    prod.style.display = visible ? "block" : "none";

                    if (visible) {
                        categoriaCount[catId] = (categoriaCount[catId] || 0) + 1;
                    }
                });

                // Activar la categoría con más coincidencias
                let maxCat = null;
                let maxCount = 0;
                for (const catId in categoriaCount) {
                    if (categoriaCount[catId] > maxCount) {
                        maxCount = categoriaCount[catId];
                        maxCat = catId;
                    }
                }

                categorias.forEach(cat => {
                    if (cat.dataset.id === maxCat) {
                        cat.classList.add("active");
                    } else {
                        cat.classList.remove("active");
                    }
                });
            }

            // ===== Eventos =====

            // Buscador en tiempo real
            buscador.addEventListener("input", () => filtrarProductos(buscador.value));

            // Botón limpiar buscador
            btnLimpiar.addEventListener("click", function () {
                buscador.value = "";
                productos.forEach(prod => prod.style.display = "block");
                categorias.forEach(cat => cat.classList.remove("active"));
            });

            // Click en categorías
            categorias.forEach(cat => {
                cat.addEventListener("click", function (e) {
                    e.preventDefault();
                    const idCategoria = this.dataset.id;

                    categorias.forEach(c => c.classList.remove("active"));
                    this.classList.add("active");

                    productos.forEach(prod => {
                        prod.style.display = (prod.dataset.categoria === idCategoria) ? "block" : "none";
                    });

                    buscador.value = ""; // limpiar búsqueda al cambiar categoría
                });
            });

            // Inicializar mostrando la categoría activa
            const catActiva = document.querySelector("#categorias-container .pill.active");
            if (catActiva) catActiva.click();

            // ===== Botones + / - de cantidad lista de productos =====
            productos.forEach(item => {
                const minus = item.querySelector(".minus");
                const plus = item.querySelector(".plus");
                const input = item.querySelector(".qty-input");

                minus.addEventListener("click", () => {
                    let val = parseInt(input.value) || 0;
                    if (val > 0) input.value = val - 1;
                });

                plus.addEventListener("click", () => {
                    let val = parseInt(input.value) || 0;
                    input.value = val + 1;
                });
            });

            // ===== Agregar al carrito =====
            const botonesCarrito = document.querySelectorAll(".add-to-cart");
            botonesCarrito.forEach(btn => {
                btn.addEventListener("click", function () {
                    const idPresentacion = this.dataset.id;
                    const inputCantidad = this.closest(".controls-inline").querySelector(".qty-input");
                    const cantidadDetalle = parseInt(inputCantidad.value) || 0;

                    if (cantidadDetalle <= 0) {
                        alert("Debe ingresar una cantidad mayor a cero.");
                        return;
                    }

                    fetch('Menu.aspx.cs/AgregarPro', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json; charset=utf-8' },
                        body: JSON.stringify({ idPresentacion, cantidadDetalle })
                    })
                        .then(res => res.json())
                        .then(data => {
                            if (data.d.success) {
                                alert("Producto agregado correctamente.");
                                inputCantidad.value = 0;
                            } else {
                                alert("Error: " + data.d.message);
                            }
                        })
                        .catch(err => console.error(err));
                });
            });


            // Función para validar cantidad en detalle
            function validarCantidad(input) {
                const value = input.value.trim();
                if (value === "" || isNaN(value) || parseInt(value) < 1) {
                    input.value = "1"; // Si está vacío o inválido, lo ponemos en 1
                    return false;
                }
                return true;
            }

            // Interceptar Enter en todos los inputs de cantidad detalle
            document.querySelectorAll(".quantity-input").forEach(function (input) {
                input.addEventListener("keydown", function (e) {
                    if (e.key === "Enter") {
                        e.preventDefault(); // Evita submit por defecto
                        if (validarCantidad(input)) {
                            const btn = input.closest(".d-flex").querySelector(".cart-btn");
                            btn.click(); // Dispara el click del botón de guardar
                        }
                    }
                });

                // Validar al perder el foco detalle
                input.addEventListener("blur", function () {
                    validarCantidad(input);
                });
            });

            // Botones de aumentar detalle
            document.querySelectorAll(".btn-increase").forEach(function (btn) {
                btn.addEventListener("click", function () {
                    const input = btn.closest(".quantity-group").querySelector(".quantity-input");
                    validarCantidad(input);
                    input.value = parseInt(input.value) + 1;
                });
            });

            // Botones de disminuir  detalle
            document.querySelectorAll(".btn-decrease").forEach(function (btn) {
                btn.addEventListener("click", function () {
                    const input = btn.closest(".quantity-group").querySelector(".quantity-input");
                    validarCantidad(input);
                    let value = parseInt(input.value);
                    if (value > 1) input.value = value - 1;
                });
            });

            // Botones de guardar detalle
            document.querySelectorAll(".cart-btn").forEach(function (btn) {
                btn.addEventListener("click", function () {
                    const input = btn.closest(".d-flex").querySelector(".quantity-input");
                    if (validarCantidad(input)) {
                        const id = btn.getAttribute("data-id");
                        const quantity = input.value;
                        __doPostBack('ActualizarCantidad', id + '|' + quantity);
                    }
                });
            });


            /********** en pesamos el bloque del boton eliminar ***********/
            let detalleIdAEliminar = null;

            // Abrir modal
            document.querySelectorAll(".icon-btn.danger").forEach(function (btn) {
                btn.addEventListener("click", function () {
                    detalleIdAEliminar = btn.getAttribute("data-id");
                    document.getElementById("notaEliminar").value = "";

                    const modalEl = document.getElementById("modalEliminarDetalle");
                    const modal = new bootstrap.Modal(modalEl);
                    modal.show();

                    // Cuando el modal se muestra, enfocamos el textarea
                    modalEl.addEventListener('shown.bs.modal', function () {
                        document.getElementById("notaEliminar").focus();
                    }, { once: true }); // { once: true } asegura que solo se dispare una vez
                });
            });

            // Confirmar eliminación
            document.getElementById("btnConfirmarEliminar").addEventListener("click", function () {
                const nota = document.getElementById("notaEliminar").value.trim();
                if (nota === "") {
                    alert("Debe ingresar una nota para eliminar el detalle.");
                    return;
                }

                if (detalleIdAEliminar) {
                    __doPostBack("EliminarDetalle", detalleIdAEliminar + "|" + nota);
                    const modal = bootstrap.Modal.getInstance(document.getElementById("modalEliminarDetalle"));
                    modal.hide();
                }
            });
            //fin de script
        });
    </script>

</asp:Content>









