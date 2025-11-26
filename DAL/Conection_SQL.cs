using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace DAL
{
    public class Conection_SQL : IDisposable
    {
        private SqlConnection _conexion;

        public Conection_SQL(string db)
        {
            ConexionBase(db);
            _conexion = new SqlConnection(connectionString);
            _conexion.Open();
        }

        public static string connectionString;
        public static void ConexionBase(string db)
        {
            connectionString = $"data source=www.serinsispc.com; initial catalog={db}; user id=emilianop; password=Ser1ns1s@2020*";
        }

        public async Task<string> EjecutarConsulta(string consulta, [Optional] bool lista_)
        {
            try
            {
                string respuesta = "Error";

                using (var cmd = new SqlCommand(consulta, _conexion))
                {
                    cmd.CommandType = CommandType.Text;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        var lista = new List<Dictionary<string, object>>();
                        foreach (DataRow row in dt.Rows)
                        {
                            var dict = new Dictionary<string, object>();
                            foreach (DataColumn col in dt.Columns)
                            {
                                dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                            }
                            lista.Add(dict);
                        }

                        respuesta = JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true });
                        if (lista_ == false)
                        {
                            respuesta = JsonSerializer.Serialize(lista.FirstOrDefault(), new JsonSerializerOptions { WriteIndented = true });
                        }
                    }
                }

                if (respuesta == "[]" || respuesta == "null")
                    return null;

                return respuesta;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public void Dispose()
        {
            if (_conexion != null)
            {
                _conexion.Dispose();
                _conexion = null;
            }
        }
    }
}
