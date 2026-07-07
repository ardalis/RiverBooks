---
description: Find a coverage gap, write a test, validate it catches regressions, review it, commit, and open a PR.
argument-hint: [namespace or class name to target]
allowed-tools: Bash(dotnet *), Bash(git *), Bash(gh *), Write, Edit, Read
---

<objective>
Run the full add-coverage pipeline: identify the highest-priority untested method, write a test for it, prove the test catches breaking changes, review the test for quality, commit the result, and open a pull request.

Optional: pass a namespace or class name as `$ARGUMENTS` to narrow the gap search (e.g., `Acme.Store.Api` or `PaymentService`).
</objective>

<context>
Current branch: !`git branch --show-current`
</context>

<process>

## Setup

Create the working directory for handoff files:

```bash
New-Item -ItemType Directory -Force .claude/add-coverage-work | Out-Null
```

---

## Phase 1 — Find the gap

Invoke the `coverage-gap-finder` agent with `$ARGUMENTS` as input.

The agent will:

- Run `dotnet build` (stops here if the build is broken)
- Run `dotnet test` with coverage collection
- Generate a coverage report
- Identify the single highest-CRAP uncovered method
- Write `.claude/add-coverage-work/gap-report.md`

**Read gap-report.md** before proceeding. If it says no meaningful gap was found, report this to the user and stop.

---

## Phase 2 — Write the test

Invoke the `test-author` agent (no additional input beyond the handoff files).

The agent will:

- Read `gap-report.md` to understand what to test
- Write a test file in the correct test project
- Run the test to confirm it passes green
- Write `.claude/add-coverage-work/new-test-path.txt`

---

## Phase 3 — Validate the test has teeth

Invoke the `mutation-validator` agent.

The agent will:

- Read `new-test-path.txt` and `gap-report.md`
- Apply a minimal breaking mutation to the source method
- Confirm the test fails with the mutation
- Revert the mutation
- Write `.claude/add-coverage-work/mutation-result.md`

**Read mutation-result.md.** Check the first line for `Result: PASS` or `Result: FAIL`.

**If FAIL**: The test does not catch breaking changes. Report to the user:

- Which mutation was not caught
- What the mutation-validator suggests fixing
- Stop and ask the user whether to retry or abandon

---

## Phase 4 — Review the test

Invoke the `test-reviewer` agent.

The agent will:

- Read the test file at the path in `new-test-path.txt`
- Review for naming, structure, assertions, and meaningfulness
- Write `.claude/add-coverage-work/review-notes.md`

**Read review-notes.md.**

**If LGTM**: proceed to Phase 5.

**If changes needed**: Invoke the `test-author` agent again (it will detect `review-notes.md` and apply the feedback as a refactor pass). Then invoke `test-reviewer` again to confirm LGTM. If the second review still has issues, report to the user and stop rather than looping indefinitely.

---

## Phase 5 — Commit

Determine the class name from `gap-report.md` for the branch name.

Create a branch and commit:

```bash
git checkout -b test/add-coverage-{ClassName}
git add {new-test-file-path}
git commit -m "test: add coverage for {ClassName}.{MethodName}"
git push -u origin test/add-coverage-{ClassName}
```

---

## Phase 6 — Open the PR

Read the gap report and mutation result to build the PR description. Use the `gh` CLI:

```bash
gh pr create \
  --title "test: add coverage for {ClassName}.{MethodName}" \
  --body "$(cat <<'EOF'
## What

Added a test for `{ClassName}.{MethodName}` which had a CRAP score of {score} with {n}% line coverage.

## Coverage gap addressed

- **Class**: `{FullyQualifiedClassName}`
- **Method**: `{MethodSignature}`
- **CRAP score before**: {score}

## Mutation validation

The test was validated by applying a breaking mutation (`{mutation description}`) and confirming the test caught it.

## Test plan

- [ ] `dotnet test --filter "FullyQualifiedName~{TestClassName}"` passes green
- [ ] Review the test for intent clarity
EOF
)"
```

---

## Cleanup

Remove the working directory:

```bash
Remove-Item -Recurse -Force .claude/add-coverage-work -ErrorAction SilentlyContinue
```

---

## Report to user

Output:

- The PR URL
- The test class and method name
- The CRAP score that was addressed
- The mutation that validated the test

</process>

<success_criteria>
- gap-report.md identified a real coverage gap (CRAP > 30 or 0% coverage on complex method)
- New test file passes `dotnet test --filter`
- mutation-result.md shows PASS
- review-notes.md shows LGTM
- PR created with descriptive title and body
- Working directory cleaned up
</success_criteria>
