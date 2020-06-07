/****************************************************************************
** Meta object code from reading C++ file 'portdialog.h'
**
** Created: Sat 13. Oct 17:43:04 2012
**      by: The Qt Meta Object Compiler version 63 (Qt 4.8.1)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "../../../FLoaderClient/portdialog.h"
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'portdialog.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 63
#error "This file was generated using the moc from 4.8.1. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
static const uint qt_meta_data_PortDialog[] = {

 // content:
       6,       // revision
       0,       // classname
       0,    0, // classinfo
       2,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       0,       // signalCount

 // slots: signature, parameters, type, tag, flags
      24,   12,   11,   11, 0x08,
      59,   11,   11,   11, 0x08,

       0        // eod
};

static const char qt_meta_stringdata_PortDialog[] = {
    "PortDialog\0\0cr,cc,pr,pc\0"
    "port_cell_changed(int,int,int,int)\0"
    "settings_accepted()\0"
};

void PortDialog::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        Q_ASSERT(staticMetaObject.cast(_o));
        PortDialog *_t = static_cast<PortDialog *>(_o);
        switch (_id) {
        case 0: _t->port_cell_changed((*reinterpret_cast< int(*)>(_a[1])),(*reinterpret_cast< int(*)>(_a[2])),(*reinterpret_cast< int(*)>(_a[3])),(*reinterpret_cast< int(*)>(_a[4]))); break;
        case 1: _t->settings_accepted(); break;
        default: ;
        }
    }
}

const QMetaObjectExtraData PortDialog::staticMetaObjectExtraData = {
    0,  qt_static_metacall 
};

const QMetaObject PortDialog::staticMetaObject = {
    { &QDialog::staticMetaObject, qt_meta_stringdata_PortDialog,
      qt_meta_data_PortDialog, &staticMetaObjectExtraData }
};

#ifdef Q_NO_DATA_RELOCATION
const QMetaObject &PortDialog::getStaticMetaObject() { return staticMetaObject; }
#endif //Q_NO_DATA_RELOCATION

const QMetaObject *PortDialog::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->metaObject : &staticMetaObject;
}

void *PortDialog::qt_metacast(const char *_clname)
{
    if (!_clname) return 0;
    if (!strcmp(_clname, qt_meta_stringdata_PortDialog))
        return static_cast<void*>(const_cast< PortDialog*>(this));
    return QDialog::qt_metacast(_clname);
}

int PortDialog::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QDialog::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 2)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 2;
    }
    return _id;
}
QT_END_MOC_NAMESPACE
