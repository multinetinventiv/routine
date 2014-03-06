using System;

namespace Routine.Test.Common
{
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
	}
}

