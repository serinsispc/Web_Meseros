using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class V_TablaVentasControler
    {
        public static V_TablaVentas Consultar_Id(int idventa)
        {
            try
            {
                using(DBEntities cn =new DBEntities())
                {
                    return cn.V_TablaVentas.AsNoTracking().Where(x => x.id == idventa).FirstOrDefault();
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
