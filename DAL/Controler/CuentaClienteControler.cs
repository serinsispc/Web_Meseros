using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class CuentaClienteControler
    {
        public static Respuesta_DAL CRUD(CuentaCliente cuenta,int boton)
        {
            try
            {
                string accion=string.Empty;
                using (DBEntities cn = new DBEntities())
                {
                    if (boton == 0) { cn.CuentaCliente.Add(cuenta); accion = "creada"; }
                    if (boton == 1) { cn.Entry(cuenta).State = System.Data.Entity.EntityState.Modified; accion = "editada"; }
                    if (boton == 2) { cn.Entry(cuenta).State = System.Data.Entity.EntityState.Deleted; accion = "eliminada"; }
                    cn.SaveChanges();
                }
                int nuevoid = cuenta.id;
                return new Respuesta_DAL { data=nuevoid, estado=true, mensaje=$"cuenta {accion} correctamente." };
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return new Respuesta_DAL { data=0, estado=false, mensaje=msg };
            }
        }
        public static CuentaCliente CuentaCliente(int id)
        {
            try
            {
                using (DBEntities cn =new DBEntities())
                {
                    return cn.CuentaCliente.AsNoTracking().Where(x => x.id == id).FirstOrDefault();
                }
            }
            catch(Exception ex)
            {
                string respuesta = ex.Message;
                return null;
            }
        }
    }
}
