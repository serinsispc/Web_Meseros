using DAL.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Controler
{
    public class ImagenesControler
    {
        private readonly SqlAutoDAL _autoSql;
        private readonly CrudSpHelper _crud;

        public ImagenesControler()
        {
            _autoSql = new SqlAutoDAL();
            _crud = new CrudSpHelper();
        }

        // ======================
        // 1. CONSULTAR IMAGEN POR ID (SELECT)
        // ======================
        public async Task<Imagenes> ConsultarAsync(string db, Guid guid)
        {
            try
            {
                // Usamos SqlAutoDAL (select dinámico usando expresiones)
                return await _autoSql.ConsultarUno<Imagenes>(db, x => x.id == guid);
            }
            catch
            {
                return null;
            }
        }

        // ======================
        // 2. INSERTAR (SP CRUD_Imagenes)
        // ======================
        public async Task<Respuesta_DAL> InsertarAsync(string db, Imagenes img)
        {
            return await _crud.CrudAsync(db, img, 0); // 0 = INSERT
        }

        // ======================
        // 3. ACTUALIZAR
        // ======================
        public async Task<Respuesta_DAL> ActualizarAsync(string db, Imagenes img)
        {
            return await _crud.CrudAsync(db, img, 1); // 1 = UPDATE
        }

        // ======================
        // 4. ELIMINAR
        // ======================
        public async Task<Respuesta_DAL> EliminarAsync(string db, Guid guid)
        {
            var img = new Imagenes
            {
                id = guid
            };

            return await _crud.CrudAsync(db, img, 2); // 2 = DELETE
        }
    }
}
