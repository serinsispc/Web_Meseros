using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class V_CuentasVentaControler
    {
        public static List<V_CuentasVenta> FiltrarListaCuentas(int idMesero)
        {
            try
            {
                using(DBEntities cn =new DBEntities())
                {
                    return cn.V_CuentasVenta.AsNoTracking().Where(x => x.numeroVenta > 0 && x.idvendedor == idMesero).ToList();
                }
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return new List<V_CuentasVenta>();
            }
        }
    }
}
