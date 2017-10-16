# Alias into build.ps1 to quickly publish an pre-built release

<#

.SYNOPSIS
This is a Powershell script to bootstrap a Cake build.

.DESCRIPTION
This Powershell script will download NuGet if missing, restore NuGet tools (including Cake)
and execute your Cake build script with the parameters you provide.

.PARAMETER Script
The build script to execute.
.PARAMETER Target
The build script target to run.
.PARAMETER Verbosity
Specifies the amount of information to be displayed.
.PARAMETER BuildDir
Directory where to find the build artefacts.
.PARAMETER Version
Version to be pushed. Mandatory.
.PARAMETER ExtraArgs
Remaining arguments are added here.

.LINK
http://cakebuild.net

#>

[CmdletBinding()]
Param(
    [string]$Script = "build.cake",
    
    [string]$Target = "Publish",
    
    [ValidateSet("Quiet", "Minimal", "Normal", "Verbose", "Diagnostic")]
    [string]$Verbosity = "Normal",
    
    [string]$BuildDir = "build-artefacts",
    [Parameter(Position=0,Mandatory=$false,ValueFromRemainingArguments=$true)]
    [string[]]$ExtraArgs
)

DynamicParam {

    if ($BuildDir -eq $Null)
    {
      $BuildDir = "build-artefacts"
    }
    
    $test = "Hello"
    $versionAttribute = New-Object System.Management.Automation.ParameterAttribute
    $versionAttribute.Mandatory = $true
    $versionAttribute.ParameterSetName = "__AllParameterSets"
    $versionAttribute.HelpMessage = "The version of a previously built release."
 
    #create an attributecollection object for the attribute we just created.
    $attributeCollection = new-object System.Collections.ObjectModel.Collection[System.Attribute]
    
    $arrSet = @(Get-ChildItem -Path $BuildDir | Where-Object { return $_.Name -match "^\d.*" } | Foreach-Object {$_.Name})
    Write-Debug("Message $arrSet  - $BuildDir - $test")
    if ($arrSet.Length -eq 0)
    {
      $validateSetAttribute = New-Object System.Management.Automation.ValidateSetAttribute(@(""))
    }
    else
    {
      $validateSetAttribute = New-Object System.Management.Automation.ValidateSetAttribute($arrSet)
    }

    #add our custom attribute
    $attributeCollection.Add($versionAttribute)
    $attributeCollection.Add($validateSetAttribute)
 
    #add our paramater specifying the attribute collection
    $versionParam = New-Object System.Management.Automation.RuntimeDefinedParameter('Version', [String], $attributeCollection)
    
    #expose the name of our parameter
    $paramDictionary = New-Object System.Management.Automation.RuntimeDefinedParameterDictionary
    $paramDictionary.Add('Version', $versionParam)
    return $paramDictionary
}

Begin {
    $Version = $PSBoundParameters["Version"]
}
Process {
 
    $targetArg = "'-targetdir=""$BuildDir/$Version""'"
    $argumentList = @("-Script", $Script, "-Target", $Target, "-Verbosity", $Verbosity, "-ScriptArgs", $targetArg, $ExtraArgs)
    Write-Host "& .\build.ps1 $argumentList"
    Invoke-Expression "& .\build.ps1 $argumentList"
    exit $LASTEXITCODE
}
