<#
.SYNOPSIS
    Automated build and audit script for Asunder: Earthbound (Unity 6).

.DESCRIPTION
    Runs headless Unity 6 batchmode audits or builds for PC Standalone and OpenXR VR.

.PARAMETER Target
    The target to build: 'All', 'PC', 'VR', or 'Audit'. Default is 'All'.

.EXAMPLE
    .\build.ps1 -Target All
    .\build.ps1 -Target PC
    .\build.ps1 -Target VR
    .\build.ps1 -Target Audit
#>
[CmdletBinding()]
param(
    [ValidateSet('All', 'PC', 'VR', 'Audit')]
    [string]$Target = 'All'
)

$ErrorActionPreference = 'Stop'
$projectPath = $PSScriptRoot

# Locate Unity 6 Editor
$unityPaths = @(
    "C:\Program Files\Unity\Hub\Editor\6000.6.6f1\Editor\Unity.exe",
    "${env:ProgramFiles}\Unity\Hub\Editor\6000.6.6f1\Editor\Unity.exe",
    "${env:ProgramFiles(x86)}\Unity\Hub\Editor\6000.6.6f1\Editor\Unity.exe"
)

$unityExe = $null
foreach ($p in $unityPaths) {
    if (Test-Path $p) {
        $unityExe = $p
        break
    }
}

if (-not $unityExe) {
    # Check Unity Hub default directory
    $hubEditors = Get-ChildItem "C:\Program Files\Unity\Hub\Editor" -Directory -ErrorAction SilentlyContinue | Where-Object { $_.Name -like "6000.*" }
    if ($hubEditors) {
        $candidate = Join-Path $hubEditors[0].FullName "Editor\Unity.exe"
        if (Test-Path $candidate) {
            $unityExe = $candidate
        }
    }
}

if (-not $unityExe) {
    Write-Error "Could not locate Unity 6 Editor (6000.6.6f1). Please verify Unity Hub installation."
    exit 1
}

$logsDir = Join-Path $projectPath "Logs"
if (-not (Test-Path $logsDir)) {
    New-Item -ItemType Directory -Path $logsDir | Out-Null
}

$method = switch ($Target) {
    'All'   { 'AsunderUnity6SetupAndAudit.RunAll' }
    'PC'    { 'AsunderUnity6SetupAndAudit.BuildPCStandalone' }
    'VR'    { 'AsunderUnity6SetupAndAudit.BuildVRStandalone' }
    'Audit' { 'AsunderUnity6SetupAndAudit.RunAuditOnly' }
}

$logFile = Join-Path $logsDir "build_cli_${Target}.log"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  Asunder: Earthbound — Unity 6 Automated Build Engine" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " Unity Editor : $unityExe"
Write-Host " Project Path : $projectPath"
Write-Host " Target Action: $Target ($method)"
Write-Host " Log File     : $logFile"
Write-Host "----------------------------------------------------------"
Write-Host "Starting Unity batchmode process... Please wait." -ForegroundColor Yellow

$cmd = "`"$unityExe`" -batchmode -quit -projectPath `"$projectPath`" -executeMethod $method -logFile `"$logFile`""
cmd.exe /c $cmd

$exitCode = $LASTEXITCODE

Write-Host "----------------------------------------------------------"
if ($exitCode -eq 0) {
    Write-Host "SUCCESS: Operation '$Target' completed with exit code 0!" -ForegroundColor Green
    if (Test-Path (Join-Path $logsDir "unity6_final_audit.json")) {
        $audit = Get-Content (Join-Path $logsDir "unity6_final_audit.json") | ConvertFrom-Json
        Write-Host "Audit Result : $($audit.buildResult)" -ForegroundColor Green
        Write-Host "Total Errors : $($audit.buildTotalErrors)" -ForegroundColor Green
        Write-Host "Audited Scenes: $($audit.totalScenesAudited) / 13" -ForegroundColor Green
    }
} else {
    Write-Host "FAILURE: Unity exited with code $exitCode. Check log: $logFile" -ForegroundColor Red
    exit $exitCode
}
