using ServerEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer.Controllers
{
    public record User(string Name, string Surname, string Login);

    public class UsersController : IController
    {
        public User[] Index()
        {
            return new User[]
            {
                new User("Ivan", "Ivanov", "Ivan212"),
                new User("Petr", "Petrov", "Petr333")
            };
        }

        public async Task<User[]> IndexAsync()
        {   
            return new User[]
            {
                new User("Ivan", "Ivanov", "Ivan212"),
                new User("Petr", "Petrov", "Petr333")
            };
        }
    }
}
