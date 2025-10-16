using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class TablaVentasControler
    {
        public static Respuesta_DAL CRUD(TablaVentas venta, int boton)
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    if (boton == 0) { cn.TablaVentas.Add(venta); }
                    if (boton == 1) { cn.Entry(venta).State = System.Data.Entity.EntityState.Modified; }
                    if (boton == 2) { cn.Entry(venta).State = System.Data.Entity.EntityState.Deleted; }
                    cn.SaveChanges();
                }
                if (venta.id > 0)
                {
                    return new Respuesta_DAL { data = venta.id, estado = true, mensaje = "proceso terminado con éxito" };
                }
                else
                {
                    return new Respuesta_DAL { data = venta.id, estado = false, mensaje = "proceso terminado con error" };
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return new Respuesta_DAL { data = null, estado = false, mensaje = msg };
            }
        }
        public static Respuesta_DAL Consultar_Id(int id)
        {
            try
            {
                var venta = new TablaVentas();
                using (DBEntities cn =new DBEntities())
                {
                    venta= cn.TablaVentas.AsNoTracking().Where(x => x.id == id).FirstOrDefault();                        
                }
                if (venta == null)
                {
                    return new Respuesta_DAL { data = null, estado = false, mensaje = "error" };
                }
                else
                {
                    return new Respuesta_DAL { data = null, estado = false, mensaje = "error" };
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return new Respuesta_DAL { data = null, estado = false, mensaje = error };
            }
        }
    }
}
