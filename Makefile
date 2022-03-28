.PHONY: test
build:
	dotnet build
test:
	dotnet test
run:
	dotnet run --project ./samples/Routine.Samples.SmartObject
