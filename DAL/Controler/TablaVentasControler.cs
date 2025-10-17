using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class TablaVentasControler
    {
        public static Respuesta_DAL CRUD(TablaVentas venta, int boton)
        {
            try
            {
                using (var cn = new DBEntities())
                {
                    switch (boton)
                    {
                        case 0: // INSERT
                                // Opcional: validar que sea nuevo
                            if (venta.id > 0)
                                throw new InvalidOperationException("Para insertar, el id debe ser 0 (o no asignado).");

                            cn.TablaVentas.Add(venta);
                            break;

                        case 1: // UPDATE
                            if (venta.id <= 0)
                                throw new InvalidOperationException("Id requerido para actualizar.");

                            // Carga la que existe en este contexto
                            var existente = cn.TablaVentas.Find(venta.id);
                            if (existente == null)
                                throw new KeyNotFoundException($"No existe TablaVentas con id={venta.id}.");

                            // Copia TODOS los valores escalares de 'venta' sobre 'existente'
                            cn.Entry(existente).CurrentValues.SetValues(venta);

                            // Si tienes colecciones o nav props, actualízalas aquí según tu lógica.
                            // (evita adjuntar otra entidad con la misma PK)
                            break;

                        case 2: // DELETE
                            if (venta.id <= 0)
                                throw new InvalidOperationException("Id requerido para eliminar.");

                            // ¿Ya está trackeada?
                            var local = cn.TablaVentas.Local.FirstOrDefault(x => x.id == venta.id);
                            if (local != null)
                            {
                                // Ya está en el contexto: márcala Deleted
                                cn.Entry(local).State = System.Data.Entity.EntityState.Deleted;
                            }
                            else
                            {
                                // No está trackeada: adjunta stub y marca Deleted
                                var stub = new TablaVentas { id = venta.id };
                                cn.TablaVentas.Attach(stub);
                                cn.Entry(stub).State = System.Data.Entity.EntityState.Deleted;
                            }
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(boton), "Valor de botón inválido (0=Insert,1=Update,2=Delete).");
                    }

                    cn.SaveChanges();

                    // Para insert EF llena venta.id (identidad). Para update/delete ya lo tienes.
                    return new Respuesta_DAL
                    {
                        data = venta.id,
                        estado = true,
                        mensaje = "Proceso terminado con éxito"
                    };
                }
            }
            catch (Exception ex)
            {
                // Devuelve el mensaje base (más útil que el wrapper)
                return new Respuesta_DAL
                {
                    data = null,
                    estado = false,
                    mensaje = ex.GetBaseException().Message
                };
            }
        }

        public static Respuesta_DAL Consultar_Id(int id)
        {
            try
            {
                TablaVentas venta = new TablaVentas();
                using (DBEntities cn =new DBEntities())
                {
                    venta= cn.TablaVentas.AsNoTracking().Where(x => x.id == id).FirstOrDefault();                        
                }
                if (venta == null)
                {
                    return new Respuesta_DAL { data = null, estado = false, mensaje = "error" };
                }
                else
                {
                    return new Respuesta_DAL { data = venta, estado = true, mensaje = "ok" };
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return new Respuesta_DAL { data = null, estado = false, mensaje = error };
            }
        }
    }
}
