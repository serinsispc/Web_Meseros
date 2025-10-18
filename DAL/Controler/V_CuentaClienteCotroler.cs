using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class V_CuentaClienteCotroler
    {
        public static List<V_CuentaCliente> Lista(bool eliminada, [Optional] int IdVenta)
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    if (IdVenta > 0)
                    {
                        return cn.V_CuentaCliente.AsNoTracking().Where(x => x.eliminada == eliminada && x.idVenta==IdVenta).ToList();
                    }
                    else
                    {
                        return cn.V_CuentaCliente.AsNoTracking().Where(x => x.eliminada == eliminada).ToList();
                    }
                    
                }
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
        public static V_CuentaCliente Consultar(int id)
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    return cn.V_CuentaCliente.AsNoTracking().Where(x => x.id == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}
