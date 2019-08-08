using System;

namespace Routine.Core.Configuration
{
	public class Layer
	{
		public static readonly Layer LeastSpecific = new Layer(0, true);
		public static readonly Layer MostSpecific = new Layer(int.MaxValue, true);

		private readonly int order;

		public Layer(int order) : this(order, false) { }
		private Layer(int order, bool internalCall)
		{
			if (!internalCall)
			{
				if (order <= 0)
				{
					throw new ArgumentOutOfRangeException("order", order, "\"order\" must be greater than zero");
				}

				if (order == int.MaxValue)
				{
					throw new ArgumentOutOfRangeException("order", order, string.Format("\"order\" must be less than {0}", int.MaxValue));
				}
			}

			this.order = order;
		}

		public int Order { get { return order; } }

		public Layer MoreSpecific()
		{
			if (this == MostSpecific)
			{
				throw new InvalidOperationException("Layer cannot get any more specific than this");
			}

			if (Order + 1 == MostSpecific.Order)
			{
				return MostSpecific;
			}

			return new Layer(Order + 1);
		}

		public Layer LessSpecific()
		{
			if (this == LeastSpecific)
			{
				throw new InvalidOperationException("Layer cannot get any less specific than this");
			}

			if (Order - 1 == LeastSpecific.Order)
			{
				return LeastSpecific;
			}

			return new Layer(Order - 1);
		}

		public override string ToString()
		{
			return string.Format("Layer ({0})", order);
		}

		#region Equality & HashCode

		public static bool operator ==(Layer a, Layer b) { return Equals(a, b); }
		public static bool operator !=(Layer a, Layer b) { return !(a == b); }

		protected bool Equals(Layer other)
		{
			return Order == other.Order;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Layer)obj);
		}

		public override int GetHashCode()
		{
			return Order;
		}

		#endregion
	}
}