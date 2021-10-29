using System;
using System.Collections.Generic;
using TimeReportingSystem.Models;

namespace TimeReportingSystem.DAL{
    public interface IUserRepository : IDisposable
    {
        IEnumerable<User> GetUsers();
        User GetUserByuserName();
    }
}