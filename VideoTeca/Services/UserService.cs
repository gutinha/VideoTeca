using System;
using System.Collections.Generic;
using System.Linq;
using VideoTeca.Models;
using VideoTeca.Services;

public class UserService : IUserService
{
    private readonly dbContext _db;

    public UserService(dbContext dbContext)
    {
        _db = dbContext;
    }

    public usuarioDTO DecryptUser(string encryptedData)
    {
        return Util.Decrypt(encryptedData);
    }

    public usuario GetUserByEmail(string email)
    {
        return _db.usuario.FirstOrDefault(u => u.email.Equals(email));
    }

    public bool GetUserTerms(long id)
    {
        return _db.usuario.Where(u => u.id == id).Select(x => x.accept_terms).First();
    }

    public bool ValidateUser(string email, string password)
    {
        string hashedPassword = Util.hash(password);
        return _db.usuario.Any(u => u.email.Equals(email) && u.password.Equals(hashedPassword));
    }

    public void CreateUser(string name, string email, string password)
    {
        using (var transaction = _db.Database.BeginTransaction())
        {
            try
            {
                usuario user = new usuario
                {
                    nome = name,
                    email = email,
                    password = Util.hash(password),
                    permission = 1,
                    active = true
                };

                _db.usuario.Add(user);
                _db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    public bool IsUserExists(string email)
    {
        return _db.usuario.Any(u => u.email.Equals(email));
    }

    public List<usuario> GetAllUsers()
    {
        return _db.usuario.ToList();
    }

    public IEnumerable<usuario> GetFilteredUsers(string search, string sort, string order, int? grupo, int page, int pageSize, out int totalItems)
    {
        var query = _db.usuario.AsQueryable();

        // Filtro por grupo
        if (grupo.HasValue && grupo != 0)
        {
            query = query.Where(u => u.permission == grupo);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.nome.ToLower().Contains(search.ToLower()) || u.email.Contains(search.ToLower()));
        }

        switch (sort)
        {
            case "nome":
                query = order == "asc" ? query.OrderBy(u => u.nome) : query.OrderByDescending(u => u.nome);
                break;
            case "email":
                query = order == "asc" ? query.OrderBy(u => u.email) : query.OrderByDescending(u => u.email);
                break;
            case "permission":
                query = order == "asc" ? query.OrderBy(u => u.permission) : query.OrderByDescending(u => u.permission);
                break;
            default:
                query = query.OrderBy(u => u.nome);
                break;
        }

        totalItems = query.Count();
        return query.Skip(page).Take(pageSize).ToList();
    }

    public void DeactivateUser(int userId)
    {
        var user = _db.usuario.Find(userId);
        if (user != null)
        {
            user.active = false;
            _db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
        }
    }

    public void ActivateUser(int userId)
    {
        var user = _db.usuario.Find(userId);
        if (user != null)
        {
            user.active = true;
            _db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
        }
    }

    public void EditUser(long userId, long permission, List<long> areaIds)
    {
        var user = _db.usuario.Find(userId);
        if (user == null)
        {
            throw new Exception("Usuário não encontrado.");
        }

        user.permission = permission;

        // Limpa todas as áreas pertencentes ao usuário
        user.area.Clear();

        if (areaIds != null && areaIds.Count > 0)
        {
            var selectedAreas = _db.area.Where(a => areaIds.Contains(a.id)).ToList();
            foreach (var area in selectedAreas)
            {
                user.area.Add(area);
            }
        }

        _db.Entry(user).State = System.Data.Entity.EntityState.Modified;
        _db.SaveChanges();
    }

    public usuario GetUserById(long id)
    {
        return _db.usuario.Find(id);
    }
}
