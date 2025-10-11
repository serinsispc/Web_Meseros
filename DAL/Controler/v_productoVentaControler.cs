using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class v_productoVentaControler
    {
        public static List<v_productoVenta> Lista_IdCategoria(int idcetagoria)
        {
            try
            {
                using (DBEntities cn = new DBEntities()) 
                {
                    return cn.v_productoVenta.AsNoTracking().Where(x =>
                    x.estadoProducto == 1 &&
                    x.idCategoria == idcetagoria).ToList();
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
