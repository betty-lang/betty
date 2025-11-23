# Betty CI/CD Pipeline Documentation

This directory contains the GitHub Actions workflows that automate testing, building, and releasing the Betty programming language.

## Workflows Overview

### 1. CI - Build and Test (`ci.yml`)

**Triggers:**
- Every push to `main` branch
- Every pull request to `main` branch
- Manual trigger via workflow_dispatch

**Purpose:** Validates code quality and ensures the project builds correctly across all platforms.

**Jobs:**

1. **test** - Runs on Ubuntu, Windows, and macOS
   - Restores .NET dependencies
   - Builds the solution in Release mode
   - Executes all xUnit tests
   - Uploads test results as artifacts

2. **build-artifacts** - Creates platform-specific binaries
   - Builds self-contained executables for:
     - Windows (x64)
     - Linux (x64)
     - macOS (x64 Intel)
     - macOS (arm64 Apple Silicon)
   - Creates compressed archives (.zip for Windows, .tar.gz for Unix)
   - Uploads artifacts with 90-day retention

3. **ci-success** - Gate job that confirms all CI steps passed

**Artifacts Produced:**
- Test results (TRX format) for each platform
- Compressed binaries for all platforms (retained for 90 days)

---

### 2. CD - Create Release (`release.yml`)

**Triggers:**
- Push to `main` branch (excluding documentation-only changes)
- Manual trigger with version bump selection

**Purpose:** Automatically creates versioned releases with platform-specific binaries.

**Jobs:**

1. **verify-ci** - Runs full CI pipeline to ensure tests pass
   - Restores, builds, and tests the project
   - Acts as a quality gate before release

2. **create-release** - Calculates version and creates GitHub release
   - Determines version bump type from commit message or manual input
   - Calculates next version number
   - Generates changelog from commit history
   - Creates and pushes Git tag
   - Creates GitHub release with changelog

3. **build-and-upload-assets** - Builds and uploads release binaries
   - Builds self-contained executables for all platforms
   - Creates compressed archives
   - Uploads assets to the GitHub release

4. **release-success** - Confirms successful release

**Version Bump Logic:**
- **MAJOR**: Commit message starts with `breaking:` or `major:`
- **MINOR**: Commit message starts with `feat:`, `feature:`, or `minor:`
- **PATCH**: All other commits (default)

**Outputs:**
- Git tag (e.g., `v0.1.0`)
- GitHub release with changelog
- Platform-specific binary assets

---

### 3. Manual Release Trigger (`manual-release.yml`)

**Triggers:**
- Manual dispatch only (via GitHub UI or CLI)

**Purpose:** Provides explicit control over version increments with dry-run capability.

**Inputs:**
- `version-type`: patch, minor, or major
- `dry-run`: Preview version without creating release (optional)

**Usage:**

Via GitHub UI:
1. Navigate to Actions → Manual Release Trigger
2. Click "Run workflow"
3. Select version type
4. Optionally enable dry-run
5. Click "Run workflow"

Via GitHub CLI:
```bash
gh workflow run manual-release.yml -f version-type=minor -f dry-run=false
```

---

## Workflow Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     COMMIT TO MAIN                          │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ├──────────────────────────────────────┐
                      │                                      │
                      ▼                                      ▼
            ┌─────────────────┐                   ┌──────────────────┐
            │   CI Workflow   │                   │ Release Workflow │
            │                 │                   │                  │
            │  - Test (3 OS)  │                   │ 1. Verify CI     │
            │  - Build        │                   │ 2. Calculate Ver │
            │  - Artifacts    │                   │ 3. Create Tag    │
            └─────────────────┘                   │ 4. Build Assets  │
                                                  │ 5. Publish       │
                                                  └──────────────────┘
                                                           │
                                                           ▼
                                                  ┌──────────────────┐
                                                  │  GitHub Release  │
                                                  │                  │
                                                  │  - Changelog     │
                                                  │  - Win binary    │
                                                  │  - Linux binary  │
                                                  │  - macOS binaries│
                                                  └──────────────────┘
```

---

## Security Configuration

### Permissions

Workflows use least-privilege permissions:

**CI Workflow:**
- `contents: read` - Checkout code only

**Release Workflows:**
- `contents: write` - Create tags and releases
- `packages: write` - Upload release assets

### Secrets

No custom secrets are required. The workflows use the built-in `GITHUB_TOKEN` which is automatically provided by GitHub Actions.

**For future enhancements** (when needed):
- `NPM_TOKEN` - For publishing to npm registry
- `DOCKER_TOKEN` - For publishing Docker images
- `SIGNING_KEY` - For code signing binaries

---

## Platform Support

### Runners

The workflows use GitHub-hosted runners:
- **ubuntu-latest** - Ubuntu 22.04 LTS
- **windows-latest** - Windows Server 2022
- **macos-latest** - macOS 12 Monterey (for Intel builds)

### Target Platforms

Binaries are built for:
- **Windows x64** - Self-contained .NET executable
- **Linux x64** - Self-contained .NET executable
- **macOS x64** - Intel-based Macs
- **macOS arm64** - Apple Silicon Macs (M1, M2, M3)

### .NET Version

All workflows use **.NET 9.0** as specified in the project files.

---

## Build Configuration

### Self-Contained Builds

All release binaries are built with:
- **PublishSingleFile**: Creates a single executable
- **SelfContained**: Includes .NET runtime (no installation needed)
- **PublishTrimmed**: Removes unused code for smaller size
- **IncludeNativeLibrariesForSelfExtract**: Bundles native dependencies

This ensures users can download and run Betty without installing .NET.

### Build Performance

**Caching Strategy:**
- Dependencies are restored once per job
- Build outputs are cached between steps using `--no-restore` and `--no-build`

**Parallel Builds:**
- Matrix strategy builds all platforms simultaneously
- Independent jobs run in parallel when possible

---

## Troubleshooting

### Common Issues

#### 1. CI Failing on One Platform

**Symptom:** Tests pass on some platforms but fail on others

**Diagnosis:**
- Check test results artifact for the failing platform
- Look for platform-specific file path or line ending issues

**Fix:**
- Ensure paths use `Path.Combine()` instead of hardcoded separators
- Configure Git attributes for consistent line endings

#### 2. Release Created Without Assets

**Symptom:** Release appears in GitHub but has no binary attachments

**Diagnosis:**
- Check the `build-and-upload-assets` job logs
- Look for compilation or publish errors

**Fix:**
1. Delete the incomplete release: `gh release delete vX.Y.Z`
2. Delete the tag: `git push --delete origin vX.Y.Z`
3. Fix the build issue
4. Trigger a new release

#### 3. Version Number Not Incrementing

**Symptom:** New release has same version as previous

**Diagnosis:**
- Check if tag already exists: `git tag -l`
- Verify workflow actually ran: Check Actions tab

**Fix:**
- Ensure you pushed to `main` branch
- Check that changes weren't documentation-only
- Manually trigger release if needed

#### 4. Workflow Not Triggering

**Symptom:** Push to main doesn't trigger workflows

**Diagnosis:**
- Check if commit message contains `[skip ci]`
- Verify only documentation files weren't changed
- Check workflow permissions

**Fix:**
- Remove `[skip ci]` if present
- Ensure at least one code file changed
- Verify repository settings allow Actions

---

## Monitoring and Logs

### Viewing Workflow Runs

1. Navigate to the **Actions** tab
2. Select a workflow from the left sidebar
3. Click on a specific run to view details
4. Expand jobs and steps to see logs

### Downloading Artifacts

Artifacts are available for 30-90 days depending on type:

**Via GitHub UI:**
1. Go to Actions → Select workflow run
2. Scroll to "Artifacts" section
3. Click artifact name to download

**Via GitHub CLI:**
```bash
gh run download <run-id>
```

### Notifications

Enable notifications for workflow failures:
1. Go to repository Settings
2. Navigate to Notifications
3. Enable "Actions" notifications

---

## Maintenance

### Updating Dependencies

GitHub Actions automatically updates to the latest patch versions of actions. For major version updates:

1. Review changelogs for breaking changes
2. Update version in workflow files
3. Test in a feature branch first

**Common Actions Used:**
- `actions/checkout@v4`
- `actions/setup-dotnet@v4`
- `actions/upload-artifact@v4`
- `actions/create-release@v1`
- `actions/upload-release-asset@v1`

### Updating .NET Version

When upgrading .NET (e.g., 9.0 → 10.0):

1. Update `TargetFramework` in all `.csproj` files
2. Update `dotnet-version` in all workflow files
3. Test locally before pushing
4. Update this documentation

---

## Performance Metrics

Typical workflow run times:

- **CI Workflow**: 8-12 minutes
  - Test job: 3-5 minutes per platform
  - Build artifacts: 5-7 minutes per platform

- **Release Workflow**: 15-20 minutes
  - Verify CI: 3-5 minutes
  - Create release: 1-2 minutes
  - Build assets: 5-7 minutes per platform

**Optimization Opportunities:**
- Implement dependency caching (potential 30% reduction)
- Use warm-up containers (potential 20% reduction)
- Parallel test execution (requires test isolation)

---

## Cost Considerations

GitHub Actions minutes usage (free tier: 2000 min/month):

**Per CI run:** ~30 minutes (3 platforms × 10 min avg)
**Per Release:** ~40 minutes (4 platforms × 10 min avg)

**Estimated monthly usage:**
- 20 commits/month × 30 min = 600 minutes (CI)
- 10 releases/month × 40 min = 400 minutes (CD)
- **Total: ~1000 minutes/month** (within free tier)

**For higher usage:**
- Optimize workflows to reduce runtime
- Use self-hosted runners for private repositories
- Enable caching strategies

---

## Future Roadmap

### Planned Enhancements

1. **Dependency Caching**
   - Cache NuGet packages between runs
   - Reduce restore time by 50%

2. **Code Coverage Reports**
   - Integrate Coverlet for coverage
   - Upload to Codecov or Coveralls
   - Add coverage badge to README

3. **Code Signing**
   - Sign Windows executables
   - Sign macOS binaries with Apple Developer ID
   - Build user trust and bypass security warnings

4. **Package Distribution**
   - Publish to Homebrew (macOS/Linux)
   - Publish to Chocolatey (Windows)
   - Publish to Scoop (Windows)
   - Publish to APT/YUM repositories

5. **Docker Images**
   - Build and publish Docker images
   - Multi-architecture support (AMD64, ARM64)
   - Push to GitHub Container Registry

6. **Enhanced Security**
   - Implement CodeQL scanning
   - Dependency vulnerability scanning
   - SBOM (Software Bill of Materials) generation

7. **Release Automation**
   - Auto-generate detailed changelogs
   - Create draft releases for review
   - Integration with project management tools

---

## Contributing

When contributing to the CI/CD pipeline:

1. **Test Locally:** Use `act` to test workflows locally when possible
2. **Small Changes:** Make incremental improvements
3. **Document:** Update this README with any changes
4. **Branch Protection:** Test in feature branches before merging

**Resources:**
- [GitHub Actions Documentation](https://docs.github.com/actions)
- [Workflow Syntax Reference](https://docs.github.com/actions/reference/workflow-syntax-for-github-actions)
- [.NET CLI Reference](https://docs.microsoft.com/dotnet/core/tools/)

---

## Support

For issues with the CI/CD pipeline:
1. Check this documentation
2. Review workflow logs in Actions tab
3. Open an issue with label `ci/cd`
4. Tag maintainers if urgent

---

**Last Updated:** 2025-11-23
**Maintained By:** Betty Language Team
