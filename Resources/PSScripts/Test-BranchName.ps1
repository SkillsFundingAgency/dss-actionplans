[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$BranchName
)

# --- RegEx Patterns
$V1MasterBranchRegEx = "^master$"
$V1FeatureBranchRegEx = "^CDS-\d+-\w+[-v\d]{0}$"

$V2OrHigherMasterBranchRegEx = "^master-v\d$"
$V2OrHigherFeatureBranchRegEx = "^CDS-\d+-\w+-v\d$"

if ($BranchName -match $V1MasterBranchRegEx) {

    Write-Verbose -Message "$BranchName is a a version 1 master branch"
    Write-Host "##vso[task.setvariable variable=FunctionAppVersion;isOutput=true]Version1"

}
elseif ($BranchName -match $V1FeatureBranchRegEx) {

    Write-Verbose -Message "$BranchName is a version 1 feature branch"
    Write-Host "##vso[task.setvariable variable=FunctionAppVersion;isOutput=true]Version1"

}
elseif ($BranchName -match $V2OrHigherMasterBranchRegEx) {
    
    Write-Verbose -Message "$BranchName is a version 2 or higher master branch"
    Write-Host "##vso[task.setvariable variable=FunctionAppVersion;isOutput=true]Version2+"

}
elseif ($BranchName -match $V2OrHigherFeatureBranchRegEx) {

    Write-Verbose -Message "$BranchName is a version 2 or higher feature branch"
    Write-Host "##vso[task.setvariable variable=FunctionAppVersion;isOutput=true]Version2+"

}
else {

    throw "$BranchName doesn't conform to the branch naming rules and will not build"
}

