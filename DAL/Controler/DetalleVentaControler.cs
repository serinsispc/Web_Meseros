using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class DetalleVentaControler
    {
        public static bool CRUD(DetalleVenta dv,int boton)
        {
            try
            {
                using (DBEntities cn = new DBEntities()) 
                {
                    if (boton == 0) { cn.DetalleVenta.Add(dv); }
                    if (boton == 1) { cn.Entry(dv).State=System.Data.Entity.EntityState.Modified; }
                    if (boton == 2) { cn.Entry(dv).State = System.Data.Entity.EntityState.Deleted; }
                    cn.SaveChanges();
                }
                return true;
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return false;
            }
        }
    }
}
