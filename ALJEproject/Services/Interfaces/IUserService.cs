using ALJEproject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALJEproject.Services.Interfaces
{
    public interface IUserService
    {
        IEnumerable<UserRoleView> GetPaginatedUsers(int page, int pageSize);
        int GetTotalUsersCount();
        int GetTotalUsersCount(string search);
        void AddUser(User user);
        void UpdateUser(User user);
        User GetUserById(int id);
        bool DeleteUser(int id);
        IEnumerable<Role> GetRoles(); // To retrieve the list of roles
        List<UserRoleView> SearchUsers(string search, int page, int pageSize);
        Task<IEnumerable<UserRoleView>> SearchUsersAsync(string search, int page, int pageSize);
        Task<int> GetTotalUsersCountAsync(string search);
        Task<IEnumerable<UserRoleView>> GetPaginatedUsersAsync(int page, int pageSize);
        Task<int> GetTotalUsersCountAsync();
        //Role
        List<RoleView> SearchRoles(string search, int page, int pageSize);
        int GetTotalRolesCount(string search);
        IEnumerable<RoleView> GetPaginatedRoles(int page, int pageSize);
        int GetTotalRolesCount();

        // Option-related methods
        List<Option> SearchOptions(string search, int page, int pageSize);
        int GetTotalOptionsCount(string search);
        IEnumerable<Option> GetPaginatedOptions(int page, int pageSize);
        int GetTotalOptionsCount();

        // Menu-related methods
        List<Menu> SearchMenus(string search, int page, int pageSize);
        int GetTotalMenusCount(string search);
        IEnumerable<Menu> GetPaginatedMenus(int page, int pageSize);
        int GetTotalMenusCount();

        // UserAccess-related methods
        List<UserAccessView> SearchUserAccesses(string search, int page, int pageSize);
        int GetTotalUserAccessesCount(string search);
        IEnumerable<UserAccessView> GetPaginatedUserAccesses(int page, int pageSize);
        int GetTotalUserAccessesCount();

        //setting menu
        Task<List<Menu>> GetActiveMenusAsync();

    }

}
