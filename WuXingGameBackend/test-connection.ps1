# Test if the server is running and accepting connections
Write-Host "Testing connection to WuXing Game Backend Server..."

# Test HTTP endpoint
try {
    Write-Host "Testing HTTP connection to http://localhost:5000..."
    $response = Invoke-WebRequest -Uri "http://localhost:5000/swagger" -TimeoutSec 10
    Write-Host "HTTP Connection successful! Status code: $($response.StatusCode)"
} catch {
    Write-Host "HTTP Connection failed: $($_.Exception.Message)"
}

# Test HTTPS endpoint
try {
    Write-Host "Testing HTTPS connection to https://localhost:5001..."
    $response = Invoke-WebRequest -Uri "https://localhost:5001/swagger" -TimeoutSec 10
    Write-Host "HTTPS Connection successful! Status code: $($response.StatusCode)"
} catch {
    Write-Host "HTTPS Connection failed: $($_.Exception.Message)"
}

Write-Host "Test complete."