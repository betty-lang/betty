# Quick Start Guide - Betty CI/CD

This guide helps you understand how to work with Betty's automated CI/CD system in 5 minutes.

## For Developers - Daily Workflow

### Making Changes

Just commit and push to `main` as usual. The CI/CD system handles the rest.

```bash
# Make your changes
git add .
git commit -m "fix: correct parser error handling"
git push origin main
```

**What happens automatically:**
1. CI runs tests on Windows, macOS, and Linux
2. If tests pass, a new patch release is created (e.g., 0.1.0 → 0.1.1)
3. Binaries are built for all platforms
4. A GitHub release is published with all assets

### Commit Message Guide

Your commit message determines the version bump:

```bash
# Patch release (0.1.0 → 0.1.1) - Default for most commits
git commit -m "fix: resolve null reference exception"
git commit -m "docs: update installation guide"
git commit -m "refactor: simplify parser logic"

# Minor release (0.1.0 → 0.2.0) - New features
git commit -m "feat: add string interpolation support"
git commit -m "feature: implement lambda expressions"

# Major release (0.1.0 → 1.0.0) - Breaking changes
git commit -m "breaking: redesign AST structure"
git commit -m "major: complete rewrite of type system"
```

### Skipping Releases

If you're making documentation changes that don't need a release:

```bash
# Option 1: Add [skip ci] to commit message
git commit -m "docs: update README [skip ci]"

# Option 2: Just edit .md files (automatically skipped)
git commit -m "Update CONTRIBUTING.md"
```

---

## For Maintainers - Release Management

### Manual Release Control

Sometimes you want explicit control over when and what type of release to create.

**Using GitHub UI:**
1. Go to **Actions** tab
2. Click **Manual Release Trigger**
3. Click **Run workflow**
4. Select version type (patch/minor/major)
5. Click **Run workflow**

**Using GitHub CLI:**
```bash
# Create a minor release
gh workflow run manual-release.yml -f version-type=minor

# Preview version without releasing (dry run)
gh workflow run manual-release.yml -f version-type=major -f dry-run=true
```

### Checking Current Version

```bash
# View latest version tag
git describe --tags --abbrev=0

# List all releases
gh release list

# View latest release
gh release view
```

### Rolling Back a Release

If you need to remove a bad release:

```bash
# Delete the release
gh release delete v0.1.5

# Delete the tag locally and remotely
git tag -d v0.1.5
git push --delete origin v0.1.5

# Fix the issue, then create new release
git commit -m "fix: critical bug in parser"
git push origin main
```

---

## For Users - Downloading Releases

### Latest Release

Visit the [Releases page](../../releases/latest) or use:

```bash
gh release download
```

### Specific Version

```bash
gh release download v0.1.0
```

### Platform-Specific Downloads

- **Windows**: Download `betty-windows-x64.zip`
- **Linux**: Download `betty-linux-x64.tar.gz`
- **macOS (Intel)**: Download `betty-macos-x64.tar.gz`
- **macOS (M1/M2/M3)**: Download `betty-macos-arm64.tar.gz`

---

## Monitoring

### View Workflow Status

```bash
# List recent workflow runs
gh run list

# View specific workflow
gh run view <run-id>

# Watch workflow in real-time
gh run watch
```

### Check Test Results

After each CI run:
1. Go to **Actions** tab
2. Click on the workflow run
3. Scroll to **Artifacts** section
4. Download test results

---

## Troubleshooting

### Tests Failing

```bash
# Run tests locally first
dotnet test Betty.sln --configuration Release

# Check specific platform
# (Test on Windows/Mac/Linux if available)
```

### Release Not Created

**Check:**
- Did CI pass? (check Actions tab)
- Did you push to `main` branch?
- Did you only change documentation files?
- Is there a `[skip ci]` in commit message?

**Fix:**
```bash
# Trigger manual release
gh workflow run manual-release.yml -f version-type=patch
```

### Wrong Version Released

```bash
# Delete incorrect release
gh release delete v0.1.5
git push --delete origin v0.1.5

# Trigger correct version
gh workflow run manual-release.yml -f version-type=minor
```

---

## Common Scenarios

### Scenario 1: Bug Fix

```bash
git checkout main
git pull
# Fix bug in code
git add .
git commit -m "fix: resolve memory leak in parser"
git push origin main
# Result: 0.1.0 → 0.1.1 (automatic)
```

### Scenario 2: New Feature

```bash
git checkout main
git pull
# Implement new feature
git add .
git commit -m "feat: add support for async/await"
git push origin main
# Result: 0.1.0 → 0.2.0 (automatic)
```

### Scenario 3: Breaking Change (Use Manual Trigger)

```bash
git checkout main
git pull
# Make breaking change
git add .
git commit -m "breaking: redesign module system"
git push origin main

# Then manually trigger major release
gh workflow run manual-release.yml -f version-type=major
# Result: 0.1.0 → 1.0.0
```

### Scenario 4: Documentation Only

```bash
# Edit documentation files
git add README.md
git commit -m "docs: improve installation instructions"
git push origin main
# Result: No release created (docs are auto-skipped)
```

### Scenario 5: Multiple Commits Before Release

```bash
# Make several small commits
git commit -m "refactor: extract parser utilities"
git push origin main  # Creates release 0.1.1

git commit -m "test: add parser edge cases"
git push origin main  # Creates release 0.1.2

# To avoid multiple releases, work in a feature branch:
git checkout -b feature/parser-improvements
git commit -m "refactor: extract parser utilities"
git commit -m "test: add parser edge cases"
git push origin feature/parser-improvements
# Create PR, merge when ready → single release
```

---

## Best Practices

### DO

- Write clear, descriptive commit messages
- Use conventional commit prefixes (`feat:`, `fix:`, `docs:`)
- Run tests locally before pushing
- Use feature branches for large changes
- Review CI logs when builds fail

### DON'T

- Don't commit directly to `main` for large features
- Don't force push to `main` (it breaks versioning)
- Don't manually edit version numbers in files
- Don't create Git tags manually (let CI handle it)

---

## Quick Reference

| Task | Command |
|------|---------|
| Check current version | `git describe --tags --abbrev=0` |
| List releases | `gh release list` |
| Manual patch release | `gh workflow run manual-release.yml -f version-type=patch` |
| Manual minor release | `gh workflow run manual-release.yml -f version-type=minor` |
| Manual major release | `gh workflow run manual-release.yml -f version-type=major` |
| Dry run (preview) | `gh workflow run manual-release.yml -f version-type=minor -f dry-run=true` |
| Delete release | `gh release delete vX.Y.Z` |
| Delete tag | `git push --delete origin vX.Y.Z` |
| View workflow runs | `gh run list` |
| Download latest release | `gh release download` |
| Skip CI | Add `[skip ci]` to commit message |

---

## Getting Help

- **Full Documentation**: See [README.md](.github/README.md)
- **Versioning Guide**: See [VERSIONING.md](.github/VERSIONING.md)
- **Open Issue**: Create issue with `ci/cd` label
- **GitHub Actions Docs**: https://docs.github.com/actions

---

**Pro Tip:** Enable GitHub notifications for Actions to get alerts when builds fail or releases are created.

**Happy coding!**
