<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        /* === Fondo a pantalla completa === */
        html, body {
            height: 100%;
            margin: 0;
            padding: 0;
        }

        .login-page {
            min-height: 100vh;
            width: 100%;
            display: flex;
            align-items: center;
            justify-content: center;
            background: linear-gradient(135deg, #003973 0%, #0079c1 100%);
        }

        /* === Tarjeta del login === */
        .login-card {
            width: 100%;
            max-width: 400px;
            background-color: #ffffff;
            border-radius: 16px;
            box-shadow: 0 15px 40px rgba(0, 0, 0, 0.25);
            padding: 32px 28px;
            text-align: center;
        }

        /* === Logo === */
        .login-logo {
            width: 60px;
            height: 60px;
            border-radius: 15px;
            margin: 0 auto 15px auto;
            display: flex;
            align-items: center;
            justify-content: center;
            background: linear-gradient(145deg, #b983ff, #7a5cff);
            color: white;
            font-weight: 700;
            font-size: 1.5rem;
            box-shadow: 0 6px 20px rgba(122, 92, 255, 0.35);
        }

        /* === Campos y botones === */
        .form-label {
            font-weight: 600;
            text-align: left;
            display: block;
        }

        .form-control {
            min-height: 44px;
            border-radius: 8px;
        }

        .btn-login {
            min-height: 46px;
            font-weight: 600;
            background-color: #007bff;
            border: none;
            border-radius: 8px;
        }

        .btn-login:hover {
            background-color: #0056b3;
        }

        .login-title {
            color: #0062ff;
            font-weight: 700;
            margin-bottom: 1.2rem;
        }
    </style>

    <main class="login-page">
        <div class="login-card">

            <div class="login-logo">
                S
            </div>

            <h2 class="login-title">Iniciar Sesión</h2>

            <div class="mb-3 text-start">
                <label for="txtCelular" class="form-label">Celular</label>
                <asp:TextBox 
                    ID="txtCelular" 
                    runat="server" 
                    CssClass="form-control" 
                    TextMode="Number" 
                    MaxLength="10" 
                    pattern="\d{10}"
                    title="Ingrese un número de celular válido de 10 dígitos"
                    oninput="if(this.value.length>10) this.value=this.value.slice(0,10);"/>
            </div>

            <div class="mb-3 text-start">
                <label for="txtContrasena" class="form-label">Contraseña</label>
                <asp:TextBox ID="txtContrasena" runat="server" TextMode="Password" CssClass="form-control" />
            </div>

            <div class="d-grid">
                <asp:Button ID="btnIngresar" runat="server" CssClass="btn btn-login text-white" Text="Ingresar" UseSubmitBehavior="true" OnClick="btnIngresar_Click" />
            </div>

        </div>
    </main>

</asp:Content>




