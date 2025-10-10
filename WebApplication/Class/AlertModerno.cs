using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace WebApplication.Class
{
    /// <summary>
    /// Clase estática para mostrar alertas modernas (SweetAlert2) en ASP.NET WebForms.
    /// </summary>
    public static class AlertModerno
    {
        // =====================================================
        // ALERTAS BÁSICAS (centradas por defecto)
        // =====================================================

        public static void Success(Page page, string titulo, string mensaje, bool esToast = false, int ms = 3000, string posicion = "center")
        {
            Show(page, "success", titulo, mensaje, ms, posicion, esToast);
        }

        public static void Error(Page page, string titulo, string mensaje, bool esToast = false, int ms = 3000, string posicion = "center")
        {
            Show(page, "error", titulo, mensaje, ms, posicion, esToast);
        }

        public static void Warning(Page page, string titulo, string mensaje, bool esToast = false, int ms = 3000, string posicion = "center")
        {
            Show(page, "warning", titulo, mensaje, ms, posicion, esToast);
        }

        public static void Info(Page page, string titulo, string mensaje, bool esToast = false, int ms = 3000, string posicion = "center")
        {
            Show(page, "info", titulo, mensaje, ms, posicion, esToast);
        }

        // =====================================================
        // CONFIRMACIONES
        // =====================================================
        public static void Confirm(Page page, string titulo, string mensaje, string jsCallback, string posicion = "center")
        {
            string script = $@"
Swal.fire({{
    title: '{Escape(titulo)}',
    text: '{Escape(mensaje)}',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Sí, continuar',
    cancelButtonText: 'Cancelar',
    confirmButtonColor: '#3085d6',
    cancelButtonColor: '#d33',
    position: '{posicion}'
}}).then((result) => {{
    if (result.isConfirmed) {{
        {jsCallback}
    }}
}});
";
            ScriptManager.RegisterStartupScript(page, page.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        // =====================================================
        // MÉTODO BASE GENERAL
        // =====================================================
        private static void Show(Page page, string tipo, string titulo, string mensaje, int ms, string posicion, bool esToast)
        {
            // construye la llamada JS al método _toast
            string script = $"AlertModerno._toast('{tipo}', '{Escape(titulo)}', '{Escape(mensaje)}', {ms}, '{posicion}', {(esToast.ToString().ToLower())});";
            ScriptManager.RegisterStartupScript(page, page.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        // =====================================================
        // UTILIDADES
        // =====================================================
        private static string Escape(string s)
        {
            return (s ?? "")
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\r", "")
                .Replace("\n", " ");
        }
    }
}