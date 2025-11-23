# Betty CI/CD Documentation Index

Welcome to the Betty CI/CD pipeline documentation. This index will help you find the right documentation for your needs.

## Quick Navigation

### I want to...

| Goal | Document | Reading Time |
|------|----------|--------------|
| Get started quickly | [QUICK_START.md](QUICK_START.md) | 5 minutes |
| Set up CI/CD for the first time | [SETUP_INSTRUCTIONS.md](SETUP_INSTRUCTIONS.md) | 10 minutes |
| Understand versioning | [VERSIONING.md](VERSIONING.md) | 15 minutes |
| Learn the complete system | [README.md](README.md) | 30 minutes |
| See workflow diagrams | [WORKFLOW_DIAGRAM.md](WORKFLOW_DIAGRAM.md) | 10 minutes |
| Add status badges | [BADGES.md](BADGES.md) | 5 minutes |
| Create a pull request | [pull_request_template.md](pull_request_template.md) | N/A |

## Documentation Files

### Core Documentation

#### [README.md](README.md)
**Comprehensive CI/CD documentation**
- Complete workflow overview
- Security configuration
- Platform support
- Troubleshooting guide
- Performance metrics
- Future roadmap

**Audience:** Maintainers, DevOps engineers, experienced contributors

---

#### [QUICK_START.md](QUICK_START.md)
**5-minute quick reference guide**
- Daily developer workflow
- Commit message guide
- Manual release control
- Common scenarios
- Quick reference table

**Audience:** All developers working on Betty

---

#### [VERSIONING.md](VERSIONING.md)
**Versioning strategy and guidelines**
- Semantic versioning explained
- Automatic version bump detection
- Manual release triggers
- Best practices for trunk-based development
- Troubleshooting version issues

**Audience:** All contributors, especially those managing releases

---

#### [SETUP_INSTRUCTIONS.md](SETUP_INSTRUCTIONS.md)
**First-time setup guide**
- Step-by-step activation instructions
- GitHub Actions configuration
- Branch protection setup
- Verification checklist
- Troubleshooting setup issues

**Audience:** Repository administrators, first-time setup

---

#### [WORKFLOW_DIAGRAM.md](WORKFLOW_DIAGRAM.md)
**Visual workflow diagrams**
- High-level architecture
- CI workflow flow
- Release workflow flow
- Version calculation flow
- Complete end-to-end flow

**Audience:** Visual learners, those wanting to understand the big picture

---

### Supporting Documentation

#### [BADGES.md](BADGES.md)
**GitHub Actions status badges**
- CI build status badge
- Release version badge
- Platform and technology badges
- Customization instructions

**Audience:** Maintainers updating README

---

#### [pull_request_template.md](pull_request_template.md)
**Pull request template**
- Structured PR format
- Version impact selection
- Testing checklist
- Release notes section

**Audience:** All contributors creating PRs

---

## Workflow Files

### [workflows/ci.yml](workflows/ci.yml)
**Continuous Integration workflow**
- Runs on every push to main and pull requests
- Tests on Windows, macOS, and Linux
- Builds cross-platform artifacts
- Uploads test results

**Triggers:**
- Push to main
- Pull requests
- Manual dispatch

---

### [workflows/release.yml](workflows/release.yml)
**Continuous Deployment workflow**
- Creates versioned releases automatically
- Analyzes commits for version bumping
- Builds and uploads platform-specific binaries
- Generates changelog

**Triggers:**
- Push to main (excluding docs)
- Manual dispatch with version type

---

### [workflows/manual-release.yml](workflows/manual-release.yml)
**Manual release control**
- Explicit version bump selection
- Dry-run capability
- Triggers release workflow

**Triggers:**
- Manual dispatch only

---

## File Organization

```
.github/
├── workflows/
│   ├── ci.yml                    # CI workflow
│   ├── release.yml               # Release workflow
│   └── manual-release.yml        # Manual release trigger
│
├── README.md                     # Complete documentation
├── QUICK_START.md               # Quick reference
├── VERSIONING.md                # Versioning guide
├── SETUP_INSTRUCTIONS.md        # Setup guide
├── WORKFLOW_DIAGRAM.md          # Visual diagrams
├── BADGES.md                    # Status badges
├── INDEX.md                     # This file
└── pull_request_template.md    # PR template
```

---

## Recommended Reading Path

### For New Contributors
1. [QUICK_START.md](QUICK_START.md) - Learn the basics
2. [VERSIONING.md](VERSIONING.md) - Understand versioning
3. [pull_request_template.md](pull_request_template.md) - Create your first PR

### For Repository Administrators
1. [SETUP_INSTRUCTIONS.md](SETUP_INSTRUCTIONS.md) - Set up CI/CD
2. [README.md](README.md) - Deep dive into workflows
3. [WORKFLOW_DIAGRAM.md](WORKFLOW_DIAGRAM.md) - Visualize the system

### For Maintainers
1. [README.md](README.md) - Complete system understanding
2. [VERSIONING.md](VERSIONING.md) - Version management
3. [WORKFLOW_DIAGRAM.md](WORKFLOW_DIAGRAM.md) - Architecture overview
4. [BADGES.md](BADGES.md) - Update status badges

---

## External Resources

- [GitHub Actions Documentation](https://docs.github.com/actions)
- [Semantic Versioning](https://semver.org/)
- [.NET CLI Reference](https://docs.microsoft.com/dotnet/core/tools/)
- [GitHub CLI Documentation](https://cli.github.com/manual/)

---

## Quick Commands

```bash
# Check current version
git describe --tags --abbrev=0

# List all releases
gh release list

# Trigger manual release
gh workflow run manual-release.yml -f version-type=patch

# View workflow runs
gh run list

# Download latest release
gh release download

# View this index
cat .github/INDEX.md
```

---

## Document Status

| Document | Last Updated | Status |
|----------|-------------|---------|
| README.md | 2025-11-23 | ✅ Complete |
| QUICK_START.md | 2025-11-23 | ✅ Complete |
| VERSIONING.md | 2025-11-23 | ✅ Complete |
| SETUP_INSTRUCTIONS.md | 2025-11-23 | ✅ Complete |
| WORKFLOW_DIAGRAM.md | 2025-11-23 | ✅ Complete |
| BADGES.md | 2025-11-23 | ✅ Complete |
| ci.yml | 2025-11-23 | ✅ Production Ready |
| release.yml | 2025-11-23 | ✅ Production Ready |
| manual-release.yml | 2025-11-23 | ✅ Production Ready |

---

## Getting Help

1. Check the relevant documentation file above
2. Review workflow logs in GitHub Actions tab
3. Search existing issues with `ci/cd` label
4. Create new issue with `ci/cd` label

---

## Contributing to Documentation

To improve this documentation:

1. Create a feature branch
2. Make your changes
3. Test if applicable
4. Create a PR with clear description
5. Update this index if adding new files

---

**This CI/CD system is production-ready and fully documented.**

Start with [QUICK_START.md](QUICK_START.md) if you're new, or [SETUP_INSTRUCTIONS.md](SETUP_INSTRUCTIONS.md) if you're setting up for the first time.
