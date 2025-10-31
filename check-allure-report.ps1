#!/usr/bin/env pwsh

Write-Host "🔍 Checking Allure report generation..." -ForegroundColor Green

# Проверяем структуру allure-report
if (Test-Path "allure-report") {
    Write-Host "✅ Allure report directory exists" -ForegroundColor Green
    
    $indexFile = "allure-report\index.html"
    if (Test-Path $indexFile) {
        Write-Host "✅ index.html found" -ForegroundColor Green
        Write-Host "📊 Report is ready for deployment" -ForegroundColor Cyan
    } else {
        Write-Host "❌ index.html not found in allure-report" -ForegroundColor Red
        Write-Host "📁 Contents of allure-report:" -ForegroundColor Yellow
        Get-ChildItem -Path "allure-report" -Recurse | Select-Object Name
    }
} else {
    Write-Host "❌ Allure report directory not found" -ForegroundColor Red
    Write-Host "💡 Generate it first with: allure generate allure-results --clean -o allure-report" -ForegroundColor Yellow
}

# Проверяем содержимое allure-results
if (Test-Path "allure-results") {
    $resultFiles = Get-ChildItem -Path "allure-results" -File
    Write-Host "📊 Allure results: $($resultFiles.Count) files" -ForegroundColor Cyan
    if ($resultFiles.Count -eq 0) {
        Write-Host "⚠️ No test results found in allure-results" -ForegroundColor Yellow
    }
} else {
    Write-Host "❌ Allure results directory not found" -ForegroundColor Red
}