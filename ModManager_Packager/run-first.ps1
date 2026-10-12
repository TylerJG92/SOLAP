# Tells PowerShell if anything Errors to stop running the code
$ErrorActionPreference = "Stop"
# Folder conatining this packaging script
$packagerRoot = $PSScriptRoot
$refRoot = Join-Path $packagerRoot "ref"
$extraRoot = Join-Path $refRoot "extra"
$bepinexRoot = Join-Path $extraRoot "BepInEx"
$configRoot = Join-Path $bepinexRoot "config"
$coreRoot = Join-Path $bepinexRoot "core"
$monomodRoot = Join-Path $bepinexRoot "monomod"
$patchersRoot = Join-Path $bepinexRoot "patchers"
$pluginsRoot = Join-Path $bepinexRoot "plugins"

$layout = @(
    $refRoot
    $extraRoot
    $bepinexRoot
    $configRoot
    $coreRoot
    $monomodRoot
    $patchersRoot
    $pluginsRoot
)



foreach($folder in $layout) {
    if ( -not(Test-Path $folder)) {
        Write-Host "$folder not found, Creating folder" 
        New-Item -ItemType Directory -Path $folder
        } 
    else 
    {
        Write-Host "$folder exists, skipping creation of new folder."
    }
}