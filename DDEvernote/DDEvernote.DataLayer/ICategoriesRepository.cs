using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDEvernote.Model;

namespace DDEvernote.DataLayer
{
    public interface ICategoriesRepository
    {
        Category Create(Guid userId, string categoryTitle);
        Category Get(Guid categoryId);
        Category Update(Category category);
        void Delete(Guid categoryId);
        IEnumerable<Category> GetUserCategories(Guid userId);
        IEnumerable<Category> GetCategoriesOfNote(Guid noteId);
        bool IsExist(Guid categoryId);
    }
}
