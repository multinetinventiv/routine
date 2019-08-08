using System;

namespace Routine.Samples.Basic.CustomDto
{
	public class CustomDtoService
	{
		public CustomDto GetCustomDto(string text, int number)
		{
			return new CustomDto(text, number);
		}
	}

	public class CustomDto
	{
		public Guid Id { get; }
		public string Text { get; }
		public int Number { get; }

		public CustomDto(string text, int number)
		{
			Id = Guid.NewGuid();
			Text = text;
			Number = number;
		}
	}
}