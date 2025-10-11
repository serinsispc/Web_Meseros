using DAL.Controler;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Funciones
{
    public class R_VentaVendedor_f
    {
        public static bool Relacionar_Vendedor_Venta(int idventa,int idvendedor)
        {
            try
            {
                int idr = 0;
                int funcion = 0;
                //primero consultamos si ya la venta tiene un vendedor
                var rvv = R_VentaVendedorControler.Consultar_idventa(idventa);
                if (rvv!=null)
                {
                    funcion = 1;
                }
                else
                {
                    rvv = new R_VentaVendedor();
                    rvv.id = 0;
                }
                rvv.idVenta = idventa;
                rvv.idVendedor = idvendedor;
                return R_VentaVendedorControler.CRUD(rvv,funcion);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
        }
    }
}
