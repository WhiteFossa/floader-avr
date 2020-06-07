#-------------------------------------------------
#
# Project created by QtCreator 2011-04-18T23:02:45
#
#-------------------------------------------------

QT       += core gui sql

TARGET = FLoaderClient
TEMPLATE = app


SOURCES += main.cpp\
        mainwindow.cpp \
    portdialog.cpp \
    devicedriver.cpp \
    ihexparser.cpp

HEADERS  += mainwindow.h \
    portdialog.h \
    devicedriver.h \
    ihexparser.h

FORMS    += mainwindow.ui \
    portdialog.ui

CONFIG(debug, debug|release):LIBS  += -lqextserialportd
else:LIBS  += -lqextserialport


