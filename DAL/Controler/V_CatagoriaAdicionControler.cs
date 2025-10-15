using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class V_CatagoriaAdicionControler
    {
        public static List<V_CatagoriaAdicion> Lista()
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    return cn.V_CatagoriaAdicion.AsNoTracking().Where(x=>x.estado==1).ToList();
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
