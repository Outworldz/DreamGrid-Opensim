# Re-applies the ApplicationIcon each project needs, since prebuild.xml has no icon
# support (Prebuild is cross-platform for Linux+Windows) and regenerates these .csproj
# files from scratch on every runprebuild.bat run, wiping any icon set by hand in VS.
$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot

$icons = @(
    @{ CsProj = Join-Path $root 'OpenSim\Region\Application\OpenSim.csproj';               Icon = 'cube_blue.ico' },
    @{ CsProj = Join-Path $root 'OpenSim\Server\Robust.csproj';                             Icon = 'server.ico' },
    @{ CsProj = Join-Path $root 'OpenSim\ConsoleClient\OpenSim.ConsoleClient.csproj';       Icon = 'cube_green.ico' },
    @{ CsProj = Join-Path $root 'OpenSim\Tools\pCampBot\pCampBot.csproj';                   Icon = 'cube_green.ico' }
)

foreach ($entry in $icons) {
    $csproj = $entry.CsProj
    $icon = $entry.Icon

    if (-not (Test-Path $csproj)) {
        Write-Warning "Skipping missing project: $csproj"
        continue
    }

    $content = Get-Content -Path $csproj -Raw

    if ($content -match '<ApplicationIcon>[^<]*</ApplicationIcon>') {
        $content = $content -replace '<ApplicationIcon>[^<]*</ApplicationIcon>', "<ApplicationIcon>$icon</ApplicationIcon>"
    }
    else {
        $content = $content -replace '(<PropertyGroup>)', "`$1`r`n    <ApplicationIcon>$icon</ApplicationIcon>"
    }

    Set-Content -Path $csproj -Value $content -NoNewline -Encoding UTF8
    Write-Host "Set ApplicationIcon=$icon in $csproj"
}
