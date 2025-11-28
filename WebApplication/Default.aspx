<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication._Default" Async="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link href="<%: ResolveUrl("~/Content/css/login.css") %>" rel="stylesheet" />

    <!-- Estilos mínimos para las tarjetas de usuario (puedes moverlos a login.css) -->
    <style>
        .users-grid-title {
            font-size: 0.9rem;
            font-weight: 600;
            margin-bottom: 0.5rem;
            color: #0050b8;
        }

        /* BOTONES DE MESEROS SIEMPRE VISIBLES */
        .user-card {
            border-radius: 0.75rem;
            padding: 0.6rem 0.4rem;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            min-height: 80px;
            font-size: 0.85rem;
            text-align: center;

            /* NADA DE TRANSPARENCIAS */
            opacity: 1 !important;
            visibility: visible !important;
            filter: none !important;

            /* Fondo y borde bien marcados */
            background: rgba(255, 255, 255, 0.98) !important;
            border: 1px solid rgba(0, 0, 0, 0.08) !important;

            /* Color de texto por defecto */
            color: #0050b8 !important;

            cursor: pointer;
            transition: 0.18s ease;
        }

        .user-card .user-avatar {
            font-size: 2rem;
            line-height: 1;
        }

        /* Forzar color del ícono */
        .user-card .user-avatar i {
            color: #0050b8 !important;
        }

        /* Forzar color y peso del nombre */
        .user-card .user-name {
            margin-top: 0.2rem;
            word-break: break-word;
            color: #003366 !important;
            font-weight: 600;
        }

        .user-card.selected {
            border-color: #00c6ff !important;
            box-shadow: 0 0 0 2px rgba(0, 198, 255, 0.45);
        }

        .user-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 16px rgba(0, 0, 0, 0.15);
        }
    </style>

    <main class="login-page">
        <div class="login-card">

            <!-- Logo (no tapa el contenido) -->
            <div class="logo-badge">
                <img src="<%:ResolveUrl($"~/Recursos/Imagenes/Logo/{Session["db"]}.png") %>" alt="Logo" />
            </div>

            <h2 class="login-title">Iniciar Sesión</h2>
            <div class="login-subtitle">Selecciona tu usuario y escribe tu clave</div>

            <!-- Selección de mesero -->
            <div class="mb-3">
                <div class="users-grid-title">Meseros disponibles</div>

                <asp:Repeater ID="rptUsuarios" runat="server">
                    <HeaderTemplate>
                        <div class="row row-cols-2 g-2">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="col">
                            <button type="button"
                                    class="btn user-card w-100"
                                    data-celular='<%# Eval("telefonoVendedor") %>'>
                                <div class="user-avatar mb-1">
                                    <i class="bi bi-person-circle"></i>
                                </div>
                                <div class="user-name">
                                    <%# Eval("nombreVendedor") %>
                                </div>
                            </button>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
            </div>

            <!-- Celular (OCULTO, solo para postback) -->
            <div class="mb-3 input-icon d-none">
                <span class="icon" aria-hidden="true">
                    <!-- SVG Teléfono -->
                    <svg viewBox="0 0 24 24" width="18" height="18" fill="currentColor">
                        <path d="M6.62 10.79a15.05 15.05 0 006.59 6.59l2.2-2.2a1 1 0 011.01-.24 11.36 11.36 0 003.56.57 1 1 0 011 1v3.61a1 1 0 01-1 1A17 17 0 013 5a1 1 0 011-1h3.61a1 1 0 011 1 11.36 11.36 0 00.57 3.56 1 1 0 01-.24 1.01l-2.32 2.22z" />
                    </svg>
                </span>
                <label for="txtCelular" class="form-label">Celular</label>
                <asp:TextBox ID="txtCelular" runat="server" CssClass="form-control with-icon"
                    TextMode="Number" MaxLength="10" pattern="\d{10}"
                    title="Ingrese un número de celular válido de 10 dígitos"
                    oninput="if(this.value.length>10) this.value=this.value.slice(0,10);" />
            </div>

            <!-- Contraseña -->
            <div class="mb-3 input-icon">
                <span class="icon" aria-hidden="true">
                    <!-- SVG Candado -->
                    <svg viewBox="0 0 24 24" width="18" height="18" fill="currentColor">
                        <path d="M12 1a5 5 0 00-5 5v3H6a3 3 0 00-3 3v7a3 3 0 003 3h12a3 3 0 003-3v-7a3 3 0 00-3-3h-1V6a5 5 0 00-5-5zm3 8H9V6a3 3 0 016 0v3zM12 13a2 2 0 012 2 2 2 0 01-1 1.73V19a1 1 0 01-2 0v-2.27A2 2 0 0110 15a2 2 0 012-2z" />
                    </svg>
                </span>
                <label for="txtContrasena" class="form-label">Contraseña</label>
                <asp:TextBox ID="txtContrasena" runat="server" TextMode="Password" CssClass="form-control with-icon" />
            </div>

            <div class="d-grid">
                <asp:Button ID="btnIngresar" runat="server" CssClass="btn btn-login text-white"
                    Text="Ingresar" UseSubmitBehavior="true" OnClick="btnIngresar_Click" />
            </div>

            <div class="login-foot">
                © <%: DateTime.Now.Year %> · Todos los derechos reservados
            </div>

        </div>
    </main>

    <!-- Script para que al hacer clic en un mesero se rellene el celular y se enfoque la clave -->
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            var cards = document.querySelectorAll('.user-card');
            var txtCelular = document.getElementById('<%= txtCelular.ClientID %>');
            var txtContrasena = document.getElementById('<%= txtContrasena.ClientID %>');

            cards.forEach(function (card) {
                card.addEventListener('click', function () {
                    var celular = this.getAttribute('data-celular');
                    if (txtCelular) {
                        txtCelular.value = celular;
                    }

                    // marcar tarjeta seleccionada
                    cards.forEach(function (c) { c.classList.remove('selected'); });
                    this.classList.add('selected');

                    // limpiar y enfocar contraseña
                    if (txtContrasena) {
                        txtContrasena.value = '';
                        txtContrasena.focus();
                    }
                });
            });
        });
    </script>

</asp:Content>
