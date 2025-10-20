using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class V_DetalleCajaControler
    {
        public static List<V_DetalleCaja>Lista_IdVenta(int idVenta,int idCuanta)
        {
            try
            {
                using (DBEntities cn = new DBEntities()) {
                    if (idCuanta > 0)
                    {
                        return cn.V_DetalleCaja.AsNoTracking().Where(x => 
                        x.idVenta == idVenta &&
                        x.idCuentaCliente==idCuanta &&
                        x.estadoDetalle == 1).ToList();
                    }
                    else
                    {
                        return cn.V_DetalleCaja.AsNoTracking().Where(x => x.idVenta == idVenta && x.estadoDetalle == 1).ToList();
                    }
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
