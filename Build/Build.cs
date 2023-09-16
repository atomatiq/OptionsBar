using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.GitVersion;

sealed partial class Build : NukeBuild
{
    string[] Configurations;
    Dictionary<Project, Project> InstallersMap;

    [Parameter] string GitHubToken;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion(NoFetch = true)] readonly GitVersion GitVersion;
    [Solution(GenerateProjects = true)] Solution Solution;

    public static int Main() => Execute<Build>(x => x.Clean);
}