using DAL;          // <-- para SqlAutoDAL
using DAL.Model;
using System;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class VendedorControler
    {
        public static async Task<Vendedor> Consultar_usuario_clave(string db, string usuario, string clave)
        {
            try
            {
                var auto = new SqlAutoDAL();
                var vendedor = await auto.ConsultarUno<Vendedor>(
                    db,
                    x => x.telefonoVendedor == usuario && x.calveVendedor == clave
                );

                return vendedor; 
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}
