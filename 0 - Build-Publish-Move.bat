@echo off

dotnet restore

dotnet build --no-restore -c Release

move /Y Panosen.CodeDom.EFCore\bin\Release\Panosen.CodeDom.EFCore.*.nupkg D:\LocalSavoryNuget\
move /Y Panosen.CodeDom.EFCore.Engine\bin\Release\Panosen.CodeDom.EFCore.Engine.*.nupkg D:\LocalSavoryNuget\

pause