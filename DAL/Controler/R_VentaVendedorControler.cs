using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class R_VentaVendedorControler
    {
        public static bool CRUD(R_VentaVendedor rvv,int funcion)
        {
            try
            {
                using (DBEntities cn = new DBEntities()) 
                {
                    if (funcion == 0) { cn.R_VentaVendedor.Add(rvv); };
                    if (funcion == 1) { cn.Entry(rvv).State = System.Data.Entity.EntityState.Modified; };
                    if (funcion == 2) { cn.Entry(rvv).State = System.Data.Entity.EntityState.Deleted; };
                    cn.SaveChanges();
                }
                return true;
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
        }
        public static R_VentaVendedor Consultar_idventa(int idventa)
        {
            try
            {
                using (DBEntities cn = new DBEntities()) {
                    return cn.R_VentaVendedor.AsNoTracking().Where(x => x.id == idventa).FirstOrDefault();
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
