namespace Routine.Core.Rest
{

	public class RestParameter
	{
		public string Name{get;private set;}
		public string Value{get;private set;}

		public RestParameter(string name, string value)
		{
			Name = name;
			Value = value;
		}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[RestParameter: Name={0}, Value={1}]", Name, Value);
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(RestParameter))
				return false;
			RestParameter other = (RestParameter)obj;
			return Name == other.Name && Value == other.Value;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Name != null ?Name.GetHashCode():0) ^ (Value != null ?Value.GetHashCode():0);
			}
		}

		#endregion
	}
}
