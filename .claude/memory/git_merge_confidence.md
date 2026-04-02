---
name: git-merge-confidence
description: User overcame fear of merging divergent uncommitted work with remote changes — stash/rebase/pop/resolve workflow now understood
type: user
---

On 2026-04-01 the user had uncommitted local changes on one machine and new commits pushed from a laptop. Was nervous about smashing them together.

Walked through the process step by step:
1. Stash local uncommitted changes
2. Rebase onto origin/main to pick up remote commits
3. Pop the stash — conflicts surface here as git replays local work on the new baseline
4. Resolve conflicts — just delete the marker lines and the code blocks you don't want, keep what you do
5. Tweak until it builds

Key realization: conflict resolution isn't fancy editing. Git does the hard part — it merges what it can and leaves markers where it couldn't decide. You're just the tiebreaker. Delete what you don't want, save, `git add`, done.

User is no longer afraid of this process. Slowing down and understanding each step was what made it click.
