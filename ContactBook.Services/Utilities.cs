using ContactBook.Data;
using ContactBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactBook.Services
{
    public class Utilities
    {
        private readonly ContactBookContext _ctx;

        public Utilities(ContactBookContext context)
        {
            _ctx = context;
        }
        public List<AppUser> GetAllUsers(PaginParameter usersParameter)
        {
            var contacts = _ctx.Users.OrderBy(on => on.FirstName)
                    .Skip((usersParameter.PageNumber - 1) * usersParameter.PageSize)
                    .Take(usersParameter.PageSize)
                    .ToList();
            return contacts;
        }
    }
    
}
