;NSIS Modern User Interface
;Basic Example Script
;Written by Joost Verburg

;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"

!define /date TIMESTAMPCRAP "%Y%m%d_%H%M"
;--------------------------------
;General

  ;Name and file
  Name "OSL Admin"
  OutFile "OSLAdmin.${TIMESTAMPCRAP}.Update.exe"
  Unicode True

  ;Default installation folder
  InstallDir "$LOCALAPPDATA\OpenSoftwareLauncher.DesktopWinForms"
  
  ;Get installation folder from registry if available
  InstallDirRegKey HKCU "Software\OpenSoftwareLauncher_DesktopWinForms" ""

  ;Request application privileges for Windows Vista
  RequestExecutionLevel user

;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING

;--------------------------------
;Pages

  ;!insertmacro MUI_PAGE_LICENSE "${NSISDIR}\Docs\Modern UI\License.txt"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  
;--------------------------------
;Languages
 
  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Macros

; ################################################################
; appends \ to the path if missing
!macro GetCleanDir INPUTDIR
  !define Index_GetCleanDir 'GetCleanDir_Line${__LINE__}'
  Push $R0
  Push $R1
  StrCpy $R0 "${INPUTDIR}"
  StrCmp $R0 "" ${Index_GetCleanDir}-finish
  StrCpy $R1 "$R0" "" -1
  StrCmp "$R1" "\" ${Index_GetCleanDir}-finish
  StrCpy $R0 "$R0\"
${Index_GetCleanDir}-finish:
  Pop $R1
  Exch $R0
  !undef Index_GetCleanDir
!macroend
 
; ################################################################
; similar to "RMDIR /r DIRECTORY", but does not remove DIRECTORY itself
!macro RemoveFilesAndSubDirs DIRECTORY
  !define Index_RemoveFilesAndSubDirs 'RemoveFilesAndSubDirs_${__LINE__}'
 
  Push $R0
  Push $R1
  Push $R2
 
  !insertmacro GetCleanDir "${DIRECTORY}"
  Pop $R2
  FindFirst $R0 $R1 "$R2*.*"
${Index_RemoveFilesAndSubDirs}-loop:
  StrCmp $R1 "" ${Index_RemoveFilesAndSubDirs}-done
  StrCmp $R1 "." ${Index_RemoveFilesAndSubDirs}-next
  StrCmp $R1 ".." ${Index_RemoveFilesAndSubDirs}-next
  IfFileExists "$R2$R1\*.*" ${Index_RemoveFilesAndSubDirs}-directory
  ; file
  Delete "$R2$R1"
  goto ${Index_RemoveFilesAndSubDirs}-next
${Index_RemoveFilesAndSubDirs}-directory:
  ; directory
  RMDir /r "$R2$R1"
${Index_RemoveFilesAndSubDirs}-next:
  FindNext $R0 $R1
  Goto ${Index_RemoveFilesAndSubDirs}-loop
${Index_RemoveFilesAndSubDirs}-done:
  FindClose $R0
 
  Pop $R2
  Pop $R1
  Pop $R0
  !undef Index_RemoveFilesAndSubDirs
!macroend

;--------------------------------
;Installer Sections

Section "Desktop Client (required)" SecInstallLauncher

  SectionIn RO
  SetOutPath "$LOCALAPPDATA\OpenSoftwareLauncher.DesktopWinForms"
  
  ;ADD YOUR OWN FILES HERE...
  File    ".\release\*.exe"
  File    ".\release\*.dll"
  File    ".\release\*.exe.config"
  File    ".\release\*.txt"
  File /r ".\release\Locale"
  File    ".\..\..\CHANGELOG.txt"
  File    ".\..\..\LICENSE"
  
  ;Store installation folder
  WriteRegStr HKCU "Software\OpenSoftwareLauncher_DesktopWinForms" "" "$LOCALAPPDATA\OpenSoftwareLauncher.DesktopWinForms"
  
  ;Create uninstaller
  WriteUninstaller "$LOCALAPPDATA\OpenSoftwareLauncher.DesktopWinForms\Uninstall.exe"

SectionEnd

Section "Start Menu Shortcuts" SecMenuShortcut

  CreateDirectory "$SMPROGRAMS\Open Software Launcher"
  CreateDirectory "$SMPROGRAMS\Open Software Launcher\Desktop"
  CreateShortcut "$SMPROGRAMS\Open Software Launcher\Desktop\Uninstall OSL Desktop.lnk" "$LOCALAPPDATA\OpenSoftwareLauncher.DesktopWinForms\Uninstall.exe"
  CreateShortcut "$SMPROGRAMS\Open Software Launcher\Desktop\OSL Desktop.lnk" "$LOCALAPPDATA\OpenSoftwareLauncher.DesktopWinForms\OpenSoftwareLauncher.DesktopWinForms.exe"
  
SectionEnd

;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_SecInstallLauncher ${LANG_ENGLISH} "Desktop Client"
  LangString DESC_SecMenuShortcut ${LANG_ENGLISH} "Create Start Menu Shortcuts"
  LangString MUI_BUTTONTEXT_FINISH ${LANG_ENGLISH} "Close"
  LangString MUI_TEXT_FINISH_INFO_TITLE ${LANG_ENGLISH} "Open Software Launcher Desktop"
  LangString MUI_TEXT_FINISH_REBOOTNOW ${LANG_ENGLISH} "MUI_TEXT_FINISH_REBOOTNOW"
  LangString MUI_TEXT_FINISH_INFO_TEXT ${LANG_ENGLISH} "OSL Desktop has finished installing. Enjoy!"

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SecInstallLauncher} $(DESC_SecInstallLauncher)
    !insertmacro MUI_DESCRIPTION_TEXT ${SecMenuShortcut} $(DESC_SecMenuShortcut)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END
  
;--------------------------------
;Start after Install
  Function LaunchLink
    ExecShell "" "$SMPROGRAMS\Open Software Launcher\Desktop\OSL Desktop.lnk"
  FunctionEnd
  # These indented statements modify settings for MUI_PAGE_FINISH
    !define MUI_FINISHPAGE_NOAUTOCLOSE
    !define MUI_FINISHPAGE_RUN
    !define MUI_FINISHPAGE_RUN_TEXT "Start OSL Desktop"
    !define MUI_FINISHPAGE_RUN_FUNCTION "LaunchLink"
  !insertmacro MUI_PAGE_FINISH

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  Delete "$LOCALAPPDATA\OpenSoftwareLauncher.DesktopWinForms\Uninstall.exe"
  Delete "$LOCALAPPDATA\OpenSoftwareLauncher.DesktopWinForms\*"
  Delete "$SMPROGRAMS\Open Software Launcher\Desktop\*"
  RMDir "$SMPROGRAMS\Open Software Launcher\Desktop"

  RMDir "$LOCALAPPDATA\OpenSoftwareLauncher.DesktopWinForms"

  DeleteRegKey /ifempty HKCU "Software\OpenSoftwareLauncher_DesktopWinForms"

SectionEnd