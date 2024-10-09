using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoTeca.Models.ViewModels;

namespace VideoTeca.Services
{
    public interface IAvaliacaoService
    {
        bool LockVideo(int videoId, string userName);
        bool IsVideoLockedByAnotherUser(int videoId, string userName);
        void SaveAvaliacao(long userId, long videoId, string justificativa);
        IEnumerable<VideoViewModel> GetVideosToReview(long userId, string userName, int? areaId, int? subAreaId, string search, string sort, string order, int page, int pageSize, out int totalItems);
    }
}
