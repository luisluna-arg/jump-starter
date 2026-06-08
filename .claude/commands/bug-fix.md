---
name: bug-fix
description: "Fix a bug safely: create a fix branch from main, apply the minimal targeted fix, push, and open a PR. Use when correcting incorrect behavior, crashes, or regressions."
argument-hint: "Describe the bug and its symptoms"
---

# Bug Fix Workflow

Before touching any files, set up a branch. Apply the fix in focused commits. Finally push and open a PR.

## Step 0 — Pre-flight checks

Run the following checks before doing anything else. If any check fails, stop and inform the user — do not proceed.

1. **Check for uncommitted or unstaged changes**:
   ```
   git status --porcelain
   ```
   If the output is non-empty, cancel with:
   > "There are uncommitted or unstaged changes in the working tree. Please commit, stash, or discard them before starting a bug fix."

2. **Ensure you are on `main`**:
   ```
   git branch --show-current
   ```
   If the current branch is not `main`, cancel with:
   > "You are currently on branch `<branch>`, not `main`. Please switch to `main` before starting a bug fix."

3. **Pull the latest `main`**:
   ```
   git pull origin main
   ```
   Confirm the pull succeeded before continuing.

## Step 1 — Understand the bug before writing code

Before making any changes:
- Identify the root cause, not just the symptom.
- Locate the affected file(s) and the exact code responsible.
- State the fix plan in one sentence before proceeding.

## Step 2 — Create a branch

Derive a short, descriptive branch name from the bug description using the prefix `fix/` (e.g. `fix/session-timeout-crash`, `fix/wrong-locale-on-reload`).

Run:
```
git checkout -b <branch-name>
```

Confirm the branch was created before proceeding.

## Step 3 — Apply the fix

Keep the fix minimal and targeted — only change what is necessary to correct the bug. Do not refactor unrelated code.

If the fix spans multiple concerns (e.g. a data layer bug that also requires a UI guard), split into separate commits per layer:

1. Make the file edits.
2. Stage only the files for that commit: `git add <files>`
3. Commit with a concise imperative message: `git commit -m "fix: <what was wrong and how it's fixed>"`

## Step 4 — Push the branch

```
git push -u origin <branch-name>
```

## Step 5 — Open a Pull Request

Use the `github-pull-request_create_pull_request` tool:

- **Title**: imperative mood, under 72 chars, prefixed with `fix:` (e.g. `fix: prevent session crash on missing user id`)
- **Body**: include root cause, what changed, and how to verify the fix is working
- **Base**: `main`
- **Draft**: default to `false`

Report the PR URL to the user as a markdown link once created.
