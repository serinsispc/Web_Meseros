using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class ImagenesControler
    {
        public static Imagenes Consultar(Guid guid)
        {
            try
            {
                using (DBEntities cn = new DBEntities())
                {
                    return cn.Imagenes.AsNoTracking().Where(x => x.id == guid).FirstOrDefault();
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
