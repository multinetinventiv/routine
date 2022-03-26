using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Routine.Core
{
    public class ObjectModel
    {
        public string Id { get; set; }
        public List<string> Marks { get; set; }

        public string Name { get; set; }
        public string Module { get; set; }
        public bool IsValueModel { get; set; }
        public bool IsViewModel { get; set; }

        public List<string> ViewModelIds { get; set; }
        public List<string> ActualModelIds { get; set; }
        public InitializerModel Initializer { get; set; }
        internal Dictionary<string, DataModel> Data { get; private set; }
        internal Dictionary<string, OperationModel> Operation { get; private set; }
        public List<ObjectData> StaticInstances { get; set; }

        public ObjectModel()
            : this(new Dictionary<string, object>
            {
                {"Id", null},
                {"Marks", new List<string>()},

                {"Name", null},
                {"Module", null},
                {"IsValueModel", false},
                {"IsViewModel", false},

                {"ViewModelIds", new List<string>()},
                {"ActualModelIds", new List<string>()},
                {
                    "Initializer", new Dictionary<string, object>
                    {
                        {"Marks", new List<string>()},
                        {"GroupCount", 0},
                        {"Parameters", new List<Dictionary<string, object>>()}
                    }
                },
                {"Datas", new List<Dictionary<string, object>>()},
                {"Operations", new List<Dictionary<string, object>>()},
                {"StaticInstances", new List<Dictionary<string, object>>()}
            })
        { }
        public ObjectModel(IDictionary<string, object> model)
        {
            Id = (string)model["Id"];
            Marks = ((IEnumerable)model["Marks"]).Cast<string>().ToList();

            Name = (string)model["Name"];
            Module = (string)model["Module"];
            IsValueModel = (bool)model["IsValueModel"];
            IsViewModel = (bool)model["IsViewModel"];

            ViewModelIds = ((IEnumerable)model["ViewModelIds"]).Cast<string>().ToList();
            ActualModelIds = ((IEnumerable)model["ActualModelIds"]).Cast<string>().ToList();
            Initializer = new InitializerModel((IDictionary<string, object>)model["Initializer"]);
            Datas = ((IEnumerable)model["Datas"]).Cast<IDictionary<string, object>>().Select(o => new DataModel(o)).ToList();
            Operations = ((IEnumerable)model["Operations"]).Cast<IDictionary<string, object>>().Select(o => new OperationModel(o)).ToList();
            StaticInstances = ((IEnumerable)model["StaticInstances"]).Cast<IDictionary<string, object>>().Select(o => new ObjectData(o)).ToList();
        }

        public List<DataModel> Datas
        {
            get => Data.Values.ToList();
            set => Data = value.ToDictionary(o => o.Name, o => o);
        }

        public List<OperationModel> Operations
        {
            get => Operation.Values.ToList();
            set => Operation = value.ToDictionary(o => o.Name, o => o);
        }

        public DataModel GetData(string name)
        {
            Data.TryGetValue(name, out var result);

            return result;
        }

        public OperationModel GetOperation(string name)
        {
            Operation.TryGetValue(name, out var result);

            return result;
        }

        public void AddData(string name, DataModel model) => Data.Add(name, model);
        public void AddOperation(string name, OperationModel model) => Operation.Add(name, model);

        #region ToString & Equality

        public override string ToString()
        {
            return string.Format("[ObjectModel: [Id: {0}, Marks: {1}, Name: {2}, Module: {3}, IsValueModel: {4}, IsViewModel: {5}, " +
                                 "ViewModelIds: {6}, ActualModelIds: {7}, Initializer: {8}, " +
                                 "Datas: {9}, Operations: {10}, StaticInstances: {11}]]",
                                 Id, Marks.ToItemString(), Name, Module, IsValueModel, IsViewModel,
                                 ViewModelIds.ToItemString(), ActualModelIds.ToItemString(), Initializer,
                                 Datas.ToItemString(), Operations.ToItemString(), StaticInstances.ToItemString());
        }

        protected bool Equals(ObjectModel other)
        {
            return
                string.Equals(Id, other.Id) && Marks.ItemEquals(other.Marks) && string.Equals(Name, other.Name) &&
                string.Equals(Module, other.Module) && IsValueModel == other.IsValueModel && IsViewModel == other.IsViewModel &&
                ViewModelIds.ItemEquals(other.ViewModelIds) && ActualModelIds.ItemEquals(other.ActualModelIds) &&
                Equals(Initializer, other.Initializer) && Datas.ItemEquals(other.Datas) &&
                Operations.ItemEquals(other.Operations) && StaticInstances.ItemEquals(other.StaticInstances);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((ObjectModel)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Marks != null ? Marks.GetItemHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Module != null ? Module.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsValueModel.GetHashCode();
                hashCode = (hashCode * 397) ^ IsViewModel.GetHashCode();
                hashCode = (hashCode * 397) ^ (ViewModelIds != null ? ViewModelIds.GetItemHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ActualModelIds != null ? ActualModelIds.GetItemHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Initializer != null ? Initializer.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Datas != null ? Datas.GetItemHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Operations != null ? Operations.GetItemHashCode() : 0);
                hashCode = (hashCode * 397) ^ (StaticInstances != null ? StaticInstances.GetItemHashCode() : 0);
                return hashCode;
            }
        }

        #endregion
    }
}
