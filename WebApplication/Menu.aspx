<%@ Page Title="Servicios" Language="C#" MasterPageFile="~/Menu.Master" AutoEventWireup="true" CodeBehind="Menu.aspx.cs" Inherits="WebApplication.Menu"
    MaintainScrollPositionOnPostback="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- =========================
     Estilos específicos de la página
     ========================= -->
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

        /* Pequeñas utilidades usadas en el markup */
        .service-chip.active {
            box-shadow: 0 0 0 3px rgba(0,123,255,0.12);
        }

        .mesa-card {
            padding: 10px;
            border-radius: 6px;
            display: block;
            text-decoration: none;
        }

            .mesa-card.libre {
                background: #f8f9fa;
                color: #212529;
            }

            .mesa-card.ocupada {
                background: #ffdede;
                color: #212529;
            }

        .producto-item {
            border-bottom: 1px solid #eee;
            padding: 8px 0;
        }

        .controls-inline .qty-input, .quantity-input {
            width: 70px;
        }

        .icon-btn {
            background: transparent;
            border: none;
            cursor: pointer;
            padding: 6px;
        }

            .icon-btn.danger {
                color: #dc3545;
            }

        .price-badge {
            padding: 6px 10px;
            background: #f1f3f5;
            border-radius: 6px;
        }

        .cta {
            font-weight: 600;
            border-radius: 8px;
        }

        .cta-orange {
            background: linear-gradient(90deg,#ff7a00,#ff9a33);
            color: white;
            border: none;
        }

        .cta-purple {
            background: linear-gradient(90deg,#7b61ff,#b992ff);
            color: white;
            border: none;
        }

        .cta-green {
            background: linear-gradient(90deg,#28a745,#52d172);
            color: white;
            border: none;
        }
    </style>

    <!-- =========================
         Scripts globales pequeños (scroll)
         ========================= -->
    <script>
        function scrollToTop() {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
        function scrollToBottom() {
            window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
        }
    </script>

    <!-- =========================
         Cálculo rápido de propina (se ejecuta en render)
         Nota: mantenido como código server-side (inline) para compatibilidad.
         Si prefieres, podemos mover esto al code-behind.
         ========================= -->
    <%
        // Variables calculadas en servidor para mostrar totales con propina
        int porpropina = 0;
        int valorpropina = 0;
        int totalapagar = 0;
        if (Models?.venta != null)
        {
            totalapagar = Convert.ToInt32(Models.venta.totalVenta);
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
                    valorpropina = WebApplication.Class.ClassPropina.CalcularValoPropina(
                        Models.venta.por_propina.ToString(),
                        Models.venta.subtotalVenta.ToString()
                    );
                }
                else
                {
                    valorpropina = Convert.ToInt32(Models.venta.subtotalVenta);
                }
            }
            totalapagar = totalapagar + valorpropina;
            Models.venta.total_A_Pagar = totalapagar;
            /* DataBind se llama en code-behind; si necesitas que este bloque refresque
               controles declarados con  debes llamar DataBind en servidor. */
        }
    %>

    <!-- Botones flotantes -->
    <div class="scroll-buttons">
        <button type="button" class="scroll-btn" onclick="scrollToTop()">▲</button>
        <button type="button" class="scroll-btn" onclick="scrollToBottom()">▼</button>
    </div>

    <!-- Hidden fields / botones ocultos para postback -->
    <asp:HiddenField ID="hfMesaId" runat="server" />
    <asp:HiddenField ID="hfServicioId" runat="server" />


    <asp:Button ID="btnMesaNuevaCuenta" runat="server"
        OnClick="MesaNuevaCuenta" Style="display: none" UseSubmitBehavior="false" />

    <asp:Button ID="btnMesaAmarrar" runat="server"
        OnClick="MesaAmarar" Style="display: none" UseSubmitBehavior="false" />

    <!-- =========================
         Main layout (columns)
         ========================= -->
    <div class="container-fluid menu-wrap py-3 py-lg-4">
        <!-- fila: barra de servicios + acciones --> 
        <div class="row g-3 align-items-center mb-3">
            <div class="col-12 col-xl">
                <div class="d-flex flex-wrap gap-2">
<asp:Repeater runat="server" ID="rpCuentas"
    DataSource="<%# Models.cuentas %>"
    OnItemCommand="rpServicios_ItemCommand">
    <ItemTemplate>
        <div class="position-relative">
            <!-- Botón flotante del lápiz -->
            <button type="button"
                    class="btn btn-link position-absolute top-0 end-0 p-0 m-1 z-3 btn-alias"
                    data-id='<%# Eval("id") %>'
                    data-alias='<%# Eval("aliasVenta") %>'
                    title="Editar alias">
                <i class="bi bi-pencil-fill text-warning fs-5"></i>
            </button>

            <!-- LinkButton principal -->
            <asp:LinkButton ID="btnServicio" runat="server"
                CommandName="AbrirServicio"
                CommandArgument='<%# Eval("id") %>'
                CssClass='<%# "service-chip w-100 d-block text-start p-3 border rounded shadow-sm bg-white position-relative" + ((Eval("id").ToString() == Models.IdCuentaActiva.ToString()) ? " active" : "") %>'>

                <span class="fw-bold text-primary d-block fs-5"><%# Eval("aliasVenta") %></span>
                <small class="text-muted d-block"><%# Eval("mesa") %></small>
            </asp:LinkButton>
        </div>
    </ItemTemplate>
</asp:Repeater>


                </div>
            </div>

            <div class="col-12 col-xl-auto">
                <div class="d-flex gap-2 justify-content-start justify-content-xl-end">
                    <button runat="server" id="btnNuevoServicio"
                        class="btn btn-primary btn-sm">
                        <i class="bi bi-plus-circle me-1"></i>Nuevo servicio
                    </button>

                    <button runat="server" id="btnEliminarServicio"
                        type="button"
                        class="btn btn-warning btn-sm text-dark"
                        onserverclick="btnEliminarServicio_ServerClick">
                        <i class="bi bi-trash3 me-1"></i>Eliminar servicio
                    </button>

                    <button id="btnLiberarMesa" type="button" class="btn btn-outline-danger btn-sm">
                        <i class="bi bi-x-circle me-1"></i>Liberar mesa
                    </button>
                </div>
            </div>
        </div>

        <!-- banner servicio activo -->
        <div class="alert alert-primary-soft d-flex align-items-center justify-content-between px-3 py-2 mb-3">
            <div class="fw-semibold">Mesero Activo: <%# Models.NombreMesero %> </div>
            <div class="small text-muted"></div>
        </div>

        <!-- 3 columnas -->
        <div class="row g-3">
            <!-- Columna 1: Zonas / Mesas -->
            <div class="col-12 col-lg-6 col-xl-4">
                <div class="card h-100">
                    <div class="card-body">
                        <ul class="nav nav-pills mb-3 zone-tabs">
                            <asp:Repeater ID="rpZonas" runat="server"
                                DataSource="<%# Models.zonas %>"
                                OnItemCommand="rpZonas_ItemCommand">
                                <ItemTemplate>
                                    <li class="nav-item m-1">
                                        <asp:LinkButton ID="lnkZona" runat="server"
                                            CommandName="CambiarZona"
                                            CommandArgument='<%# Eval("id") %>'
                                            CssClass='<%# (Session["zonaactiva"]?.ToString() == Eval("id").ToString()) ? "nav-link bg-primary text-white" : "nav-link" %>'>
                                            <%# Eval("nombreZona") %>
                                        </asp:LinkButton>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>

                        <!-- grilla de mesas -->
                        <div class="row g-2">
                            <asp:Repeater runat="server" ID="rpMesas"
                                DataSource="<%# Models.Mesas.Where(x=>x.idZona==Models.IdZonaActiva).ToList() %>"
                                OnItemCommand="rpMesas_ItemCommand">
                                <ItemTemplate>
                                    <div class="col-2 col-lg-6 col-xl-4" style="min-width: 100px; max-width: 110px">
                                        <asp:LinkButton ID="lnkMesa" runat="server"
                                            data-name='<%# Eval("nombreMesa") %>'
                                            CommandName="AbrirMesa"
                                            CommandArgument='<%# Eval("id") %>'
                                            CssClass='<%# (Convert.ToInt32(Eval("estadoMesa")) == 1) ? "mesa-card ocupada d-block text-start" : "mesa-card libre d-block text-start" %>'>
                                            <div class="mesa-titulo"><%# Eval("nombreMesa") %></div>
                                            <div class="mesa-sub"><%# (Convert.ToInt32(Eval("estadoMesa")) == 1) ? "Ocupada" : "Libre" %></div>
                                        </asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Columna 2: Productos -->
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
                            <div id="categorias-container">
                                <% foreach (var cat in Models.categorias)
                                    { %>
                                <a href="#" class="pill <%= (cat.id == Models.IdCategoriaActiva ? "active" : "") %>" data-id="<%= cat.id %>">
                                    <%= cat.nombreCategoria %>
                                </a>
                                <% } %>
                            </div>
                        </div>

                        <!-- lista de productos -->
                        <div class="vstack gap-2">
                            <div id="productos-container">
                                <asp:Repeater ID="rpProductos" runat="server" OnItemCommand="rpProductos_ItemCommand">
                                    <ItemTemplate>
                                        <div class="producto-item" data-categoria='<%# Eval("idCategoria") %>'>
                                            <div class="prod-main d-flex justify-content-between align-items-center">
                                                <div class="prod-info">
                                                    <div class="prod-name"><%# Eval("nombreProducto") %></div>
                                                    <div class="prod-meta">
                                                        $<%# string.Format("{0:N0}", Eval("precioVenta")) %>
                                                        <a href="#" class="link-primary small ms-2">Ver detalle</a>
                                                    </div>
                                                </div>

                                                <div class="product-actions d-flex align-items-center">
                                                    <div class="controls-inline d-flex align-items-center gap-2">
                                                        <button type="button" class="btn btn-light btn-sm minus" title="Disminuir">
                                                            <i class="bi bi-dash"></i>
                                                        </button>

                                                        <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control qty-input text-center" Text="0" />

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

            <!-- Columna 3: Pedido -->
            <div class="col-12 col-xl-4">
                <div class="card h-100 order-card">
                    <div class="card-body">
                        <div class="d-flex align-items-center gap-2 mb-2">
                            <button runat="server" id="btnNuevaCuentaUI" type="button"
                                class="btn btn-success-subtle btn-sm border-success text-success"
                                onclick="abrirModalCuenta('crear', '', '');">
                                <i class="bi bi-plus-lg me-1"></i>Nueva cuenta
                            </button>

                            <div class="flex-grow-1">
                                <asp:LinkButton ID="btnCuentaGeneral" runat="server"
                                    CssClass="btn btn-primary w-100 text-white d-flex justify-content-between align-items-center">
                                    <span>Cuenta General</span>
                                    <span> <%= string.Format(new System.Globalization.CultureInfo("es-CO"), "{0:C0}", Models.venta.total_A_Pagar) %></span>
                                </asp:LinkButton>
                            </div>


                        </div>

                        <!-- Lista Cuentas Clientes -->
                        <div class="mb-3">
                            <div class="d-flex flex-wrap gap-2">
                                <% 
                                    // Asegurarse que la colección no sea null y filtrar por move
                                    if (Models?.v_CuentaClientes != null && Models.v_CuentaClientes.Any(x => x.idVenta == Models.IdCuentaActiva))
                                    {
                                        var cuentasFiltradas = Models.v_CuentaClientes.Where(x => x.idVenta == Models.IdCuentaActiva);


                                        foreach (var cuenta in cuentasFiltradas)
                                        { %>
                                <div class="card cuenta-card border-success-subtle shadow-sm" style="min-width: 160px; flex: 1;">
                                    <div class="card-body p-2 d-flex flex-column justify-content-between">
                                        <div class="d-flex justify-content-between align-items-start mb-1">
                                            <span class="fw-semibold text-uppercase text-truncate" style="max-width: 120px;">
                                                <%= cuenta.nombreCuenta %>
                                            </span>
                                            <button type="button" class="btn btn-link p-0 text-success"
                                                title="Editar cuenta"
                                                onclick="abrirModalCuenta('editar', '<%= cuenta.id %>', '<%= cuenta.nombreCuenta %>');">
                                                <i class="bi bi-pencil-square"></i>
                                            </button>
                                        </div>
                                        <div class="text-end fw-bold text-success fs-6">
                                            <%= string.Format(new System.Globalization.CultureInfo("es-CO"), "{0:C0}", cuenta.total_A_Pagar) %>
                                        </div>
                                    </div>
                                </div>
                                <%      }
                                    }
                                    else
                                    { %>
                                <div class="alert alert-light border text-center w-100 p-2">
                                    No hay cuentas activas
                                </div>
                                <% } %>
                            </div>
                        </div>

                        <!-- lista detalle pedido -->
                        <asp:Repeater runat="server" ID="rpDetalleCaja" DataSource="<%# Models.detalleCaja %>">
                            <ItemTemplate>
                                <div class="pedido-item mb-2 p-2 border rounded" style="width: 100%; box-sizing: border-box;">
                                    <div class="d-flex flex-column">
                                        <asp:Panel runat="server" CssClass="account-badge"
                                            Visible='<%# !String.IsNullOrEmpty(Eval("nombreCuenta") as string) %>'>
                                            <%# Eval("nombreCuenta") %>
                                        </asp:Panel>

                                        <div class="d-flex justify-content-between align-items-start mb-2">
                                            <div class="nombre-producto fw-semibold lh-sm text-uppercase"><%# Eval("nombreProducto") %></div>
                                            <div class="small text-muted precio-pequenho"><%# string.Format(new System.Globalization.CultureInfo("es-CO"), "{0:C0}", Eval("precioVenta")) %></div>
                                        </div>

                                        <div class="d-flex justify-content-start align-items-center gap-2 flex-wrap mb-2">
                                            <div class="quantity-group btn-group btn-group-sm" role="group" aria-label="Cantidad">
                                                <button type="button" class="btn btn-light btn-qty btn-square btn-decrease">
                                                    <i class="bi bi-dash"></i>
                                                </button>

                                                <input type="number" class="form-control quantity-input text-center" value='<%# Convert.ToInt32(Eval("unidad")) %>' min="1" style="width: 100px;" />

                                                <button type="button" class="btn btn-light btn-qty btn-square btn-increase">
                                                    <i class="bi bi-plus"></i>
                                                </button>
                                            </div>

                                            <button type="button" class="icon-btn cart-btn ms-auto" title="Guardar" data-id='<%# Eval("id") %>'>
                                                <i class="bi bi-floppy"></i>
                                            </button>
                                        </div>

                                        <div class="d-flex flex-wrap gap-2 mb-2">
<button type="button" class="icon-btn btn-comentario" title="Comentario"
        data-id='<%# Eval("id") %>'
        data-idcategoria='<%# Eval("idCategoria") %>'
        data-adiciones='<%# Eval("adiciones") %>'
        data-bs-toggle="modal" data-bs-target="#modalNotasDetalle">
  <i class="bi bi-chat"></i>
</button>

                                            <button type="button" class="icon-btn btn-anclar" title="Anclar" data-id='<%# Eval("id") %>'><i class="bi bi-link-45deg"></i></button>
                                            <button type="button" class="icon-btn btn-eliminar danger" title="Eliminar" data-id='<%# Eval("id") %>'><i class="bi bi-trash"></i></button>
                                            <button type="button" class="icon-btn btn-dividir" title="Dividir" data-id='<%# Eval("id") %>' data-cantidadActual='<%# Eval("unidad") %>'><i class="bi bi-scissors"></i></button>
                                        </div>



                                        <div class="d-flex justify-content-between align-items-center">
                                            <div class="text-muted">
                                                <i class="bi bi-journal-text me-1"></i>
                                                <%# Eval("adiciones") %>
                                            </div>
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

                        <div class="row g-3">
                            <div class="col-12 col-md-4">
                                <button class="cta cta-orange w-100" style="height: 80px;"><i class="bi bi-send me-2"></i>Comandar</button>
                            </div>
                            <div class="col-12 col-md-4">
                                <button class="cta cta-purple w-100" style="height: 80px;">
                                    <i class="bi bi-chat-left-text me-2"></i>Solicitar<br />
                                    Cuenta</button>
                            </div>
                            <div class="col-12 col-md-4">
                                <button class="cta cta-green w-100" style="height: 80px;"><i class="bi bi-cash-coin me-2"></i>Cobrar</button>
                            </div>
                        </div>

                    </div>
                    <!-- card-body -->
                </div>
                <!-- card -->
            </div>
            <!-- col -->
        </div>
        <!-- /row 3 cols -->
    </div>
    <!-- /container-fluid -->

    <!-- Modal: seleccionar servicio existente -->
    <div class="modal fade" id="mdlServicios" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Seleccionar servicio para <span id="lblMesaSeleccionada" class="fw-semibold"></span></h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>
                <div class="modal-body">
                    <p class="text-muted mb-2">Servicios activos:</p>

                    <div id="listaServicios" class="list-group">
                        <asp:Repeater ID="rpServiciosActivos" runat="server" DataSource="<%# Models.cuentas %>">
                            <ItemTemplate>
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

    <!-- Modal alerta sesión -->
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

    <!-- Hidden fields para mantener estado -->
    <asp:HiddenField ID="hfCuentaId" runat="server" />
    <asp:HiddenField ID="hfCuentaMode" runat="server" />

    <!-- Modal: Crear / Editar Nombre de Cuenta -->
    <div class="modal fade" id="modalCuentaCliente" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalCuentaTitulo">Nueva Cuenta</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>

                <div class="modal-body">
                    <div class="mb-2">
                        <label for="txtCuentaNombre" class="form-label">Nombre de la cuenta</label>
                        <input type="text" id="txtCuentaNombre" class="form-control" maxlength="100" placeholder="Ingrese el nombre de la cuenta" />
                        <div id="cuentaError" class="form-text text-danger d-none">Ingrese un nombre válido (mín. 2 caracteres).</div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>

                    <!-- botón de guardar-client (no server control) -->
                    <button type="button" id="btnGuardarCuenta" class="btn btn-primary" onclick="guardarCuenta()">Guardar</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal: Anclar detalle a cuenta -->
    <div class="modal fade" id="modalAnclar" tabindex="-1" aria-labelledby="modalAnclarLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalAnclarLabel">Anclar detalle a cuenta</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div id="anclar-cuentas-list" class="d-grid gap-2">
                        <!-- botones de cuentas se agregan vía JS -->
                    </div>
                    <div id="anclar-empty" class="text-muted small d-none">No hay cuentas disponibles.</div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
                </div>
            </div>
        </div>
    </div>

<!-- Hidden field para detalle seleccionado (opcional) -->
<asp:HiddenField ID="hfDetalleId" runat="server" />


<!-- Modal Dividir Detalle -->
<div class="modal fade" id="modalDividirDetalle" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content p-3">
            <div class="modal-header">
                <h5 class="modal-title">Dividir detalle</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <div class="mb-2">
                    <label class="form-label">Cantidad actual:</label>
                    <input type="number" id="txtCantidadActual" class="form-control" readonly />
                </div>
                <div class="mb-2">
                    <label class="form-label">Cantidad a dividir:</label>
                    <input type="number" id="txtCantidadDividir" class="form-control" min="1" />
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                <button type="button" class="btn btn-primary" id="btnConfirmarDividir">Dividir</button>
            </div>
        </div>
    </div>
</div>



    <!-- Modal: Notas / Adiciones del detalle -->
<div class="modal fade" id="modalNotasDetalle" tabindex="-1" aria-labelledby="modalNotasDetalleLabel" aria-hidden="true">
  <div class="modal-dialog modal-md modal-dialog-centered">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="modalNotasDetalleLabel">Comentario / Adiciones</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
      </div>

      <div class="modal-body">
        <!-- Lista de adiciones -->
        <div class="mb-2">
          <div class="small text-muted mb-1">Toque para agregar o quitar:</div>
          <div id="notas-adiciones-list" class="d-flex flex-wrap gap-2"></div>
          <div id="notas-adiciones-empty" class="text-muted small d-none">No hay adiciones para esta categoría.</div>
        </div>

        <!-- Textarea -->
        <label for="notas-adiciones-textarea" class="form-label small text-muted">Comentario (puede escribir manualmente):</label>
        <textarea id="notas-adiciones-textarea" class="form-control" rows="4" placeholder="Ej: con hielo; sin verduras;"></textarea>
      </div>

      <div class="modal-footer">
        <button type="button" class="btn btn-outline-secondary" id="btnNotasLimpiar">
          <i class="bi bi-eraser"></i> Limpiar
        </button>
        <button type="button" class="btn btn-primary" id="btnNotasGuardar">
          <i class="bi bi-check2"></i> Guardar
        </button>
        <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cancelar</button>
      </div>
    </div>
  </div>
</div>


    <!-- Modal: Editar Alias -->
<div class="modal fade" id="modalAlias" tabindex="-1" aria-labelledby="modalAliasLabel" aria-hidden="true">
  <div class="modal-dialog modal-dialog-centered">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="modalAliasLabel">Editar alias de la cuenta</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
      </div>

      <div class="modal-body">
        <!-- ID cuenta oculto -->
        <asp:HiddenField ID="HiddenField1" runat="server" />

        <div class="mb-3">
          <label for="txtAlias" class="form-label">Alias</label>
          <asp:TextBox ID="txtAlias" runat="server" CssClass="form-control" MaxLength="100" />
          <div class="form-text">Ej.: agrega el nombre personalizado para este servicio.</div>
        </div>
      </div>

      <div class="modal-footer">
        <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cancelar</button>
        <asp:Button ID="btnGuardarAlias" runat="server" CssClass="btn btn-primary"
                    Text="Guardar" OnClick="btnGuardarAlias_Click" />
      </div>
    </div>
  </div>
</div>



    <script>
        (function () {
            // --- Guardar inicialización para evitar doble wiring en UpdatePanel / postbacks parciales ---
            if (window.__menuInit) return;
            window.__menuInit = true;

            // -----------------------
            // Helpers DOM / utilidades
            // -----------------------
            function $id(id) { return document.getElementById(id); }
            function toArray(nodeList) { return nodeList ? Array.prototype.slice.call(nodeList) : []; }
            function formatSafe(val, fallback) { return val == null ? (fallback || '') : val; }

            // Obtener colecciones (accede a ClientID server controls con inline expressions)
            var hfMesaClientId = '<%= hfMesaId.ClientID %>';
            var hfServicioClientId = '<%= hfServicioId.ClientID %>';
            var btnMesaAmarrarUniqueId = '<%= btnMesaAmarrar.UniqueID %>';
            var btnMesaAmarrarClientId = '<%= btnMesaAmarrar.ClientID %>'; // en caso de necesitar
            // campos de cuentas (modal)
            var hfCuentaIdClientId = '<%= hfCuentaId.ClientID %>';
            var hfCuentaModeClientId = '<%= hfCuentaMode.ClientID %>';

            // -----------------------
            // Funciones exportadas (globales para uso en HTML)
            // -----------------------
            window.abrirModalServicios = function (nombreMesa, idMesa) {
                var lbl = $id('lblMesaSeleccionada');
                if (lbl) lbl.textContent = nombreMesa || '';

                var hf = $id(hfMesaClientId);
                if (hf) hf.value = idMesa || '';

                var el = $id('mdlServicios');
                if (!el) { console.warn('#mdlServicios no existe'); return; }

                if (window.bootstrap && bootstrap.Modal) {
                    var m = bootstrap.Modal.getInstance(el) || new bootstrap.Modal(el);
                    m.show();
                } else if (window.jQuery && $.fn.modal) {
                    $(el).modal('show');
                } else {
                    console.error('Bootstrap no está cargado.');
                }
            };

            window.abrirModalCuenta = function (mode, id, name) {
                var titulo = $id('modalCuentaTitulo');
                if (titulo) titulo.textContent = (mode === 'editar') ? 'Editar nombre de cuenta' : 'Nueva cuenta';

                var input = $id('txtCuentaNombre');
                if (input) input.value = name || '';

                var err = $id('cuentaError');
                if (err) err.classList.add('d-none');

                var hfId = $id(hfCuentaIdClientId);
                var hfMode = $id(hfCuentaModeClientId);
                if (hfId) hfId.value = id || '';
                if (hfMode) hfMode.value = mode || 'crear';

                var modalEl = $id('modalCuentaCliente');
                if (!modalEl) return console.warn('modalCuentaCliente no encontrado');
                var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                modal.show();

                modalEl.addEventListener('shown.bs.modal', function () {
                    if (input) input.focus();
                }, { once: true });
            };

            window.guardarCuenta = function () {
                var input = $id('txtCuentaNombre');
                var err = $id('cuentaError');
                var hfId = $id(hfCuentaIdClientId);
                var hfMode = $id(hfCuentaModeClientId);

                var nombre = input ? input.value.trim() : '';
                var mode = hfMode ? hfMode.value : 'crear';
                var id = hfId ? hfId.value : '';

                if (!nombre || nombre.length < 2) {
                    if (err) err.classList.remove('d-none');
                    if (input) input.focus();
                    return;
                } else {
                    if (err) err.classList.add('d-none');
                }

                var safeNombre = nombre.replace(/\|/g, ' ');
                var argumento = mode + '|' + (id || '') + '|' + safeNombre;

                __doPostBack('GuardarCuenta', argumento);
            };

            // -----------------------
            // DOMContentLoaded: inicializadores y delegación
            // -----------------------
            document.addEventListener('DOMContentLoaded', function () {
                // referencias containers
                var serviciosContainer = $id('listaServicios');
                var buscador = $id('buscador-productos');
                var btnLimpiar = $id('limpiar-buscador');
                var categoriaContainer = $id('categorias-container');
                var productosContainer = $id('productos-container');

                // helpers para productos/categorias
                function obtenerProductosLista() {
                    return productosContainer ? toArray(productosContainer.querySelectorAll('.producto-item')) : [];
                }
                function obtenerCategoriasLista() {
                    return categoriaContainer ? toArray(categoriaContainer.querySelectorAll('.pill')) : [];
                }

                // Filtrado por texto
                function filtrarProductos(texto) {
                    texto = (texto || '').toLowerCase().trim();
                    var productos = obtenerProductosLista();
                    var categoriaCount = {};

                    productos.forEach(function (prod) {
                        var nameEl = prod.querySelector('.prod-name');
                        var nombre = nameEl ? nameEl.textContent.toLowerCase() : '';
                        var catId = prod.dataset.categoria || '';
                        var visible = nombre.indexOf(texto) !== -1;
                        prod.style.display = visible ? 'block' : 'none';
                        if (visible) categoriaCount[catId] = (categoriaCount[catId] || 0) + 1;
                    });

                    // activar categoria con más coincidencias
                    var maxCat = null, maxCount = 0;
                    for (var k in categoriaCount) {
                        if (categoriaCount[k] > maxCount) { maxCount = categoriaCount[k]; maxCat = k; }
                    }
                    var cats = obtenerCategoriasLista();
                    cats.forEach(function (c) {
                        if (c.dataset.id === maxCat) c.classList.add('active'); else c.classList.remove('active');
                    });
                }

                // Inicializar buscador
                if (buscador) {
                    buscador.addEventListener('input', function () {
                        filtrarProductos(buscador.value);
                    });
                }

                if (btnLimpiar) {
                    btnLimpiar.addEventListener('click', function () {
                        if (buscador) buscador.value = '';
                        obtenerProductosLista().forEach(function (p) { p.style.display = 'block'; });
                        obtenerCategoriasLista().forEach(function (c) { c.classList.remove('active'); });
                    });
                }

                // Manejo de categorias (delegación sobre container)
                if (categoriaContainer) {
                    categoriaContainer.addEventListener('click', function (ev) {
                        var a = ev.target.closest('.pill');
                        if (!a) return;
                        ev.preventDefault();
                        var idCategoria = a.dataset.id;
                        obtenerCategoriasLista().forEach(function (c) { c.classList.remove('active'); });
                        a.classList.add('active');
                        obtenerProductosLista().forEach(function (prod) {
                            prod.style.display = (prod.dataset.categoria === idCategoria) ? 'block' : 'none';
                        });
                        if (buscador) buscador.value = '';
                    });

                    // activar la categoría que tenga la clase active (si existe)
                    var primeraActiva = categoriaContainer.querySelector('.pill.active');
                    if (primeraActiva) primeraActiva.click();
                }

                // -----------------------
                // Delegación unificada de click sobre document.body
                // - minus / plus (productos)
                // - cart-btn (productos)
                // - btn-decrease / btn-increase / cart-btn en detalle pedido
                // - eliminar (icon-btn.danger)
                // -----------------------
                document.body.addEventListener('click', function (ev) {
                    // servicios modal: selección
                    var servicioBtn = ev.target.closest('.servicio-item');
                    if (servicioBtn && serviciosContainer && serviciosContainer.contains(servicioBtn)) {
                        var idServicio = servicioBtn.getAttribute('data-id');
                        if (idServicio) {
                            var hfServicio = $id(hfServicioClientId);
                            if (hfServicio) hfServicio.value = idServicio;

                            var modalEl = $id('mdlServicios');
                            if (modalEl && window.bootstrap && bootstrap.Modal) {
                                var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                                modal.hide();
                                setTimeout(function () { __doPostBack(btnMesaAmarrarUniqueId, ''); }, 200);
                            } else {
                                __doPostBack(btnMesaAmarrarUniqueId, '');
                            }
                        }
                        return;
                    }

                    // Productos - decrease / increase (lista de productos)
                    var minus = ev.target.closest('.minus');
                    if (minus) {
                        var container = minus.closest('.producto-item');
                        var input = container ? container.querySelector('.qty-input') : null;
                        var val = parseInt(input?.value || '0') || 0;
                        if (val > 0 && input) input.value = val - 1;
                        return;
                    }
                    var plus = ev.target.closest('.plus');
                    if (plus) {
                        var container = plus.closest('.producto-item');
                        var input = container ? container.querySelector('.qty-input') : null;
                        var val = parseInt(input?.value || '0') || 0;
                        if (input) input.value = val + 1;
                        return;
                    }
                    var add = ev.target.closest('.cart-btn');
                    if (add && add.closest('.producto-item')) {
                        // Si el botón es LinkButton server-side, el postback lo gestiona ASP.NET automáticamente
                        // Si se quisiera manejar con fetch/axios, poner aquí la lógica
                        return;
                    }

                    // Detalle pedido - cantidad / guardar
                    var dec = ev.target.closest('.btn-decrease');
                    if (dec) {
                        var grp = dec.closest('.quantity-group');
                        var input = grp ? grp.querySelector('.quantity-input') : null;
                        if (!input) return;
                        var val = parseInt(input.value || '0') || 0;
                        if (val > 1) input.value = val - 1;
                        return;
                    }
                    var inc = ev.target.closest('.btn-increase');
                    if (inc) {
                        var grp2 = inc.closest('.quantity-group');
                        var input2 = grp2 ? grp2.querySelector('.quantity-input') : null;
                        if (!input2) return;
                        var val2 = parseInt(input2.value || '0') || 0;
                        input2.value = val2 + 1;
                        return;
                    }
                    var guardar = ev.target.closest('.pedido-item .cart-btn');
                    if (guardar) {
                        var id = guardar.getAttribute('data-id');
                        var row = guardar.closest('.pedido-item');
                        var inputCantidad = row ? row.querySelector('.quantity-input') : null;
                        if (!inputCantidad || parseInt(inputCantidad.value || '0') < 1) {
                            alert('Ingrese una cantidad válida.');
                            return;
                        }
                        // Llamada estándar al server; el code-behind debe manejar el target "ActualizarCantidad"
                        __doPostBack('ActualizarCantidad', id + '|' + inputCantidad.value);
                        return;
                    }

                    // Eliminar detalle (icon-btn.danger)
                    var eliminar = ev.target.closest('.icon-btn.danger');
                    if (eliminar) {
                        var idEliminar = eliminar.getAttribute('data-id');
                        var nota = $id('notaEliminar');
                        if (nota) nota.value = '';
                        var modalEl = $id('modalEliminarDetalle');
                        if (modalEl && window.bootstrap && bootstrap.Modal) {
                            var m = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                            m.show();
                            modalEl._idToDelete = idEliminar;
                        } else {
                            var motivo = prompt('Motivo de eliminación:');
                            if (motivo) __doPostBack('EliminarDetalle', idEliminar + '|' + motivo);
                        }
                        return;
                    }
                });

                // -----------------------
                // Confirmar eliminar (modal)
                // -----------------------
                var btnConfirmarEliminar = $id('btnConfirmarEliminar');
                if (btnConfirmarEliminar) {
                    btnConfirmarEliminar.addEventListener('click', function () {
                        var notaEl = $id('notaEliminar');
                        var motivo = notaEl ? notaEl.value.trim() : '';
                        var modalEl = $id('modalEliminarDetalle');
                        var idEliminar = modalEl ? modalEl._idToDelete : null;
                        if (!motivo) { alert('Debe ingresar una nota para eliminar el detalle.'); return; }
                        if (!idEliminar) { alert('No se determinó el detalle a eliminar.'); return; }
                        __doPostBack('EliminarDetalle', idEliminar + '|' + motivo);
                        if (modalEl && window.bootstrap && bootstrap.Modal) {
                            var m = bootstrap.Modal.getInstance(modalEl);
                            if (m) m.hide();
                        }
                    });
                }

                // -----------------------
                // Interceptar ENTER en inputs de cantidad (detalle)
                // -----------------------
                document.body.addEventListener('keydown', function (e) {
                    var input = e.target;
                    if (input && input.classList && input.classList.contains('quantity-input') && (e.key === 'Enter' || e.keyCode === 13)) {
                        e.preventDefault();
                        var row = input.closest('.pedido-item');
                        var btn = row ? row.querySelector('.cart-btn') : null;
                        if (!btn) return;
                        var v = parseInt(input.value || '0') || 0;
                        if (v < 1) { input.value = 1; return; }
                        btn.click();
                    }
                });

                // -----------------------
                // Sesión: mostrar modal al expirar (timeout calculado en servidor)
                // -----------------------
                (function () {
                    try {
                        var sessionTimeoutMinutes = <%= Session.Timeout %>;
                        var sessionTimeoutMs = sessionTimeoutMinutes * 60 * 1000;
                        var nombreDB = '<%= Session["db"] != null ? Session["db"].ToString() : "" %>';
                        setTimeout(function () {
                            var modalEl = $id('sessionModal');
                            if (!modalEl) return;
                            if (window.bootstrap && bootstrap.Modal) {
                                var m = new bootstrap.Modal(modalEl, { backdrop: 'static', keyboard: false });
                                m.show();
                                setTimeout(function () { window.location.href = 'Default.aspx?db=' + encodeURIComponent(nombreDB); }, 5000);
                            } else {
                                window.location.href = 'Default.aspx?db=' + encodeURIComponent(nombreDB);
                            }
                        }, sessionTimeoutMs);

                        var btnGo = $id('btnGoDefault');
                        if (btnGo) btnGo.addEventListener('click', function () {
                            window.location.href = 'Default.aspx?db=' + encodeURIComponent(nombreDB);
                        });
                    } catch (err) { console.warn(err); }


                    // Capturamos Enter en cualquier TextBox de cantidad dentro del repeater
                    document.querySelectorAll('.qty-input').forEach(function (input) {
                        input.addEventListener('keydown', function (e) {
                            if (e.key === 'Enter') {
                                e.preventDefault(); // Evita que el formulario se envíe por defecto

                                // Buscar el botón de carrito dentro del mismo contenedor de producto
                                const productoItem = input.closest('.producto-item');
                                if (productoItem) {
                                    const btn = productoItem.querySelector('.cart-btn');
                                    if (btn) {
                                        btn.click(); // Dispara el postback del LinkButton
                                    }
                                }
                            }
                        });
                    });

                })();

            }); // DOMContentLoaded
        })();
    </script>



  <script type="text/javascript">
      (function () {
          // --- Evita doble inicialización ---
          if (window.__initAnclar) return;
          window.__initAnclar = true;

          // --- Helpers ---
          function $id(id) { return document.getElementById(id); }
          function toArray(nodeList) { return nodeList ? Array.prototype.slice.call(nodeList) : []; }

          // --- Variable global con cuentas (desde code-behind) ---
          window.cuentas = <%= CuentasJson ?? "[]" %>;
          console.log("Cuentas cargadas:", window.cuentas);

          // --- Inicializar modal ---
          var modalEl = $id('modalAnclar');
          if (!modalEl) { console.error('modalAnclar no encontrado'); return; }

          var bsModal;
          function initModal() {
              if (!bsModal && window.bootstrap && bootstrap.Modal) {
                  bsModal = new bootstrap.Modal(modalEl, { backdrop: 'static', keyboard: true });
              }
          }
          initModal();

          // --- Función para abrir modal y crear botones ---
          function openAnclarModal(detalleId) {
              initModal(); // asegúrate que bootstrap.Modal esté inicializado

              var container = $id('anclar-cuentas-list');
              var empty = $id('anclar-empty');
              if (!container || !empty) return console.error('Elementos del modal no encontrados');

              container.innerHTML = '';

              if (!window.cuentas || !Array.isArray(window.cuentas) || window.cuentas.length === 0) {
                  empty.classList.remove('d-none');
                  empty.textContent = 'No hay cuentas disponibles';
                  bsModal.show();
                  return;
              }

              empty.classList.add('d-none');

              window.cuentas.forEach(function (c) {
                  if (!c) return;

                  var btn = document.createElement('button');
                  btn.type = 'button';
                  btn.className = 'btn btn-outline-primary btn-sm d-flex justify-content-between align-items-center';
                  btn.style.gap = '8px';
                  btn.setAttribute('data-cuenta-id', c.id ?? '');
                  btn.setAttribute('data-detalle-id', detalleId ?? '');

                  var left = document.createElement('span');
                  left.textContent = c.nombre ?? '(sin nombre)';

                  var right = document.createElement('span');
                  right.className = 'badge bg-light text-dark';
                  right.textContent = (!isNaN(Number(c.total)))
                      ? Number(c.total).toLocaleString('es-CO', { style: 'currency', currency: 'COP', maximumFractionDigits: 0 })
                      : c.total ?? '';

                  btn.appendChild(left);
                  btn.appendChild(right);

                  btn.addEventListener('click', function () {
                      var cuentaId = this.getAttribute('data-cuenta-id');
                      var detId = this.getAttribute('data-detalle-id');
                      var arg = (detId ?? '') + '|' + (cuentaId ?? '');
                      if (typeof __doPostBack === 'function') {
                          __doPostBack('AnclarDetalle', arg);
                      } else {
                          alert('No se puede hacer postback, __doPostBack no definido.');
                      }
                  });

                  container.appendChild(btn);
              });

              bsModal.show();
          }

          // --- Delegación global para abrir modal ---
          document.addEventListener('click', function (e) {
              var btn = e.target.closest('.btn-anclar');
              if (!btn) return;

              // Alert para verificar que el click está llegando
              console.log("Click detectado en .btn-anclar");
              //alert("Click detectado en .btn-anclar");

              var detalleId = btn.getAttribute('data-id');
              if (!detalleId) return console.warn('data-id no definido en el botón .btn-anclar');

              openAnclarModal(detalleId);
          });

      })();
  </script>


<script>
    (function () {
        // --- Evita doble inicialización ---
        if (window.__initDividirDetalle) return;
        window.__initDividirDetalle = true;

        function $id(id) { return document.getElementById(id); }

        // --- Inicializar modal ---
        const modalEl = $id('modalDividirDetalle');
        if (!modalEl) { console.error('modalDividirDetalle no encontrado'); return; }

        let bsModal;
        function initModal() {
            if (!bsModal && window.bootstrap && bootstrap.Modal) {
                bsModal = new bootstrap.Modal(modalEl, { backdrop: 'static', keyboard: true });
            }
        }
        initModal();

        const inputActual = $id('txtCantidadActual');
        const inputDividir = $id('txtCantidadDividir');
        const btnConfirmar = $id('btnConfirmarDividir');

        let detalleIdGlobal = null;
        let cantidadActualGlobal = 0;

        // --- Función para abrir modal ---
        function openDividirModal(detalleId, cantidadActual) {
            initModal();

            detalleIdGlobal = detalleId;
            cantidadActualGlobal = cantidadActual;

            if (inputActual) inputActual.value = cantidadActualGlobal;
            if (inputDividir) {
                inputDividir.value = 1;
                inputDividir.max = cantidadActualGlobal - 1;
            }

            bsModal.show();
        }

        // --- Delegación click en botones .btn-dividir ---
        document.addEventListener('click', function (e) {
            const btn = e.target.closest('.btn-dividir');
            if (!btn) return;

            const cantidadActual = parseInt(btn.getAttribute('data-cantidadActual') || '0');
            const detalleId = btn.getAttribute('data-id');

            if (!detalleId) {
                console.warn('data-id no definido en el botón .btn-dividir');
                return;
            }

            if (cantidadActual <= 1) {
                Swal.fire({
                    icon: 'warning',
                    title: '¡Atención!',
                    text: 'No se puede dividir un detalle con cantidad menor o igual a 1.',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'Aceptar'
                });
                return;
            }

            console.log('Click detectado en .btn-dividir, detalleId:', detalleId);
            openDividirModal(detalleId, cantidadActual);
        });

        // --- Confirmar división ---
        if (btnConfirmar) {
            btnConfirmar.addEventListener('click', function () {
                const cantidadDividir = parseInt(inputDividir.value || '0');
                if (cantidadDividir < 1 || cantidadDividir >= cantidadActualGlobal) {
                    Swal.fire({
                        icon: 'warning',
                        title: '¡Atención!',
                        text: 'Cantidad inválida para dividir.',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'Aceptar'
                    });
                    return;
                }

                if (typeof __doPostBack === 'function') {
                    const arg = detalleIdGlobal + '|' + cantidadActualGlobal + '|' + cantidadDividir;
                    __doPostBack('DividirDetalle', arg);
                    bsModal.hide();
                } else {
                    Swal.fire({
                        icon: 'success',
                        title: 'División realizada',
                        html: `DetalleId: <b>${detalleIdGlobal}</b><br>
                           Cantidad a dividir: <b>${cantidadDividir}</b>`,
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'Aceptar'
                    });
                    bsModal.hide();
                }
            });
        }

    })();
</script>



<script runat="server">
  protected string AdicionesJson => Newtonsoft.Json.JsonConvert.SerializeObject(
      Models.adiciones ?? new List<DAL.Model.V_CatagoriaAdicion>()
  );
</script>

<!-- 👇 ESTO sí es cliente (JS en el navegador) -->
<script>
  // V_CatagoriaAdicion => { id, idCategoria, idAdicion, nombreCategoria, nombreAdicion, estado }
  window.adiciones = <%= AdicionesJson %>;
    console.log('[NotasDetalle] adiciones cargadas:', Array.isArray(window.adiciones) ? window.adiciones.length : window.adiciones);
</script>



<script type="text/javascript">
    (function () {
        if (window.__initNotasDetalle2) return;
        window.__initNotasDetalle2 = true;

        function $id(id) { return document.getElementById(id); }
        function tokenize(str) {
            const seen = new Set(), out = [];
            (str || '').split(';').map(s => s.trim()).filter(Boolean).forEach(s => {
                const k = s.toLowerCase(); if (!seen.has(k)) { seen.add(k); out.push(s); }
            });
            return out;
        }
        function joinCanon(tokens) { return tokens.length ? (tokens.join('; ') + ';') : ''; }

        var modalEl = $id('modalNotasDetalle');
        var listEl = $id('notas-adiciones-list');
        var emptyEl = $id('notas-adiciones-empty');
        var txtEl = $id('notas-adiciones-textarea');
        var btnGuardar = $id('btnNotasGuardar');
        var btnLimpiar = $id('btnNotasLimpiar');

        // Render de adiciones para la categoría
        function renderAdiciones(idCategoria, selectedTokens) {
            listEl.innerHTML = '';
            const cat = Number(idCategoria);

            const items = (window.adiciones || []).filter(a => Number(a.idCategoria) === cat);
            console.log('[NotasDetalle] filtrando por categoría', { cat, total: (window.adiciones || []).length, filtradas: items.length });

            if (!items.length) {
                emptyEl.classList.remove('d-none');
                return;
            }
            emptyEl.classList.add('d-none');

            const setSel = new Set((selectedTokens || []).map(t => t.toLowerCase()));

            items.forEach(a => {
                const name = String(a.nombreAdicion || '').trim();
                if (!name) return;

                const btn = document.createElement('button');
                btn.type = 'button';
                btn.className = 'btn btn-sm btn-outline-secondary adicion-btn';
                btn.dataset.name = name;
                btn.textContent = name;

                if (setSel.has(name.toLowerCase())) btn.classList.add('active');

                btn.onclick = function () {
                    const toks = tokenize(txtEl.value);
                    const i = toks.findIndex(t => t.toLowerCase() === name.toLowerCase());
                    if (i >= 0) { toks.splice(i, 1); btn.classList.remove('active'); }
                    else { toks.push(name); btn.classList.add('active'); }
                    txtEl.value = joinCanon(toks);
                };

                listEl.appendChild(btn);
            });
        }


        function syncFromTextarea() {
            const toks = tokenize(txtEl.value);
            const set = new Set(toks.map(t => t.toLowerCase()));
            listEl.querySelectorAll('.adicion-btn').forEach(btn => {
                const name = (btn.dataset.name || '').toLowerCase();
                btn.classList.toggle('active', set.has(name));
            });
        }
        txtEl.addEventListener('input', syncFromTextarea);

        // ⚡ Punto clave: cuando el modal va a mostrarse, Bootstrap nos da el botón que lo abrió
        modalEl.addEventListener('show.bs.modal', function (ev) {
            const triggerBtn = ev.relatedTarget; // <-- el botón "Comentario" que se clickeó
            if (!triggerBtn) return;

            const id = triggerBtn.getAttribute('data-id');
            const idCategoria = triggerBtn.getAttribute('data-idcategoria');
            const adicionesIniciales = triggerBtn.getAttribute('data-adiciones') || '';

            // Precarga textarea y lista
            txtEl.value = adicionesIniciales;
            renderAdiciones(idCategoria, tokenize(adicionesIniciales));

            // Configurar Guardar para este id
            btnGuardar.onclick = function () {
                const texto = joinCanon(tokenize(txtEl.value));
                const payload = String(id) + '|' + texto;
                if (typeof __doPostBack === 'function') {
                    __doPostBack('NotasDetalle', payload);
                } else {
                    console.error('__doPostBack no disponible');
                }
            };

            // Limpiar con confirmación moderna
            btnLimpiar.onclick = function () {
                if (!txtEl.value.trim()) return;

                AlertModerno.Confirm(
                    "¿Deseas limpiar el comentario/adiciones?",
                    "Esta acción eliminará todo el texto actual.",
                    function (ok) {
                        if (ok) {
                            txtEl.value = '';
                            syncFromTextarea();
                            AlertModerno.Success(null, "¡Listo!", "Comentario limpiado correctamente.", false, 800);
                        }
                    }
                );
            };

        });

    })();
</script>




    <script>
        (function () {
            if (window.__initModalAlias) return;
            window.__initModalAlias = true;

            function $id(id) { return document.getElementById(id); }

            // Instancia del modal
            var modalEl = $id('modalAlias');
            var modalAlias = null;
            function ensureModal() {
                if (!modalAlias && window.bootstrap && bootstrap.Modal) {
                    modalAlias = new bootstrap.Modal(modalEl);
                }
                return modalAlias;
            }

            // Delegación de eventos: cualquier .btn-alias abre el modal
            document.addEventListener('click', function (ev) {
                var btn = ev.target.closest('.btn-alias');
                if (!btn) return;

                // Evita que el click “pase” al LinkButton debajo
                ev.preventDefault();
                ev.stopPropagation();

                var id = btn.getAttribute('data-id') || '';
                // Si quieres pre-cargar el alias actual, puedes guardarlo en data-alias
                var aliasActual = btn.getAttribute('data-alias') || '';

                $id('<%= hfCuentaId.ClientID %>').value = id;
    $id('<%= txtAlias.ClientID %>').value = aliasActual;

      var m = ensureModal();
      if (m) m.show();
  }, true);
        })();
    </script>



</asp:Content>
