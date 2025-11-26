using DAL;
using DAL.Controler;
using DAL.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication.Class;

namespace WebApplication
{
    public partial class SiteMaster : MasterPage
    {

        // Regex simple: letras, números y guiones bajos (ajústalo si lo necesitas)
        private static readonly Regex DbNameRegex = new Regex(@"^[A-Za-z0-9_]+$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
        protected void Page_Load(object sender, EventArgs e)
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
                Sede sede = SedeControler.Consultar();
                if (sede != null)
                {
                    Session["NombreEmpresa"] = sede.nombreSede;
                    Session["Sede"] = JsonConvert.SerializeObject(sede);
                    Session["estadopropina"] = sede.estadoPropina;
                    Session["porpropina"] = sede.porcentaje_propina;

                    // 5) Consultar imágenes de la sede
                    Imagenes imagenes = ImagenesControler.Consultar(sede.guidSede);
                    if (imagenes != null && imagenes.imagenBytes != null)
                    {
                        Session["Imagenes"] = JsonConvert.SerializeObject(imagenes);
                        Session["logo"] = imagenes.imagenBytes;

                        // Guardar físicamente el logo solo si hay datos
                        ClassImagenes.GuardarImagen(Session["logo"].ToString());
                    }
                }
                else
                {
                    // No hubo sede → poner un nombre genérico
                    Session["NombreEmpresa"] = "Mi empresa";
                }
            }
        }

    }
}