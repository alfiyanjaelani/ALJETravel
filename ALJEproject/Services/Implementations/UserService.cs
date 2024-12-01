using ALJEproject.Models;
using ALJEproject.Services.Interfaces;
using ALJEproject.Data; // Ensure this namespace is correct
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ALJEproject.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ALJEprojectDbContext _context;

        public UserService(ALJEprojectDbContext context)
        {
            _context = context;
        }

        public IEnumerable<UserRoleView> GetPaginatedUsers(int page, int pageSize)
        {
            return _context.UserRoles
                .OrderBy(u => u.UserId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserRoleView
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    FullName = u.FullName,
                    RoleName = u.RoleName,
                    CompanyName = u.CompanyName,
                    EmailAddress = u.EmailAddress,
                    Phone = u.Phone,
                    CreatedBy = u.CreatedBy,
                    CreatedDate = u.CreatedDate,
                    UpdatedBy = u.UpdatedBy,
                    UpdatedDate = u.UpdatedDate
                })
                .ToList();
        }

        public List<UserRoleView> SearchUsers(string search, int page, int pageSize)
        {
            // Start building the query
            var query = _context.UserRoles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower(); // Convert the search term to lowercase

                query = query.Where(u =>  u.UserId.ToString().ToLower().Contains(searchLower) ||
                                          u.UserName.ToLower().Contains(searchLower) ||                                          
                                          u.RoleName.ToLower().Contains(searchLower) ||
                                          u.CompanyName.ToLower().Contains(searchLower));
            }

            var totalUsersCount = query.Count();

            // Perform paging and select the desired fields
            var users = query.OrderBy(u => u.UserId)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .Select(u => new UserRoleView
                             {
                                 UserId = u.UserId,
                                 UserName = u.UserName,
                                 FullName = u.FullName,
                                 RoleName = u.RoleName,
                                 CompanyName = u.CompanyName,
                                 EmailAddress = u.EmailAddress,
                                 Phone = u.Phone,
                                 CreatedBy = u.CreatedBy,
                                 CreatedDate = u.CreatedDate,
                                 UpdatedBy = u.UpdatedBy,
                                 UpdatedDate = u.UpdatedDate
                             })
                             .ToList(); // Execute the query

            return users; // Return the list of users
        }

        public int GetTotalUsersCount(string search = null) // New method for total count based on search
        {
            var query = _context.UserRoles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower(); // Convert the search term to lowercase

                query = query.Where(u => u.UserName.ToLower().Contains(searchLower) ||
                                          u.FullName.ToLower().Contains(searchLower) ||
                                          u.EmailAddress.ToLower().Contains(searchLower) ||
                                          u.Phone.ToLower().Contains(searchLower) ||
                                          u.CompanyName.ToLower().Contains(searchLower));
            }

            return query.Count(); // Returns the total count based on the current query
        }

        public int GetTotalUsersCount()
        {
            return _context.Users.Count();
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public User GetUserById(int id)
        {
            return _context.Users.Find(id);
        }

        public bool DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public IEnumerable<Role> GetRoles()
        {
            return _context.Roles.ToList(); // Assuming you have a Roles DbSet
        }

        public async Task<IEnumerable<UserRoleView>> SearchUsersAsync(string search, int page, int pageSize)
        {
            return await _context.UserRoles
                .Where(u => u.UserName.Contains(search) || u.FullName.Contains(search))
                .OrderBy(u => u.UserId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserRoleView
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    FullName = u.FullName,
                    RoleName = u.RoleName, // Asumsi Anda memiliki relasi antara pengguna dan peran
                CompanyName = u.CompanyName, // Asumsi Anda memiliki relasi antara pengguna dan perusahaan
                EmailAddress = u.EmailAddress,
                    Phone = u.Phone,
                    CreatedBy = u.CreatedBy,
                    CreatedDate = u.CreatedDate,
                    UpdatedBy = u.UpdatedBy,
                    UpdatedDate = u.UpdatedDate
                })
                .ToListAsync();
        }

        public async Task<int> GetTotalUsersCountAsync(string search)
        {
            return await _context.UserRoles
                .Where(u => u.UserName.Contains(search) || u.FullName.Contains(search))
                .CountAsync();
        }

        public async Task<IEnumerable<UserRoleView>> GetPaginatedUsersAsync(int page, int pageSize)
        {
            return await _context.UserRoles
                .OrderBy(u => u.UserId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserRoleView
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    FullName = u.FullName,
                    RoleName = u.RoleName, // Asumsi Anda memiliki relasi antara pengguna dan peran
                CompanyName = u.CompanyName, // Asumsi Anda memiliki relasi antara pengguna dan perusahaan
                EmailAddress = u.EmailAddress,
                    Phone = u.Phone,
                    CreatedBy = u.CreatedBy,
                    CreatedDate = u.CreatedDate,
                    UpdatedBy = u.UpdatedBy,
                    UpdatedDate = u.UpdatedDate
                })
                .ToListAsync();
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _context.UserRoles.CountAsync();
        }

        //role
        public List<RoleView> SearchRoles(string search, int page, int pageSize)
        {
            // Start building the query
            var query = _context.Roles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower(); // Convert the search term to lowercase

                query = query.Where(u => u.RoleID.ToString().ToLower().Contains(searchLower) ||
                                          u.RoleName.ToLower().Contains(searchLower) ||
                                          u.CreatedBy.ToLower().Contains(searchLower));
            }

            var totalRolesCount = query.Count();

            // Perform paging and select the desired fields
            var roles = query.OrderBy(u => u.RoleID)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .Select(u => new RoleView
                             {
                                 RoleID = u.RoleID,
                                 RoleName = u.RoleName,                            
                                 CreatedBy = u.CreatedBy,
                                 CreatedDate = u.CreatedDate,
                                 UpdatedBy = u.UpdatedBy,
                                 UpdatedDate = u.UpdatedDate
                             })
                             .ToList(); // Execute the query

            return roles; // Return the list of users
        }

        public int GetTotalRolesCount(string search = null) // New method for total count based on search
        {
            var query = _context.Roles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower(); // Convert the search term to lowercase

                query = query.Where(u => u.RoleID.ToString().ToLower().Contains(searchLower) ||
                             u.RoleName.ToLower().Contains(searchLower) ||
                             u.CreatedBy.ToLower().Contains(searchLower));
            }

            return query.Count(); // Returns the total count based on the current query
        }

        public IEnumerable<RoleView> GetPaginatedRoles(int page, int pageSize)
        {
            return _context.Roles
                .OrderBy(u => u.RoleID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new RoleView
                {
                    RoleID = u.RoleID,
                    RoleName = u.RoleName,                    
                    CreatedBy = u.CreatedBy,
                    CreatedDate = u.CreatedDate,
                    UpdatedBy = u.UpdatedBy,
                    UpdatedDate = u.UpdatedDate
                })
                .ToList();
        }

        public int GetTotalRolesCount()
        {
            return _context.Roles.Count();
        }

        // Option-related methods in UserService
        public List<Option> SearchOptions(string search, int page, int pageSize)
        {
            // Start building the query
            var query = _context.Options.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower(); // Convert the search term to lowercase

                query = query.Where(o => o.OptionsID.ToString().ToLower().Contains(searchLower) ||
                                         o.FieldName.ToLower().Contains(searchLower) ||
                                         o.ShortName.ToLower().Contains(searchLower) ||
                                         o.FieldValue.ToLower().Contains(searchLower));

            }

            // Perform paging and select the desired fields
            var options = query.OrderBy(o => o.OptionsID)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .Select(o => new Option
                               {
                                   OptionsID = o.OptionsID,
                                   FieldName = o.FieldName,
                                   FieldValue = o.FieldValue,
                                   LongName = o.LongName,
                                   ShortName = o.ShortName,
                                   CreatedBy = o.CreatedBy,
                                   CreatedDate = o.CreatedDate,
                                   UpdatedBy = o.UpdatedBy,
                                   UpdatedDate = o.UpdatedDate
                               })
                               .ToList();

            return options; // Return the list of options
        }

        public int GetTotalOptionsCount(string search = null)
        {
            var query = _context.Options.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();

                query = query.Where(o => o.OptionsID.ToString().ToLower().Contains(searchLower) ||
                                         o.FieldName.ToLower().Contains(searchLower) ||
                                         o.ShortName.ToLower().Contains(searchLower) ||
                                         o.FieldValue.ToLower().Contains(searchLower));
            }

            return query.Count(); // Returns the total count based on the current query
        }

        public IEnumerable<Option> GetPaginatedOptions(int page, int pageSize)
        {
            return _context.Options
                .OrderBy(o => o.OptionsID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new Option
                {
                    OptionsID = o.OptionsID,
                    FieldName = o.FieldName,
                    FieldValue = o.FieldValue,
                    LongName = o.LongName,
                    ShortName = o.ShortName,
                    CreatedBy = o.CreatedBy,
                    CreatedDate = o.CreatedDate,
                    UpdatedBy = o.UpdatedBy,
                    UpdatedDate = o.UpdatedDate
                })
                .ToList();
        }

        public int GetTotalOptionsCount()
        {
            return _context.Options.Count();
        }

        // Menu-related methods in UserService
        public List<Menu> SearchMenus(string search, int page, int pageSize)
        {
            var query = _context.Menus.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();

                query = query.Where(m => m.MenuID.ToString().ToLower().Contains(searchLower) ||
                                         m.ControllerName.ToLower().Contains(searchLower) ||
                                         m.MenuName.ToLower().Contains(searchLower) ||
                                         m.MenuDesc.ToLower().Contains(searchLower));
            }

            var menus = query.OrderBy(m => m.MenuID)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .Select(m => new Menu
                             {
                                 MenuID = m.MenuID,
                                 ControllerName = m.ControllerName,
                                 MenuName = m.MenuName,
                                 MenuDesc = m.MenuDesc,
                                 CreatedBy = m.CreatedBy,
                                 CreatedDate = m.CreatedDate,
                                 UpdatedBy = m.UpdatedBy,
                                 UpdatedDate = m.UpdatedDate,
                                 Active = m.Active,
                                 MenuURL = m.MenuURL,
                                 MenuOrder = m.MenuOrder
                             })
                             .ToList();

            return menus;
        }

        public int GetTotalMenusCount(string search = null)
        {
            var query = _context.Menus.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();

                query = query.Where(m => m.MenuID.ToString().ToLower().Contains(searchLower) ||
                                         m.ControllerName.ToLower().Contains(searchLower) ||
                                         m.MenuName.ToLower().Contains(searchLower) ||
                                         m.MenuDesc.ToLower().Contains(searchLower));
            }

            return query.Count();
        }

        public IEnumerable<Menu> GetPaginatedMenus(int page, int pageSize)
        {
            return _context.Menus
                .OrderBy(m => m.MenuID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new Menu
                {
                    MenuID = m.MenuID,
                    ControllerName = m.ControllerName,
                    MenuName = m.MenuName,
                    MenuDesc = m.MenuDesc,
                    CreatedBy = m.CreatedBy,
                    CreatedDate = m.CreatedDate,
                    UpdatedBy = m.UpdatedBy,
                    UpdatedDate = m.UpdatedDate,
                    Active = m.Active,
                    MenuURL = m.MenuURL,
                    MenuOrder = m.MenuOrder
                })
                .ToList();
        }

        public int GetTotalMenusCount()
        {
            return _context.Menus.Count();
        }

        //UserAccess
        public List<UserAccessView> SearchUserAccesses(string search, int page, int pageSize)
        {
            var query = _context.UserAccessesView.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();

                query = query.Where(ua => ua.UserAccessID.ToString().ToLower().Contains(searchLower) ||
                                          ua.RoleName.ToLower().Contains(searchLower) ||
                                          ua.MenuName.ToLower().Contains(searchLower) ||
                                          ua.CreatedBy.ToLower().Contains(searchLower));
            }

            var userAccesses = query.OrderBy(ua => ua.UserAccessID)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .Select(ua => new UserAccessView
                                     {
                                         UserAccessID = ua.UserAccessID,
                                         RoleID = ua.RoleID,
                                         RoleName = ua.RoleName,
                                         MenuID = ua.MenuID,
                                         MenuName = ua.MenuName,
                                         Views = ua.Views,
                                         Inserts = ua.Inserts,
                                         Edits = ua.Edits,
                                         Deletes = ua.Deletes,
                                         CreatedBy = ua.CreatedBy,
                                         CreatedDate = ua.CreatedDate,
                                         UpdatedBy = ua.UpdatedBy,
                                         UpdatedDate = ua.UpdatedDate
                                     })
                                     .ToList();

            return userAccesses;
        }

        public int GetTotalUserAccessesCount(string search = null)
        {
            var query = _context.UserAccessesView.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();

                query = query.Where(ua => ua.UserAccessID.ToString().ToLower().Contains(searchLower) ||
                                          ua.RoleName.ToLower().Contains(searchLower) ||
                                          ua.MenuName.ToLower().Contains(searchLower) ||
                                          ua.CreatedBy.ToLower().Contains(searchLower));
            }

            return query.Count();
        }

        public IEnumerable<UserAccessView> GetPaginatedUserAccesses(int page, int pageSize)
        {
            return _context.UserAccessesView
                .OrderBy(ua => ua.UserAccessID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ua => new UserAccessView
                {
                    UserAccessID = ua.UserAccessID,
                    RoleID = ua.RoleID,
                    RoleName = ua.RoleName,
                    MenuID = ua.MenuID,
                    MenuName = ua.MenuName,
                    Views = ua.Views,
                    Inserts = ua.Inserts,
                    Edits = ua.Edits,
                    Deletes = ua.Deletes,
                    CreatedBy = ua.CreatedBy,
                    CreatedDate = ua.CreatedDate,
                    UpdatedBy = ua.UpdatedBy,
                    UpdatedDate = ua.UpdatedDate
                })
                .ToList();
        }

        public int GetTotalUserAccessesCount()
        {
            return _context.UserAccesses.Count();
        }

        public async Task<List<Menu>> GetActiveMenusAsync()
        {
            return await _context.Menus
         .Where(m => m.Active && m.ParentMenuID == null) // Ambil root menu (tidak memiliki parent)
         .Include(m => m.SubMenus) // Sertakan submenu
         .OrderBy(m => m.MenuOrder)
         .ToListAsync();
        }
    }
}
