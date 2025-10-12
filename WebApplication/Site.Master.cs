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
            // Si necesitas lógica de pre-carga (p.ej., llenar ddlSede desde DB), hazlo aquí.
            if (!IsPostBack)
            {
                // 1) Lee "db" del QueryString
                var dbRaw = Request.QueryString["db"];

                // 2) Si viene, valida y guarda (Session o lo que uses)
                if (!string.IsNullOrWhiteSpace(dbRaw))
                {
                    dbRaw = dbRaw.Trim();

                    if (!DbNameRegex.IsMatch(dbRaw))
                    {
                        // Si no cumple el patrón, puedes mostrar error o ignorar
                        // ShowError("Nombre de base de datos inválido.");
                        return;
                    }

                    // 3) Guarda para el resto de la app
                    Session["db"] = dbRaw;
                    ClassConexionDinamica.Conectar("www.serinsispc.com", dbRaw);
                    /*en esta parte consultamos la informacion de la empresa*/
                    Sede sede = new Sede();
                    sede = SedeControler.Consultar();
                    if (sede != null)
                    {
                        Session["NombreEmpresa"] = sede.nombreSede;
                        Session["Sede"]=JsonConvert.SerializeObject(sede);
                        Imagenes imagenes = new Imagenes();
                        imagenes = ImagenesControler.Consultar(sede.guidSede);
                        if (imagenes != null)
                        {
                            Session["Imagenes"] = JsonConvert.SerializeObject(imagenes);
                            Session["logo"] = imagenes.imagenBytes;
                            ClassImagenes.GuardarImagen(Session["logo"].ToString());
                        }
                    }
                }
            }
        }
    }
}