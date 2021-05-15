dotnet build ..\RazorEngineCore.Extensions\RazorEngineCore.Extensions.csproj -f net5.0 -c Release -o lib\net5.0
dotnet build ..\RazorEngineCore.Extensions\RazorEngineCore.Extensions.csproj -f netcoreapp3.1 -c Release -o lib\netcoreapp3.1
dotnet build ..\RazorEngineCore.Extensions\RazorEngineCore.Extensions.csproj -f netstandard2.0 -c Release -o lib\netstandard2.0
dotnet build ..\RazorEngineCore.Extensions\RazorEngineCore.Extensions.csproj -f netstandard2.1 -c Release -o lib\netstandard2.1

dotnet build -f netcoreapp3.1 -c Release -o tools\netcoreapp3.1
dotnet build -f net5.0 -c Release -o tools\net5.0
# dotnet build -f net5.0 -c Release -o tools
nuget pack RazorEngineCore.Extensions.MSBuild.Tasks.nuspec -Version 0.5.0-alpha.1 -OutputDirectory ..\..\artifacts -Symbols -SymbolPackageFormat snupkg