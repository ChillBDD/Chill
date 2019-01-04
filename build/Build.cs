using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Tools.Xunit;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

public class Build : NukeBuild
{
    public static int Main()
    {
        return Execute<Build>(x => x.Package);
    }

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly string Configuration = IsLocalBuild ? "Debug" : "Release";

    [Solution("Chill.sln")] 
    readonly Solution Solution;
        
    [GitVersion] 
    readonly GitVersion GitVersion;

    [PackageExecutable("ILRepack", "ilrepack.exe")]
    readonly Tool ILRepack;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Executes(() =>
        {
            DeleteDirectories(GlobDirectories(SourceDirectory, "**/bin", "**/obj"));
            DeleteDirectories(GlobDirectories(TestsDirectory, "**/bin", "**/obj"));
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
                .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target UnitTests => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(TestsDirectory / "Chill.Specs/Chill.Specs.csproj")
                .SetConfiguration(Configuration)
                .SetNoBuild(true)
                );
        });

    Target Package => _ => _
        .DependsOn(UnitTests)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetNoBuild(true)
                .SetProject(SourceDirectory / "Chill/Chill.csproj")
                .SetConfiguration(Configuration)
                .SetFramework("netstandard1.1"));
            
            DotNetPublish(s => s
                .SetNoBuild(true)
                .SetProject(SourceDirectory / "Chill/Chill.csproj")
                .SetConfiguration(Configuration)
                .SetFramework("netstandard2.0"));

            ILRepack($"/out:{ArtifactsDirectory}\\netstandard1.1\\Chill.dll /xmldocs " +
                     $"{SourceDirectory}\\Chill\\bin/{Configuration}\\netstandard1.1\\publish\\Chill.dll " +
                     $"{SourceDirectory}\\Chill\\bin/{Configuration}\\netstandard1.1\\publish\\Autofac.dll ");

            ILRepack($"/out:{ArtifactsDirectory}\\netstandard2.0\\Chill.dll /xmldocs " +
                     $"{SourceDirectory}\\Chill\\bin/{Configuration}\\netstandard2.0\\publish\\Chill.dll " +
                     $"{SourceDirectory}\\Chill\\bin/{Configuration}\\netstandard2.0\\publish\\Autofac.dll ");

            NuGetTasks.NuGetPack(SourceDirectory / "Chill/.nuspec", GitVersion.NuGetVersionV2, s => s
                .SetBasePath(ArtifactsDirectory)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetBuild(false));
        });
}