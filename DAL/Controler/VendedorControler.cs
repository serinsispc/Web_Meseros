using DAL;          // <-- para SqlAutoDAL
using DAL.Model;
using System;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class VendedorControler
    {
        /// <summary>
        /// Consulta un vendedor por usuario (teléfono) y clave.
        /// Equivalente a:
        /// SELECT TOP 1 * FROM Vendedor
        /// WHERE telefonoVendedor = @usuario AND calveVendedor = @clave
        /// </summary>
        public static async Task<Vendedor> Consultar_usuario_clave(string db, string usuario, string clave)
        {
            try
            {
                var auto = new SqlAutoDAL();

                // Genera y ejecuta:
                // SELECT TOP 1 * FROM Vendedor
                // WHERE telefonoVendedor = N'{usuario}' AND calveVendedor = N'{clave}'
                var vendedor = await auto.ConsultarUno<Vendedor>(
                    db,
                    x => x.telefonoVendedor == usuario && x.calveVendedor == clave
                );

                return vendedor; // puede ser null si no encuentra
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}
