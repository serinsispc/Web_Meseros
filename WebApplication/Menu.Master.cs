using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication
{
    public partial class Menu1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["db"] == null)
            {
                Session["db"] = ClassConexionDinamica.db;
                Response.Redirect("Salir.aspx");
            }
        }

    }
}