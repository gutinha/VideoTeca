using System;
using System.Collections.Generic;
using System.Linq;
using VideoTeca.Models;
using VideoTeca.Models.ViewModels;

namespace VideoTeca.Services
{
    public class VideoService : IVideoService
    {
        private readonly dbContext _db;

        public VideoService(dbContext dbContext)
        {
            _db = dbContext;
        }

        public VideoViewModel GetVideoById(int videoId)
        {
            var video = _db.video.Find(videoId);
            if (video == null) return null;

            return new VideoViewModel
            {
                Id = video.id,
                Titulo = video.titulo,
                EnviadoEm = video.enviadoEm,
                AreaNome = video.area?.nome,
                SubareaNome = video.subarea?.nome,
                Aprovado = video.aprovado,
                Active = video.active
            };
        }

        public IEnumerable<VideoViewModel> GetVideos(string area, string subarea, string titulo, int page, int pageSize, out int totalVideos)
        {
            var query = _db.video.Where(x => x.active == true && x.aprovado == true);

            if (!string.IsNullOrEmpty(area))
            {
                int areaId = int.Parse(area);
                query = query.Where(x => x.area.id == areaId);
            }

            if (!string.IsNullOrEmpty(subarea))
            {
                int subareaId = int.Parse(subarea);
                query = query.Where(x => x.subarea.id == subareaId);
            }

            if (!string.IsNullOrEmpty(titulo))
            {
                query = query.Where(x => x.titulo.Contains(titulo));
            }

            totalVideos = query.Count();

            return query.OrderByDescending(x => x.enviadoEm)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(video => new VideoViewModel
                        {
                            Id = video.id,
                            Titulo = video.titulo,
                            EnviadoEm = video.enviadoEm,
                            AreaNome = video.area.nome,
                            SubareaNome = video.subarea.nome,
                            Aprovado = video.aprovado,
                            Active = video.active
                        })
                        .ToList();
        }

        public IEnumerable<area> GetAllAreas()
        {
            return _db.area.ToList();
        }

        public IEnumerable<subarea> GetSubAreasByAreaId(int areaId)
        {
            return _db.area
                      .Where(a => a.id == areaId)
                      .SelectMany(a => a.subarea)
                      .ToList();
        }

        public IEnumerable<VideoViewModel> GetFilteredVideos(string search, string sort, string order, int? areaId, int? subareaId, int page, int pageSize, out int totalItems)
        {
            var query = _db.video.AsQueryable();

            // Filtro por área
            if (areaId.HasValue && areaId != 0)
            {
                query = query.Where(v => v.id_area == areaId);
            }

            // Filtro por subárea
            if (subareaId.HasValue && subareaId != 0)
            {
                query = query.Where(v => v.id_subarea == subareaId);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(v => v.titulo.ToLower().Contains(search.ToLower()) ||
                                          v.status.nome.Contains(search.ToLower()) ||
                                          v.area.nome.Contains(search.ToLower()) ||
                                          v.subarea.nome.Contains(search.ToLower()));
            }

            switch (sort)
            {
                case "titulo":
                    query = order == "asc" ? query.OrderBy(v => v.titulo) : query.OrderByDescending(v => v.titulo);
                    break;
                case "id_status":
                    query = order == "asc" ? query.OrderBy(v => v.status.nome) : query.OrderByDescending(v => v.status.nome);
                    break;
                case "id_area":
                    query = order == "asc" ? query.OrderBy(v => v.area.nome) : query.OrderByDescending(v => v.area.nome);
                    break;
                case "id_subarea":
                    query = order == "asc" ? query.OrderBy(v => v.subarea.nome) : query.OrderByDescending(v => v.subarea.nome);
                    break;
                default:
                    query = query.OrderBy(v => v.titulo);
                    break;
            }

            totalItems = query.Count();
            return query.Skip(page).Take(pageSize).Select(v => new VideoViewModel
            {
                Id = v.id,
                Titulo = v.titulo,
                Active = v.active,
                StatusNome = v.status.nome,
                AreaNome = v.area.nome,
                SubareaNome = v.subarea != null ? v.subarea.nome : "Nenhum"
            }).ToList();
        }


        public void CreateVideo(VideoViewModel videoViewModel, long userId)
        {
            var video = new video
            {
                titulo = videoViewModel.Titulo,
                descricao = videoViewModel.Descricao,
                url = videoViewModel.Url,
                id_area = videoViewModel.AreaId,
                id_subarea = videoViewModel.SubareaId,
                enviadoPor = userId,
                active = true,
                id_status = 0,
                aprovado = false,
                enviadoEm = DateTime.Now
            };

            _db.video.Add(video);
            _db.SaveChanges();
        }

        public void EditVideo(VideoViewModel videoViewModel)
        {
            var video = _db.video.Find(videoViewModel.Id);
            if (video == null) throw new Exception("Video not found.");

            video.titulo = videoViewModel.Titulo;
            video.descricao = videoViewModel.Descricao;
            video.url = videoViewModel.Url;
            video.id_area = videoViewModel.AreaId;
            video.id_subarea = videoViewModel.SubareaId;

            _db.Entry(video).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
        }

        public void DeleteVideo(int videoId)
        {
            var video = _db.video.Find(videoId);
            if (video != null)
            {
                video.active = false;
                _db.Entry(video).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
        }

        public void ActivateVideo(int videoId)
        {
            var video = _db.video.Find(videoId);
            if (video != null)
            {
                video.active = true;
                _db.Entry(video).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
        }
    }
}
