# Records the current git HEAD at the start of each user prompt.
# The Stop hook compares against this to detect commits made during Claude's response.
$ErrorActionPreference = 'SilentlyContinue'
$head = git rev-parse HEAD 2>$null
if ($LASTEXITCODE -eq 0 -and $head) {
    $head.Trim() | Set-Content ".claude\documenter_baseline" -Encoding utf8 -NoNewline
}
exit 0
