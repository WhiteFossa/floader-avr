/****************************************************************************
** Meta object code from reading C++ file 'devicedriver.h'
**
** Created: Sat 13. Oct 17:43:05 2012
**      by: The Qt Meta Object Compiler version 63 (Qt 4.8.1)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "../../../FLoaderClient/devicedriver.h"
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'devicedriver.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 63
#error "This file was generated using the moc from 4.8.1. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
static const uint qt_meta_data_DeviceDriver[] = {

 // content:
       6,       // revision
       0,       // classname
       0,    0, // classinfo
       2,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       1,       // signalCount

 // signals: signature, parameters, type, tag, flags
      14,   13,   13,   13, 0x05,

 // slots: signature, parameters, type, tag, flags
      37,   13,   13,   13, 0x0a,

       0        // eod
};

static const char qt_meta_stringdata_DeviceDriver[] = {
    "DeviceDriver\0\0eeprom_write_byte(int)\0"
    "rx_timer_timeout()\0"
};

void DeviceDriver::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        Q_ASSERT(staticMetaObject.cast(_o));
        DeviceDriver *_t = static_cast<DeviceDriver *>(_o);
        switch (_id) {
        case 0: _t->eeprom_write_byte((*reinterpret_cast< int(*)>(_a[1]))); break;
        case 1: _t->rx_timer_timeout(); break;
        default: ;
        }
    }
}

const QMetaObjectExtraData DeviceDriver::staticMetaObjectExtraData = {
    0,  qt_static_metacall 
};

const QMetaObject DeviceDriver::staticMetaObject = {
    { &QObject::staticMetaObject, qt_meta_stringdata_DeviceDriver,
      qt_meta_data_DeviceDriver, &staticMetaObjectExtraData }
};

#ifdef Q_NO_DATA_RELOCATION
const QMetaObject &DeviceDriver::getStaticMetaObject() { return staticMetaObject; }
#endif //Q_NO_DATA_RELOCATION

const QMetaObject *DeviceDriver::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->metaObject : &staticMetaObject;
}

void *DeviceDriver::qt_metacast(const char *_clname)
{
    if (!_clname) return 0;
    if (!strcmp(_clname, qt_meta_stringdata_DeviceDriver))
        return static_cast<void*>(const_cast< DeviceDriver*>(this));
    return QObject::qt_metacast(_clname);
}

int DeviceDriver::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QObject::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 2)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 2;
    }
    return _id;
}

// SIGNAL 0
void DeviceDriver::eeprom_write_byte(int _t1)
{
    void *_a[] = { 0, const_cast<void*>(reinterpret_cast<const void*>(&_t1)) };
    QMetaObject::activate(this, &staticMetaObject, 0, _a);
}
QT_END_MOC_NAMESPACE
