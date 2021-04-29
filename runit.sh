#!/bin/bash
make --directory csharp_module
cp csharp_module/csharp_module.so bin/Debug/net5.0
dotnet run
