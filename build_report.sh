#!/bin/bash

dotnet test --settings ./tests/test.runsettings ./tests
reportgenerator -reports:"./tests/TestResults/**/coverage.cobertura.xml" -targetdir:"coverage" -reporttypes:Html
