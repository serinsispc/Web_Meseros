using DAL.Funciones;
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
        public static Respuesta_DAL CRUD(DetalleVenta dv,int boton)
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
                if (boton > 0)
                {
                    //llamamos a la función que se encarga de eliminar ComandImpresaa
                    ComandImpresaa_f.LiberarDetalle(dv.id);
                }
                return new Respuesta_DAL { data=dv.id, estado=true, mensaje="ok" };
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return new Respuesta_DAL { data = 0, estado = false, mensaje = "error" };
            }
        }
        public static DetalleVenta ConsultarId(int id)
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    return cn.DetalleVenta.AsNoTracking().Where(x=>x.id== id).FirstOrDefault();
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
