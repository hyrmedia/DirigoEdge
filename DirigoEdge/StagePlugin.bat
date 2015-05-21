@echo on

SET AssemblyName=%1
SET PluginDir=%~dp0

if exist %PluginDir%\Packaging\content\Plugins rd /s /q %PluginDir%\Packaging\content\Plugins
if exist %PluginDir%\Packaging\content\lib rd /s /q %PluginDir%\Packaging\content\lib

xcopy %PluginDir%bin\%AssemblyName%.dll %PluginDir%\Packaging\content\lib\
xcopy  /E %PluginDir%CSS %PluginDir%\Packaging\content\Plugins\%AssemblyName%\CSS\
xcopy  /E %PluginDir%Views %PluginDir%\Packaging\content\Plugins\%AssemblyName%\Views\
xcopy  /E %PluginDir%Scripts %PluginDir%\Packaging\content\Plugins\%AssemblyName%\Scripts\
xcopy  /E %PluginDir%Areas %PluginDir%\Packaging\content\Plugins\%AssemblyName%\Areas\

nuget.exe pack %PluginDir%PageBuilder.csproj -OutputDirectory  %PluginDir%Packaging\