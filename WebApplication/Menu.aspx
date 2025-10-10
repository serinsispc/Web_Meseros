<%@ Page Title="Servicios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Menu.aspx.cs" Inherits="WebApplication.Menu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

  <!-- Estilos específicos de esta vista -->
    <link href="Content/css/menu.css" rel="stylesheet" />

  <div class="container-fluid menu-wrap py-3 py-lg-4">

    <!-- ====== fila: barra de servicios + acciones ====== -->
    <div class="row g-3 align-items-center mb-3">
      <div class="col-12 col-xl">
        <div class="d-flex flex-wrap gap-2">

<asp:Repeater runat="server" ID="rpServicios"
              DataSource="<%# Models.cuentas %>"
              OnItemCommand="rpServicios_ItemCommand">
    <ItemTemplate>
        <!-- Botón “chip” que hace postback -->
        <asp:LinkButton ID="btnServicio" runat="server"
                        CommandName="AbrirServicio"
                        CommandArgument='<%# Eval("id") %>'
                        CssClass="service-chip">
            <span class="chip-title"><%# Eval("aliasServicio") %></span>
            <small class="text-muted d-block">
                <%# Eval("id") %> - <%# Eval("nombreMesero") %>
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
      <div class="fw-semibold">Servicio Activo: 4 - s1</div>
      <div class="small text-muted"> </div>
    </div>

    <!-- ====== 3 columnas ====== -->
    <div class="row g-3">
      <!-- === Columna 1: Zonas / Mesas === -->
      <div class="col-12 col-lg-4 col-xl-3">
        <div class="card h-100">
          <div class="card-body">
            <!-- tabs de zona -->
            <ul class="nav nav-pills mb-3 zone-tabs">
              <li class="nav-item">
                <a class="nav-link active" href="#">ZONA 1</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="#">DOMICILIOS</a>
              </li>
            </ul>

            <div class="d-flex align-items-center gap-2 mb-2 small">
              <span class="text-muted">Mesas en ZONA 1 :</span>
              <span class="badge bg-secondary">10</span>
              <span class="badge bg-success-subtle text-success">Libres: 7</span>
              <span class="badge bg-danger-subtle text-danger">Ocupadas: 3</span>
            </div>

            <!-- grilla de mesas -->
            <div class="row g-2">
              <%-- muestra/oculta según quieras; aquí solo UI de ejemplo --%>
              <div class="col-6">
                <div class="mesa-card libre">
                  <div class="mesa-titulo">MESA1</div>
                  <div class="mesa-sub">Servicio: 4</div>
                </div>
              </div>
              <div class="col-6">
                <div class="mesa-card ocupada">
                  <div class="mesa-titulo">MESA2</div>
                  <div class="mesa-sub">Servicio: 6</div>
                </div>
              </div>
              <div class="col-6">
                <div class="mesa-card libre">
                  <div class="mesa-titulo">MESA3</div>
                  <div class="mesa-sub">Servicio: 0</div>
                </div>
              </div>
              <div class="col-6">
                <div class="mesa-card libre">
                  <div class="mesa-titulo">MESA4</div>
                  <div class="mesa-sub">Servicio: 0</div>
                </div>
              </div>
              <div class="col-6">
                <div class="mesa-card libre">
                  <div class="mesa-titulo">MESA5</div>
                  <div class="mesa-sub">Servicio: 0</div>
                </div>
              </div>
              <div class="col-6">
                <div class="mesa-card ocupada">
                  <div class="mesa-titulo">MESA6</div>
                  <div class="mesa-sub">Servicio: 5</div>
                </div>
              </div>
              <div class="col-6">
                <div class="mesa-card libre">
                  <div class="mesa-titulo">MESA7</div>
                  <div class="mesa-sub">Servicio: 0</div>
                </div>
              </div>
              <div class="col-6">
                <div class="mesa-card libre">
                  <div class="mesa-titulo">MESA8</div>
                  <div class="mesa-sub">Servicio: 0</div>
                </div>
              </div>
              <div class="col-6">
                <div class="mesa-card libre">
                  <div class="mesa-titulo">MESA9</div>
                  <div class="mesa-sub">Servicio: 0</div>
                </div>
              </div>
              <div class="col-6">
                <div class="mesa-card libre">
                  <div class="mesa-titulo">MESA10</div>
                  <div class="mesa-sub">Servicio: 0</div>
                </div>
              </div>
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
              <a href="#" class="pill active">CAMARAS <span class="count">2</span></a>
              <a href="#" class="pill">DISCOS <span class="count">1</span></a>
              <a href="#" class="pill">EQUIPOS <span class="count">6</span></a>
              <a href="#" class="pill">OTRO <span class="count">2</span></a>
              <a href="#" class="pill">PADRE <span class="count">1</span></a>
              <a href="#" class="pill">SERVICIOS <span class="count">3</span></a>
              <a href="#" class="pill">SOFTWARE <span class="count">7</span></a>
            </div>

            <!-- lista de productos -->
            <div class="vstack gap-2">

              <div class="producto-item">
                <div class="prod-main">
                  <div>
                    <div class="prod-name">CAMARA IP V380</div>
                    <div class="prod-meta">$101.150 <a href="#" class="link-primary small ms-2">Ver detalle</a></div>
                  </div>
                  <div class="qty">
                    <button class="btn btn-light btn-sm minus"><i class="bi bi-dash"></i></button>
                    <input class="form-control qty-input" value="0" />
                    <button class="btn btn-light btn-sm plus"><i class="bi bi-plus"></i></button>
                    <button class="btn btn-primary btn-sm ms-2"><i class="bi bi-cart2"></i></button>
                  </div>
                </div>
              </div>

              <div class="producto-item">
                <div class="prod-main">
                  <div>
                    <div class="prod-name">MEMORIA DE 64 GB</div>
                    <div class="prod-meta">$41.055 <a href="#" class="link-primary small ms-2">Ver detalle</a></div>
                  </div>
                  <div class="qty">
                    <button class="btn btn-light btn-sm minus"><i class="bi bi-dash"></i></button>
                    <input class="form-control qty-input" value="0" />
                    <button class="btn btn-light btn-sm plus"><i class="bi bi-plus"></i></button>
                    <button class="btn btn-primary btn-sm ms-2"><i class="bi bi-cart2"></i></button>
                  </div>
                </div>
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
                <i class="bi bi-plus-lg me-1"></i> Nueva cuenta
              </button>
              <div class="flex-grow-1">
                <div class="input-group input-group-sm">
                  <span class="input-group-text bg-white"><i class="bi bi-person-badge"></i></span>
                  <input class="form-control" value="Cuenta General" />
                  <span class="input-group-text bg-white fw-semibold">$ 110.000</span>
                </div>
              </div>
            </div>

            <!-- item pedido -->
            <div class="pedido-item mb-2">
              <div class="fw-semibold lh-sm">SERVICIO DE SOFTWARE PARA EL CONTROL DE INGRESOS Y EGRESOS</div>
              <div class="small text-muted">$ 50.000</div>

              <div class="d-flex align-items-center gap-2 mt-2">
                <div class="btn-group btn-group-sm" role="group">
                  <button class="btn btn-light"><i class="bi bi-dash"></i></button>
                  <button class="btn btn-light disabled">2</button>
                  <button class="btn btn-light"><i class="bi bi-plus"></i></button>
                </div>

                <div class="btn-icon">
                  <i class="bi bi-floppy2"></i>
                </div>
                <div class="btn-icon">
                  <i class="bi bi-emoji-smile"></i>
                </div>
                <div class="btn-icon">
                  <i class="bi bi-printer"></i>
                </div>
                <div class="btn-icon">
                  <i class="bi bi-patch-check"></i>
                </div>
                <div class="btn-icon">
                  <i class="bi bi-x-lg"></i>
                </div>

                <div class="ms-auto badge bg-secondary-subtle text-dark fw-semibold px-3">$ 100.000</div>
              </div>
            </div>

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
                  <i class="bi bi-send me-2"></i> Comandar
                </button>
              </div>
              <div class="col-12 col-md-4">
                <button class="cta cta-purple w-100">
                  <i class="bi bi-chat-left-text me-2"></i> Solicitar<br/>Cuenta
                </button>
              </div>
              <div class="col-12 col-md-4">
                <button class="cta cta-green w-100">
                  <i class="bi bi-cash-coin me-2"></i> Cobrar
                </button>
              </div>
            </div>

          </div>
        </div>
      </div>

    </div> <!-- /row 3 cols -->

  </div><!-- /container-fluid -->
</asp:Content>

