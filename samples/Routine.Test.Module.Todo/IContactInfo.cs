using System;

namespace Routine.Test.Module.Todo
{
	public interface IContactInfo
	{
		string Name{get;}
		string Info{get;}
		DateTime DateCreated{get;}

		void Poke();
	}
}

