---
name: infra-changes
description: "Make infrastructure changes safely: create a branch first, apply changes in logically grouped commits, push, and open a PR. Use when modifying docker-compose files, CI/CD workflows, Traefik config, Dockerfiles, or any file under infra/ or .github/workflows/."
argument-hint: "Describe what infra change needs to be made"
---

# Infra Changes Workflow

Before touching any files, set up a branch. Then apply changes in logical batches, each with its own commit. Finally push and open a PR.

## Step 0 — Pre-flight checks

Run the following checks before doing anything else. If any check fails, stop and inform the user — do not proceed.

1. **Check for uncommitted or unstaged changes**:
   ```
   git status --porcelain
   ```
   If the output is non-empty, cancel with:
   > "There are uncommitted or unstaged changes in the working tree. Please commit, stash, or discard them before proceeding with infra changes."

2. **Ensure you are on `main`**:
   ```
   git branch --show-current
   ```
   If the current branch is not `main`, cancel with:
   > "You are currently on branch `<branch>`, not `main`. Please switch to `main` before starting infra changes."

3. **Pull the latest `main`**:
   ```
   git pull origin main
   ```
   Confirm the pull succeeded before continuing.

## Step 1 — Create a branch

Derive a short, descriptive branch name from the user's request using the prefix `infra/` (e.g. `infra/split-deploy-jobs`, `infra/traefik-ssl-config`).

Run:
```
git checkout -b <branch-name>
```

Confirm the branch was created before proceeding.

## Step 2 — Apply changes in logical batches

Group the planned changes by concern. Each group becomes one commit. Typical groupings for this repo:

- **CI/CD** — files under `.github/workflows/`
- **Container config** — `Dockerfile`, `Dockerfile.prod`, `Dockerfile.migrate`
- **Compose / infra** — files under `infra/` (docker-compose files, traefik config)
- **Mixed** — if a change spans groups, split by layer (e.g. compose changes first, then workflow changes that depend on them)

For each batch:
1. Make the file edits.
2. Stage only the files for that batch: `git add <files>`
3. Commit with a concise imperative message: `git commit -m "<what and why>"`

Do not mix unrelated files in a single commit.

## Step 3 — Push the branch

```
git push -u origin <branch-name>
```

## Step 4 — Open a Pull Request

Use the `github-pull-request_create_pull_request` tool:

- **Title**: imperative mood, under 72 chars, describes what changed
- **Body**: short summary of what changed and why; list each logical batch as a bullet
- **Base**: `main`
- **Draft**: default to `false` unless the user says the work is not ready

Report the PR URL to the user as a markdown link once created.
