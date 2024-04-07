using Bogus;
using Domain.Data.Entities;
using Infrastraction.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppThread.Data;

namespace Infrastraction.Services
{
    public class UserService
    {
        private readonly DatabaseContext _dbContext;

        public event UserInsertItemDelegate InsertUserEvent;
        public UserService()
        {
            _dbContext = new DatabaseContext();
            _dbContext.Database.Migrate();
        }

        public void InsertRandomUser(int count)
        {
            var faker = new Faker<UserEntity>("uk")
                .RuleFor(u => u.LastName, f => f.Person.LastName)
                .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.Phone, f => f.Person.Phone);
            var users = faker.Generate(count);
            int i = 0;
            foreach (var user in users)
            {
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
                InsertUserEvent(++i);
            }
        }

    }
}
