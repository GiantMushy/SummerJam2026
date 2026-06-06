# Documenter sub-agent: detects commits made since the last evaluated baseline
# and suggests CLAUDE.md updates via Haiku. Runs as an asyncRewake Stop hook.
# The baseline only advances here (never on every prompt), so commits made
# between sessions are detected. Exits 2 to trigger a rewake when a suggestion exists.
$ErrorActionPreference = 'SilentlyContinue'

$repoRoot = (git rev-parse --show-toplevel 2>$null).Trim().Replace('/', '\')
if (-not $repoRoot) { exit 0 }

$baselineFile = Join-Path $repoRoot ".claude\documenter_baseline"
if (-not (Test-Path $baselineFile)) { exit 0 }

$baseline = (Get-Content $baselineFile -Raw).Trim()
$current  = (git -C $repoRoot rev-parse HEAD 2>$null).Trim()
if (-not $current -or $baseline -eq $current) { exit 0 }

$commitLog = (git -C $repoRoot log --oneline "$baseline..$current" 2>$null) -join "`n"
if (-not $commitLog) {
    # No commits in range (e.g. history was rewound) — resync the baseline and bail.
    $current | Set-Content $baselineFile -Encoding utf8 -NoNewline
    exit 0
}

if (-not (Get-Command claude -ErrorAction SilentlyContinue)) { exit 0 }

$diffLines = git -C $repoRoot diff "$baseline" "$current" -- "*.cs" "CLAUDE.md" 2>$null
$diff      = ($diffLines | Select-Object -First 150) -join "`n"

$claudeMdPath = Join-Path $repoRoot "CLAUDE.md"
$claudeMd     = if (Test-Path $claudeMdPath) { Get-Content $claudeMdPath -Raw } else { "(none)" }

$prompt = @"
You are Documenter, a documentation assistant for a Unity C# game project. Analyze these recent git commits and suggest a specific addition to CLAUDE.md — but ONLY if something genuinely new or non-obvious emerged that future Claude sessions should know.

Worth documenting: new architectural patterns, new manager types or systems, important design decisions, non-obvious conventions or constraints.
NOT worth documenting: tweaked values, routine additions that follow existing patterns, minor bug fixes.

Recent commits:
$commitLog

Diff (key files, truncated):
$diff

Current CLAUDE.md:
$claudeMd

If you have a suggestion: respond with ONLY the exact markdown text to APPEND to CLAUDE.md. No preamble — start directly with the content (e.g., a new section heading and bullets).
If nothing needs documenting: respond with exactly [NO_UPDATE]
"@

$response   = $prompt | & claude -p --model claude-haiku-4-5-20251001 2>$null
$suggestion = if ($response) { ($response -join "`n").Trim() } else { "" }

# If the Haiku call produced no output (transient failure), leave the baseline
# untouched so the same commits are retried on the next stop.
if (-not $suggestion) { exit 0 }

# Got a definitive answer — advance the baseline so this range is evaluated only once,
# whether or not there was something worth documenting.
$current | Set-Content $baselineFile -Encoding utf8 -NoNewline

if ($suggestion -match '^\[NO_UPDATE\]') { exit 0 }

Write-Output $suggestion
exit 2
