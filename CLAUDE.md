# JumpStarter — Claude Code Instructions

See [AGENTS.md](AGENTS.md) for full project context: architecture, tech stack, key files, build commands, configuration schema, and coding conventions.

## Workflow Skills

Project-specific skills live in `.claude/commands/`:

| Skill | When to use |
|---|---|
| `/bug-fix` | Fixing incorrect behavior, crashes, or regressions |
| `/new-feature` | Adding new functionality |
| `/infra-changes` | Modifying build, installer, or CI config |
| `/pr` | Packaging any pending changes into a PR |
