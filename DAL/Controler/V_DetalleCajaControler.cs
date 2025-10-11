using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class V_DetalleCajaControler
    {
        public static List<V_DetalleCaja>Lista_IdVenta(int idVenta)
        {
            try
            {
                using (DBEntities cn = new DBEntities()) {
                    return cn.V_DetalleCaja.AsNoTracking().Where(x => x.idVenta == idVenta).ToList();
                }
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}
