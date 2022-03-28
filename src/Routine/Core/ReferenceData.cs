namespace Routine.Core
{
    public class ReferenceData
    {
        private string viewModelId;

        public string ModelId { get; set; }
        public string Id { get; set; }
        public string ViewModelId { get => viewModelId ?? ModelId; set => viewModelId = value; }

        #region ToString & Equality

        public override string ToString()
        {
            return $"[ReferenceData: [ModelId: {ModelId}, Id: {Id}, ViewModelId: {ViewModelId}]]";
        }
        protected bool Equals(ReferenceData other)
        {
            return string.Equals(ModelId, other.ModelId) && string.Equals(Id, other.Id) && string.Equals(ViewModelId, other.ViewModelId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((ReferenceData)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ModelId != null ? ModelId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ViewModelId != null ? ViewModelId.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion
    }
}
