using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DDEvernote.Model
{
    public class NoteNotFoundException: Exception
    {
        private Guid _noteNotFoundId;
        public Guid NoteNotFoundId
        {
            get
            {
                return _noteNotFoundId;
            }
            set
            {
                _noteNotFoundId = value;
            }
        }

        public NoteNotFoundException(string message, Guid NoteNotFoundId) : base(message)
        {
            this._noteNotFoundId = NoteNotFoundId;
        }
        protected NoteNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info != null)
            {
                this._noteNotFoundId = new Guid(info.GetString("NoteNotFoundId"));
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("NoteNotFoundId", this.NoteNotFoundId);
        }
    }
}
