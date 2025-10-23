using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class ImprimirCuentaControler
    {
        public static Respuesta_DAL CRUD(ImprimirCuenta cuenta,int boton)
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    if (boton == 0) { cn.ImprimirCuenta.Add(cuenta); };
                    if (boton == 1) { cn.Entry(cuenta).State = System.Data.Entity.EntityState.Modified; };
                    if (boton == 2) { cn.Entry(cuenta).State = System.Data.Entity.EntityState.Deleted; };
                    cn.SaveChanges();
                }
                return new Respuesta_DAL { data=cuenta.id, estado=true, mensaje="ok" };
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return new Respuesta_DAL {  data=0, estado=false, mensaje="Error"};
            }
        }
    }
}
