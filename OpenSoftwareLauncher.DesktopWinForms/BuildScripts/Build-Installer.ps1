& "C:\Program Files (x86)\NSIS\makensis.exe" setup.nsi /V4
Write-Output "============================= SHA256 Sum"
$file = Get-ChildItem *.Update.Exe | sort LastWriteTime | select -last 1
$out = (Get-FileHash $file -Algorithm SHA256)
$fname = $file.Name
$fhash = $out.Hash
Write-Output "=== ${fname}"
Write-Output "${fhash}"
pause