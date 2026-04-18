# Build EdgeFormEditor for distribution

Write-Host "EdgeFormEditor Build Script" -ForegroundColor Cyan
Write-Host "Building self-contained single-file executable...`n" -ForegroundColor Green

$projectPath = "EdgeFormEditor\EdgeFormEditor.csproj"
$outputDir = ".\dist"

Write-Host "This includes .NET runtime and can run without .NET installed.`n" -ForegroundColor Gray

dotnet publish $projectPath `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:PublishTrimmed=false `
    -o "$outputDir"

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n? Build successful!" -ForegroundColor Green
    Write-Host "Output: $outputDir\EdgeFormEditor.exe" -ForegroundColor Cyan
    Write-Host "`nFile size: $((Get-Item "$outputDir\EdgeFormEditor.exe").Length / 1MB) MB" -ForegroundColor Yellow
} else {
    Write-Host "`n? Build failed!" -ForegroundColor Red
    exit 1
}
