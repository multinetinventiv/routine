namespace Routine.Engine.Reflection
{
	internal class VoidTypeInfo : PreloadedTypeInfo
	{
		internal VoidTypeInfo()
			: base(typeof(void))
		{
			IsVoid = true;
		}
	}
}
