---
name: commit
description: Commit all pending changes in the frontend repo (RoomBooking-frontend) with a descriptive message, then push the current branch to its remote. Use when the user runs /commit or asks to "commit and push the front".
disable-model-invocation: true
---

# Commit and push the frontend

Invoking this skill is the user's explicit request to commit **and push** the frontend repo.
Work only inside the frontend repo (the directory containing `src/` and `package.json`,
not the backend superproject). Optional argument: a commit message hint or a full message.

## 1. Inspect

```bash
git branch --show-current
git status --short
git diff --stat HEAD
git log --oneline -5
```

- Nothing to commit (clean tree) → say so, and still offer to push if the branch is ahead
  (`git status -sb`). Don't create an empty commit.
- Detached HEAD → stop and tell the user.
- On `main` or `master` → stop and ask which feature branch to use; never commit or push to
  the default branch from this skill.

## 2. Safety checks before staging

- Scan the changed/untracked file list for things that must not be committed: `.env*`,
  `*.pem`/`*.key`, credentials, `node_modules/`, `dist/`, build output, large binaries,
  editor/OS junk. Respect `.gitignore`; if something suspicious is untracked and not ignored,
  leave it out and tell the user.
- Skim `git diff HEAD` for hard-coded secrets, tokens or passwords (test seed credentials in
  documented SQL seed scripts are fine).
- Run `npx tsc -b`. If it fails, stop and report the errors instead of committing broken code.
  (Skip only if the user's argument says to.)

## 3. Commit

- Stage everything that passed the checks: `git add -A` (minus anything excluded above).
- Message: a short imperative subject (≤72 chars) in the style of the recent log
  (`Frontend: …`), a blank line, then a few bullets saying *what changed and why*, derived from
  the actual diff, not the conversation. Use the user's argument as the subject if given.
- End the message with the attribution trailer the session instructs (currently
  `Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>`).
- Pass the message with a heredoc. Never use `--no-verify`, `--amend`, or skip signing. If a
  hook fails, fix the cause and make a new commit.

```bash
git commit -m "$(cat <<'EOF'
<subject>

- <bullet>

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
)"
```

## 4. Push

```bash
git push                                  # when an upstream is configured
git push -u origin "$(git branch --show-current)"   # first push of a new branch
```

- Never `--force` / `--force-with-lease`. If the push is rejected as non-fast-forward, run
  `git fetch` and report how far behind/diverged the branch is; do not rebase or merge without
  asking.
- If there is no remote or auth fails, report the exact error and stop.

## 5. Report

Two or three lines: branch, the new commit hash and subject, files changed count, and the push
result (remote/branch). Mention anything deliberately left out of the commit. Don't open a PR
unless asked.
