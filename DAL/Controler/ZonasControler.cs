using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class ZonasControler
    {
        public static List<Zonas> Lista()
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    return cn.Zonas.AsNoTracking().ToList();
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
