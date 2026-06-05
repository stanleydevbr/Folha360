# Branch Protection Rules

This document describes the branch protection rules for the Folha360 delivery workflow.

## Flow

```
feature/*  ──(auto PR)──>  develop  ──(release PR)──>  master  ──(deploy)──>  production
    │                          │                          │
    CI runs                 CI runs                    CI + Deploy
 (build+test)            (build+test)              (build+test+docker)
```

## Branch Protection Rules

Configure these in **Settings → Rules → Rulesets** (or **Settings → Branches → Branch protection rules** for classic).

### Rule 1: `master` — Production Branch

| Setting | Value |
|---------|-------|
| Branch name pattern | `master` |
| Require a pull request before merging | ✅ Yes |
| Required approvals | **1** |
| Dismiss stale approvals | ✅ Yes |
| Require status checks to pass | ✅ Yes |
| Status checks | `build (CI)` or `build-and-push (Deploy)` |
| Require conversation resolution | ✅ Yes |
| Require signed commits | Optional |
| Require linear history | ✅ Yes |
| Do not allow bypassing | ✅ Yes (include administrators) |
| Restrict push to | Only `github-actions` and repository admins |

### Rule 2: `develop` — Integration Branch

| Setting | Value |
|---------|-------|
| Branch name pattern | `develop` |
| Require a pull request before merging | ✅ Yes |
| Required approvals | **1** |
| Require status checks to pass | ✅ Yes |
| Status checks | `build (CI)` |
| Require conversation resolution | ✅ Yes |
| Require linear history | Optional |
| Allow auto-merge | ✅ Yes (for automated PRs from features) |

### Rule 3: `feature/**` — Feature Branches

| Setting | Value |
|---------|-------|
| Branch name pattern | `feature/**` |
| Require a pull request before merging | ❌ No (direct push allowed) |
| Restrict deletions | ✅ Yes |

## Environment Protection (for Deploy)

Configure in **Settings → Environments → `production`**:

| Setting | Value |
|---------|-------|
| Required reviewers | **1** (any admin/maintainer) |
| Wait timer | 0 minutes |
| Deployment branches | `master` only |
| Secrets | Add production secrets here (not in repo secrets) |

## How to Use

### Developer Workflow (Daily)

```bash
# 1. Create feature branch
git checkout -b feature/my-feature develop

# 2. Work and push
git add .
git commit -m "feat(scope): description"
git push -u origin feature/my-feature

# 3. Auto-PR is created to develop via auto-pr.yml
# 4. CI runs on the PR
# 5. After review, merge to develop
```

### Release Workflow

```bash
# Option A: Manual
# Go to Actions → Release → Run workflow
# Enter version (e.g., 1.0.0)

# Option B: Automatic
# Every Monday at 9am UTC, a release PR is created automatically
```

### Deploy

```bash
# After release PR is merged to master:
# 1. CI runs on master
# 2. Deploy workflow triggers
# 3. Requires approval from 1 reviewer (environment protection)
# 4. Docker image is built and pushed to ghcr.io
# 5. Deploy step executes
```
