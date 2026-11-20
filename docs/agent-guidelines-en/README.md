# English Mirror of Agent Guidelines & Skills (Inactive Backup)

This directory contains an **English mirror** of the repository's active Spanish agent instructions ([AGENTS.md](../../AGENTS.md)) and project skills ([.agents/skills/](../../.agents/skills/)).

## Why the `.en.md` Suffix and Separate Folder?

To ensure **zero context overhead and zero skill-name collisions** across all coding agents:
- None of the files here are named `AGENTS.md` or `SKILL.md`.
- They live under `docs/` rather than `.agents/`, so agents do not auto-load or double-register them alongside the active Spanish directives.

## Keeping This Mirror Synchronized

Per the maintenance rule in `AGENTS.md`, whenever `AGENTS.md` or any `.agents/skills/<name>/SKILL.md` file is updated, the corresponding file in `docs/agent-guidelines-en/` must be updated in the same change.

## How to Switch the Repository to English in the Future

Run the following PowerShell commands from the repository root (`SBC.sln`):

```powershell
Copy-Item "docs/agent-guidelines-en/AGENTS.en.md" "AGENTS.md" -Force
Copy-Item "docs/agent-guidelines-en/skills/sbc-add-endpoint.en.md" ".agents/skills/sbc-add-endpoint/SKILL.md" -Force
Copy-Item "docs/agent-guidelines-en/skills/sbc-ef-migration.en.md" ".agents/skills/sbc-ef-migration/SKILL.md" -Force
Copy-Item "docs/agent-guidelines-en/skills/sbc-accounting-operations.en.md" ".agents/skills/sbc-accounting-operations/SKILL.md" -Force
Copy-Item "docs/agent-guidelines-en/skills/sbc-unit-testing.en.md" ".agents/skills/sbc-unit-testing/SKILL.md" -Force
```

Or on bash/macOS/Linux:

```bash
cp docs/agent-guidelines-en/AGENTS.en.md AGENTS.md
cp docs/agent-guidelines-en/skills/sbc-add-endpoint.en.md .agents/skills/sbc-add-endpoint/SKILL.md
cp docs/agent-guidelines-en/skills/sbc-ef-migration.en.md .agents/skills/sbc-ef-migration/SKILL.md
cp docs/agent-guidelines-en/skills/sbc-accounting-operations.en.md .agents/skills/sbc-accounting-operations/SKILL.md
cp docs/agent-guidelines-en/skills/sbc-unit-testing.en.md .agents/skills/sbc-unit-testing/SKILL.md
```

After switching, update the maintenance rule in `AGENTS.md` (and optionally archive or remove `docs/agent-guidelines-en/`).
