<a href="https://betty-lang.org"><img src="logo.svg" alt="logo" width="200"></a>

---

Betty is a dynamic programming language designed for prototyping purposes. It supports various data types, has a standard library, and offers a simple and intuitive syntax. For more information, please refer to the [official Betty documentation](https://betty-lang.github.io/betty-docs/).

## Quick Start

### Download Pre-built Binaries

Betty now has automated releases with pre-built binaries for Windows, macOS, and Linux!

**[Download the latest release](https://github.com/betty-lang/betty/releases/latest)**

Choose the appropriate binary for your platform:
- **Windows**: `betty-windows-x64.zip`
- **Linux**: `betty-linux-x64.tar.gz`
- **macOS (Intel)**: `betty-macos-x64.tar.gz`
- **macOS (Apple Silicon)**: `betty-macos-arm64.tar.gz`

Extract the archive and add the executable to your PATH.

### Build from Source

Alternatively, you can clone the repo and build the binaries yourself:

```bash
git clone https://github.com/betty-lang/betty.git
cd betty
dotnet build Betty.sln --configuration Release
```

## For Contributors

### CI/CD Pipeline

Betty uses an automated CI/CD pipeline powered by GitHub Actions:

- **Continuous Integration**: Automated testing on every push and pull request across Windows, macOS, and Linux
- **Automated Releases**: New releases are automatically created when changes are pushed to main (if tests pass)
- **Semantic Versioning**: Version numbers are automatically incremented based on commit messages

**Commit message conventions:**
- `feat:` or `minor:` → Minor version bump (0.1.0 → 0.2.0)
- `breaking:` or `major:` → Major version bump (0.1.0 → 1.0.0)
- Everything else → Patch version bump (0.1.0 → 0.1.1)

**For detailed CI/CD documentation**, see [.github/QUICK_START.md](.github/QUICK_START.md)

---

Learn more at [betty-lang.org](https://betty-lang.org) or check out the [documentation](https://betty-lang.github.io/betty-docs/).
