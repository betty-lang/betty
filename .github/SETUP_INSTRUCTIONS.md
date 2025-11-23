# CI/CD Setup Instructions

This document provides step-by-step instructions for activating and configuring the Betty CI/CD pipeline.

## Prerequisites

- GitHub repository with admin access
- Repository pushed to GitHub
- GitHub Actions enabled (usually enabled by default)

## Activation Steps

### 1. Verify Workflows Are Present

Ensure these workflow files exist in your repository:

```
.github/
├── workflows/
│   ├── ci.yml                  # Continuous Integration
│   ├── release.yml             # Continuous Deployment
│   └── manual-release.yml      # Manual release control
├── README.md                   # CI/CD documentation
├── VERSIONING.md              # Versioning guide
├── QUICK_START.md             # Quick reference
├── BADGES.md                  # Status badges
└── pull_request_template.md   # PR template
```

### 2. Enable GitHub Actions

1. Go to your repository on GitHub
2. Click **Settings** tab
3. Click **Actions** → **General** in left sidebar
4. Under "Actions permissions", select:
   - **Allow all actions and reusable workflows**
5. Under "Workflow permissions", select:
   - **Read and write permissions**
   - Check **Allow GitHub Actions to create and approve pull requests**
6. Click **Save**

### 3. Configure Branch Protection (Recommended)

Protect the `main` branch to ensure code quality:

1. Go to **Settings** → **Branches**
2. Click **Add branch protection rule**
3. Branch name pattern: `main`
4. Enable:
   - ✅ Require a pull request before merging
   - ✅ Require status checks to pass before merging
   - ✅ Require branches to be up to date before merging
   - ✅ Status checks: Select `CI Success` (appears after first workflow run)
5. Click **Create**

**Note:** For solo development, you may want less strict rules initially.

### 4. Test the CI Pipeline

Trigger the CI workflow to verify everything works:

```bash
# Make a small change
echo "# CI/CD Pipeline Active" >> .github/CI_ACTIVE.md
git add .
git commit -m "test: verify CI pipeline"
git push origin main
```

Then:
1. Go to **Actions** tab
2. You should see the **CI - Build and Test** workflow running
3. Wait for it to complete (8-12 minutes)
4. Verify all jobs pass ✅

### 5. Create the First Release

Once CI passes, the release workflow will automatically trigger. However, for your first release, you may want to manually trigger it:

**Option A: Wait for Automatic Release**
- The release workflow triggers automatically after successful CI
- It will create version `v0.1.0` (or increment from last tag)

**Option B: Manual Trigger**
```bash
gh workflow run manual-release.yml -f version-type=patch -f dry-run=false
```

Or via GitHub UI:
1. Go to **Actions** tab
2. Select **Manual Release Trigger**
3. Click **Run workflow**
4. Select `patch` as version type
5. Click **Run workflow**

### 6. Verify First Release

After the release workflow completes:

1. Go to **Releases** (right sidebar on main repo page)
2. You should see a new release (e.g., `v0.1.0`)
3. Verify all platform binaries are attached:
   - betty-windows-x64.zip
   - betty-linux-x64.tar.gz
   - betty-macos-x64.tar.gz
   - betty-macos-arm64.tar.gz

### 7. Update README (Optional)

Add status badges to your main README.md:

```markdown
# Betty Programming Language

![CI](https://github.com/YOUR_USERNAME/betty/actions/workflows/ci.yml/badge.svg)
![Release](https://img.shields.io/github/v/release/YOUR_USERNAME/betty)
![License](https://img.shields.io/github/license/YOUR_USERNAME/betty)

Your existing README content...
```

Replace `YOUR_USERNAME` with your GitHub username.

### 8. Configure Notifications (Optional)

Get notified about workflow runs:

1. Click your profile picture → **Settings**
2. Go to **Notifications** in left sidebar
3. Under "Actions", enable:
   - ✅ Failed workflows
   - ✅ Successful workflows (optional)

---

## Verification Checklist

After setup, verify everything works:

- [ ] GitHub Actions is enabled
- [ ] Workflow files are present in `.github/workflows/`
- [ ] CI workflow runs successfully on push to main
- [ ] Tests pass on all platforms (Windows, macOS, Linux)
- [ ] Artifacts are created and uploaded
- [ ] Release workflow creates a new release
- [ ] All platform binaries are attached to release
- [ ] Version tag is created correctly
- [ ] Changelog is generated

---

## First-Time Configuration

### Setting the Initial Version

If you want to start with a specific version (e.g., `v0.1.0`):

**Option 1: Let CI create it automatically**
- The first release will be `v0.1.0` by default

**Option 2: Manually create the first tag**
```bash
git tag -a v0.0.0 -m "Initial tag for versioning"
git push origin v0.0.0
```

Then the next automatic release will be `v0.1.0`.

### Customizing Version Bump Logic

The version bump detection is in `.github/workflows/release.yml`:

```yaml
# Customize these patterns if needed
if echo "$COMMIT_MSG" | grep -qiE "^(breaking|major):"; then
  echo "bump=major" >> $GITHUB_OUTPUT
elif echo "$COMMIT_MSG" | grep -qiE "^(feat|feature|minor):"; then
  echo "bump=minor" >> $GITHUB_OUTPUT
else
  echo "bump=patch" >> $GITHUB_OUTPUT
fi
```

You can modify these regex patterns to match your team's commit conventions.

---

## Troubleshooting Setup

### "Workflow not found" Error

**Cause:** Workflow files not in correct location

**Fix:**
```bash
# Verify files are in .github/workflows/
ls .github/workflows/
# Should show: ci.yml, release.yml, manual-release.yml
```

### "Permission denied" Errors

**Cause:** Insufficient workflow permissions

**Fix:**
1. Go to Settings → Actions → General
2. Under "Workflow permissions", select:
   - Read and write permissions
3. Save and re-run workflow

### Workflows Not Triggering

**Cause:** Actions may be disabled or workflow has syntax errors

**Fix:**
1. Check Actions is enabled in Settings
2. Validate workflow syntax:
   ```bash
   # Use act or actionlint to validate locally
   actionlint .github/workflows/*.yml
   ```
3. Check for YAML syntax errors

### Release Not Creating Assets

**Cause:** Build might be failing on specific platforms

**Fix:**
1. Check workflow logs in Actions tab
2. Look for compilation errors in `build-and-upload-assets` job
3. Test build locally:
   ```bash
   dotnet publish src/Betty.CLI/Betty.CLI.csproj \
     --configuration Release \
     --runtime win-x64 \
     --self-contained true \
     -p:PublishSingleFile=true
   ```

### "Tag already exists" Error

**Cause:** Trying to create a release for an existing version

**Fix:**
```bash
# Delete the existing tag
git tag -d v0.1.0
git push --delete origin v0.1.0

# Then trigger release again
gh workflow run manual-release.yml -f version-type=patch
```

---

## Testing Locally

### Validating Workflows (Optional)

Install `actionlint` to validate workflow syntax:

```bash
# macOS
brew install actionlint

# Windows (with Chocolatey)
choco install actionlint

# Linux
curl -o- https://raw.githubusercontent.com/rhysd/actionlint/main/scripts/download-actionlint.bash | bash

# Validate workflows
actionlint .github/workflows/*.yml
```

### Testing Builds Locally

Test the build process before pushing:

```bash
# Restore dependencies
dotnet restore Betty.sln

# Build in release mode
dotnet build Betty.sln --configuration Release

# Run tests
dotnet test Betty.sln --configuration Release

# Test publish (Windows example)
dotnet publish src/Betty.CLI/Betty.CLI.csproj \
  --configuration Release \
  --runtime win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  --output ./dist/win-x64
```

---

## Advanced Configuration

### Customizing Build Targets

To add or remove build targets, edit `.github/workflows/release.yml`:

```yaml
strategy:
  matrix:
    include:
      # Add new targets here
      - os: ubuntu-latest
        rid: linux-arm64
        artifact-name: betty-linux-arm64
        asset-name: betty-linux-arm64.tar.gz
        asset-type: application/gzip
```

### Adding Code Signing

For signed releases, add secrets and modify the publish step:

1. Add secrets to repository:
   - Settings → Secrets → Actions
   - Add `SIGNING_CERT` and `SIGNING_PASSWORD`

2. Modify workflow to sign binaries:
   ```yaml
   - name: Sign Windows executable
     if: runner.os == 'Windows'
     run: |
       # Add signing command here
   ```

### Enabling Dependency Caching

Add caching to speed up builds:

```yaml
- name: Cache NuGet packages
  uses: actions/cache@v4
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
    restore-keys: |
      ${{ runner.os }}-nuget-
```

---

## Maintenance

### Updating Workflows

When updating workflow files:

1. Make changes in a feature branch
2. Test using `workflow_dispatch` trigger
3. Verify changes work as expected
4. Merge to main

### Monitoring Costs

GitHub provides 2,000 free Actions minutes/month for public repositories.

Monitor usage:
1. Go to Settings → Billing
2. Check Actions minutes used
3. Optimize workflows if approaching limit

---

## Next Steps

1. ✅ Complete this setup
2. ✅ Test the CI/CD pipeline
3. ✅ Create your first release
4. Read the [Quick Start Guide](QUICK_START.md)
5. Review [Versioning Documentation](VERSIONING.md)
6. Add status badges to README
7. Configure branch protection
8. Set up notifications

---

## Support

If you encounter issues during setup:

1. Check the [Troubleshooting](#troubleshooting-setup) section above
2. Review workflow logs in the Actions tab
3. Consult the [main CI/CD README](README.md)
4. Open an issue with the `ci/cd` label

---

**Setup should take approximately 10-15 minutes.**

Once complete, your Betty repository will have:
- ✅ Automated testing on every commit
- ✅ Cross-platform builds (Windows, macOS, Linux)
- ✅ Automated semantic versioning
- ✅ Automated releases with binaries
- ✅ Professional CI/CD workflow

**Happy releasing!**
