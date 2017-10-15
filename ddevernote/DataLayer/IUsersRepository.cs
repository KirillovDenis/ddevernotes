using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ddevernote.Model;

namespace ddevernote.DataLayer
{
    public interface IUsersRepository
    {
        User Create(User user);
        void Delete(Guid userId);
        User Update(User user);
        User Get(Guid Id);
        IEnumerable<User> GetUsersBySharedNote(Guid noteId);
    }
}
