# Betty Versioning and Release Strategy

This document describes the automated versioning and release process for the Betty programming language.

## Overview

Betty uses **semantic versioning** (SemVer) with automated CI/CD pipelines that create releases based on commits to the `main` branch. The versioning system is designed for trunk-based development with automatic patch releases and manual control for major/minor bumps.

## Versioning Scheme

Betty follows [Semantic Versioning 2.0.0](https://semver.org/):

- **MAJOR.MINOR.PATCH** (e.g., 0.1.0, 1.2.3)
  - **MAJOR**: Breaking changes or significant architectural changes
  - **MINOR**: New features, backwards-compatible additions
  - **PATCH**: Bug fixes, documentation, minor improvements

### Pre-1.0 Status

Betty is currently in pre-1.0 development (0.x.x versions), which means:
- The API is still evolving and may have breaking changes
- All releases are marked as "pre-release" in GitHub
- Version 1.0.0 will signify the first stable release

## Automated Release Process

### How It Works

1. **Continuous Integration (CI)**
   - Runs on every push to `main` and on pull requests
   - Executes tests across Windows, macOS, and Linux
   - Builds release artifacts for all platforms
   - Must pass before any release is created

2. **Continuous Deployment (CD)**
   - Triggers automatically after successful commits to `main`
   - Analyzes commit messages to determine version bump type
   - Creates a new Git tag with the incremented version
   - Generates a changelog from commit messages
   - Builds and publishes platform-specific binaries
   - Creates a GitHub release with all assets

### Automatic Version Bump Detection

The release workflow automatically determines the version bump type based on your commit message:

| Commit Message Pattern | Version Bump | Example |
|------------------------|--------------|---------|
| `breaking:` or `major:` | MAJOR | `breaking: redesigned AST structure` |
| `feat:` or `feature:` or `minor:` | MINOR | `feat: add support for lambda expressions` |
| Any other commit | PATCH | `fix: resolve parser error with nested loops` |

**Examples:**

```bash
# This will trigger a PATCH release (0.1.0 -> 0.1.1)
git commit -m "fix: correct string interpolation bug"

# This will trigger a MINOR release (0.1.0 -> 0.2.0)
git commit -m "feat: implement standard library functions for strings"

# This will trigger a MAJOR release (0.1.0 -> 1.0.0)
git commit -m "breaking: complete redesign of type system"
```

### Files That Don't Trigger Releases

Changes to the following files/directories do NOT trigger releases:
- Markdown files (`*.md`)
- Documentation (`docs/**`)
- GitHub workflows (`.github/**`)

## Manual Release Triggers

### Option 1: Using GitHub UI (Recommended)

1. Go to the **Actions** tab in GitHub
2. Select **"Manual Release Trigger"** from the left sidebar
3. Click **"Run workflow"** button
4. Select version bump type:
   - **patch**: Bug fixes (0.1.0 → 0.1.1)
   - **minor**: New features (0.1.0 → 0.2.0)
   - **major**: Breaking changes (0.1.0 → 1.0.0)
5. Optionally enable **"Dry run"** to preview the version without creating a release
6. Click **"Run workflow"**

### Option 2: Using GitHub CLI

```bash
# Trigger a patch release
gh workflow run manual-release.yml -f version-type=patch

# Trigger a minor release
gh workflow run manual-release.yml -f version-type=minor

# Trigger a major release
gh workflow run manual-release.yml -f version-type=major

# Dry run to preview version
gh workflow run manual-release.yml -f version-type=minor -f dry-run=true
```

### Option 3: Direct Release Workflow Dispatch

```bash
# Trigger the release workflow directly
gh workflow run release.yml -f version-bump=patch
```

## Release Artifacts

Each release includes platform-specific binaries:

- **betty-windows-x64.zip** - Windows 64-bit executable
- **betty-linux-x64.tar.gz** - Linux 64-bit executable
- **betty-macos-x64.tar.gz** - macOS Intel executable
- **betty-macos-arm64.tar.gz** - macOS Apple Silicon executable

All binaries are:
- Self-contained (no .NET runtime required)
- Single-file executables
- AOT-compiled and trimmed for optimal size
- Ready to run without installation

## Best Practices for Trunk-Based Development

### For Daily Development

1. **Frequent Small Commits**: Commit working changes directly to `main` frequently
2. **Descriptive Messages**: Use clear commit messages that indicate the type of change
3. **Use Conventional Commits**: Start messages with `feat:`, `fix:`, `docs:`, etc.
4. **Test Before Pushing**: Ensure local tests pass before pushing

### For Feature Development

1. **Short-Lived Feature Branches**: Keep feature branches under 2 days when possible
2. **Feature Flags**: Use feature flags for incomplete features rather than long-lived branches
3. **Pull Requests**: Use PRs for code review before merging to `main`

### For Version Management

1. **Patch Releases (Default)**: Most commits should be patch releases
   - Bug fixes, refactoring, documentation, minor improvements

2. **Minor Releases (Planned)**: Use when adding new functionality
   - Use manual trigger or prefix commit with `feat:`
   - Plan these around feature completion

3. **Major Releases (Rare)**: Only for breaking changes
   - Coordinate with team before pushing
   - Use manual trigger or prefix commit with `breaking:`
   - Update documentation comprehensively

## Preventing Unnecessary Releases

If you're making changes that shouldn't trigger a release:

### Option 1: Include `[skip ci]` in commit message
```bash
git commit -m "docs: update README [skip ci]"
```

### Option 2: Modify documentation files only
Changes to `*.md` files and `docs/` directory don't trigger releases automatically.

### Option 3: Use a separate documentation branch
For extensive documentation work, use a `docs` branch and merge when ready.

## Troubleshooting

### Release Failed During Artifact Build

**Issue**: Release tag created but artifacts failed to upload

**Solution**:
1. Check the failed workflow run for error details
2. Fix the issue in code
3. Delete the failed release and tag:
   ```bash
   gh release delete vX.Y.Z
   git push --delete origin vX.Y.Z
   ```
4. Trigger a new release manually

### Wrong Version Number Was Released

**Issue**: Released version doesn't match expectations

**Solution**:
1. Delete the incorrect release:
   ```bash
   gh release delete vX.Y.Z
   git tag -d vX.Y.Z
   git push --delete origin vX.Y.Z
   ```
2. Trigger a new release with the correct version bump type

### CI Tests Passing Locally But Failing in GitHub Actions

**Issue**: Tests pass on your machine but fail in CI

**Solution**:
1. Check for platform-specific issues (paths, line endings)
2. Ensure all dependencies are properly restored
3. Review test output in the Actions tab
4. Run tests on all platforms locally if possible

## Version History

Current version can always be found by checking the latest tag:

```bash
git describe --tags --abbrev=0
```

View all releases:
```bash
gh release list
```

## GitHub Permissions Required

The workflows require the following permissions (already configured):

- **contents: write** - Create tags and releases
- **packages: write** - Publish release artifacts

## Monitoring Releases

- **Actions Tab**: Monitor workflow runs in real-time
- **Releases Page**: View all published releases
- **Watch Repository**: Get notified of new releases

## Future Enhancements

Potential improvements to consider:

1. **Changelog Automation**: Enhanced changelog generation with categorized changes
2. **Release Notes Template**: Structured release notes with breaking changes highlighted
3. **Canary Releases**: Automated pre-release builds for testing
4. **Docker Images**: Automated Docker image builds and publishes
5. **Package Managers**: Integration with Homebrew, Chocolatey, APT, etc.

## Questions or Issues?

If you encounter issues with the release process:
1. Check the workflow run logs in the Actions tab
2. Review this documentation
3. Open an issue with the `ci/cd` label
