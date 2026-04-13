# Portcullis task runner

run:
    dotnet run --project src/Portcullis.Api

test:
    dotnet build tests/Portcullis.Api.Tests
    dotnet exec tests/Portcullis.Api.Tests/bin/Debug/net10.0/Portcullis.Api.Tests.dll -stopOnFail

build:
    dotnet build
