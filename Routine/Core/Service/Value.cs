using System.Collections.Generic;

namespace Routine.Core.Service
{
	//TODO may represent single value/reference, list of values/references or table of values/references
	public class ValueData
	{
		public bool IsList{get;set;}
		public List<SingleValueData> Values{get;set;}

		public ValueData() {Values = new List<SingleValueData>();}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ValueData: IsList={0}, Values={1}]", IsList, Values.ToItemString());
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(ValueData))
				return false;
			ValueData other = (ValueData)obj;
			return IsList == other.IsList && Values.ItemEquals(other.Values);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return IsList.GetHashCode() ^ (Values != null ?Values.GetItemHashCode():0);
			}
		}
		
		#endregion
	}

	public class SingleValueData
	{
		public ObjectReferenceData Reference{get;set;}
		public string Value{get;set;}

		public SingleValueData() {Reference = new ObjectReferenceData();}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[SingleValueData: Reference={0}, Value={1}]", Reference, Value);
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(SingleValueData))
				return false;
			SingleValueData other = (SingleValueData)obj;
			return object.Equals(Reference, other.Reference) && Value == other.Value;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Reference != null ?Reference.GetHashCode():0) ^ (Value != null ?Value.GetHashCode():0);
			}
		}

		#endregion
	}
}
