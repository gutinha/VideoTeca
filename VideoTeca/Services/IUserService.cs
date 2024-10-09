using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoTeca.Models;

namespace VideoTeca.Services
{
    public interface IUserService
    {
        usuarioDTO DecryptUser(string encryptedData);
        usuario GetUserByEmail(string email);
        usuario GetUserById(long id);
        bool ValidateUser(string email, string password);
        void CreateUser(string name, string email, string password);
        void DeactivateUser(int userId);
        void ActivateUser(int userId);
        bool IsUserExists(string email);
        bool GetUserTerms(long id);
        List<usuario> GetAllUsers();
        IEnumerable<usuario> GetFilteredUsers(string search, string sort, string order, int? grupo, int page, int pageSize, out int totalItems);
        void EditUser(long userId, long permission, List<long> areaIds);
    }
}
