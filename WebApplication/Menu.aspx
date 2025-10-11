<%@ Page Title="Servicios" Language="C#" MasterPageFile="~/Menu.Master" AutoEventWireup="true" CodeBehind="Menu.aspx.cs" Inherits="WebApplication.Menu"
    MaintainScrollPositionOnPostback="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

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
            <div class="col-12 col-lg-4 col-xl-3">
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
                                DataSource="<%# Models.Mesas %>"
                                OnItemCommand="rpMesas_ItemCommand">
                                <ItemTemplate>
                                    <div class="col-6">
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
            <div class="col-12 col-lg-8 col-xl-5">
                <div class="card h-100">
                    <div class="card-body">

                        <!-- buscador -->
                        <div class="input-group mb-3">
                            <span class="input-group-text bg-white"><i class="bi bi-search"></i></span>
                            <input type="text" class="form-control" placeholder="Buscar producto por nombre..." />
                            <button class="btn btn-outline-secondary"><i class="bi bi-x-lg"></i></button>
                        </div>

                        <!-- categorías -->
                        <div class="d-flex flex-wrap gap-2 mb-3">

                            <asp:Repeater runat="server" ID="rpCategorias"
                                DataSource="<%# Models.categorias %>"
                                OnItemCommand="rpCategorias_ItemCommand">
                                <ItemTemplate>
                                    <asp:LinkButton
                                        runat="server"
                                        ID="btnCategoria"
                                        CommandName="SeleccionarCategoria"
                                        CommandArgument='<%# Eval("id") %>'
                                        CssClass='<%# (Convert.ToInt32(Eval("id")) == Models.IdCategoriaActiva) ? "pill active" : "pill" %>'>
                                        <%# Eval("nombreCategoria") %>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:Repeater>


                        </div>

                        <!-- lista de productos -->
                        <div class="vstack gap-2">

                            <asp:Repeater runat="server" ID="rpProductos" OnItemCommand="rpProductos_ItemCommand" DataSource="<%# Models.productos %>">
                                <ItemTemplate>
                                    <div class="producto-item">
                                        <div class="prod-main">
                                            <div class="prod-info">
                                                <div class="prod-name"><%# Eval("nombreProducto") %></div>
                                                <div class="prod-meta">
                                                    <%# "$" + string.Format("{0:N0}", Eval("precioVenta")) %>
                                                    <a href="#" class="link-primary small ms-2">Ver detalle</a>
                                                </div>
                                            </div>

                                            <!-- actions container: en móvil se apila en 2 filas (cantidad+carrito) y (acciones) -->
                                            <div class="product-actions d-flex flex-column align-items-stretch">
                                                <!-- fila 1: cantidad + carrito -->
                                                <div class="controls-inline d-flex align-items-center gap-2">
                                                    <button type="button" class="btn btn-light btn-sm minus" title="Disminuir">
                                                        <i class="bi bi-dash"></i>
                                                    </button>

                                                    <!-- TextBox server-side para leer valor desde server -->
                                                    <asp:TextBox runat="server" ID="txtCantidad" CssClass="form-control qty-input text-center" Text="0" />

                                                    <button type="button" class="btn btn-light btn-sm plus" title="Aumentar">
                                                        <i class="bi bi-plus"></i>
                                                    </button>

                                                    <!-- carrito: LinkButton server-side que dispara ItemCommand -->
                                                    <asp:LinkButton runat="server" ID="btnAgregarCarrito"
                                                        CommandName="AgregarAlCarrito"
                                                        CommandArgument='<%# Eval("idPresentacion") %>'
                                                        CssClass="icon-btn cart-btn ms-2"
                                                        title="Agregar al carrito">
                        <i class="bi bi-cart2"></i>
                                                    </asp:LinkButton>
                                                </div>

                                                <!-- fila 2: iconos de acción (opcional). Si no los usas, queda oculta/irrelevante -->
                                                <div class="action-icons d-flex align-items-center gap-2 mt-2">
                                                    <!-- Ejemplo: si quieres agregar iconos (no cambian la lógica): -->
                                                    <!--
                    <button type="button" class="icon-btn" title="Guardar"><i class="bi bi-floppy"></i></button>
                    <button type="button" class="icon-btn" title="Comentario"><i class="bi bi-chat"></i></button>
                    <button type="button" class="icon-btn" title="Anclar"><i class="bi bi-anchor"></i></button>
                    -->
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

<!-- === Columna 3: Pedido === -->
<div class="col-12 col-xl-4">
    <div class="card h-100 order-card">
        <div class="card-body">

            <div class="d-flex align-items-center gap-2 mb-2">
                <button class="btn btn-success-subtle btn-sm border-success text-success">
                    <i class="bi bi-plus-lg me-1"></i>Nueva cuenta
                </button>
                <div class="flex-grow-1">
                    <div class="input-group input-group-sm">
                        <span class="input-group-text bg-white"><i class="bi bi-person-badge"></i></span>
                        <input class="form-control" value="Cuenta General" />
                        <span class="input-group-text bg-white fw-semibold">
                            <%# string.Format(new System.Globalization.CultureInfo("es-CO"), "{0:C0}", Models.venta.totalVenta) %>
                        </span>
                    </div>
                </div>
            </div>

            <asp:Repeater runat="server" ID="rpDetalleCaja" DataSource="<%# Models.detalleCaja %>">
    <ItemTemplate>
        <!-- item pedido -->
        <div class="pedido-item mb-2 p-2 border rounded" style="width:100%; box-sizing:border-box;">

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
                        <button type="button" class="btn btn-light btn-qty btn-square btn-decrease" data-id='<%# Eval("id") %>'>
                            <i class="bi bi-dash"></i>
                        </button>
                        <button type="button" class="btn btn-light btn-qty disabled">
                            <%# Convert.ToInt32(Eval("unidad")) %>
                        </button>
                        <button type="button" class="btn btn-light btn-qty btn-square btn-increase" data-id='<%# Eval("id") %>'>
                            <i class="bi bi-plus"></i>
                        </button>
                    </div>

                    <button type="button" class="icon-btn cart-btn ms-auto" title="Agregar al carrito" data-id='<%# Eval("id") %>'>
                        <i class="bi bi-cart"></i>
                    </button>
                </div>

                <!-- Fila 2: Iconos de acción -->
                <div class="d-flex flex-wrap gap-2 mb-2">
                    <button type="button" class="icon-btn" title="Guardar" data-id='<%# Eval("id") %>'><i class="bi bi-floppy"></i></button>
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
                <span>$ 100.000</span>
            </div>
            <div class="d-flex justify-content-between small mb-2">
                <span class="text-muted">Impuestos (8%)</span>
                <span>$ 0</span>
            </div>
            <div class="d-flex justify-content-between fw-semibold mb-2">
                <span>Total 1:</span>
                <span>$ 100.000</span>
            </div>
            <div class="d-flex justify-content-between align-items-center mb-3">
                <span>Servicio (10%)</span>
                <div>
                    <span class="badge bg-primary-subtle text-primary fw-semibold me-2">Editar</span>
                    <span>$ 10.000</span>
                </div>
            </div>
            <div class="d-flex justify-content-between fs-6 fw-bold mb-3">
                <span>Total 2:</span>
                <span>$ 110.000</span>
            </div>

            <!-- acciones grandes -->
            <div class="row g-3">
                <div class="col-12 col-md-4">
                    <button class="cta cta-orange w-100">
                        <i class="bi bi-send me-2"></i>Comandar
                    </button>
                </div>
                <div class="col-12 col-md-4">
                    <button class="cta cta-purple w-100">
                        <i class="bi bi-chat-left-text me-2"></i>Solicitar<br />Cuenta
                    </button>
                </div>
                <div class="col-12 col-md-4">
                    <button class="cta cta-green w-100">
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



    <script>
        // ====== MANTIENE TU LÓGICA, SOLO HAGO MÁS ROBUSTO EL USO DE bootstrap.Modal ======
        document.addEventListener('DOMContentLoaded', function () {
            var contenedor = document.getElementById('listaServicios');
            if (contenedor && !contenedor._wired) {
                contenedor._wired = true;
                contenedor.addEventListener('click', function (ev) {
                    var btn = ev.target.closest('.servicio-item');
                    if (!btn) return;

                    var idServicio = btn.getAttribute('data-id');
                    // setea el hidden
                    document.getElementById('<%= hfServicioId.ClientID %>').value = idServicio;

                    // cierra el modal y postea (con chequeo seguro de Bootstrap)
                    var modalEl = document.getElementById('mdlServicios');
                    if (modalEl) {
                        if (window.bootstrap && bootstrap.Modal) {
                            var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                            modal.hide();
                        } else if (window.jQuery && $.fn.modal) {
                            // Fallback si por alguna razón hay Bootstrap 4
                            $(modalEl).modal('hide');
                        }
                    }
                    document.getElementById('<%= btnMesaAmarrar.ClientID %>').click();
                });
            }
        });
    </script>

    <script>
        // === Helper global para abrir el modal de servicios ===
        // Lo llama el code-behind (ConfirmDual -> jsDeny)
        window.abrirModalServicios = function (nombreMesa, idMesa) {
            // Setea título e ID de mesa
            document.getElementById('lblMesaSeleccionada').textContent = nombreMesa || '';
            document.getElementById('<%= hfMesaId.ClientID %>').value = idMesa || '';

            // Apertura robusta (Bootstrap 5 + fallback Bootstrap 4)
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
        };

        // (Opcional) si usas UpdatePanel: vuelve a enlazar la lista tras partial postback
        if (window.Sys && Sys.Application) {
            Sys.Application.add_load(function () {
                var contenedor = document.getElementById('listaServicios');
                if (!contenedor || contenedor._wired) return;
                contenedor._wired = true;
                contenedor.addEventListener('click', function (ev) {
                    var btn = ev.target.closest('.servicio-item');
                    if (!btn) return;

                    var idServicio = btn.getAttribute('data-id');
                    document.getElementById('<%= hfServicioId.ClientID %>').value = idServicio;

                    var modalEl = document.getElementById('mdlServicios');
                    if (modalEl) {
                        if (window.bootstrap && bootstrap.Modal) {
                            var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                            modal.hide();
                        } else if (window.jQuery && $.fn.modal) {
                            $(modalEl).modal('hide');
                        }
                    }
                    document.getElementById('<%= btnMesaAmarrar.ClientID %>').click();
                });
            });
        }
    </script>


    <script>
        document.addEventListener('DOMContentLoaded', function () {
            // Maneja todos los botones + y - del listado
            document.querySelectorAll('.producto-item').forEach(item => {
                const minus = item.querySelector('.minus');
                const plus = item.querySelector('.plus');
                const input = item.querySelector('.qty-input');

                minus.addEventListener('click', () => {
                    let val = parseInt(input.value) || 0;
                    if (val > 0) input.value = val - 1;
                });

                plus.addEventListener('click', () => {
                    let val = parseInt(input.value) || 0;
                    input.value = val + 1;
                });
            });
        });
    </script>


    <script>
        function ajustarBloquesProducto() {
            document.querySelectorAll('.col-xl-4 .product-actions .row').forEach(row => {
                const col3 = row.closest('.col-xl-4');
                if (!col3) return;

                const anchoCol = col3.clientWidth;

                row.classList.remove('vertical', 'horizontal-two', 'horizontal-three');

                if (anchoCol < 400) {
                    row.classList.add('vertical');
                } else if (anchoCol >= 400 && anchoCol <= 550) {
                    row.classList.add('horizontal-two');
                } else {
                    row.classList.add('horizontal-three');
                }
            });
        }

        window.addEventListener('load', ajustarBloquesProducto);
        window.addEventListener('resize', ajustarBloquesProducto);
    </script>


</asp:Content>









