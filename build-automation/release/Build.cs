using JetBrains.Annotations;
using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using System.IO;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
public partial class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.UpdateChangeLog);

    public event Action BuildInitialized;

    partial void OnGitFlowInitialized();

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = Configuration.Release;

    AbsolutePath ChangeLogFile => RootDirectory / "CHANGELOG.md";

    [Parameter("Build Tool Parameters")]
    readonly string BuildToolParameters;

    [Solution]
    readonly Solution Solution;

    [GitRepository]
    [UsedImplicitly]
    readonly GitRepository GitRepository;

    readonly GitFlow GitFlow;

    [LocalExecutable("./build.ps1")]
    Tool BuildScript;

    public Build()
    {
        GitFlow = new GitFlow(this);
    }

    protected override void OnBuildInitialized()
    {
        base.OnBuildInitialized();
        BuildInitialized?.Invoke();
        OnGitFlowInitialized();

        if (IsWin)
        {
            BuildScript = ToolResolver.GetLocalTool(RootDirectory / "build.cmd");
        }
        else
        {
            BuildScript = ToolResolver.GetLocalTool(RootDirectory / "build.sh");
        }
    }


    AbsolutePath ArtifactsDirectory => RootDirectory / "output";
    AbsolutePath ArtifactsArchiveDirectory => RootDirectory / "build-artefacts";

    Target Clean => _ =>
        _.Description("Removes all previous build artefacts from the archive directory")
         .Executes(() =>
         {
             RootDirectory.GlobDirectories("bin", "obj").ForEach(DeleteDirectory);
             EnsureCleanDirectory(ArtifactsDirectory);
             EnsureCleanDirectory(ArtifactsArchiveDirectory);
         });


    Target PrepareReleaseStagingBranch => _ =>
        _.Description("Switches the repository to the current release staging branch - or creates one if needed.")
         .Requires(() => GitFlow.EnsureNoUncommittedChanges())
         .Executes(() =>
         {
             var state = GetOrCreateBuildState();
             GitFlow.PrepareStagingBranch(state);
         });

    Target BuildStagingBuild => _ =>
        _.Description("Invokes the builds script for a staging build to validate that the build passes and all tests run without errors. This is a pre-requisite for a release.")
         .DependsOn(PrepareReleaseStagingBranch)
         .Requires(() => GitFlow.EnsureOnReleaseStagingBranch(GetOrCreateBuildState()))
         .Executes(() =>
         {
             var state = GetOrCreateBuildState();
             GitFlow.AttemptStagingBuild(state, x => PerformBuild(BuildType.Staging));
         });


    Target Release => _ =>
        _.DependsOn(BuildStagingBuild)
         .Executes(PerformRelease);
    
    void PerformRelease()
    {
        var state = GetOrCreateBuildState();
        var releaseId = Guid.NewGuid();
        var releaseBranchTag = "_release-state-" + releaseId;
        var stagingBranchTag = "_staging-state-" + releaseId;
        
        GitFlow.EnsureOnReleaseStagingBranch(state);
        
        GitTools.Tag(stagingBranchTag, state.ReleaseStagingBranch);

        try
        {
            if (TryPrepareChangeLogForRelease(state, out var sectionFile))
            {
                GitTools.Commit($"Updated change log for release {state.Version.MajorMinorPatch}");
            }
            
            
            // record the current master branch state.
            // We will use that later to potentially undo any changes we made during that build.
            GitTools.Tag(releaseBranchTag, state.ReleaseTargetBranch);

            try
            {
                // this takes the current staging branch state and merges it into
                // the release branch (usually master). This changes the active 
                // branch of the working copy.
                GitTools.MergeRelease(state.ReleaseTargetBranch, state.ReleaseStagingBranch);

                // attempt to build the release again.
                GitFlow.AttemptStagingBuild(state, x => PerformBuild(BuildType.Release, sectionFile));
                BuildScript($"upload --configuration {Configuration} --package-release-notes-file {sectionFile} {BuildToolParameters}");
            }
            catch
            {
                Logger.Error("Error: Unable to build the release on the release branch. Attempting to roll back changes on release branch.");
                GitTools.Reset(GitTools.ResetType.Hard, releaseBranchTag);

            }
            finally
            {
                GitTools.DeleteTag(releaseBranchTag);
            }
        }
        catch
        {
            // In case of errors, roll back all commits and restore the current state 
            // to be back on the release-staging branch.
            GitTools.Checkout(stagingBranchTag);
            GitTools.ResetBranch(state.ReleaseStagingBranch, stagingBranchTag);
        }
        finally
        {
            GitTools.DeleteTag(stagingBranchTag);
        }
        
    }

    bool TryPrepareChangeLogForRelease(BuildState state, out string sectionFile)
    {
        if (!File.Exists(ChangeLogFile))
        {
            sectionFile = default;
            return false;
        }

        var (cl, section) = ChangeLogGenerator.UpdateChangeLogFile(ChangeLogFile, state.VersionTag, state.ReleaseTargetBranch);
        File.WriteAllText(ChangeLogFile, cl);
        sectionFile = Path.GetTempFileName();
        File.WriteAllText(sectionFile, section);
        return true;
    }

    Target UpdateChangeLog => _ =>
        _.Executes(() =>
        {
            var state = GetOrCreateBuildState();
            var (cl, section) = ChangeLogGenerator.UpdateChangeLogFile(ChangeLogFile, state.VersionTag, state.ReleaseTargetBranch);
            File.WriteAllText(ChangeLogFile, cl);
        });


    void PerformBuild(BuildType type, string changeLogSection = null)
    {
        if (changeLogSection != null)
        {
            BuildScript($"default --configuration {Configuration} --package-release-notes-file {changeLogSection.DoubleQuoteIfNeeded()} {BuildToolParameters}");
        }
        else
        {
            BuildScript($"default --configuration {Configuration} {BuildToolParameters}");
        }
    }

    enum BuildType
    {
        Staging,
        Release
    }
}
