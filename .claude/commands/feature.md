---
name: feature
description: "Implement a new feature safely: create a feature branch from main, apply changes in logically grouped commits, push, and open a PR. Use when adding new functionality, routes, components, services, or any non-bug-fix work."
argument-hint: "Describe the feature to implement"
---

# New Feature Workflow

Before touching any files, set up a branch. Apply changes in logical batches, each with its own commit. Finally push and open a PR.

## Step 0 — Pre-flight checks

Run the following checks before doing anything else. If any check fails, stop and inform the user — do not proceed.

1. **Check for uncommitted or unstaged changes**:
   ```
   git status --porcelain
   ```
   If the output is non-empty, cancel with:
   > "There are uncommitted or unstaged changes in the working tree. Please commit, stash, or discard them before starting a new feature."

2. **Ensure you are on `main`**:
   ```
   git branch --show-current
   ```
   If the current branch is not `main`, cancel with:
   > "You are currently on branch `<branch>`, not `main`. Please switch to `main` before starting a new feature."

3. **Pull the latest `main`**:
   ```
   git pull origin main
   ```
   Confirm the pull succeeded before continuing.

## Step 1 — Create a branch

Derive a short, descriptive branch name from the user's request using the prefix `feat/` (e.g. `feat/workout-export`, `feat/i18n-support`).

Run:
```
git checkout -b <branch-name>
```

Confirm the branch was created before proceeding.

## Step 2 — Apply changes in logical batches

Group the planned changes by concern. Each group becomes one commit. Typical groupings for this repo:

- **Data layer** — schema changes, migrations, database services (`drizzle/`, `app/services/database/`)
- **Backend / API** — server-side routes, loaders, actions (`app/routes/`)
- **Frontend** — components, UI, styles (`app/components/`, `app/routes/*.tsx`)
- **Infrastructure** — Dockerfile, compose, worker changes needed to support the feature
- **Tests / scripts** — any test or utility scripts added

For each batch:
1. Make the file edits.
2. Stage only the files for that batch: `git add <files>`
3. Commit with a concise imperative message: `git commit -m "<what and why>"`

Do not mix unrelated layers in a single commit.

## Step 3 — Push the branch

```
git push -u origin <branch-name>
```

## Step 4 — Open a Pull Request

Use the `github-pull-request_create_pull_request` tool:

- **Title**: imperative mood, under 72 chars, prefixed with `feat:` (e.g. `feat: add workout export to CSV`)
- **Body**: short summary of what was added and why; list each logical batch as a bullet
- **Base**: `main`
- **Draft**: default to `false` unless the user says the work is not ready

Report the PR URL to the user as a markdown link once created.
