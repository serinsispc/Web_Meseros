using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class V_CategoriaControler
    {
        public static List<V_Categoria> lista()
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    return cn.V_Categoria.AsNoTracking().Where(x => x.visible == 1).ToList();
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
