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
        bool IsExist(Guid userId);
        void Delete(Guid userId);
        User Update(User user);
        User Get(Guid Id);
        IEnumerable<User> GetUsersBySharedNote(Guid noteId);
    }
}
