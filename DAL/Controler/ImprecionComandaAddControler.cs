using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class ImprecionComandaAddControler
    {
        public static Respuesta_DAL CRUD(ImprecionComandaAdd imprecion, int boton)
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    if (boton == 0) { cn.ImprecionComandaAdd.Add(imprecion); };
                    if (boton == 1) { cn.Entry(imprecion).State = System.Data.Entity.EntityState.Modified; };
                    if (boton == 2) { cn.Entry(imprecion).State = System.Data.Entity.EntityState.Deleted; };
                    cn.SaveChanges();
                }
                return new Respuesta_DAL { data=imprecion.id, estado=true, mensaje="ok" };
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return new Respuesta_DAL() { data = null, estado = false, mensaje = "error" };
            }
        }
    }
}
