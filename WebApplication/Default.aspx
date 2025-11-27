<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication._Default" Async="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

 <link href="<%: ResolveUrl("~/Content/css/login.css") %>" rel="stylesheet" />

<main class="login-page">
  <div class="login-card">

    <!-- Logo (no tapa el contenido) -->
    <div class="logo-badge">
      
<img src="<%: ResolveUrl("~/Recursos/Imagenes/Logo/Logo.png") %>" alt="Logo" />

    </div>

    <h2 class="login-title">Iniciar Sesión</h2>
    <div class="login-subtitle">Accede con tus credenciales</div>

    <!-- Celular -->
    <div class="mb-3 input-icon">
      <span class="icon" aria-hidden="true">
        <!-- SVG Teléfono -->
        <svg viewBox="0 0 24 24" width="18" height="18" fill="currentColor">
          <path d="M6.62 10.79a15.05 15.05 0 006.59 6.59l2.2-2.2a1 1 0 011.01-.24 11.36 11.36 0 003.56.57 1 1 0 011 1v3.61a1 1 0 01-1 1A17 17 0 013 5a1 1 0 011-1h3.61a1 1 0 011 1 11.36 11.36 0 00.57 3.56 1 1 0 01-.24 1.01l-2.32 2.22z"/>
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
          <path d="M12 1a5 5 0 00-5 5v3H6a3 3 0 00-3 3v7a3 3 0 003 3h12a3 3 0 003-3v-7a3 3 0 00-3-3h-1V6a5 5 0 00-5-5zm3 8H9V6a3 3 0 016 0v3zM12 13a2 2 0 012 2 2 2 0 01-1 1.73V19a1 1 0 01-2 0v-2.27A2 2 0 0110 15a2 2 0 012-2z"/>
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



</asp:Content>




