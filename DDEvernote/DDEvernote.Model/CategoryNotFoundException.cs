using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DDEvernote.Model
{

    [Serializable]
    public class CategoryNotFoundException : Exception
    {
        private Guid _categoryNotFoundId;
        public Guid CategoryNotFoundId
        {
            get
            {
                return _categoryNotFoundId;
            }
            set
            {
                _categoryNotFoundId = value;
            }
        }

        public CategoryNotFoundException(string message, Guid categoryNotFoundId) : base(message)
        {
            this._categoryNotFoundId = categoryNotFoundId;
        }
        protected CategoryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info != null)
            {
                this._categoryNotFoundId = new Guid(info.GetString("CategoryNotFoundId"));
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("CategoryNotFoundId", this.CategoryNotFoundId);
        }
    }
}
