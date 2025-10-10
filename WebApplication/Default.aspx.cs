using DAL;
using DAL.Controler;
using DAL.Model;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication.Class;
using WebGrease.Activities;

namespace WebApplication
{
    public partial class _Default : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            if (txtCelular.Text != string.Empty &&
                txtContrasena.Text != string.Empty)
            {
                string usuario = txtCelular.Text;
                string clave= txtContrasena.Text;
                /*procedemos hacer la consulta del usuario*/
                Vendedor vendedor = new Vendedor();
                vendedor = VendedorControler.Consultar_usuario_clave(usuario,clave);
                if (vendedor != null)
                {
                    Session["vendedor"]=JsonConvert.SerializeObject(vendedor);
                    AlertModerno.Success(this,"Ok",$"Bienvenido {vendedor.nombreVendedor}",true);
                }
                else
                {
                    AlertModerno.Error(this,"Error","Usuario no existe.",true);
                }
            }
        }
    }
}