#!/usr/bin/env pwsh

param(
    [string]$Environment = "Production",
    [switch]$OpenReport,
    [switch]$SkipSecurity
)

Write-Host "🚀 Starting Detective Agency API Tests for $Environment..." -ForegroundColor Green

# Check if .NET 8 is installed
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ .NET 8 SDK is required but not found"
    exit 1
}

Write-Host "✅ .NET Version: $dotnetVersion" -ForegroundColor Green

# Clean previous results in correct locations
$pathsToClean = @("TestResults", "allure-results", "TestLogs", "allure-report")
foreach ($path in $pathsToClean) {
    if (Test-Path $path) {
        Remove-Item -Recurse -Force $path
        Write-Host "🧹 Cleaned: $path" -ForegroundColor Yellow
    }
}

# Restore dependencies
Write-Host "📦 Restoring dependencies..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Failed to restore dependencies"
    exit 1
}

# Build project
Write-Host "🔨 Building project..." -ForegroundColor Yellow
dotnet build --no-restore --configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Build failed"
    exit 1
}

# Set environment variable for config
$env:ASPNETCORE_ENVIRONMENT = $Environment

# Ensure directories exist
New-Item -ItemType Directory -Force -Path "TestResults", "allure-results", "TestLogs" | Out-Null

# Run tests
Write-Host "🧪 Running tests against $Environment environment..." -ForegroundColor Yellow
$testCommand = "dotnet test --no-build --configuration Release --verbosity normal " +
               "--logger `"trx;LogFileName=test-results.trx`" " +
               "--logger `"html;LogFileName=test-results.html`" " +
               "--logger `"console;verbosity=normal`" " +
               "--results-directory ./TestResults"

Invoke-Expression $testCommand

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Some tests failed" -ForegroundColor Red
} else {
    Write-Host "✅ All tests passed!" -ForegroundColor Green
}

# Generate Allure report if available
if (Get-Command "allure" -ErrorAction SilentlyContinue) {
    Write-Host "📊 Generating Allure report..." -ForegroundColor Yellow
    allure generate allure-results --clean -o allure-report
    
    if ($OpenReport) {
        Write-Host "🌐 Opening Allure report..." -ForegroundColor Yellow
        Start-Process "allure-report\index.html"
    }
} else {
    Write-Host "⚠️ Allure CLI not found. Install with: scoop install allure" -ForegroundColor Yellow
    Write-Host "   Or download from: https://github.com/allure-framework/allure2/releases" -ForegroundColor Yellow
}

# Security scan
if (-not $SkipSecurity) {
    Write-Host "🔒 Running security scan..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-retire --version 3.0.0
    dotnet retire --output-format json --output-file security-scan-results.json
}

# Summary
Write-Host "`n📋 TEST SUMMARY" -ForegroundColor Cyan
Write-Host "   Environment: $Environment" -ForegroundColor White
Write-Host "   Test Results: ./TestResults/test-results.html" -ForegroundColor White
if (Test-Path "allure-report") {
    Write-Host "   Allure Report: ./allure-report/index.html" -ForegroundColor White
}
if (Test-Path "TestLogs") {
    Write-Host "   Test Logs: ./TestLogs/" -ForegroundColor White
}
if (Test-Path "security-scan-results.json") {
    Write-Host "   Security Report: ./security-scan-results.json" -ForegroundColor White
}

Write-Host "`n🎯 Next steps:" -ForegroundColor Cyan
Write-Host "   1. Commit and push to trigger GitHub Actions" -ForegroundColor White
Write-Host "   2. Go to Actions tab and run workflow manually" -ForegroundColor White
Write-Host "   3. View Allure report on GitHub Pages" -ForegroundColor White

if ($LASTEXITCODE -ne 0) {
    exit 1
}