using DAL.Controler;
using DAL.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication.Class;
using WebApplication.ViewModels;

namespace WebApplication
{
    public partial class Menu : Page
    {
        public MenuViewModels Models { get; private set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var vendedor = JsonConvert.DeserializeObject<Vendedor>(Session["Vendedor"].ToString());
                V_Cuentas cuentas= new V_Cuentas();
                
            }
        }

        protected void rpServicios_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AbrirServicio")
            {
                var idStr = Convert.ToString(e.CommandArgument);
                // TODO: valida/convierte a int si aplica
                // int id = int.Parse(idStr);

                // Aquí haces tu lógica (cargar servicio, marcar activo, etc.)
                // Ejemplo: CargarServicio(id);

                // Si prefieres ir a otra página:
                // Response.Redirect($"~/OtraPagina.aspx?servicioId={Server.UrlEncode(idStr)}", false);
                // Context.ApplicationInstance.CompleteRequest();

                AlertModerno.Success(this,"ok",$"servicio seleccionado {idStr}",true);
            }
        }

        protected void btnNuevoServicio_Click(object sender, EventArgs e)
        {
            AlertModerno.Success(this, "ok", $"boton seleccionado {e.ToString()}", true);
        }

        protected void btnEliminarServicio_Clik(object sender, EventArgs e)
        {
            var btn = (System.Web.UI.HtmlControls.HtmlButton)sender;
            AlertModerno.Success(this, "ok", $"boton seleccionado {btn.ID}", true);
        }
    }
}