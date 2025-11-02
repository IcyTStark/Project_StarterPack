# Marvel Rivals Auto Power Plan Script

# === CONFIG ===
$ultimateGUID = "4fedf76a-4413-42fb-9986-71090b0efc5f" # Ultimate Performance
$balancedGUID = "381b4222-f694-41f0-9685-ff5bb260df2e" # Balanced
$gamePath = "D:\Steam\steamapps\common\MarvelRivals\MarvelRivals_Launcher.exe"
$processName = "MarvelRivals"  # Without .exe

Write-Host "🟢 Switching to Ultimate Performance..."
powercfg /setactive $ultimateGUID

Write-Host "🚀 Launching Marvel Rivals..."
Start-Process -FilePath $gamePath

# Wait for the game to close
Write-Host "⏳ Waiting for Marvel Rivals to exit..."
do {
    Start-Sleep -Seconds 5
} while (Get-Process -Name $processName -ErrorAction SilentlyContinue)

Write-Host "🔁 Game closed. Reverting to Balanced Power Plan..."
powercfg /setactive $balancedGUID

Write-Host "✅ Done. Power plan restored."
