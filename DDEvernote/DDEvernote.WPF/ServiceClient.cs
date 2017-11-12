using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using DDEvernote.Model;
using System.Net.Http.Headers;

namespace DDEvernote.WPF
{
    public class ServiceClient
    {
        private readonly HttpClient _client;

        public ServiceClient(string connectionString)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(connectionString);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public User CreateUser(User user)
        {
            user = _client.PostAsJsonAsync("users", user).Result.Content.ReadAsAsync<User>().Result;
            return user;
        }

        public User UpdateUser(User user)
        {
            return _client.PutAsJsonAsync("users", user).Result.Content.ReadAsAsync<User>().Result;
        }

        public bool IsExistUserByName(string userName)
        {
            var tmp = _client.GetAsync($"users/name/{userName}").Result.Content.ReadAsAsync<User>().Result;
            return tmp.Name == null ? false : true;
        }


        public User GetUser(Guid userId)
        {
            return _client.GetAsync($"users/{userId}").Result.Content.ReadAsAsync<User>().Result;
        }

        public User GetUserByName(String userName)
        {
            var user = new User();
            user = _client.GetAsync($"users/name/{userName}").Result.Content.ReadAsAsync<User>().Result;
            return user;
        }

        public Note CreateNote(Note note)
        {
            note = _client.PostAsJsonAsync("notes", note).Result.Content.ReadAsAsync<Note>().Result;
            return note;
        }

        public Note GetNote(Guid noteId)
        {
            Note note;
            note = _client.GetAsync($"notes/{noteId}").Result.Content.ReadAsAsync<Note>().Result;
            return note;
        }

        public IEnumerable<Note> GetNotes(Guid userId)
        {
            return _client.GetAsync($"users/{userId}/notes").Result.Content.ReadAsAsync<IEnumerable<Note>>().Result;
        }

        public IEnumerable<Note> GetNotesBySharedUser(Guid ownerUserId, Guid sharedUserId)
        {
            return _client.GetAsync($"users/{ownerUserId}/shared/{sharedUserId}").Result.Content.ReadAsAsync<IEnumerable<Note>>().Result;
        }


        public Note UpdateNote(Note note)
        {
            return _client.PutAsJsonAsync("notes", note).Result.Content.ReadAsAsync<Note>().Result;
        }


        public void DeleteNote(Guid noteId)
        {
            _client.DeleteAsync($"notes/{noteId}");
        }
        public Category CreateCategory(Guid userId, String categoryTitle)
        {
            return _client.PostAsJsonAsync($"users/{userId}/categories", new Category { Title = categoryTitle }).Result.Content.ReadAsAsync<Category>().Result;
        }

        public IEnumerable<Category> GetCategoriesByUser(Guid userId)
        {
            return _client.GetAsync($"users/{userId}/categories").Result.Content.ReadAsAsync<IEnumerable<Category>>().Result;
        }

        public void DeleteCategory(Guid categoryId)
        {
            _client.DeleteAsync($"categories/{categoryId}");
        }

        public void AddNoteInCategory(Guid noteId, Guid categoryId)
        {
            var answer = _client.PostAsJsonAsync($"notes/{noteId}/add_category", categoryId).Result.Content.ReadAsAsync<Guid>().Result;
            return;
        }

        public void DeleteNoteFromCategory(Guid noteId, Guid categoryId)
        {
            _client.DeleteAsync($"notes/{noteId}/delete_category/{categoryId}");
        }

        public IEnumerable<Note> GetNotesByCategory(Guid categoryId)
        {
            return _client.GetAsync($"categories/{categoryId}/notes").Result.Content.ReadAsAsync<IEnumerable<Note>>().Result;
        }
        public IEnumerable<Category> GetCategoriesByNote(Guid noteId)
        {
            return _client.GetAsync($"notes/{noteId}/categories").Result.Content.ReadAsAsync<IEnumerable<Category>>().Result;
        }
        
        public IEnumerable<User> GetUsers()
        {
            return _client.GetAsync("users").Result.Content.ReadAsAsync<IEnumerable<User>>().Result;
        }

        public void ShareNote(Guid noteId, Guid userId)
        {
            var answer = _client.PostAsJsonAsync($"notes/{noteId}/shares", userId).Result.Content.ReadAsAsync<Guid>().Result;
        }

        public void DenyShareNote(Guid noteId, Guid userId)
        {
            _client.DeleteAsync($"notes/{noteId}/shares/{userId}");
        }

        public IEnumerable<Note> GetSharedNotesByUser(Guid userId)
        {
            return _client.GetAsync($"users/{userId}/shared").Result.Content.ReadAsAsync<IEnumerable<Note>>().Result;
        }

        public IEnumerable<User> GetUsersBySharedNote(Guid noteId)
        {
            return _client.GetAsync($"notes/{noteId}/shared").Result.Content.ReadAsAsync<IEnumerable<User>>().Result;
        }
        
    }
}
