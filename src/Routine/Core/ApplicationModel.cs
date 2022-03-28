using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Routine.Core
{
    public class ApplicationModel
    {
        internal Dictionary<string, ObjectModel> Model { get; private set; } = new Dictionary<string, ObjectModel>();

        public ApplicationModel() { }
        public ApplicationModel(IDictionary<string, object> model)
        {
            if(model == null) return;

            if (model.TryGetValue("Models", out object models))
            {
                Models = ((IEnumerable)models).Cast<IDictionary<string, object>>().Select(o => new ObjectModel(o)).ToList();
            }
        }

        public List<ObjectModel> Models
        {
            get => Model.Values.ToList();
            set => Model = value.ToDictionary(om => om.Id, om => om);
        }

        public ObjectModel GetModel(string name)
        {
            Model.TryGetValue(name, out var result);

            return result;
        }

        public void AddModel(string name, ObjectModel model) => Model.Add(name, model);

        #region ToString & Equality

        public override string ToString()
        {
            return $"[ApplicationModel: [Models: {Models.ToItemString()}]]";
        }

        protected bool Equals(ApplicationModel other)
        {
            return Models.ItemEquals(other.Models);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((ApplicationModel)obj);
        }

        public override int GetHashCode()
        {
            return (Models != null ? Models.GetItemHashCode() : 0);
        }

        #endregion
    }
}
