using DAL;
using DAL.Controler;
using DAL.Model;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication.Class;
using WebGrease.Activities;

namespace WebApplication
{
    public partial class _Default : Page
    {
        // Regex simple: letras, números y guiones bajos (ajústalo si lo necesitas)
        private static readonly Regex DbNameRegex = new Regex(@"^[A-Za-z0-9_]+$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1) Leer db desde querystring o desde Session si ya está
                string dbRaw = Request.QueryString["db"];

                if (string.IsNullOrWhiteSpace(dbRaw))
                {
                    dbRaw = Session["db"] as string;
                }

                // 2) Si seguimos sin base, aquí decides qué hacer:
                //    - Redirigir al login genérico
                //    - Mostrar mensaje de error
                if (string.IsNullOrWhiteSpace(dbRaw))
                {
                    // SIN base definida → login genérico
                    Session["NombreEmpresa"] = "Mi empresa";
                    return;
                }

                dbRaw = dbRaw.Trim();

                // Validar nombre de base
                if (!DbNameRegex.IsMatch(dbRaw))
                {
                    // Nombre inválido → mejor no conectar
                    Session["NombreEmpresa"] = "Mi empresa";
                    return;
                }

                // 3) Guardar en sesión y conectar
                Session["db"] = dbRaw;

                // 4) Consultar sede
                Sede sede = await SedeControler.Consultar(Session["db"].ToString());
                if (sede != null)
                {
                    Session["NombreEmpresa"] = sede.nombreSede;
                    Session["Sede"] = JsonConvert.SerializeObject(sede);
                    Session["estadopropina"] = sede.estadoPropina;
                    Session["porpropina"] = sede.porcentaje_propina;

                    // 5) Consultar imágenes de la sede
                    Imagenes imagenes = await ImagenesControler.Consultar(Session["db"].ToString(), sede.guidSede);
                    if (imagenes != null && imagenes.imagenBytes != null)
                    {
                        Session["Imagenes"] = JsonConvert.SerializeObject(imagenes);
                        string base64 = Convert.ToBase64String(imagenes.imagenBytes);
                        Session["logo"] = base64;
                        // Guardar físicamente el logo solo si hay datos
                        ClassImagenes.GuardarImagen(base64, Session["db"].ToString());
                    }
                }
                else
                {
                    // No hubo sede → poner un nombre genérico
                    Session["NombreEmpresa"] = "Mi empresa";
                }
            }
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            btnIngresarAsync();
            AlertModerno.SuccessGoTo(this, "Ok", $"Bienvenido emilaino", "~/menu.aspx", esToast: false, ms: 1200);
        }
        private async void btnIngresarAsync()
        {
            if (Session["db"] == null) return;
            if (txtCelular.Text != string.Empty &&
                txtContrasena.Text != string.Empty)
            {
                string usuario = txtCelular.Text;
                string clave = txtContrasena.Text;
                /*procedemos hacer la consulta del usuario*/
                Vendedor vendedor = new Vendedor();
                string db = Session["db"].ToString();
                vendedor = await VendedorControler.Consultar_usuario_clave(db, usuario, clave);
                if (vendedor != null)
                {
                    Session["cajero"] = vendedor.cajaMovil;
                    Session["idvendedor"] = vendedor.id;
                    Session["NombreMesero"] = vendedor.nombreVendedor;
                    string vendedorJson = JsonConvert.SerializeObject(vendedor);
                    Session["vendedor"] = vendedorJson;
                    //llamamor a la pagina menu.aspx
                    Response.Redirect("~/menu.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();

                }
                else
                {
                    //AlertModerno.Error(this, "Error", "Usuario no existe.", true);
                }
            }
        }
    }
}