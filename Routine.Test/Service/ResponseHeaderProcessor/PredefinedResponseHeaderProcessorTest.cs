using System.Collections.Generic;
using NUnit.Framework;
using Routine.Service;

namespace Routine.Test.Service.ResponseHeaderProcessor
{
	[TestFixture]
	public class PredefinedResponseHeaderProcessorTest
	{
		[Test]
		public void Processes_headers_via_given_delegate()
		{
			var processed = false;

			IResponseHeaderProcessor testing =
				BuildRoutine.ResponseHeaderProcessor()
					.For("header")
					.Do(header =>
					{
						Assert.AreEqual("header value", header);
						processed = true;
					});

			testing.Process(new Dictionary<string, string> {{"header", "header value"}});

			Assert.IsTrue(processed);
		}

		[Test]
		public void Includes_only_given_headers__even_if_there_exists_other_headers()
		{
			var processed = false;

			IResponseHeaderProcessor testing =
				BuildRoutine.ResponseHeaderProcessor()
					.For("header1", "header2")
					.Do((header1, header2) =>
					{
						Assert.AreEqual("header value 1", header1);
						Assert.AreEqual("header value 2", header2);
						processed = true;
					});

			testing.Process(new Dictionary<string, string> {
				{"header1", "header value 1"} ,
				{"header2", "header value 2"} ,
				{"header3", "header value 3"} 
			});

			Assert.IsTrue(processed);
		}

		[Test]
		public void Passes_empty_string_for_header_keys_not_found_in_headers()
		{
			var processed = false;

			IResponseHeaderProcessor testing =
				BuildRoutine.ResponseHeaderProcessor()
					.For("header1", "header2")
					.Do((header1, header2) =>
					{
						Assert.AreEqual("header value 1", header1);
						Assert.AreEqual(string.Empty, header2);
						processed = true;
					});

			testing.Process(new Dictionary<string, string> {
				{"header1", "header value 1"}
			});

			Assert.IsTrue(processed);
		}
	}
}