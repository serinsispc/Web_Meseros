using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class R_CuentaCliente_DetalleVentaControler
    {
        public static R_CuentaCliente_DetalleVenta Consultar_idDetalle(int iddetalle)
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    return cn.R_CuentaCliente_DetalleVenta.AsNoTracking().Where(x => x.idDetalleVenta == iddetalle).FirstOrDefault();
                }
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return null;
            }
        }
        public static Respuesta_DAL CRUD(R_CuentaCliente_DetalleVenta relacion, int boton)
        {
            try
            {
                using (DBEntities cn = new DBEntities()) 
                {
                    if (boton == 0) { cn.R_CuentaCliente_DetalleVenta.Add(relacion); }
                    if (boton == 1) { cn.Entry(relacion).State = System.Data.Entity.EntityState.Modified; }
                    if (boton == 2) { cn.Entry(relacion).State = System.Data.Entity.EntityState.Deleted; }
                    cn.SaveChanges();
                }
                return new Respuesta_DAL { data = relacion.id, estado = true, mensaje = "ok" };
            }
            catch(Exception ex)
            {
                string error= ex.Message;
                return new Respuesta_DAL { data=0, estado=false, mensaje=error };
            }
        }
    }
}
