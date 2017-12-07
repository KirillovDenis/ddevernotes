using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using DDEvernote.Model;
using System.Threading.Tasks;
using DDEvernote.DataLayer.Sql;
using DDEvernote.DataLayer;
using System.Resources;
using System.Reflection;

namespace DDEvernote.Api.Hubs
{
    public class ServerHub : Hub
    {
        static Dictionary<Guid, string> Users = new Dictionary<Guid, string>();

       
        public void SendDelete(Note note, bool sendToOwner)
        {
            IEnumerable<User> users = note.Shared;
            foreach (var user in users)
            {
                string connId = "";
                if (Users.TryGetValue(user.Id, out connId))
                {
                    if (connId != Context.ConnectionId)
                    {
                        Clients.Client(connId).DeleteNotes(note.Id);
                    }
                }
            }
            if (sendToOwner)
            {
                string connId = "";
                if (Users.TryGetValue(note.Owner.Id, out connId))
                {
                    Clients.Client(connId).DeleteNotes(note.Id);
                }
            }
        }
        public void SendUpdate(Note note, bool sendToOwner)
        {
            IEnumerable<User> users = note.Shared;
            foreach (var user in users)
            {
                string connId = "";
                if (Users.TryGetValue(user.Id, out connId))
                {
                    if (connId != Context.ConnectionId)
                    {
                        Clients.Client(connId).UpdateNotes(note.Id);
                    }
                }
            }

            if (sendToOwner)
            {
                string connId = "";
                if (Users.TryGetValue(note.Owner.Id, out connId))
                {
                    Clients.Client(connId).UpdateNotes(note.Id);
                }
            }
        }
        public void AddNote(IEnumerable<Guid> usersId, Note note, bool sendToOwner)
        {
            foreach (var userId in usersId)
            {
                string connId = "";
                if (Users.TryGetValue(userId, out connId))
                {
                    if (connId != Context.ConnectionId)
                    {
                        Clients.Client(connId).UpdateNotes(note.Id);
                    }
                }
            }
            if (sendToOwner)
            {
                string connId = "";
                if (Users.TryGetValue(note.Owner.Id, out connId))
                {
                    Clients.Client(connId).UpdateNotes(note.Id);
                }
            }
        }
        public void RemoveNote(IEnumerable<Guid> usersId, Note note, bool sendToOwner)
        {
            foreach (var userId in usersId)
            {
                string connId = "";
                if (Users.TryGetValue(userId, out connId))
                {
                    if (connId != Context.ConnectionId)
                    {
                        Clients.Client(connId).DeleteNotes(note.Id);
                    }
                }
            }
            if (sendToOwner)
            {
                string connId = "";
                if (Users.TryGetValue(note.Owner.Id, out connId))
                {
                    Clients.Client(connId).DeleteNotes(note.Id);
                }
            }
        }

        private IEnumerable<Guid> GetUsersToUpdate(Guid noteId)
        {
            ResourceManager rm = new ResourceManager("DDEvernote.Api.ConnectionResource",
                            Assembly.GetExecutingAssembly());
            string connString = rm.GetString("ConnectionDBString");
            IUsersRepository userRepo = new UsersRepository(connString);

            return userRepo.GetUsersForNotifyByNote(noteId);
        }
        public override Task OnConnected()
        {
            return base.OnConnected();
        }
        public void Connect(Guid userId)
        {
            var id = Context.ConnectionId;

            Users.Add(userId, id);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Users.Remove(Users.Where(u => u.Value == Context.ConnectionId).Single().Key);

            return base.OnDisconnected(stopCalled);
        }
    }
}