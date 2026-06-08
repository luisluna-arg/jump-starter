---
name: pr
description: "Handles the full Git Flow lifecycle: updates main, branches, and commits changes in logical batches."
argument-hint: "Short description of the feature or fix (e.g., 'user-auth-api')"
---

# Git Flow & PR Management Skill

## Workflow Steps
1. **Sync Base:** 
   * Ensure GH CLI is installed and authenticated.
   * Ensure the current branch is `main` (or the repository's default branch).
   * Execute `git pull origin main` to ensure the local environment is up to date.
2. **Branching:**
   * Create a new branch using Git Flow naming conventions: `feature/[arg]`, `fix/[arg]`, `refactor/[arg]`, or `docs/[arg]`.
   * Switch to the new branch immediately.
3. **Atomic Commits:**
   * Analyze staged/unstaged changes.
   * Verify no ignored files are accidentally staged (`git status` should not show files that belong in `.gitignore`).
   * Group related file changes into "logical batches" (e.g., all database migrations together, then all service logic). Treat `.gitignore` additions or updates as their own commit (repo config layer).
   * Commit each batch with a concise, imperative message (e.g., "Add user schema", "Implement auth controller").
4. **Open Pull Request:**
   * Use GH CLI: `gh pr create --title "<title>" --body-file <temp-file>`.
   * PR title must follow Conventional Commits: `type(scope): short description (issue #N)`.
   * PR body must follow the template in `.github/PULL_REQUEST_TEMPLATE.md`:
     * **Purpose** — one paragraph explaining _what_ and _why_.
     * **Changes** — grouped by layer/category (e.g., `Repo Config`, `Infrastructure`, `Client Scaffold`, `Docs`). Each group is a `###` heading with bullet points.
     * **Verification** — numbered steps a reviewer can follow to manually verify the change works.
   * Write the body to a temp file and pass via `--body-file` to avoid shell escaping issues (backticks, special chars).

## Constraints
* **Branch Guard:** If the current branch is not `main`, alert the user and stop — continuing might be destructive.
* **Safety Check:** If there are merge conflicts during the `git pull`, stop and alert the user.
* **Batching Intelligence:** Do not commit all files at once if they touch different layers of the stack (e.g., keep `.css` changes separate from `.cs` or `.js` backend logic). Treat `.gitignore` additions or updates as their own commit (repo config layer).
* **No Preamble:** Execute commands directly and report status only.