# Microsoft Developer Studio Generated NMAKE File, Format Version 40001
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Application" 0x0101

!IF "$(CFG)" == ""
CFG=PLC_SCAN - Win32 Debug
!MESSAGE No configuration specified.  Defaulting to PLC_SCAN - Win32 Debug.
!ENDIF 

!IF "$(CFG)" != "PLC_SCAN - Win32 Release" && "$(CFG)" !=\
 "PLC_SCAN - Win32 Debug"
!MESSAGE Invalid configuration "$(CFG)" specified.
!MESSAGE You can specify a configuration when running NMAKE on this makefile
!MESSAGE by defining the macro CFG on the command line.  For example:
!MESSAGE 
!MESSAGE NMAKE /f "PLC_SCAN.mak" CFG="PLC_SCAN - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "PLC_SCAN - Win32 Release" (based on "Win32 (x86) Application")
!MESSAGE "PLC_SCAN - Win32 Debug" (based on "Win32 (x86) Application")
!MESSAGE 
!ERROR An invalid configuration is specified.
!ENDIF 

!IF "$(OS)" == "Windows_NT"
NULL=
!ELSE 
NULL=nul
!ENDIF 
################################################################################
# Begin Project
# PROP Target_Last_Scanned "PLC_SCAN - Win32 Debug"
CPP=cl.exe
RSC=rc.exe
MTL=mktyplib.exe

!IF  "$(CFG)" == "PLC_SCAN - Win32 Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Release"
# PROP BASE Intermediate_Dir "Release"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "d:\win\cat16\exe"
# PROP Intermediate_Dir "d:\obj\win95\plc_scan"
# PROP Target_Dir ""
OUTDIR=d:\win\cat16\exe
INTDIR=d:\obj\win95\plc_scan

ALL : "$(OUTDIR)\PLC_SCAN.exe"

CLEAN : 
	-@erase "..\exe\PLC_SCAN.exe"
	-@erase "..\..\..\obj\win95\plc_scan\user.obj"
	-@erase "..\..\..\obj\win95\plc_scan\PT-L.obj"
	-@erase "..\..\..\obj\win95\plc_scan\PM-B.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Scanmain.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Com-232.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Tmtc.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Scanenum.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Wr3380.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Dpm2.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Glofa.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Abplc5.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Pro_main.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Pm_170e.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Workasci.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Omron.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Gantner.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Commmain.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Mf2.obj"
	-@erase "..\..\..\obj\win95\plc_scan\SYS-K017.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Comtcpip.obj"
	-@erase "..\..\..\obj\win95\plc_scan\adam.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Scanmsg.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Autobase.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Mj71e71.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Scanwork.obj"
	-@erase "..\..\..\obj\win95\plc_scan\La250.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Master-k.obj"
	-@erase "..\..\..\obj\win95\plc_scan\RLink.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Spc-300.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Modicon.obj"
	-@erase "..\..\..\obj\win95\plc_scan\SYS-K016.obj"
	-@erase "..\..\..\obj\win95\plc_scan\sp30.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Plc_conf.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Maxcom.obj"
	-@erase "..\..\..\obj\win95\plc_scan\pro_dde.obj"
	-@erase "..\..\..\obj\win95\plc_scan\ComCard.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Scanfile.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Plcwrite.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Scanconf.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Bl_2300.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Fuji.obj"
	-@erase "..\..\..\obj\win95\plc_scan\VCR.obj"
	-@erase "..\..\..\obj\win95\plc_scan\A0j2c214.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Totalcfg.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Scanstat.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Kdialog.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Pro_lib.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Plc_scan.res"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

"$(INTDIR)" :
    if not exist "$(INTDIR)/$(NULL)" mkdir "$(INTDIR)"

# ADD BASE CPP /nologo /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /YX /c
# ADD CPP /nologo /Zp1 /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /YX /c
CPP_PROJ=/nologo /Zp1 /ML /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS"\
 /Fp"$(INTDIR)/PLC_SCAN.pch" /YX /Fo"$(INTDIR)/" /c 
CPP_OBJS=d:\obj\win95\plc_scan/
CPP_SBRS=
# ADD BASE MTL /nologo /D "NDEBUG" /win32
# ADD MTL /nologo /D "NDEBUG" /win32
MTL_PROJ=/nologo /D "NDEBUG" /win32 
# ADD BASE RSC /l 0x412 /d "NDEBUG"
# ADD RSC /l 0x412 /d "NDEBUG"
RSC_PROJ=/l 0x412 /fo"$(INTDIR)/Plc_scan.res" /d "NDEBUG" 
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
BSC32_FLAGS=/nologo /o"$(OUTDIR)/PLC_SCAN.bsc" 
BSC32_SBRS=
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /machine:I386
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib wsock32.lib /nologo /subsystem:windows /machine:I386
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib\
 advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib\
 odbccp32.lib wsock32.lib /nologo /subsystem:windows /incremental:no\
 /pdb:"$(OUTDIR)/PLC_SCAN.pdb" /machine:I386 /out:"$(OUTDIR)/PLC_SCAN.exe" 
LINK32_OBJS= \
	"$(INTDIR)/user.obj" \
	"$(INTDIR)/PT-L.obj" \
	"$(INTDIR)/PM-B.obj" \
	"$(INTDIR)/Scanmain.obj" \
	"$(INTDIR)/Com-232.obj" \
	"$(INTDIR)/Tmtc.obj" \
	"$(INTDIR)/Scanenum.obj" \
	"$(INTDIR)/Wr3380.obj" \
	"$(INTDIR)/Dpm2.obj" \
	"$(INTDIR)/Glofa.obj" \
	"$(INTDIR)/Abplc5.obj" \
	"$(INTDIR)/Pro_main.obj" \
	"$(INTDIR)/Pm_170e.obj" \
	"$(INTDIR)/Workasci.obj" \
	"$(INTDIR)/Omron.obj" \
	"$(INTDIR)/Gantner.obj" \
	"$(INTDIR)/Commmain.obj" \
	"$(INTDIR)/Mf2.obj" \
	"$(INTDIR)/SYS-K017.obj" \
	"$(INTDIR)/Comtcpip.obj" \
	"$(INTDIR)/adam.obj" \
	"$(INTDIR)/Scanmsg.obj" \
	"$(INTDIR)/Autobase.obj" \
	"$(INTDIR)/Mj71e71.obj" \
	"$(INTDIR)/Scanwork.obj" \
	"$(INTDIR)/La250.obj" \
	"$(INTDIR)/Master-k.obj" \
	"$(INTDIR)/RLink.obj" \
	"$(INTDIR)/Spc-300.obj" \
	"$(INTDIR)/Modicon.obj" \
	"$(INTDIR)/SYS-K016.obj" \
	"$(INTDIR)/sp30.obj" \
	"$(INTDIR)/Plc_conf.obj" \
	"$(INTDIR)/Maxcom.obj" \
	"$(INTDIR)/pro_dde.obj" \
	"$(INTDIR)/ComCard.obj" \
	"$(INTDIR)/Scanfile.obj" \
	"$(INTDIR)/Plcwrite.obj" \
	"$(INTDIR)/Scanconf.obj" \
	"$(INTDIR)/Bl_2300.obj" \
	"$(INTDIR)/Fuji.obj" \
	"$(INTDIR)/VCR.obj" \
	"$(INTDIR)/A0j2c214.obj" \
	"$(INTDIR)/Totalcfg.obj" \
	"$(INTDIR)/Scanstat.obj" \
	"$(INTDIR)/Kdialog.obj" \
	"$(INTDIR)/Pro_lib.obj" \
	"$(INTDIR)/Plc_scan.res" \
	"..\..\Wintools\Lib\Win95\wintools.lib"

"$(OUTDIR)\PLC_SCAN.exe" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ELSEIF  "$(CFG)" == "PLC_SCAN - Win32 Debug"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "Debug"
# PROP BASE Intermediate_Dir "Debug"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "d:\win\cat16\exe"
# PROP Intermediate_Dir "d:\obj\win95\plc_scan"
# PROP Target_Dir ""
OUTDIR=d:\win\cat16\exe
INTDIR=d:\obj\win95\plc_scan

ALL : "$(OUTDIR)\PLC_SCAN.exe"

CLEAN : 
	-@erase "..\..\..\obj\win95\plc_scan\vc40.pdb"
	-@erase "..\..\..\obj\win95\plc_scan\vc40.idb"
	-@erase "..\exe\PLC_SCAN.exe"
	-@erase "..\..\..\obj\win95\plc_scan\user.obj"
	-@erase "..\..\..\obj\win95\plc_scan\PT-L.obj"
	-@erase "..\..\..\obj\win95\plc_scan\PM-B.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Scanmain.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Com-232.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Tmtc.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Scanenum.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Wr3380.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Dpm2.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Glofa.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Abplc5.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Pro_main.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Pm_170e.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Workasci.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Omron.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Gantner.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Commmain.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Mf2.obj"
	-@erase "..\..\..\obj\win95\plc_scan\SYS-K017.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Comtcpip.obj"
	-@erase "..\..\..\obj\win95\plc_scan\adam.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Scanmsg.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Autobase.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Mj71e71.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Scanwork.obj"
	-@erase "..\..\..\obj\win95\plc_scan\La250.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Master-k.obj"
	-@erase "..\..\..\obj\win95\plc_scan\RLink.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Spc-300.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Modicon.obj"
	-@erase "..\..\..\obj\win95\plc_scan\SYS-K016.obj"
	-@erase "..\..\..\obj\win95\plc_scan\sp30.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Plc_conf.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Maxcom.obj"
	-@erase "..\..\..\obj\win95\plc_scan\pro_dde.obj"
	-@erase "..\..\..\obj\win95\plc_scan\ComCard.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Scanfile.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Plcwrite.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Scanconf.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Bl_2300.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Fuji.obj"
	-@erase "..\..\..\obj\win95\plc_scan\VCR.obj"
	-@erase "..\..\..\obj\win95\plc_scan\A0j2c214.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Totalcfg.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Scanstat.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Kdialog.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Pro_lib.obj"
	-@erase "..\..\..\obj\win95\plc_scan\Plc_scan.res"
	-@erase "..\exe\PLC_SCAN.ilk"
	-@erase "..\exe\PLC_SCAN.pdb"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

"$(INTDIR)" :
    if not exist "$(INTDIR)/$(NULL)" mkdir "$(INTDIR)"

# ADD BASE CPP /nologo /W3 /Gm /GX /Zi /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /YX /c
# ADD CPP /nologo /Zp1 /W3 /Gm /GX /Zi /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /YX /c
CPP_PROJ=/nologo /Zp1 /MLd /W3 /Gm /GX /Zi /Od /D "WIN32" /D "_DEBUG" /D\
 "_WINDOWS" /Fp"$(INTDIR)/PLC_SCAN.pch" /YX /Fo"$(INTDIR)/" /Fd"$(INTDIR)/" /c 
CPP_OBJS=d:\obj\win95\plc_scan/
CPP_SBRS=
# ADD BASE MTL /nologo /D "_DEBUG" /win32
# ADD MTL /nologo /D "_DEBUG" /win32
MTL_PROJ=/nologo /D "_DEBUG" /win32 
# ADD BASE RSC /l 0x412 /d "_DEBUG"
# ADD RSC /l 0x412 /d "_DEBUG"
RSC_PROJ=/l 0x412 /fo"$(INTDIR)/Plc_scan.res" /d "_DEBUG" 
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
BSC32_FLAGS=/nologo /o"$(OUTDIR)/PLC_SCAN.bsc" 
BSC32_SBRS=
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /debug /machine:I386
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib wsock32.lib /nologo /subsystem:windows /debug /machine:I386
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib\
 advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib\
 odbccp32.lib wsock32.lib /nologo /subsystem:windows /incremental:yes\
 /pdb:"$(OUTDIR)/PLC_SCAN.pdb" /debug /machine:I386\
 /out:"$(OUTDIR)/PLC_SCAN.exe" 
LINK32_OBJS= \
	"$(INTDIR)/user.obj" \
	"$(INTDIR)/PT-L.obj" \
	"$(INTDIR)/PM-B.obj" \
	"$(INTDIR)/Scanmain.obj" \
	"$(INTDIR)/Com-232.obj" \
	"$(INTDIR)/Tmtc.obj" \
	"$(INTDIR)/Scanenum.obj" \
	"$(INTDIR)/Wr3380.obj" \
	"$(INTDIR)/Dpm2.obj" \
	"$(INTDIR)/Glofa.obj" \
	"$(INTDIR)/Abplc5.obj" \
	"$(INTDIR)/Pro_main.obj" \
	"$(INTDIR)/Pm_170e.obj" \
	"$(INTDIR)/Workasci.obj" \
	"$(INTDIR)/Omron.obj" \
	"$(INTDIR)/Gantner.obj" \
	"$(INTDIR)/Commmain.obj" \
	"$(INTDIR)/Mf2.obj" \
	"$(INTDIR)/SYS-K017.obj" \
	"$(INTDIR)/Comtcpip.obj" \
	"$(INTDIR)/adam.obj" \
	"$(INTDIR)/Scanmsg.obj" \
	"$(INTDIR)/Autobase.obj" \
	"$(INTDIR)/Mj71e71.obj" \
	"$(INTDIR)/Scanwork.obj" \
	"$(INTDIR)/La250.obj" \
	"$(INTDIR)/Master-k.obj" \
	"$(INTDIR)/RLink.obj" \
	"$(INTDIR)/Spc-300.obj" \
	"$(INTDIR)/Modicon.obj" \
	"$(INTDIR)/SYS-K016.obj" \
	"$(INTDIR)/sp30.obj" \
	"$(INTDIR)/Plc_conf.obj" \
	"$(INTDIR)/Maxcom.obj" \
	"$(INTDIR)/pro_dde.obj" \
	"$(INTDIR)/ComCard.obj" \
	"$(INTDIR)/Scanfile.obj" \
	"$(INTDIR)/Plcwrite.obj" \
	"$(INTDIR)/Scanconf.obj" \
	"$(INTDIR)/Bl_2300.obj" \
	"$(INTDIR)/Fuji.obj" \
	"$(INTDIR)/VCR.obj" \
	"$(INTDIR)/A0j2c214.obj" \
	"$(INTDIR)/Totalcfg.obj" \
	"$(INTDIR)/Scanstat.obj" \
	"$(INTDIR)/Kdialog.obj" \
	"$(INTDIR)/Pro_lib.obj" \
	"$(INTDIR)/Plc_scan.res" \
	"..\..\Wintools\Lib\Win95\wintools.lib"

"$(OUTDIR)\PLC_SCAN.exe" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ENDIF 

.c{$(CPP_OBJS)}.obj:
   $(CPP) $(CPP_PROJ) $<  

.cpp{$(CPP_OBJS)}.obj:
   $(CPP) $(CPP_PROJ) $<  

.cxx{$(CPP_OBJS)}.obj:
   $(CPP) $(CPP_PROJ) $<  

.c{$(CPP_SBRS)}.sbr:
   $(CPP) $(CPP_PROJ) $<  

.cpp{$(CPP_SBRS)}.sbr:
   $(CPP) $(CPP_PROJ) $<  

.cxx{$(CPP_SBRS)}.sbr:
   $(CPP) $(CPP_PROJ) $<  

################################################################################
# Begin Target

# Name "PLC_SCAN - Win32 Release"
# Name "PLC_SCAN - Win32 Debug"

!IF  "$(CFG)" == "PLC_SCAN - Win32 Release"

!ELSEIF  "$(CFG)" == "PLC_SCAN - Win32 Debug"

!ENDIF 

################################################################################
# Begin Source File

SOURCE=.\Plc_conf.cpp
DEP_CPP_PLC_C=\
	{$(INCLUDE)}"\Kwl\Kdialog.h"\
	".\Protocol\..\plc_scan.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\Tools.h"\
	

"$(INTDIR)\Plc_conf.obj" : $(SOURCE) $(DEP_CPP_PLC_C) "$(INTDIR)"


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Plc_scan.rc
DEP_RSC_PLC_S=\
	".\mainfram.ico"\
	".\viewmem.ico"\
	".\viewcode.ico"\
	

"$(INTDIR)\Plc_scan.res" : $(SOURCE) $(DEP_RSC_PLC_S) "$(INTDIR)"
   $(RSC) $(RSC_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Plcwrite.cpp
DEP_CPP_PLCWR=\
	{$(INCLUDE)}"\Kwl\Kdialog.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Plcwrite.obj" : $(SOURCE) $(DEP_CPP_PLCWR) "$(INTDIR)"


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Scanconf.cpp
DEP_CPP_SCANC=\
	{$(INCLUDE)}"\Tools.h"\
	".\Protocol\..\plc_scan.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	

"$(INTDIR)\Scanconf.obj" : $(SOURCE) $(DEP_CPP_SCANC) "$(INTDIR)"


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Scanenum.cpp
DEP_CPP_SCANE=\
	".\Protocol\..\plc_scan.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\Tools.h"\
	

"$(INTDIR)\Scanenum.obj" : $(SOURCE) $(DEP_CPP_SCANE) "$(INTDIR)"


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Scanfile.cpp
DEP_CPP_SCANF=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Scanfile.obj" : $(SOURCE) $(DEP_CPP_SCANF) "$(INTDIR)"


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Scanmain.cpp
DEP_CPP_SCANM=\
	{$(INCLUDE)}"\Gclass.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\..\catlib\totalcfg.h"\
	".\..\main\cversion.h"\
	".\..\main\pub_msg.h"\
	".\Protocol\..\plc_scan.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Scanmain.obj" : $(SOURCE) $(DEP_CPP_SCANM) "$(INTDIR)"


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Scanmsg.cpp
DEP_CPP_SCANMS=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	{$(INCLUDE)}"\Dataswap.h"\
	".\Protocol\..\plc_scan.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Scanmsg.obj" : $(SOURCE) $(DEP_CPP_SCANMS) "$(INTDIR)"


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Scanstat.cpp
DEP_CPP_SCANS=\
	{$(INCLUDE)}"\Tools.h"\
	".\..\main\pub_msg.h"\
	".\Protocol\..\plc_scan.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	

"$(INTDIR)\Scanstat.obj" : $(SOURCE) $(DEP_CPP_SCANS) "$(INTDIR)"


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Scanwork.cpp
DEP_CPP_SCANW=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	{$(INCLUDE)}"\Menubutn.h"\
	".\Protocol\..\plc_scan.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Scanwork.obj" : $(SOURCE) $(DEP_CPP_SCANW) "$(INTDIR)"


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Workasci.cpp
DEP_CPP_WORKA=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	{$(INCLUDE)}"\Menubutn.h"\
	{$(INCLUDE)}"\Totaldef.h"\
	".\Protocol\..\plc_scan.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Workasci.obj" : $(SOURCE) $(DEP_CPP_WORKA) "$(INTDIR)"


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Device\Comtcpip.cpp
DEP_CPP_COMTC=\
	{$(INCLUDE)}"\Tools.h"\
	".\Protocol\..\plc_scan.h"\
	".\Device\Commmain.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	

"$(INTDIR)\Comtcpip.obj" : $(SOURCE) $(DEP_CPP_COMTC) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Device\Commmain.cpp
DEP_CPP_COMMM=\
	{$(INCLUDE)}"\Tools.h"\
	".\Device\Commmain.h"\
	".\Protocol\..\plc_scan.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	

"$(INTDIR)\Commmain.obj" : $(SOURCE) $(DEP_CPP_COMMM) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=".\Device\Com-232.cpp"
DEP_CPP_COM_2=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Dataswap.h"\
	".\Device\Commmain.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	

"$(INTDIR)\Com-232.obj" : $(SOURCE) $(DEP_CPP_COM_2) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Wr3380.cpp

!IF  "$(CFG)" == "PLC_SCAN - Win32 Release"

DEP_CPP_WR338=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	
NODEP_CPP_WR338=\
	".\Protocol\plc_scan.h"\
	

"$(INTDIR)\Wr3380.obj" : $(SOURCE) $(DEP_CPP_WR338) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "PLC_SCAN - Win32 Debug"

DEP_CPP_WR338=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Wr3380.obj" : $(SOURCE) $(DEP_CPP_WR338) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Abplc5.cpp

!IF  "$(CFG)" == "PLC_SCAN - Win32 Release"

DEP_CPP_ABPLC=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	
NODEP_CPP_ABPLC=\
	".\Protocol\plc_scan.h"\
	

"$(INTDIR)\Abplc5.obj" : $(SOURCE) $(DEP_CPP_ABPLC) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "PLC_SCAN - Win32 Debug"

DEP_CPP_ABPLC=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Abplc5.obj" : $(SOURCE) $(DEP_CPP_ABPLC) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Autobase.cpp
DEP_CPP_AUTOB=\
	{$(INCLUDE)}"\Tools.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	

"$(INTDIR)\Autobase.obj" : $(SOURCE) $(DEP_CPP_AUTOB) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Bl_2300.cpp
DEP_CPP_BL_23=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	{$(INCLUDE)}"\Dataswap.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Bl_2300.obj" : $(SOURCE) $(DEP_CPP_BL_23) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Dpm2.cpp

!IF  "$(CFG)" == "PLC_SCAN - Win32 Release"

DEP_CPP_DPM2_=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	{$(INCLUDE)}"\Totaldef.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	
NODEP_CPP_DPM2_=\
	".\Protocol\plc_scan.h"\
	

"$(INTDIR)\Dpm2.obj" : $(SOURCE) $(DEP_CPP_DPM2_) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "PLC_SCAN - Win32 Debug"

DEP_CPP_DPM2_=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	{$(INCLUDE)}"\Totaldef.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Dpm2.obj" : $(SOURCE) $(DEP_CPP_DPM2_) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Fuji.cpp
DEP_CPP_FUJI_=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Totaldef.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	

"$(INTDIR)\Fuji.obj" : $(SOURCE) $(DEP_CPP_FUJI_) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Gantner.cpp

!IF  "$(CFG)" == "PLC_SCAN - Win32 Release"

DEP_CPP_GANTN=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	
NODEP_CPP_GANTN=\
	".\Protocol\plc_scan.h"\
	

"$(INTDIR)\Gantner.obj" : $(SOURCE) $(DEP_CPP_GANTN) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "PLC_SCAN - Win32 Debug"

DEP_CPP_GANTN=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Gantner.obj" : $(SOURCE) $(DEP_CPP_GANTN) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Glofa.cpp

!IF  "$(CFG)" == "PLC_SCAN - Win32 Release"

DEP_CPP_GLOFA=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	
NODEP_CPP_GLOFA=\
	".\Protocol\plc_scan.h"\
	

"$(INTDIR)\Glofa.obj" : $(SOURCE) $(DEP_CPP_GLOFA) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "PLC_SCAN - Win32 Debug"

DEP_CPP_GLOFA=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	

"$(INTDIR)\Glofa.obj" : $(SOURCE) $(DEP_CPP_GLOFA) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\La250.cpp
DEP_CPP_LA250=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\La250.obj" : $(SOURCE) $(DEP_CPP_LA250) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=".\Protocol\Master-k.cpp"
DEP_CPP_MASTE=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	

"$(INTDIR)\Master-k.obj" : $(SOURCE) $(DEP_CPP_MASTE) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Mf2.cpp

!IF  "$(CFG)" == "PLC_SCAN - Win32 Release"

DEP_CPP_MF2_C=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	
NODEP_CPP_MF2_C=\
	".\Protocol\plc_scan.h"\
	

"$(INTDIR)\Mf2.obj" : $(SOURCE) $(DEP_CPP_MF2_C) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "PLC_SCAN - Win32 Debug"

DEP_CPP_MF2_C=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Mf2.obj" : $(SOURCE) $(DEP_CPP_MF2_C) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Mj71e71.cpp
DEP_CPP_MJ71E=\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	

"$(INTDIR)\Mj71e71.obj" : $(SOURCE) $(DEP_CPP_MJ71E) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Modicon.cpp
DEP_CPP_MODIC=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Modicon.obj" : $(SOURCE) $(DEP_CPP_MODIC) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Omron.cpp

!IF  "$(CFG)" == "PLC_SCAN - Win32 Release"

DEP_CPP_OMRON=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	
NODEP_CPP_OMRON=\
	".\Protocol\plc_scan.h"\
	

"$(INTDIR)\Omron.obj" : $(SOURCE) $(DEP_CPP_OMRON) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "PLC_SCAN - Win32 Debug"

DEP_CPP_OMRON=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Omron.obj" : $(SOURCE) $(DEP_CPP_OMRON) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Pm_170e.cpp

!IF  "$(CFG)" == "PLC_SCAN - Win32 Release"

DEP_CPP_PM_17=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	
NODEP_CPP_PM_17=\
	".\Protocol\plc_scan.h"\
	

"$(INTDIR)\Pm_170e.obj" : $(SOURCE) $(DEP_CPP_PM_17) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "PLC_SCAN - Win32 Debug"

DEP_CPP_PM_17=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Pm_170e.obj" : $(SOURCE) $(DEP_CPP_PM_17) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Pro_lib.cpp
DEP_CPP_PRO_L=\
	{$(INCLUDE)}"\Dataswap.h"\
	".\Protocol\Pro_lib.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	".\Protocol\..\plc_scan.h"\
	{$(INCLUDE)}"\Tools.h"\
	

"$(INTDIR)\Pro_lib.obj" : $(SOURCE) $(DEP_CPP_PRO_L) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Pro_main.cpp
DEP_CPP_PRO_M=\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_main.h"\
	".\Protocol\pro_dde.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\Tools.h"\
	

"$(INTDIR)\Pro_main.obj" : $(SOURCE) $(DEP_CPP_PRO_M) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=".\Protocol\Spc-300.cpp"
DEP_CPP_SPC_3=\
	{$(INCLUDE)}"\Tools.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	

"$(INTDIR)\Spc-300.obj" : $(SOURCE) $(DEP_CPP_SPC_3) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Tmtc.cpp
DEP_CPP_TMTC_=\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	
NODEP_CPP_TMTC_=\
	".\Protocol\plc_scan.h"\
	

"$(INTDIR)\Tmtc.obj" : $(SOURCE) $(DEP_CPP_TMTC_) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\A0j2c214.cpp
DEP_CPP_A0J2C=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	

"$(INTDIR)\A0j2c214.obj" : $(SOURCE) $(DEP_CPP_A0J2C) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=\Win\Wintools\Lib\Win95\wintools.lib

!IF  "$(CFG)" == "PLC_SCAN - Win32 Release"

!ELSEIF  "$(CFG)" == "PLC_SCAN - Win32 Debug"

!ENDIF 

# End Source File
################################################################################
# Begin Source File

SOURCE=\Win\Wintools\Source\Kwl\Kdialog.cpp
DEP_CPP_KDIAL=\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	{$(INCLUDE)}"\Kwl\Kdialog.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Kdialog.obj" : $(SOURCE) $(DEP_CPP_KDIAL) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=\Win\Cat16\Catlib\Totalcfg.cpp
DEP_CPP_TOTAL=\
	{$(INCLUDE)}"\Compiler.hpp"\
	".\..\catlib\totalcfg.h"\
	

"$(INTDIR)\Totalcfg.obj" : $(SOURCE) $(DEP_CPP_TOTAL) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=".\Protocol\PM-B.cpp"
DEP_CPP_PM_B_=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	

"$(INTDIR)\PM-B.obj" : $(SOURCE) $(DEP_CPP_PM_B_) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=".\Protocol\PT-L.cpp"
DEP_CPP_PT_L_=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	

"$(INTDIR)\PT-L.obj" : $(SOURCE) $(DEP_CPP_PT_L_) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Device\ComCard.cpp
DEP_CPP_COMCA=\
	{$(INCLUDE)}"\Tools.h"\
	".\Protocol\..\plc_scan.h"\
	".\Device\Commmain.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	

"$(INTDIR)\ComCard.obj" : $(SOURCE) $(DEP_CPP_COMCA) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=".\Protocol\SYS-K016.cpp"
DEP_CPP_SYS_K=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	

"$(INTDIR)\SYS-K016.obj" : $(SOURCE) $(DEP_CPP_SYS_K) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=".\Protocol\SYS-K017.cpp"
DEP_CPP_SYS_K0=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\SYS-K017.obj" : $(SOURCE) $(DEP_CPP_SYS_K0) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\RLink.cpp
DEP_CPP_RLINK=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\RLink.obj" : $(SOURCE) $(DEP_CPP_RLINK) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\pro_dde.cpp
DEP_CPP_PRO_D=\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	{$(INCLUDE)}"\Dataswap.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	".\Protocol\pro_dde.h"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\pro_dde.obj" : $(SOURCE) $(DEP_CPP_PRO_D) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\VCR.cpp
DEP_CPP_VCR_C=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\VCR.obj" : $(SOURCE) $(DEP_CPP_VCR_C) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\adam.cpp
DEP_CPP_ADAM_=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\adam.obj" : $(SOURCE) $(DEP_CPP_ADAM_) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\sp30.cpp
DEP_CPP_SP30_=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\sp30.obj" : $(SOURCE) $(DEP_CPP_SP30_) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\Maxcom.cpp
DEP_CPP_MAXCO=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\Maxcom.obj" : $(SOURCE) $(DEP_CPP_MAXCO) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
################################################################################
# Begin Source File

SOURCE=.\Protocol\user.cpp
DEP_CPP_USER_=\
	{$(INCLUDE)}"\Totaldef.h"\
	{$(INCLUDE)}"\Tools.h"\
	{$(INCLUDE)}"\Glib.h"\
	".\Protocol\..\plc_scan.h"\
	".\Protocol\Pro_lib.h"\
	".\Protocol\Pro_main.h"\
	{$(INCLUDE)}"\Compiler.hpp"\
	{$(INCLUDE)}"\mfc\glib.h"\
	{$(INCLUDE)}"\standard\glib.h"\
	

"$(INTDIR)\user.obj" : $(SOURCE) $(DEP_CPP_USER_) "$(INTDIR)"
   $(CPP) $(CPP_PROJ) $(SOURCE)


# End Source File
# End Target
# End Project
################################################################################
