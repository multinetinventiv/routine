.PHONY: build test run

build:
	dotnet build
test:
	dotnet test
run:
	@ \
	echo "(1) Sample.Basic" ; \
	echo "(2) Sample.Client" ; \
	echo "(3) Sample.SmartObject" ; \
	echo "" ; \
	echo "Please select 1-3: " ; \
	read app ; \
	if test $$app -eq "1" ; then \
		dotnet run --project samples/Routine.Samples.Basic ; \
	fi ; \
	if test $$app -eq "2" ; then \
		dotnet run --project samples/Routine.Samples.Client ; \
	fi ; \
	if test $$app -eq "3" ; then \
		dotnet run --project samples/Routine.Samples.SmartObject ; \
	fi
