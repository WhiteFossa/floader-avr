#############################################################################
# Makefile for building: FLoaderClient
# Generated by qmake (2.01a) (Qt 4.7.4) on: ?? ???. 13 16:33:32 2012
# Project:  ../../FLoaderClient/FLoaderClient.pro
# Template: app
# Command: /usr/bin/qmake -spec /usr/share/qt4/mkspecs/linux-g++-64 CONFIG+=debug -o Makefile ../../FLoaderClient/FLoaderClient.pro
#############################################################################

####### Compiler, tools and options

CC            = gcc
CXX           = g++
DEFINES       = -DQT_SQL_LIB -DQT_GUI_LIB -DQT_CORE_LIB -DQT_SHARED
CFLAGS        = -m64 -pipe -g -Wall -W -D_REENTRANT $(DEFINES)
CXXFLAGS      = -m64 -pipe -g -Wall -W -D_REENTRANT $(DEFINES)
INCPATH       = -I/usr/share/qt4/mkspecs/linux-g++-64 -I../../FLoaderClient -I/usr/include/QtCore -I/usr/include/QtGui -I/usr/include/QtSql -I/usr/include -I. -I. -I../../FLoaderClient -I.
LINK          = g++
LFLAGS        = -m64
LIBS          = $(SUBLIBS)  -L/usr/lib64 -lqextserialportd -lQtSql -L/usr/lib64 -lQtGui -L/usr/X11R6/lib64 -lQtCore -lpthread 
AR            = ar cqs
RANLIB        = 
QMAKE         = /usr/bin/qmake
TAR           = tar -cf
COMPRESS      = gzip -9f
COPY          = cp -f
SED           = sed
COPY_FILE     = $(COPY)
COPY_DIR      = $(COPY) -r
STRIP         = strip
INSTALL_FILE  = install -m 644 -p
INSTALL_DIR   = $(COPY_DIR)
INSTALL_PROGRAM = install -m 755 -p
DEL_FILE      = rm -f
SYMLINK       = ln -f -s
DEL_DIR       = rmdir
MOVE          = mv -f
CHK_DIR_EXISTS= test -d
MKDIR         = mkdir -p

####### Output directory

OBJECTS_DIR   = ./

####### Files

SOURCES       = ../../FLoaderClient/main.cpp \
		../../FLoaderClient/mainwindow.cpp \
		../../FLoaderClient/portdialog.cpp \
		../../FLoaderClient/devicedriver.cpp \
		../../FLoaderClient/ihexparser.cpp moc_mainwindow.cpp \
		moc_portdialog.cpp \
		moc_devicedriver.cpp
OBJECTS       = main.o \
		mainwindow.o \
		portdialog.o \
		devicedriver.o \
		ihexparser.o \
		moc_mainwindow.o \
		moc_portdialog.o \
		moc_devicedriver.o
DIST          = /usr/share/qt4/mkspecs/common/g++.conf \
		/usr/share/qt4/mkspecs/common/unix.conf \
		/usr/share/qt4/mkspecs/common/linux.conf \
		/usr/share/qt4/mkspecs/qconfig.pri \
		/usr/share/qt4/mkspecs/features/qt_functions.prf \
		/usr/share/qt4/mkspecs/features/qt_config.prf \
		/usr/share/qt4/mkspecs/features/exclusive_builds.prf \
		/usr/share/qt4/mkspecs/features/default_pre.prf \
		/usr/share/qt4/mkspecs/features/debug.prf \
		/usr/share/qt4/mkspecs/features/default_post.prf \
		/usr/share/qt4/mkspecs/features/warn_on.prf \
		/usr/share/qt4/mkspecs/features/qt.prf \
		/usr/share/qt4/mkspecs/features/unix/thread.prf \
		/usr/share/qt4/mkspecs/features/moc.prf \
		/usr/share/qt4/mkspecs/features/resources.prf \
		/usr/share/qt4/mkspecs/features/uic.prf \
		/usr/share/qt4/mkspecs/features/yacc.prf \
		/usr/share/qt4/mkspecs/features/lex.prf \
		/usr/share/qt4/mkspecs/features/include_source_dir.prf \
		../../FLoaderClient/FLoaderClient.pro
QMAKE_TARGET  = FLoaderClient
DESTDIR       = 
TARGET        = FLoaderClient

first: all
####### Implicit rules

.SUFFIXES: .o .c .cpp .cc .cxx .C

.cpp.o:
	$(CXX) -c $(CXXFLAGS) $(INCPATH) -o "$@" "$<"

.cc.o:
	$(CXX) -c $(CXXFLAGS) $(INCPATH) -o "$@" "$<"

.cxx.o:
	$(CXX) -c $(CXXFLAGS) $(INCPATH) -o "$@" "$<"

.C.o:
	$(CXX) -c $(CXXFLAGS) $(INCPATH) -o "$@" "$<"

.c.o:
	$(CC) -c $(CFLAGS) $(INCPATH) -o "$@" "$<"

####### Build rules

all: Makefile $(TARGET)

$(TARGET): ui_mainwindow.h ui_portdialog.h $(OBJECTS)  
	$(LINK) $(LFLAGS) -o $(TARGET) $(OBJECTS) $(OBJCOMP) $(LIBS)

Makefile: ../../FLoaderClient/FLoaderClient.pro  /usr/share/qt4/mkspecs/linux-g++-64/qmake.conf /usr/share/qt4/mkspecs/common/g++.conf \
		/usr/share/qt4/mkspecs/common/unix.conf \
		/usr/share/qt4/mkspecs/common/linux.conf \
		/usr/share/qt4/mkspecs/qconfig.pri \
		/usr/share/qt4/mkspecs/features/qt_functions.prf \
		/usr/share/qt4/mkspecs/features/qt_config.prf \
		/usr/share/qt4/mkspecs/features/exclusive_builds.prf \
		/usr/share/qt4/mkspecs/features/default_pre.prf \
		/usr/share/qt4/mkspecs/features/debug.prf \
		/usr/share/qt4/mkspecs/features/default_post.prf \
		/usr/share/qt4/mkspecs/features/warn_on.prf \
		/usr/share/qt4/mkspecs/features/qt.prf \
		/usr/share/qt4/mkspecs/features/unix/thread.prf \
		/usr/share/qt4/mkspecs/features/moc.prf \
		/usr/share/qt4/mkspecs/features/resources.prf \
		/usr/share/qt4/mkspecs/features/uic.prf \
		/usr/share/qt4/mkspecs/features/yacc.prf \
		/usr/share/qt4/mkspecs/features/lex.prf \
		/usr/share/qt4/mkspecs/features/include_source_dir.prf \
		/usr/lib64/libQtSql.prl \
		/usr/lib64/libQtCore.prl \
		/usr/lib64/libQtGui.prl
	$(QMAKE) -spec /usr/share/qt4/mkspecs/linux-g++-64 CONFIG+=debug -o Makefile ../../FLoaderClient/FLoaderClient.pro
/usr/share/qt4/mkspecs/common/g++.conf:
/usr/share/qt4/mkspecs/common/unix.conf:
/usr/share/qt4/mkspecs/common/linux.conf:
/usr/share/qt4/mkspecs/qconfig.pri:
/usr/share/qt4/mkspecs/features/qt_functions.prf:
/usr/share/qt4/mkspecs/features/qt_config.prf:
/usr/share/qt4/mkspecs/features/exclusive_builds.prf:
/usr/share/qt4/mkspecs/features/default_pre.prf:
/usr/share/qt4/mkspecs/features/debug.prf:
/usr/share/qt4/mkspecs/features/default_post.prf:
/usr/share/qt4/mkspecs/features/warn_on.prf:
/usr/share/qt4/mkspecs/features/qt.prf:
/usr/share/qt4/mkspecs/features/unix/thread.prf:
/usr/share/qt4/mkspecs/features/moc.prf:
/usr/share/qt4/mkspecs/features/resources.prf:
/usr/share/qt4/mkspecs/features/uic.prf:
/usr/share/qt4/mkspecs/features/yacc.prf:
/usr/share/qt4/mkspecs/features/lex.prf:
/usr/share/qt4/mkspecs/features/include_source_dir.prf:
/usr/lib64/libQtSql.prl:
/usr/lib64/libQtCore.prl:
/usr/lib64/libQtGui.prl:
qmake:  FORCE
	@$(QMAKE) -spec /usr/share/qt4/mkspecs/linux-g++-64 CONFIG+=debug -o Makefile ../../FLoaderClient/FLoaderClient.pro

dist: 
	@$(CHK_DIR_EXISTS) .tmp/FLoaderClient1.0.0 || $(MKDIR) .tmp/FLoaderClient1.0.0 
	$(COPY_FILE) --parents $(SOURCES) $(DIST) .tmp/FLoaderClient1.0.0/ && $(COPY_FILE) --parents ../../FLoaderClient/mainwindow.h ../../FLoaderClient/portdialog.h ../../FLoaderClient/devicedriver.h ../../FLoaderClient/ihexparser.h .tmp/FLoaderClient1.0.0/ && $(COPY_FILE) --parents ../../FLoaderClient/main.cpp ../../FLoaderClient/mainwindow.cpp ../../FLoaderClient/portdialog.cpp ../../FLoaderClient/devicedriver.cpp ../../FLoaderClient/ihexparser.cpp .tmp/FLoaderClient1.0.0/ && $(COPY_FILE) --parents ../../FLoaderClient/mainwindow.ui ../../FLoaderClient/portdialog.ui .tmp/FLoaderClient1.0.0/ && (cd `dirname .tmp/FLoaderClient1.0.0` && $(TAR) FLoaderClient1.0.0.tar FLoaderClient1.0.0 && $(COMPRESS) FLoaderClient1.0.0.tar) && $(MOVE) `dirname .tmp/FLoaderClient1.0.0`/FLoaderClient1.0.0.tar.gz . && $(DEL_FILE) -r .tmp/FLoaderClient1.0.0


clean:compiler_clean 
	-$(DEL_FILE) $(OBJECTS)
	-$(DEL_FILE) *~ core *.core


####### Sub-libraries

distclean: clean
	-$(DEL_FILE) $(TARGET) 
	-$(DEL_FILE) Makefile


check: first

mocclean: compiler_moc_header_clean compiler_moc_source_clean

mocables: compiler_moc_header_make_all compiler_moc_source_make_all

compiler_moc_header_make_all: moc_mainwindow.cpp moc_portdialog.cpp moc_devicedriver.cpp
compiler_moc_header_clean:
	-$(DEL_FILE) moc_mainwindow.cpp moc_portdialog.cpp moc_devicedriver.cpp
moc_mainwindow.cpp: ../../FLoaderClient/portdialog.h \
		../../FLoaderClient/qextserialport/qextserialenumerator.h \
		../../FLoaderClient/qextserialport/qextserialport_global.h \
		../../FLoaderClient/qextserialport/qextserialport.h \
		../../FLoaderClient/devicedriver.h \
		../../FLoaderClient/ihexparser.h \
		../../FLoaderClient/mainwindow.h
	/usr/bin/moc $(DEFINES) $(INCPATH) ../../FLoaderClient/mainwindow.h -o moc_mainwindow.cpp

moc_portdialog.cpp: ../../FLoaderClient/qextserialport/qextserialenumerator.h \
		../../FLoaderClient/qextserialport/qextserialport_global.h \
		../../FLoaderClient/qextserialport/qextserialport.h \
		../../FLoaderClient/portdialog.h
	/usr/bin/moc $(DEFINES) $(INCPATH) ../../FLoaderClient/portdialog.h -o moc_portdialog.cpp

moc_devicedriver.cpp: ../../FLoaderClient/qextserialport/qextserialport.h \
		../../FLoaderClient/qextserialport/qextserialport_global.h \
		../../FLoaderClient/qextserialport/qextserialenumerator.h \
		../../FLoaderClient/devicedriver.h
	/usr/bin/moc $(DEFINES) $(INCPATH) ../../FLoaderClient/devicedriver.h -o moc_devicedriver.cpp

compiler_rcc_make_all:
compiler_rcc_clean:
compiler_image_collection_make_all: qmake_image_collection.cpp
compiler_image_collection_clean:
	-$(DEL_FILE) qmake_image_collection.cpp
compiler_moc_source_make_all:
compiler_moc_source_clean:
compiler_uic_make_all: ui_mainwindow.h ui_portdialog.h
compiler_uic_clean:
	-$(DEL_FILE) ui_mainwindow.h ui_portdialog.h
ui_mainwindow.h: ../../FLoaderClient/mainwindow.ui
	/usr/bin/uic ../../FLoaderClient/mainwindow.ui -o ui_mainwindow.h

ui_portdialog.h: ../../FLoaderClient/portdialog.ui
	/usr/bin/uic ../../FLoaderClient/portdialog.ui -o ui_portdialog.h

compiler_yacc_decl_make_all:
compiler_yacc_decl_clean:
compiler_yacc_impl_make_all:
compiler_yacc_impl_clean:
compiler_lex_make_all:
compiler_lex_clean:
compiler_clean: compiler_moc_header_clean compiler_uic_clean 

####### Compile

main.o: ../../FLoaderClient/main.cpp ../../FLoaderClient/mainwindow.h \
		../../FLoaderClient/portdialog.h \
		../../FLoaderClient/qextserialport/qextserialenumerator.h \
		../../FLoaderClient/qextserialport/qextserialport_global.h \
		../../FLoaderClient/qextserialport/qextserialport.h \
		../../FLoaderClient/devicedriver.h \
		../../FLoaderClient/ihexparser.h
	$(CXX) -c $(CXXFLAGS) $(INCPATH) -o main.o ../../FLoaderClient/main.cpp

mainwindow.o: ../../FLoaderClient/mainwindow.cpp ../../FLoaderClient/mainwindow.h \
		../../FLoaderClient/portdialog.h \
		../../FLoaderClient/qextserialport/qextserialenumerator.h \
		../../FLoaderClient/qextserialport/qextserialport_global.h \
		../../FLoaderClient/qextserialport/qextserialport.h \
		../../FLoaderClient/devicedriver.h \
		../../FLoaderClient/ihexparser.h \
		ui_mainwindow.h
	$(CXX) -c $(CXXFLAGS) $(INCPATH) -o mainwindow.o ../../FLoaderClient/mainwindow.cpp

portdialog.o: ../../FLoaderClient/portdialog.cpp ../../FLoaderClient/portdialog.h \
		../../FLoaderClient/qextserialport/qextserialenumerator.h \
		../../FLoaderClient/qextserialport/qextserialport_global.h \
		../../FLoaderClient/qextserialport/qextserialport.h \
		ui_portdialog.h
	$(CXX) -c $(CXXFLAGS) $(INCPATH) -o portdialog.o ../../FLoaderClient/portdialog.cpp

devicedriver.o: ../../FLoaderClient/devicedriver.cpp ../../FLoaderClient/devicedriver.h \
		../../FLoaderClient/qextserialport/qextserialport.h \
		../../FLoaderClient/qextserialport/qextserialport_global.h \
		../../FLoaderClient/qextserialport/qextserialenumerator.h
	$(CXX) -c $(CXXFLAGS) $(INCPATH) -o devicedriver.o ../../FLoaderClient/devicedriver.cpp

ihexparser.o: ../../FLoaderClient/ihexparser.cpp ../../FLoaderClient/ihexparser.h
	$(CXX) -c $(CXXFLAGS) $(INCPATH) -o ihexparser.o ../../FLoaderClient/ihexparser.cpp

moc_mainwindow.o: moc_mainwindow.cpp 
	$(CXX) -c $(CXXFLAGS) $(INCPATH) -o moc_mainwindow.o moc_mainwindow.cpp

moc_portdialog.o: moc_portdialog.cpp 
	$(CXX) -c $(CXXFLAGS) $(INCPATH) -o moc_portdialog.o moc_portdialog.cpp

moc_devicedriver.o: moc_devicedriver.cpp 
	$(CXX) -c $(CXXFLAGS) $(INCPATH) -o moc_devicedriver.o moc_devicedriver.cpp

####### Install

install:   FORCE

uninstall:   FORCE

FORCE:

