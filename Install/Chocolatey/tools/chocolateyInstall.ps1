$packageName = 'markdownmonster'
$fileType = 'exe'
$url = 'https://github.com/RickStrahl/MarkdownMonster/raw/master/Install/Builds/Releases/MarkdownMonsterSetup-1.0.exe'
$silentArgs = '/SILENT'
$validExitCodes = @(0)


Install-ChocolateyPackage "packageName" "$fileType" "$silentArgs" "$url"  -validExitCodes  $validExitCodes  -checksum "5F9E4F95AB97546C1471C260BB5AF66F40D04CD5D5492BC6F3D794CABE093A02" -checksumType "sha256"
