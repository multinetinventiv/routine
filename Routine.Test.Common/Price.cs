using System;

namespace Routine.Test.Common
{
	[Serializable]
	public class Price
	{
		public static Price Parse(string value)
		{
			var index = value.LastIndexOfAny("0123456789".ToCharArray());
			decimal price = decimal.Parse(value.Substring(0, index + 1));
			string currency = value.Substring(index + 1, value.Length - index - 1);

			return new Price(price, currency);
		}

		private readonly decimal price;
		private readonly string currency;

		public Price(decimal price, string currency)
		{
			if(string.IsNullOrEmpty(currency.Trim()))
			{
				throw new ArgumentException("currency");
			}

			this.price = price;
			this.currency = currency.Trim();
		}

		public override string ToString()
		{
			return price + currency;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(Price))
				return false;

			return Equals((Price)obj);
		}

		public bool Equals(Price other)
		{
			if (other == null)
				return false;

			return price == other.price && currency == other.currency;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return price.GetHashCode() ^ (currency != null ? currency.GetHashCode() : 0);
			}
		}
	}
}

