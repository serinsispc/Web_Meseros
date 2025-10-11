using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class R_VentaMesaControler
    {
        public static bool CRUD(R_VentaMesa rvm,int funcion)
        {
            try
            {
                using (DBEntities cn = new DBEntities()) {
                    if (funcion == 0) { cn.R_VentaMesa.Add(rvm); }
                    if (funcion == 1) { cn.Entry(rvm).State = System.Data.Entity.EntityState.Modified; }
                    if (funcion == 2) { cn.Entry(rvm).State = System.Data.Entity.EntityState.Deleted; }
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
    }
}
