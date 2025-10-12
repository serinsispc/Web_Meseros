using System.Data.Entity.Core.EntityClient;

namespace DAL
{
    public static class ClassConexionDinamica
    {
        public static string db {  get; set; }
        public static string DBDinamica { get; private set; }

        public static void Conectar(string servidor, string dbBase)
        {
            var providerConn =
                $"Data Source={servidor};Initial Catalog={dbBase};" +
                "Persist Security Info=True;User ID=emilianop;Password=Ser1ns1s@2020*;" +
                "TrustServerCertificate=True;MultipleActiveResultSets=True;App=EntityFramework;";

            var conexionBuilder = new EntityConnectionStringBuilder
            {
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = providerConn,
                // Opción A (comodín)
                Metadata = "res://*/Model.Model.csdl|res://*/Model.Model.ssdl|res://*/Model.Model.msl"
                // Opción B (fijando el assembly del DAL):
                // Metadata = "res://DAL/Model.Model.csdl|res://DAL/Model.Model.ssdl|res://DAL/Model.Model.msl"
            };

            DBDinamica = conexionBuilder.ToString();
        }
    }
}
