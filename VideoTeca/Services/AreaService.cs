using System;
using System.Collections.Generic;
using System.Linq;
using VideoTeca.Models;

namespace VideoTeca.Services
{
    public class AreaService : IAreaService
    {
        private readonly dbContext _db;

        public AreaService(dbContext dbContext)
        {
            _db = dbContext;
        }

        public IEnumerable<area> GetAllAreas()
        {
            return _db.area.ToList();
        }

        public IEnumerable<subarea> GetAllSubareas()
        {
            return _db.subarea.ToList();
        }

        public IEnumerable<subarea> GetAllActiveSubareas()
        {
            return _db.subarea.Where(s => s.active == true).ToList();
        }

        public void CreateArea(string nome, List<long> subareas)
        {
            if (_db.area.Any(a => a.nome == nome))
                throw new Exception("Área já existe.");

            var newArea = new area { nome = nome, active = true };

            if (subareas != null && subareas.Count > 0)
            {
                var selectedSubareas = _db.subarea.Where(s => subareas.Contains(s.id)).ToList();
                foreach (var subarea in selectedSubareas)
                {
                    newArea.subarea.Add(subarea);
                }
            }

            _db.area.Add(newArea);
            _db.SaveChanges();
        }

        public void EditArea(long areaId, string nome, List<long> subareas)
        {
            var editArea = _db.area.Find(areaId);
            if (editArea == null) throw new Exception("Área não encontrada.");

            editArea.nome = nome;
            editArea.subarea.Clear();

            if (subareas != null && subareas.Count > 0)
            {
                var selectedSubareas = _db.subarea.Where(s => subareas.Contains(s.id)).ToList();
                foreach (var subarea in selectedSubareas)
                {
                    editArea.subarea.Add(subarea);
                }
            }

            _db.Entry(editArea).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
        }

        public void ActivateArea(int areaId)
        {
            var area = _db.area.Find(areaId);
            if (area == null) throw new Exception("Área não encontrada.");
            area.active = true;
            _db.SaveChanges();
        }

        public void DeleteArea(int areaId)
        {
            var area = _db.area.Find(areaId);
            if (area == null) throw new Exception("Área não encontrada.");
            area.active = false;
            _db.SaveChanges();
        }

        public void CreateSubarea(string nome)
        {
            if (_db.subarea.Any(s => s.nome == nome))
                throw new Exception("Subárea já existe.");

            var newSubarea = new subarea { nome = nome, active = true };
            _db.subarea.Add(newSubarea);
            _db.SaveChanges();
        }

        public void EditSubarea(long subareaId, string nome)
        {
            var editSubarea = _db.subarea.Find(subareaId);
            if (editSubarea == null) throw new Exception("Subárea não encontrada.");

            editSubarea.nome = nome;
            _db.Entry(editSubarea).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
        }

        public void ActivateSubarea(int subareaId)
        {
            var subarea = _db.subarea.Find(subareaId);
            if (subarea == null) throw new Exception("Subárea não encontrada.");
            subarea.active = true;
            _db.SaveChanges();
        }

        public void DeleteSubarea(int subareaId)
        {
            var subarea = _db.subarea.Find(subareaId);
            if (subarea == null) throw new Exception("Subárea não encontrada.");
            subarea.active = false;
            _db.SaveChanges();
        }
        public area GetAreaById(long areaId)
        {
            return _db.area.FirstOrDefault(a => a.id == areaId);
        }

        public List<subarea> GetSubareasFromArea(long id)
        {
            return _db.subarea.Where(v => v.area.Any(z => z.id == id)).ToList();
        }

        public IEnumerable<subarea> GetFilteredSubareas(string search, string sort, string order, int page, int pageSize, out int totalItems)
        {
            var query = _db.subarea.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.nome.ToLower().Contains(search.ToLower()));
            }

            switch (sort)
            {
                case "nome":
                    query = order == "asc" ? query.OrderBy(s => s.nome) : query.OrderByDescending(s => s.nome);
                    break;
                default:
                    query = query.OrderBy(s => s.nome);
                    break;
            }

            totalItems = query.Count();
            return query.Skip(page).Take(pageSize).ToList();
        }

        public IEnumerable<area> GetFilteredAreas(string search, string sort, string order, int page, int pageSize, out int totalItems)
        {
            var query = _db.area.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.nome.ToLower().Contains(search.ToLower()));
            }

            switch (sort)
            {
                case "nome":
                    query = order == "asc" ? query.OrderBy(a => a.nome) : query.OrderByDescending(a => a.nome);
                    break;
                default:
                    query = query.OrderBy(a => a.nome);
                    break;
            }

            totalItems = query.Count();
            return query.Skip(page).Take(pageSize).ToList();
        }

    }
}
