using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class MenuViewModels
    {
        public int IdCuentaActiva { get; set; }
        public int IdZonaActiva { get; set; }
        public int IdMesaActiva { get; set; }
        public int IdCategoriaActiva { get; set; }
        public int IdCuenteClienteActiva { get; set; }
        public List<V_Cuentas> cuentas { get; set; }
        public List<Zonas> zonas { get; set; }
        public List<Mesas> Mesas { get; set; }
        public List<V_Categoria> categorias { get; set; }
        public List<v_productoVenta> productos { get; set; }
        public V_TablaVentas venta { get; set; }
        public List<V_DetalleCaja> detalleCaja { get; set; }
        public List<V_CuentaCliente> v_CuentaClientes { get; set; }
    }
}