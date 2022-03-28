namespace Routine.Samples.Basic.CustomDto
{
    public class CustomDtoService
    {
        private static CustomDto customDto;

        public CustomDto GetCustomDto() => customDto;
        public void SetCustomDto(CustomDto customDto) => CustomDtoService.customDto = customDto;
    }
}
