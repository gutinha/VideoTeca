using System;
using System.Collections.Generic;
using System.Linq;
using VideoTeca.Models;
using VideoTeca.Models.ViewModels;

namespace VideoTeca.Services
{
    public class AvaliacaoService : IAvaliacaoService
    {
        private readonly dbContext _db;

        public AvaliacaoService(dbContext dbContext)
        {
            _db = dbContext;
        }

        public bool LockVideo(int videoId, string userName)
        {
            var video = _db.video.Find(videoId);
            if (video == null) return false;

            video.locked = true;
            video.lockedBy = userName;
            video.lockedExpiresAt = DateTime.Now.AddMinutes(60);
            _db.SaveChanges();
            return true;
        }

        public bool IsVideoLockedByAnotherUser(int videoId, string userName)
        {
            var video = _db.video.Find(videoId);
            return video.locked && video.lockedExpiresAt > DateTime.Now && video.lockedBy != userName;
        }

        public void SaveAvaliacao(long userId, long videoId, string justificativa)
        {
            var video = _db.video.Find(videoId);
            if (video == null) throw new Exception("Video not found.");

            if (!string.IsNullOrEmpty(justificativa))
            {
                var novaAvaliacao = new video_avaliacoes
                {
                    id_avaliador = userId,
                    id_video = videoId,
                    justificativa = justificativa,
                    data_avaliacao = DateTime.Now
                };
                _db.video_avaliacoes.Add(novaAvaliacao);
                video.id_status = 1;
            }
            else
            {
                video.aprovado = true;
                video.id_status = 2;
            }

            _db.Entry(video).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
        }

        public IEnumerable<VideoViewModel> GetVideosToReview(long userId, string userName, int? areaId, int? subAreaId, string search, string sort, string order, int page, int pageSize, out int totalItems)
        {
            var query = _db.video.Where(v =>
                v.active &&
                v.area.usuario.Any(u => u.id == userId) &&
                !v.aprovado &&
                (!v.locked || v.lockedExpiresAt <= DateTime.Now || v.lockedBy == userName) &&
                (!v.video_avaliacoes.Any() || v.video_avaliacoes.Any(a => a.justificativa == null))
            );

            if (areaId.HasValue && areaId != 0)
            {
                query = query.Where(v => v.id_area == areaId);
            }

            if (subAreaId.HasValue && subAreaId != 0)
            {
                query = query.Where(v => v.id_subarea == subAreaId);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(v => v.titulo.Contains(search) ||
                                         v.status.nome.Contains(search) ||
                                         v.area.nome.Contains(search) ||
                                         (v.subarea != null && v.subarea.nome.Contains(search)));
            }

            switch (sort)
            {
                case "titulo":
                    query = order == "asc" ? query.OrderBy(x => x.titulo) : query.OrderByDescending(x => x.titulo);
                    break;
                case "id_status":
                    query = order == "asc" ? query.OrderBy(x => x.status.nome) : query.OrderByDescending(x => x.status.nome);
                    break;
                case "id_area":
                    query = order == "asc" ? query.OrderBy(x => x.area.nome) : query.OrderByDescending(x => x.area.nome);
                    break;
                case "id_subarea":
                    query = order == "asc" ? query.OrderBy(x => x.subarea.nome) : query.OrderByDescending(x => x.subarea.nome);
                    break;
                default:
                    query = query.OrderBy(x => x.titulo);
                    break;
            }

            totalItems = query.Count();

            return query.Skip(page).Take(pageSize).Select(x => new VideoViewModel
            {
                Id = x.id,
                Titulo = x.titulo,
                AreaNome = x.area.nome,
                SubareaNome = x.subarea != null ? x.subarea.nome : "Nenhum",
                Aprovado = x.aprovado,
                Active = x.active
            }).ToList();
        }
    }
}
