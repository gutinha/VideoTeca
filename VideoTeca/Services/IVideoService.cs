using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoTeca.Models.ViewModels;
using VideoTeca.Models;

namespace VideoTeca.Services
{
    public interface IVideoService
    {
        VideoViewModel GetVideoById(int videoId);
        IEnumerable<VideoViewModel> GetVideos(string area, string subarea, string titulo, int page, int pageSize, out int totalVideos);
        IEnumerable<area> GetAllAreas();
        IEnumerable<subarea> GetSubAreasByAreaId(int areaId);
        IEnumerable<VideoViewModel> GetFilteredVideos(string search, string sort, string order, int? areaId, int? subAreaId, int page, int pageSize, out int totalItems);
        void CreateVideo(VideoViewModel videoViewModel, long userId);
        void EditVideo(VideoViewModel videoViewModel);
        void DeleteVideo(int videoId);
        void ActivateVideo(int videoId);
    }
}
