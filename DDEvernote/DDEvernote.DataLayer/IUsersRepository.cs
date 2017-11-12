using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDEvernote.Model;

namespace DDEvernote.DataLayer
{
    public interface IUsersRepository
    {
        User Create(User user);
        User Get(Guid Id);
        User Update(User user);
        void Delete(Guid userId);
        IEnumerable<User> GetUsersBySharedNote(Guid noteId);
        bool IsExist(Guid userId);
        User Get(String userName);
        IEnumerable<User> GetUsers();
    }
}
