using DAL;
using DAL.Model;
using System;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class SedeControler
    {
        public static async Task<Sede> Consultar(string db)
        {
            try
            {
                var auto = new SqlAutoDAL();

                // SELECT TOP 1 * FROM Sede
                var sede = await auto.ConsultarUno<Sede>(db, x => true);

                return sede;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
    }
}
