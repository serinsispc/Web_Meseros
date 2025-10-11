using DAL.Controler;
using DAL.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Funciones
{
    public class DetalleVenta_f
    {
        public static Respuesta_DAL AgregarProducto(int idpresentacion,int cantidad,int idventa)
        {
            try
            {
                /* consultamos el producto con el id presentación */
                var producto = v_productoVentaControler.Consultar_idpresentacion(idpresentacion);
                if (producto != null)
                {
                    /* llamamos el objeto detalle venta */
                    var dv = new DetalleVenta
                    {
                        id = 0,
                        idVenta = idventa,
                        idPresentacion = idpresentacion,
                        nombreProducto = producto.nombreProducto,
                        costoUnidad = producto.costo_mas_impuesto,
                        precioVenta = producto.precioVenta,
                        estadoDetalle = 1,
                        ivaDetalle = producto.porcentaje_impuesto,
                        cantidadDetalle = cantidad,
                        codigoProducto = producto.codigoProducto,
                        observacion = "--",
                        guidDetalle = Guid.NewGuid(),
                        opciones = "--",
                        adiciones = "--",
                        impuesto_id = producto.impuesto_id
                    };
                    string data = JsonConvert.SerializeObject(dv);
                    /* llamamos el crud para guardar */
                    bool respCRUD = DetalleVentaControler.CRUD(dv,0);
                    if (respCRUD) 
                    {
                        return new Respuesta_DAL { data=data, estado=true, mensaje=$"Producto ({producto.nombreProducto}) agregado." };
                    }
                    else
                    {
                        return new Respuesta_DAL { data = data, estado = false, mensaje = $"Producto ({producto.nombreProducto}) no fue agregado." };
                    }
                }
                else
                {
                    return new Respuesta_DAL { data = null, estado = false, mensaje = "El producto no se encontro." };
                }
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return new Respuesta_DAL { data=null, estado=false, mensaje=msg };
            }
        }
    }
}
