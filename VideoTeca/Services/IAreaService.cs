using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoTeca.Models;

namespace VideoTeca.Services
{
    public interface IAreaService
    {
        IEnumerable<area> GetAllAreas();
        IEnumerable<subarea> GetAllSubareas();
        IEnumerable<subarea> GetAllActiveSubareas();
        List<subarea> GetSubareasFromArea(long id);
        area GetAreaById(long areaId);
        IEnumerable<area> GetFilteredAreas(string search, string sort, string order, int page, int pageSize, out int totalItems);
        IEnumerable<subarea> GetFilteredSubareas(string search, string sort, string order, int page, int pageSize, out int totalItems);
        void CreateArea(string nome, List<long> subareas);
        void EditArea(long areaId, string nome, List<long> subareas);
        void ActivateArea(int areaId);
        void DeleteArea(int areaId);
        void CreateSubarea(string nome);
        void EditSubarea(long subareaId, string nome);
        void ActivateSubarea(int subareaId);
        void DeleteSubarea(int subareaId);
    }
}
