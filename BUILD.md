# Building Steropes.UI

Steropes.UI uses Cake as built orchestration system. The build scripts
here use ``autocake``, a predefined set of build scripts that follows 
the Git-Version strategy. Development of new code happens in the 
``develop`` branch, releases are built in a ``release-xxx`` branch and
when everything is ready, the main code line is merged into ``master``.

The ``build.cake`` file is responsible for building the code, running
tests and creating the NuGet packages.

The ``release.cake`` file is responsible for creating release candidate
branches and for updating version numbers and invoking the build by calling
the ``build.cake`` script.

## Incrementing version numbers

After a release, GitVersion will attempt to increase the minor version
number based on the current major branch version. If you want to force
a new major version, add a single commit with a message that contains
the text "+semver: major". 

## Building the code

``build.cake`` has the following targets

* Build - Runs all the standard tasks needed for the build.

You can also invoke the various stages manually:

* Clean - Removed all build artefacts and resolved packages.
* Compile - Invoke MSBuild to compile the sources into libraries.
* Test - Runs all unit tests. 
* Package - Creates the NuGet packages
* Publish - Publish the built NuGet packages to the current feed.

The following targets also exist, but there should be not much need
to invoke them manually.

* Report - Create human readable test reports
* RestorePackages - Restores all NuGet packages for the project.
* UpdateBuildMetaData - Update the Assembly.cs files with new version 
  information. This uses GitVersion to compute the current version
  number.
  
## Releasing the code

``release.cake`` has the following targets.

Branch management

* Create-Staging-Branch - Creates the current "release-xxx" branch. 
  Use this to manually start a release branch without attempting to
  actually build the release.
  
* Show-Version - Invokes GitVersion and prints the currently computed
  version.

* Validate-Release - Attempt to build a release, but does not try to
  publish the release to master.
* Attempt-Release - Performs a full release, invokes ``build.cake``
  and if successful merges the release into master. If started from
  the ``develop`` branch, it will create a staging branch first. If
  started from a staging branch, it will attempt to continue a previous
  release build. 
  
  If successful, the project is merged into master and rebuilt as 
  master build. When that is successful, the current repository will
  switch back to the ``develop`` branch and the script will update
  the version information for the project.
  
## Manually publishing NuGet files

