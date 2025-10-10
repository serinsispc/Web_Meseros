using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class MenuViewModels
    {
        public List<V_Cuentas> cuentas {  get; set; }
        public List<Mesas> Mesas {  get; set; }
    }
}