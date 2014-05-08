using NUnit.Framework;
using System.Text.RegularExpressions;

namespace Routine.Test
{
	[TestFixture]
	public class StringExtensionsTest
	{
		[Test]
		public void Test_String_ToUpperInitial()
		{
			Assert.AreEqual("Lower", "lower".ToUpperInitial());
			Assert.AreEqual("Upper", "Upper".ToUpperInitial());
			Assert.AreEqual("Invariant", "invariant".ToUpperInitial());
			Assert.AreEqual("I", "i".ToUpperInitial());
			Assert.AreEqual("", "".ToUpperInitial());
		}

		[Test]
		public void Test_String_ToLowerInitial()
		{
			Assert.AreEqual("lower", "lower".ToLowerInitial());
			Assert.AreEqual("upper", "Upper".ToLowerInitial());
			Assert.AreEqual("invariant", "Invariant".ToLowerInitial());
			Assert.AreEqual("i", "I".ToLowerInitial());
			Assert.AreEqual("", "".ToLowerInitial());
		}

		[Test]
		public void Test_String_SplitCamelCase()
		{
			Assert.AreEqual("camel Case", "camelCase".SplitCamelCase());
			Assert.AreEqual("Pascal Case", "PascalCase".SplitCamelCase());
			Assert.AreEqual("PASCAL Case", "PASCALCase".SplitCamelCase());
			Assert.AreEqual("Number 00 Case", "Number00Case".SplitCamelCase());

			Assert.AreEqual("camel-Case", "camelCase".SplitCamelCase('-'));
			Assert.AreEqual("Pascal-Case", "PascalCase".SplitCamelCase('-'));
			Assert.AreEqual("PASCAL-Case", "PASCALCase".SplitCamelCase('-'));
			Assert.AreEqual("Number-00-Case", "Number00Case".SplitCamelCase('-'));
		}

		[Test]
		public void Test_String_SnakeCaseToCamelCase()
		{
			Assert.AreEqual("snakeCase", "snake_case".SnakeCaseToCamelCase());
			Assert.AreEqual("snakeCase", "snake_Case".SnakeCaseToCamelCase());
			Assert.AreEqual("SnakeCase", "Snake_Case".SnakeCaseToCamelCase());
			Assert.AreEqual("camelCase", "camelCase".SnakeCaseToCamelCase());
			Assert.AreEqual("SNAKECase", "SNAKE_case".SnakeCaseToCamelCase());
			Assert.AreEqual("snake00Case", "snake_00_case".SnakeCaseToCamelCase());

			Assert.AreEqual("snakeCase", "snake-case".SnakeCaseToCamelCase('-'));
			Assert.AreEqual("snakeCase", "snake-Case".SnakeCaseToCamelCase('-'));
			Assert.AreEqual("SnakeCase", "Snake-Case".SnakeCaseToCamelCase('-'));
			Assert.AreEqual("camelCase", "camelCase".SnakeCaseToCamelCase('-'));
			Assert.AreEqual("SNAKECase", "SNAKE-case".SnakeCaseToCamelCase('-'));
			Assert.AreEqual("snake00Case", "snake-00-case".SnakeCaseToCamelCase('-'));
		}

		[Test]
		public void Test_String_Before()
		{
			Assert.AreEqual("a", "a".Before("."));
			Assert.AreEqual("a", "a.b".Before("."));
			Assert.AreEqual("a", "a.b.c".Before("."));

			Assert.AreEqual("a", "a".BeforeLast("."));
			Assert.AreEqual("a", "a.b".BeforeLast("."));
			Assert.AreEqual("a.b", "a.b.c".BeforeLast("."));

			Assert.AreEqual("Reference", "ReferenceCoreX".Before("Core"));
			Assert.AreEqual("Reference", "ReferenceCore".Before("Core"));
		}

		[Test]
		public void Test_String_After()
		{
			Assert.AreEqual("a", "a".After("."));
			Assert.AreEqual("b", "a.b".After("."));
			Assert.AreEqual("b.c", "a.b.c".After("."));

			Assert.AreEqual("a", "a".AfterLast("."));
			Assert.AreEqual("b", "a.b".AfterLast("."));
			Assert.AreEqual("c", "a.b.c".AfterLast("."));
			
			Assert.AreEqual("Reference", "CoreReference".After("Core"));
			Assert.AreEqual("Reference", "XCoreReference".After("Core"));
		}

		[Test]
		public void Test_String_SurroundWith()
		{
			Assert.AreEqual("'a'", "a".SurroundWith("'"));
			Assert.AreEqual("[a]", "a".SurroundWith("[", "]"));
			Assert.AreEqual("''", ((string) null).SurroundWith("'"));
		}

		[Test]
		public void Test_String_Append()
		{
			Assert.AreEqual("a.suffix", "a".Append(".suffix"));
		}

		[Test]
		public void Test_String_Prepend()
		{
			Assert.AreEqual("prefix.a", "a".Prepend("prefix."));
		}

		[Test]
		public void Test_String_Matches()
		{
			Assert.IsTrue("test".Matches(".es."));
			Assert.IsFalse("test".Matches("es."));
			Assert.IsFalse("test".Matches(".es"));
		}
	}
}

