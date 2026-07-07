---
name: stryker-dotnet
description: Run Stryker.NET mutation testing against a specific .NET source file. Covers installation (local tool), scoped invocation, JSON output parsing, and interpreting killed/survived results to decide pass/fail.
invocable: true
---

# Stryker.NET Mutation Testing

## When to Use This Skill

Use this skill when:

- Proving a new test actually catches real regressions (inside the /add-coverage workflow)
- Evaluating the strength of an existing test suite against a specific class or method
- Measuring mutation score before and after adding tests

---

## What Stryker Does

Stryker systematically applies hundreds of small code mutations (operator flips, constant changes, conditional negations, statement deletions) to the source under test, runs your tests against each mutation, and reports whether the test suite caught the change.

| Result | Meaning |
|---|---|
| **Killed** | The mutation caused a test to fail — the test has teeth |
| **Survived** | All tests passed despite the mutation — coverage gap |
| **NoCoverage** | No test even executes the mutated line |
| **Timeout** | Tests ran too long; mutation likely caused an infinite loop |
| **CompileError** | The mutation produced invalid code; ignored |

**Mutation score** = `Killed / (Killed + Survived + Timeout)` × 100

A score of 100% means every mutation the tool could make was caught by at least one test.

---

## Installation

### Recommended: Local Tool (team-consistent)

Add to `.config/dotnet-tools.json`:

```json
{
  "version": 1,
  "isRoot": true,
  "tools": {
    "dotnet-stryker": {
      "version": "4.6.0",
      "commands": ["dotnet-stryker"],
      "rollForward": false
    }
  }
}
```

Restore:

```bash
dotnet tool restore
```

Invoke as:

```bash
dotnet stryker [options]
```

### Alternative: Global Install

```bash
dotnet tool install -g dotnet-stryker
```

---

## Detection

Before using Stryker, check whether it is available:

```powershell
# Check local tools (run from solution root, where .config/ lives)
$local  = dotnet tool list 2>&1
$global = dotnet tool list -g 2>&1
$available = ($local -match "dotnet-stryker") -or ($global -match "dotnet-stryker")
```

If `$available` is false, fall back to manual mutation (see the mutation-validator agent).

---

## Running Stryker

Stryker is invoked from the **test project directory** (where the test `.csproj` lives).

### Scoped to One Source File

```powershell
Push-Location "tests/MyProject.Tests"

dotnet stryker `
  --mutate "**/TargetClass.cs" `
  --reporter json `
  --reporter progress `
  --output "../../stryker-output"

Pop-Location
```

`--mutate` accepts glob patterns matched against source file paths. Use `**/FileName.cs` to target a single file regardless of directory depth.

### Scope to Specific Tests Only

Add `--test-case-filter` to avoid running the whole test suite (much faster):

```powershell
dotnet stryker `
  --mutate "**/TargetClass.cs" `
  --test-case-filter "FullyQualifiedName~TargetClassTests" `
  --reporter json `
  --output "../../stryker-output"
```

The filter syntax is identical to `dotnet test --filter`.

### When the Test Project References Multiple Source Projects

Use `--project` to specify which source project to mutate:

```powershell
dotnet stryker `
  --project "../../src/MyProject/MyProject.csproj" `
  --mutate "**/TargetClass.cs" `
  --reporter json `
  --output "../../stryker-output"
```

---

## Output Location

Stryker writes the JSON report to:

```
{--output}/reports/mutation-report.json
```

If `--output` is omitted, it defaults to `StrykerOutput/` in the current directory.

---

## JSON Report Structure

```json
{
  "files": {
    "src/MyProject/TargetClass.cs": {
      "mutants": [
        {
          "id": "1",
          "mutatorName": "ConditionalBoundary",
          "replacement": ">=",
          "description": "changed > to >=",
          "location": {
            "start": { "line": 42, "column": 18 },
            "end":   { "line": 42, "column": 19 }
          },
          "status": "Killed"
        },
        {
          "id": "2",
          "mutatorName": "BooleanLiteral",
          "replacement": "false",
          "description": "changed true to false",
          "location": {
            "start": { "line": 57, "column": 24 },
            "end":   { "line": 57, "column": 28 }
          },
          "status": "Survived"
        }
      ]
    }
  },
  "projectRoot": "C:/dev/myproject"
}
```

---

## Parsing Results with PowerShell

```powershell
$json    = Get-Content "stryker-output/reports/mutation-report.json" -Raw | ConvertFrom-Json
$files   = $json.files.PSObject.Properties

$allMutants = @()
foreach ($file in $files) {
    $allMutants += $file.Value.mutants
}

$killed    = @($allMutants | Where-Object { $_.status -eq "Killed" })
$survived  = @($allMutants | Where-Object { $_.status -eq "Survived" })
$noCov     = @($allMutants | Where-Object { $_.status -eq "NoCoverage" })

$scored    = $killed.Count + $survived.Count
$score     = if ($scored -gt 0) { [math]::Round($killed.Count / $scored * 100, 1) } else { 0 }

Write-Host "Mutation score: $score%  ($($killed.Count) killed, $($survived.Count) survived, $($noCov.Count) no-coverage)"
```

### Listing Survived Mutants

```powershell
foreach ($m in $survived) {
    Write-Host "Line $($m.location.start.line): $($m.description) — replacement: $($m.replacement)"
}
```

---

## Pass / Fail Criteria

For validating a single new test (as in the mutation-validator workflow):

| Condition | Verdict |
|---|---|
| Score = 100% and ≥ 1 mutant generated | **PASS** — test catches all reachable mutations |
| Score ≥ 80% | **PASS** — test is meaningfully strong |
| Score < 80% or any Survived mutants | **FAIL** — test misses regressions; report which mutants survived |
| 0 mutants generated (all Ignored/NoCoverage) | **FAIL** — Stryker couldn't reach the method; check scoping |

---

## Common Flags Reference

| Flag | Purpose |
|---|---|
| `--mutate "**/*.cs"` | Glob filter on source files to mutate |
| `--project "path/to/src.csproj"` | Specify source project (when multiple are referenced) |
| `--test-case-filter "..."` | dotnet test filter — limits which tests run per mutation |
| `--reporter json` | Write machine-readable JSON report |
| `--reporter progress` | Show real-time progress in the terminal |
| `--output "path/"` | Override the output directory |
| `--since:main` | Only mutate lines changed since the given git ref (speeds up CI) |
| `--threshold-break 0` | Never fail the process due to score threshold (useful in automation) |
| `--ignore-methods "ToString,GetHashCode"` | Skip mutations inside specified method names |

---

## Typical Run Time

Run time scales with `(number of mutants) × (single test run duration)`. Scoping to one file and filtering to the relevant tests (`--test-case-filter`) typically reduces a full-suite run (minutes) to a per-file run (seconds to ~1 minute).

---

## Quick Reference

```powershell
# Minimal scoped invocation from test project directory
dotnet stryker --mutate "**/TargetClass.cs" --reporter json --output "../../stryker-output"

# With test filter for speed
dotnet stryker --mutate "**/TargetClass.cs" --test-case-filter "FullyQualifiedName~TargetClassTests" --reporter json --output "../../stryker-output"

# Parse score from output
$j = Get-Content "stryker-output/reports/mutation-report.json" -Raw | ConvertFrom-Json
$m = $j.files.PSObject.Properties.Value.mutants
$score = [math]::Round((@($m|?{$_.status -eq "Killed"}).Count / @($m|?{$_.status -in "Killed","Survived"}).Count) * 100, 1)
Write-Host "Score: $score%"
```
