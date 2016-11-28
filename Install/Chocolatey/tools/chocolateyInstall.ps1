$packageName = 'markdownmonster'
$fileType = 'exe'
$url = 'https://github.com/RickStrahl/MarkdownMonsterReleases/raw/master/v1.0/MarkdownMonsterSetup-1.0.18.exe'

$silentArgs = '/SILENT'
$validExitCodes = @(0)


Install-ChocolateyPackage "packageName" "$fileType" "$silentArgs" "$url"  -validExitCodes  $validExitCodes  -checksum "B4FD0E4997822F351D519BA789376E793C511354558D080DCC37BBEB70A19365" -checksumType "sha256"
