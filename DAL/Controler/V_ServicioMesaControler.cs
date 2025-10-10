using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class V_ServicioMesaControler
    {
        public static List<V_ServicioMesa> lista()
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    return cn.V_ServicioMesa.AsNoTracking().Where(x=>x.estadoServicio==1).ToList();
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
