dotnet build PlataformaEducacao.sln

dotnet test --collect:"XPlat Code Coverage" --settings coverage.runsettings

dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.5.0

reportgenerator "-reports:**/coverage.cobertura.xml" "-targetdir:coveragereport" -reporttypes:Html