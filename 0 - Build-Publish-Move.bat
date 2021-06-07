@echo off

dotnet restore

dotnet build --no-restore -c Release

dotnet nuget push Panosen.CodeDom.EFCore\bin\Release\Panosen.CodeDom.EFCore.*.nupkg -s https://package.savory.cn/v3/index.json --skip-duplicate
dotnet nuget push Panosen.CodeDom.EFCore.Engine\bin\Release\Panosen.CodeDom.EFCore.Engine.*.nupkg -s https://package.savory.cn/v3/index.json --skip-duplicate

move /Y Panosen.CodeDom.EFCore\bin\Release\Panosen.CodeDom.EFCore.*.nupkg D:\LocalSavoryNuget\
move /Y Panosen.CodeDom.EFCore.Engine\bin\Release\Panosen.CodeDom.EFCore.Engine.*.nupkg D:\LocalSavoryNuget\

pause