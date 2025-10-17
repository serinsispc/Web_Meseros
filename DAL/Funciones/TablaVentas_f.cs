using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Funciones
{
    public class TablaVentas_f
    {
        public static int NuevaVenta()
        {
            try
            {
                Guid guid = Guid.NewGuid();
                TablaVentas tablaVentas = new TablaVentas {
                    id = 0,
                    aliasVenta="--",
                    fechaVenta = DateTime.Now,
                    numeroVenta = 0,
                    descuentoVenta = 0,
                    efectivoVenta = 0,
                    cambioVenta = 0,
                    estadoVenta = "PENDIENTE",
                    numeroReferenciaPago = "-",
                    diasCredito = 0,
                    observacionVenta = "-",
                    IdSede = 0,
                    guidVenta = guid,
                    abonoTarjeta=0,
                    propina=0,
                    abonoEfectivo=0,
                    idMedioDePago=10,
                    idResolucion=0,
                    idFormaDePago=1,
                    razonDescuento="-",
                    idBaseCaja=0,
                    eliminada=false
                };
                using (DBEntities cn = new DBEntities())
                {
                    cn.TablaVentas.Add(tablaVentas);
                    cn.SaveChanges();
                }
                //ahora consultamos el guid
                var tv = new TablaVentas();
                using(DBEntities cn = new DBEntities())
                {
                    tv = cn.TablaVentas.AsNoTracking().Where(x => x.guidVenta == guid).FirstOrDefault();
                }
                if(tv != null)
                {
                    return tv.id;
                }
                else
                {
                    return 0;
                }
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
    }
}
