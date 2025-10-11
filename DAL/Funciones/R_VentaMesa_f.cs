using DAL.Controler;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Funciones
{
    public class R_VentaMesa_f
    {
        public static bool Relacionar_Venta_Mesa(int idventa,int idmesa)
        {
            try
            {
                var rvm = new R_VentaMesa();
                rvm.id = 0;
                rvm.idVenta = idventa;
                rvm.idMesa = idmesa;
                bool resp = R_VentaMesaControler.CRUD(rvm,0);
                return resp;
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
        }
    }
}
