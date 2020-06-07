/****************************************************************************
** Meta object code from reading C++ file 'mainwindow.h'
**
** Created:
**      by: The Qt Meta Object Compiler version 62 (Qt 4.7.4)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "../../FLoaderClient/mainwindow.h"
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'mainwindow.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 62
#error "This file was generated using the moc from 4.7.4. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
static const uint qt_meta_data_MainWindow[] = {

 // content:
       5,       // revision
       0,       // classname
       0,    0, // classinfo
      11,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       0,       // signalCount

 // slots: signature, parameters, type, tag, flags
      12,   11,   11,   11, 0x08,
      33,   11,   11,   11, 0x08,
      52,   11,   11,   11, 0x08,
      72,   11,   11,   11, 0x08,
     109,  101,   11,   11, 0x08,
     141,   11,   11,   11, 0x08,
     156,   11,   11,   11, 0x08,
     167,   11,   11,   11, 0x08,
     179,   11,   11,   11, 0x08,
     197,   11,   11,   11, 0x08,
     216,  101,   11,   11, 0x08,

       0        // eod
};

static const char qt_meta_stringdata_MainWindow[] = {
    "MainWindow\0\0port_settings_slot()\0"
    "set_upload_flash()\0set_upload_eeprom()\0"
    "set_backups_directory_slot()\0checked\0"
    "on_cbEepromUpload_toggled(bool)\0"
    "query_device()\0flash_it()\0reboot_it()\0"
    "read_flash_slot()\0read_eeprom_slot()\0"
    "on_cb_make_backups_toggled(bool)\0"
};

const QMetaObject MainWindow::staticMetaObject = {
    { &QMainWindow::staticMetaObject, qt_meta_stringdata_MainWindow,
      qt_meta_data_MainWindow, 0 }
};

#ifdef Q_NO_DATA_RELOCATION
const QMetaObject &MainWindow::getStaticMetaObject() { return staticMetaObject; }
#endif //Q_NO_DATA_RELOCATION

const QMetaObject *MainWindow::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->metaObject : &staticMetaObject;
}

void *MainWindow::qt_metacast(const char *_clname)
{
    if (!_clname) return 0;
    if (!strcmp(_clname, qt_meta_stringdata_MainWindow))
        return static_cast<void*>(const_cast< MainWindow*>(this));
    return QMainWindow::qt_metacast(_clname);
}

int MainWindow::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QMainWindow::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        switch (_id) {
        case 0: port_settings_slot(); break;
        case 1: set_upload_flash(); break;
        case 2: set_upload_eeprom(); break;
        case 3: set_backups_directory_slot(); break;
        case 4: on_cbEepromUpload_toggled((*reinterpret_cast< bool(*)>(_a[1]))); break;
        case 5: query_device(); break;
        case 6: flash_it(); break;
        case 7: reboot_it(); break;
        case 8: read_flash_slot(); break;
        case 9: read_eeprom_slot(); break;
        case 10: on_cb_make_backups_toggled((*reinterpret_cast< bool(*)>(_a[1]))); break;
        default: ;
        }
        _id -= 11;
    }
    return _id;
}
QT_END_MOC_NAMESPACE
