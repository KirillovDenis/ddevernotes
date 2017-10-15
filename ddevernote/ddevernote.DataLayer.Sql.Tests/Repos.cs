using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ddevernote.Model;


namespace ddevernote.DataLayer.Sql.Tests
{
    class Repos
    {
        public INotesRepository NoteRepository { get; set; }
        public ICategoriesRepository CategoryRepository { get; set; }

        public Repos(string connectionString)
        {
            
        }
    }
}
