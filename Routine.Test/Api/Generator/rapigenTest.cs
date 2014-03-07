using NUnit.Framework;

namespace Routine.Test.Api.Generator
{
	[TestFixture]
	public class rapigenTest
	{
		[Test]
		public void WriteTests()
		{
			// rapigen http://serviceurl:1111 Routine.Test.Module.Client.Todo
			// /i:Todo
			// /r:Routine.Test.Common
			// /p:Routine.Test.CustomClientApiPatterns
			// /u:UsingParseableValueTypes{valueTypePrefix:":"} /u:UsingClientClassesUnderCommonNamespace
			// /a:TodoApi
			// /s:.*Module,Instance
			// /f:Routine.Test.Module.ProjectManagement
			
			// rapigen http://serviceurl:1111 Routine.Test.Module.Client.ProjectManagement
			// /e:Todo 
			// /r:Routine.Test.Common /r:Routine.Test.Module.ProjectManagement.Client
			// /p:Routine.Test.CustomClientApiPatterns
			// /u:UsingParseableValueTypes{valueTypePrefix:":"} /u:UsingClientClassesUnderCommonNamespace
			// /a:TodoApi
			// /s:.*Module,Instance
			
			// /i => /include
			// /e => /exclude
			// /r => /reference
			// /p => /pattern
			// /u => /usepattern
			// /a => /apiname
			// /s => /singleton
			// /f => /friend

			Assert.Fail("not implemented");
		}
	}
}
