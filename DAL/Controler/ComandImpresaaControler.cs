using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class ComandImpresaaControler
    {
        public static Respuesta_DAL CRUD(ComandImpresaa comand, int boton)
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    if (boton == 0) { cn.ComandImpresaa.Add(comand); }
                    if (boton == 1) { cn.Entry(comand).State = System.Data.Entity.EntityState.Modified; }
                    if (boton == 2) { cn.Entry(comand).State=System.Data.Entity.EntityState.Deleted; }
                    cn.SaveChanges();
                }
                return new Respuesta_DAL { data=comand.id, estado=true, mensaje="ok" };
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return new Respuesta_DAL { data=0, estado=false, mensaje="error" };
            }
        }
        public static ComandImpresaa CosultarIdDetalle(int idDetalle)
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    return cn.ComandImpresaa.AsNoTracking().Where(x => x.idDetalleVenta == idDetalle).FirstOrDefault();
                }
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return null;
            }
        }
    }
}
