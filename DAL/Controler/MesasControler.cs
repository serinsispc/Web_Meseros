using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class MesasControler
    {
        public static bool CRUD(Mesas mesas,int Funcion)
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    if (Funcion == 0) { cn.Mesas.Add(mesas); }
                    if (Funcion == 1) { cn.Entry(mesas).State = System.Data.Entity.EntityState.Modified; }
                    if (Funcion == 2) { cn.Entry(mesas).State = System.Data.Entity.EntityState.Deleted; }
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
        public static List<Mesas> Lista()
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    return cn.Mesas.AsNoTracking().ToList();
                }
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
        public static Mesas Consultar_id(int id)
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    return cn.Mesas.AsNoTracking().Where(x => x.id == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}
