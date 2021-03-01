; Script generated by the HM NIS Edit Script Wizard.

; HM NIS Edit Wizard helper defines
!define PRODUCT_NAME "libsecondlife"
!define PRODUCT_MAJOR "0"
!define PRODUCT_MINOR "4"
!define PRODUCT_RELEASE "0"
!define PRODUCT_VERSION "${PRODUCT_MAJOR}.${PRODUCT_MINOR}.${PRODUCT_RELEASE}"
!define PRODUCT_SETUP_NAME "libsecondlife-${PRODUCT_MAJOR}_${PRODUCT_MINOR}_${PRODUCT_RELEASE}-installer.exe"
!define PRODUCT_PUBLISHER "libsecondlife reverse engineering team"
!define PRODUCT_WEB_SITE "http://www.libsecondlife.org"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\libsecondlife.dll"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define PRODUCT_STARTMENU_REGVAL "NSIS:StartMenuDir"

SetCompressor lzma

; MUI 1.67 compatible ------
!include "MUI.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!insertmacro MUI_PAGE_LICENSE "..\LICENSE.txt"
; Directory page
!insertmacro MUI_PAGE_DIRECTORY
; Start menu page
var ICONS_GROUP
!define MUI_STARTMENUPAGE_NODISABLE
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "libsecondlife"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "${PRODUCT_UNINST_ROOT_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "${PRODUCT_UNINST_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "${PRODUCT_STARTMENU_REGVAL}"
!insertmacro MUI_PAGE_STARTMENU Application $ICONS_GROUP
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!define MUI_FINISHPAGE_SHOWREADME "$INSTDIR\README.txt"
;!define MUI_FINISHPAGE_RUN "$INSTDIR\README.txt"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"

; Reserve files
!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS

; MUI end ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "${PRODUCT_SETUP_NAME}"
InstallDir "$PROGRAMFILES\libsecondlife"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show

Section "MainSection" SEC01
  SetOutPath "$INSTDIR"
  SetOverwrite try
  File "..\bin\ChatConsole.exe"
  File "..\bin\Decoder.exe"
  File "..\bin\groupmanager.exe"
  File "..\bin\GUITestClient.exe"
  File "..\bin\Heightmap.exe"
  File "..\bin\importprimscript.exe"
  File "..\bin\key2name.exe"
  File "..\bin\libsecondlife.dll"
  File "..\bin\libsecondlife.dll.config"
  File "..\bin\libsecondlife.Utilities.dll"
  SetOutPath "$INSTDIR\libsl_data"
  File "..\bin\libsl_data\avatar_lad.xml"
  File "..\bin\libsl_data\blush_alpha.tga"
  File "..\bin\libsl_data\bodyfreckles_alpha.tga"
  File "..\bin\libsl_data\body_skingrain.tga"
  File "..\bin\libsl_data\bump_face_wrinkles.tga"
  File "..\bin\libsl_data\bump_head_base.tga"
  File "..\bin\libsl_data\bump_lowerbody_base.tga"
  File "..\bin\libsl_data\bump_pants_wrinkles.tga"
  File "..\bin\libsl_data\bump_shirt_wrinkles.tga"
  File "..\bin\libsl_data\bump_upperbody_base.tga"
  File "..\bin\libsl_data\eyebrows_alpha.tga"
  File "..\bin\libsl_data\eyeliner_alpha.tga"
  File "..\bin\libsl_data\eyeshadow_inner_alpha.tga"
  File "..\bin\libsl_data\eyeshadow_outer_alpha.tga"
  File "..\bin\libsl_data\eyewhite.tga"
  File "..\bin\libsl_data\facehair_chincurtains_alpha.tga"
  File "..\bin\libsl_data\facehair_moustache_alpha.tga"
  File "..\bin\libsl_data\facehair_sideburns_alpha.tga"
  File "..\bin\libsl_data\facehair_soulpatch_alpha.tga"
  File "..\bin\libsl_data\freckles_alpha.tga"
  File "..\bin\libsl_data\gloves_fingers_alpha.tga"
  File "..\bin\libsl_data\glove_length_alpha.tga"
  File "..\bin\libsl_data\head_alpha.tga"
  File "..\bin\libsl_data\head_color.tga"
  File "..\bin\libsl_data\head_hair.tga"
  File "..\bin\libsl_data\head_highlights_alpha.tga"
  File "..\bin\libsl_data\head_shading_alpha.tga"
  File "..\bin\libsl_data\head_skingrain.tga"
  File "..\bin\libsl_data\jacket_length_lower_alpha.tga"
  File "..\bin\libsl_data\jacket_length_upper_alpha.tga"
  File "..\bin\libsl_data\jacket_open_lower_alpha.tga"
  File "..\bin\libsl_data\jacket_open_upper_alpha.tga"
  File "..\bin\libsl_data\lipgloss_alpha.tga"
  File "..\bin\libsl_data\lipstick_alpha.tga"
  File "..\bin\libsl_data\lips_mask.tga"
  File "..\bin\libsl_data\lowerbody_color.tga"
  File "..\bin\libsl_data\lowerbody_highlights_alpha.tga"
  File "..\bin\libsl_data\lowerbody_shading_alpha.tga"
  File "..\bin\libsl_data\nailpolish_alpha.tga"
  File "..\bin\libsl_data\pants_length_alpha.tga"
  File "..\bin\libsl_data\pants_waist_alpha.tga"
  File "..\bin\libsl_data\rosyface_alpha.tga"
  File "..\bin\libsl_data\rouge_alpha.tga"
  File "..\bin\libsl_data\shirt_bottom_alpha.tga"
  File "..\bin\libsl_data\shirt_collar_alpha.tga"
  File "..\bin\libsl_data\shirt_collar_back_alpha.tga"
  File "..\bin\libsl_data\shirt_sleeve_alpha.tga"
  File "..\bin\libsl_data\shoe_height_alpha.tga"
  File "..\bin\libsl_data\skirt_length_alpha.tga"
  File "..\bin\libsl_data\skirt_slit_back_alpha.tga"
  File "..\bin\libsl_data\skirt_slit_front_alpha.tga"
  File "..\bin\libsl_data\skirt_slit_left_alpha.tga"
  File "..\bin\libsl_data\skirt_slit_right_alpha.tga"
  File "..\bin\libsl_data\underpants_trial_female.tga"
  File "..\bin\libsl_data\underpants_trial_male.tga"
  File "..\bin\libsl_data\undershirt_trial_female.tga"
  File "..\bin\libsl_data\upperbodyfreckles_alpha.tga"
  File "..\bin\libsl_data\upperbody_color.tga"
  File "..\bin\libsl_data\upperbody_highlights_alpha.tga"
  File "..\bin\libsl_data\upperbody_shading_alpha.tga"
  SetOutPath "$INSTDIR"
  File "..\bin\name2key.exe"
  File "..\bin\openjpeg-libsl.dll"
  File "..\bin\slaccountant.exe"
  File "..\bin\sldump.exe"
  File "..\bin\SLImageUpload.exe"
  File "..\bin\SLProxy.exe"
  File "..\bin\TestClient.exe"
  File "..\bin\VoiceTest.exe"
  SetOverwrite ifnewer
  File "..\LICENSE.txt"
  CreateShortCut "$ICONS_GROUP.lnk" "$INSTDIR\LICENSE.txt"
  File "..\README.txt"
  CreateShortCut "$ICONS_GROUP.lnk" "$INSTDIR\README.txt"
SectionEnd

Section -AdditionalIcons
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
  WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\Website.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\Uninstall.lnk" "$INSTDIR\uninst.exe"
  !insertmacro MUI_STARTMENU_WRITE_END
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\libsecondlife.dll"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\libsecondlife.dll"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd


Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully uninstalled."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Do you want to uninstall $(^Name) with all its components?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  Delete "$INSTDIR\${PRODUCT_NAME}.url"
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\README.txt"
  Delete "$INSTDIR\LICENSE.txt"
  Delete "$INSTDIR\VoiceTest.exe"
  Delete "$INSTDIR\TestClient.exe"
  Delete "$INSTDIR\SLProxy.exe"
  Delete "$INSTDIR\SLImageUpload.exe"
  Delete "$INSTDIR\sldump.exe"
  Delete "$INSTDIR\slaccountant.exe"
  Delete "$INSTDIR\openjpeg-libsl.dll"
  Delete "$INSTDIR\name2key.pdb"
  Delete "$INSTDIR\name2key.exe"
  Delete "$INSTDIR\libsl_data\upperbody_shading_alpha.tga"
  Delete "$INSTDIR\libsl_data\upperbody_highlights_alpha.tga"
  Delete "$INSTDIR\libsl_data\upperbody_color.tga"
  Delete "$INSTDIR\libsl_data\upperbodyfreckles_alpha.tga"
  Delete "$INSTDIR\libsl_data\undershirt_trial_female.tga"
  Delete "$INSTDIR\libsl_data\underpants_trial_male.tga"
  Delete "$INSTDIR\libsl_data\underpants_trial_female.tga"
  Delete "$INSTDIR\libsl_data\skirt_slit_right_alpha.tga"
  Delete "$INSTDIR\libsl_data\skirt_slit_left_alpha.tga"
  Delete "$INSTDIR\libsl_data\skirt_slit_front_alpha.tga"
  Delete "$INSTDIR\libsl_data\skirt_slit_back_alpha.tga"
  Delete "$INSTDIR\libsl_data\skirt_length_alpha.tga"
  Delete "$INSTDIR\libsl_data\shoe_height_alpha.tga"
  Delete "$INSTDIR\libsl_data\shirt_sleeve_alpha.tga"
  Delete "$INSTDIR\libsl_data\shirt_collar_back_alpha.tga"
  Delete "$INSTDIR\libsl_data\shirt_collar_alpha.tga"
  Delete "$INSTDIR\libsl_data\shirt_bottom_alpha.tga"
  Delete "$INSTDIR\libsl_data\rouge_alpha.tga"
  Delete "$INSTDIR\libsl_data\rosyface_alpha.tga"
  Delete "$INSTDIR\libsl_data\pants_waist_alpha.tga"
  Delete "$INSTDIR\libsl_data\pants_length_alpha.tga"
  Delete "$INSTDIR\libsl_data\nailpolish_alpha.tga"
  Delete "$INSTDIR\libsl_data\lowerbody_shading_alpha.tga"
  Delete "$INSTDIR\libsl_data\lowerbody_highlights_alpha.tga"
  Delete "$INSTDIR\libsl_data\lowerbody_color.tga"
  Delete "$INSTDIR\libsl_data\lips_mask.tga"
  Delete "$INSTDIR\libsl_data\lipstick_alpha.tga"
  Delete "$INSTDIR\libsl_data\lipgloss_alpha.tga"
  Delete "$INSTDIR\libsl_data\jacket_open_upper_alpha.tga"
  Delete "$INSTDIR\libsl_data\jacket_open_lower_alpha.tga"
  Delete "$INSTDIR\libsl_data\jacket_length_upper_alpha.tga"
  Delete "$INSTDIR\libsl_data\jacket_length_lower_alpha.tga"
  Delete "$INSTDIR\libsl_data\head_skingrain.tga"
  Delete "$INSTDIR\libsl_data\head_shading_alpha.tga"
  Delete "$INSTDIR\libsl_data\head_highlights_alpha.tga"
  Delete "$INSTDIR\libsl_data\head_hair.tga"
  Delete "$INSTDIR\libsl_data\head_color.tga"
  Delete "$INSTDIR\libsl_data\head_alpha.tga"
  Delete "$INSTDIR\libsl_data\glove_length_alpha.tga"
  Delete "$INSTDIR\libsl_data\gloves_fingers_alpha.tga"
  Delete "$INSTDIR\libsl_data\freckles_alpha.tga"
  Delete "$INSTDIR\libsl_data\facehair_soulpatch_alpha.tga"
  Delete "$INSTDIR\libsl_data\facehair_sideburns_alpha.tga"
  Delete "$INSTDIR\libsl_data\facehair_moustache_alpha.tga"
  Delete "$INSTDIR\libsl_data\facehair_chincurtains_alpha.tga"
  Delete "$INSTDIR\libsl_data\eyewhite.tga"
  Delete "$INSTDIR\libsl_data\eyeshadow_outer_alpha.tga"
  Delete "$INSTDIR\libsl_data\eyeshadow_inner_alpha.tga"
  Delete "$INSTDIR\libsl_data\eyeliner_alpha.tga"
  Delete "$INSTDIR\libsl_data\eyebrows_alpha.tga"
  Delete "$INSTDIR\libsl_data\bump_upperbody_base.tga"
  Delete "$INSTDIR\libsl_data\bump_shirt_wrinkles.tga"
  Delete "$INSTDIR\libsl_data\bump_pants_wrinkles.tga"
  Delete "$INSTDIR\libsl_data\bump_lowerbody_base.tga"
  Delete "$INSTDIR\libsl_data\bump_head_base.tga"
  Delete "$INSTDIR\libsl_data\bump_face_wrinkles.tga"
  Delete "$INSTDIR\libsl_data\body_skingrain.tga"
  Delete "$INSTDIR\libsl_data\bodyfreckles_alpha.tga"
  Delete "$INSTDIR\libsl_data\blush_alpha.tga"
  Delete "$INSTDIR\libsl_data\avatar_lad.xml"
  Delete "$INSTDIR\libsecondlife.Utilities.dll"
  Delete "$INSTDIR\libsecondlife.dll.config"
  Delete "$INSTDIR\libsecondlife.dll"
  Delete "$INSTDIR\key2name.pdb"
  Delete "$INSTDIR\key2name.exe"
  Delete "$INSTDIR\importprimscript.exe"
  Delete "$INSTDIR\Heightmap.exe"
  Delete "$INSTDIR\GUITestClient.exe"
  Delete "$INSTDIR\groupmanager.exe"
  Delete "$INSTDIR\Decoder.exe"
  Delete "$INSTDIR\ChatConsole.exe"

  Delete "$SMPROGRAMS\libsecondlife\Uninstall.lnk"
  Delete "$SMPROGRAMS\libsecondlife\Website.lnk"
  Delete "$ICONS_GROUP.lnk"

  RMDir "$SMPROGRAMS\libsecondlife"
  RMDir "$INSTDIR\libsl_data"
  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd
