using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class SedeControler
    {
        public static Sede Consultar()
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    return cn.Sede.AsNoTracking().FirstOrDefault();
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
