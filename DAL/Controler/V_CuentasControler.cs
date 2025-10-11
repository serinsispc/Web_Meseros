using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class V_CuentasControler
    {
        public static List<V_Cuentas> Lista_IdVendedor(int idvendedor)
        {
            try
            {
                using (DBEntities cn = new DBEntities()) {
                    return cn.V_Cuentas.Where(x => x.idVendedor == idvendedor).ToList();
                }
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public static List<V_Cuentas> Lista_Mesa(string mesa)
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    return cn.V_Cuentas.Where(x => x.mesa.Contains(mesa)).ToList();
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
