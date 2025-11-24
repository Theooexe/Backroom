$Destination = "C:\test_destination"
$ErreurFolder = "C:\test_erreurs"

if (-not (Test-Path $ErreurFolder)) {
    New-Item -ItemType Directory -Path $ErreurFolder | Out-Null
}

$file = Get-ChildItem -Path $Destination -File | Select-Object -First 1

if ($null -eq $file) { exit }

Write-Host "Traitement de test sur : $($file.Name)"

# Simulation d'un traitement
Start-Sleep -Seconds 1

# Exemple : suppression
Remove-Item $file.FullName
