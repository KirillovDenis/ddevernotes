using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ddevernote.Model;

namespace ddevernote.DataLayer
{
    public interface ICategoriesRepository
    {
        Category Create(Guid userId, string categoryTitle);
        Category Get(Guid categoryId);
        Category UpdateTitle(Guid categoryId, string newTitle);
        void Delete(Guid categoryId);
        IEnumerable<Category> GetUserCategories(Guid userId);
    }
}
