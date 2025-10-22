using DAL.Controler;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Funciones
{
    public class ComandImpresaa_f
    {
        public static Respuesta_DAL LiberarDetalle(int idDetalle)
        {
            try
            {
                var comanda =ComandImpresaaControler.CosultarIdDetalle(idDetalle);
                if (comanda != null)
                {
                    return  ComandImpresaaControler.CRUD(comanda,2);
                }
                else
                {
                    return new Respuesta_DAL();
                }
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return new Respuesta_DAL { mensaje = msg, data=null, estado=false };
            }
        }
    }
}
