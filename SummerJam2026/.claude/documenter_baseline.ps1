# Initializes the documenter baseline once, if it doesn't exist yet.
# After bootstrap, the Stop hook (documenter.ps1) owns advancing the baseline,
# so commits made between sessions get detected instead of being skipped.
$ErrorActionPreference = 'SilentlyContinue'

$repoRoot = (git rev-parse --show-toplevel 2>$null).Trim().Replace('/', '\')
if (-not $repoRoot) { exit 0 }

$baselineFile = Join-Path $repoRoot ".claude\documenter_baseline"
if (Test-Path $baselineFile) { exit 0 }

$head = git -C $repoRoot rev-parse HEAD 2>$null
if ($LASTEXITCODE -eq 0 -and $head) {
    $head.Trim() | Set-Content $baselineFile -Encoding utf8 -NoNewline
}
exit 0
