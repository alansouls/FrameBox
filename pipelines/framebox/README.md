# FrameBox Pipelines

This directory contains GitHub Actions pipelines for building, testing, and publishing NuGet packages for the FrameBox framework components.

## Overview

The FrameBox pipelines are designed to provide automated CI/CD for the following packages:

- **FrameBox.Core** - Core library with event handling capabilities
- **FrameBox.Storage.EFCore** - Entity Framework Core storage implementation
- **FrameBox.MessageBroker.RabbitMQ** - RabbitMQ message broker implementation

## Pipeline Structure

### Templates (`templates/`)

- **`dotnet-package.yml`** - Reusable workflow template for building, testing, and publishing individual .NET packages

### Individual Package Pipelines

- **`framebox-core.yml`** - Pipeline for FrameBox.Core package
- **`framebox-storage-efcore.yml`** - Pipeline for FrameBox.Storage.EFCore package
- **`framebox-messagebroker-rabbitmq.yml`** - Pipeline for FrameBox.MessageBroker.RabbitMQ package

### Composite Pipelines

- **`framebox-all.yml`** - Builds all FrameBox packages and runs integration tests

## Pipeline Features

### ✅ **Automated Building**
- Restores dependencies
- Compiles code in Release configuration
- Creates NuGet packages (.nupkg files)

### ✅ **Testing Support**
- Configurable test execution
- Test result reporting
- Support for .trx test result format

### ✅ **Caching**
- NuGet package caching for faster builds
- Cache key based on project file changes

### ✅ **Artifact Management**
- Uploads packages as build artifacts
- 30-day retention for debugging and manual deployment

### ✅ **Automated Publishing**
- Publishes to NuGet.org on main branch (when API key is configured)
- Skip duplicates to handle version conflicts gracefully

### ✅ **Smart Triggering**
- Triggers on changes to specific package source code
- Triggers on pipeline configuration changes
- Manual dispatch with optional force-publish

## Configuration

### Required Secrets

To enable NuGet publishing, configure the following repository secret:

- **`NUGET_API_KEY`** - Your NuGet.org API key for publishing packages

### Environment Variables

The pipelines use these environment variables for better performance:
- `DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1`
- `DOTNET_NOLOGO=true`
- `DOTNET_CLI_TELEMETRY_OPTOUT=1`

## Usage

### Automatic Triggering

Pipelines automatically trigger on:

1. **Push to main/develop branches** - when package source code changes
2. **Pull requests** - for validation before merging
3. **Schedule** - nightly builds for the complete solution (framebox-all.yml)

### Manual Triggering

You can manually trigger pipelines from the GitHub Actions tab:

1. Go to the **Actions** tab in the repository
2. Select the pipeline you want to run
3. Click **"Run workflow"**
4. Optionally enable **"Force publish"** to publish even from non-main branches

### Adding New Packages

To add a new FrameBox package to the pipeline system:

1. **Create the project** in `src/framebox/YourNewPackage/`

2. **Create a pipeline file** `pipelines/framebox/framebox-yournewpackage.yml`:

```yaml
name: FrameBox.YourNewPackage Pipeline

on:
  push:
    branches: [ main, develop ]
    paths:
      - 'src/framebox/FrameBox.YourNewPackage/**'
      - 'pipelines/framebox/framebox-yournewpackage.yml'
      - 'pipelines/framebox/templates/**'
  pull_request:
    branches: [ main, develop ]
    paths:
      - 'src/framebox/FrameBox.YourNewPackage/**'
      - 'pipelines/framebox/framebox-yournewpackage.yml'
      - 'pipelines/framebox/templates/**'
  workflow_dispatch:
    inputs:
      force-publish:
        description: 'Force publish to NuGet even on non-main branch'
        required: false
        type: boolean
        default: false

jobs:
  build-and-test:
    uses: ./.github/workflows/dotnet-package.yml
    with:
      project-path: 'src/framebox/FrameBox.YourNewPackage/FrameBox.YourNewPackage.csproj'
      package-name: 'FrameBox.YourNewPackage'
      dotnet-version: '9.0.x'
      run-tests: true  # Set to true if you have tests
      test-project-path: 'tests/FrameBox.YourNewPackage.Tests/FrameBox.YourNewPackage.Tests.csproj'  # If tests exist
    secrets:
      NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}
```

3. **Copy the pipeline to GitHub workflows**:
```bash
cp pipelines/framebox/framebox-yournewpackage.yml .github/workflows/
```

4. **Update `framebox-all.yml`** to include your new package in the integration pipeline

5. **Add to solution file** `src/framebox/Framebox.sln`

### Package Configuration

Ensure your `.csproj` file includes proper package metadata:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    
    <!-- Package Information -->
    <PackageId>FrameBox.YourPackage</PackageId>
    <Version>1.0.0</Version>
    <Authors>Your Name</Authors>
    <Description>Description of your package</Description>
    <PackageTags>framebox;your;tags</PackageTags>
    <RepositoryUrl>https://github.com/alansouls/FrameBox</RepositoryUrl>
    
    <!-- Generate package on build -->
    <GeneratePackageOnBuild>false</GeneratePackageOnBuild>
    
    <!-- Include symbols for debugging -->
    <IncludeSymbols>true</IncludeSymbols>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>
  </PropertyGroup>
</Project>
```

## Pipeline Status

You can check the status of all pipelines in the repository:

- **Individual packages**: Each package has its own pipeline status badge
- **All packages**: The `framebox-all.yml` provides overall health status
- **Build artifacts**: Available in the Actions tab under each successful run

## Troubleshooting

### Common Issues

1. **Build failures**: Check the build logs for compilation errors
2. **Test failures**: Review test output in the pipeline logs
3. **Publishing failures**: Verify `NUGET_API_KEY` secret is configured
4. **Cache issues**: Manually clear cache or wait for cache expiration

### Local Testing

Test your changes locally before pushing:

```bash
# Navigate to FrameBox solution
cd src/framebox

# Restore dependencies
dotnet restore

# Build all packages
dotnet build --configuration Release

# Run tests (if any)
dotnet test --configuration Release

# Create packages
dotnet pack --configuration Release --output ./packages
```

## Contributing

When contributing to FrameBox packages:

1. Follow the existing project structure
2. Include appropriate tests when adding new features
3. Update package version numbers appropriately
4. Test locally before creating pull requests
5. Update documentation if needed

---

*This pipeline system follows GitHub Actions best practices and is designed for scalability and maintainability.*