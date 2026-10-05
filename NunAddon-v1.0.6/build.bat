@echo off
setlocal
set "AU=C:\Program Files (x86)\Steam\steamapps\common\Among Us - Copy"
if not "%~1"=="" set "AU=%~1"

echo === Nun Addon v1.0 build ===
echo Target Among Us: "%AU%"

for %%F in (
  "%AU%\BepInEx\plugins\MiraAPI.dll"
  "%AU%\BepInEx\plugins\TownOfUsMira.dll"
  "%AU%\BepInEx\plugins\Reactor.dll"
  "%AU%\BepInEx\interop\Assembly-CSharp.dll"
) do (
  if not exist %%F (
    echo ERROR: Missing %%F
    exit /b 1
  )
)

dotnet build NunAddon\NunAddon.csproj -c Release -p:AmongUs="%AU%"
if errorlevel 1 exit /b %errorlevel%

echo.
echo SUCCESS. DLL:
echo %CD%\NunAddon\bin\Release\net6.0\NunAddon.dll
endlocal
