using DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class VendedorControler
    {
        public static Vendedor Consultar_usuario_clave(string usuario,string clave)
        {
            try
            {
                using (var cn = new DBEntities())
                {
                    return cn.Vendedor.AsNoTracking().Where(x => x.telefonoVendedor == usuario && x.calveVendedor == clave).FirstOrDefault();
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
