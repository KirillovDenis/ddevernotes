using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DDEvernote.Model
{
    public class UserNotFoundException : Exception
    {
        private Guid _userNotFoundId;
        public Guid UserNotFoundId
        {
            get
            {
                return _userNotFoundId;
            }
            set
            {
                _userNotFoundId = value;
            }
        }
        
        public UserNotFoundException(String message, Guid userNotFoundId): base(message)
        {
            this._userNotFoundId = userNotFoundId;
        }
        protected UserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info != null)
            {
                this._userNotFoundId = new Guid(info.GetString("UserNotFoundId"));
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("UserNotFoundId", this.UserNotFoundId);
        }
    }
}
