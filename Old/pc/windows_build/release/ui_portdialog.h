/********************************************************************************
** Form generated from reading UI file 'portdialog.ui'
**
** Created: Sat 13. Oct 16:58:10 2012
**      by: Qt User Interface Compiler version 4.8.1
**
** WARNING! All changes made in this file will be lost when recompiling UI file!
********************************************************************************/

#ifndef UI_PORTDIALOG_H
#define UI_PORTDIALOG_H

#include <QtCore/QVariant>
#include <QtGui/QAction>
#include <QtGui/QApplication>
#include <QtGui/QButtonGroup>
#include <QtGui/QComboBox>
#include <QtGui/QDialog>
#include <QtGui/QGroupBox>
#include <QtGui/QHeaderView>
#include <QtGui/QLabel>
#include <QtGui/QPushButton>
#include <QtGui/QTableWidget>

QT_BEGIN_NAMESPACE

class Ui_PortDialog
{
public:
    QGroupBox *gbPort;
    QTableWidget *tblPorts;
    QPushButton *btnApply;
    QPushButton *btnCancel;
    QLabel *label;
    QComboBox *cbBaudrate;
    QComboBox *cbBits;
    QLabel *label_2;
    QComboBox *cbFlow;
    QLabel *label_3;
    QComboBox *cbParity;
    QLabel *label_4;
    QComboBox *cbStop;
    QLabel *label_5;

    void setupUi(QDialog *PortDialog)
    {
        if (PortDialog->objectName().isEmpty())
            PortDialog->setObjectName(QString::fromUtf8("PortDialog"));
        PortDialog->resize(300, 334);
        gbPort = new QGroupBox(PortDialog);
        gbPort->setObjectName(QString::fromUtf8("gbPort"));
        gbPort->setGeometry(QRect(0, 0, 301, 181));
        tblPorts = new QTableWidget(gbPort);
        if (tblPorts->columnCount() < 2)
            tblPorts->setColumnCount(2);
        tblPorts->setObjectName(QString::fromUtf8("tblPorts"));
        tblPorts->setGeometry(QRect(5, 15, 291, 161));
        tblPorts->setEditTriggers(QAbstractItemView::NoEditTriggers);
        tblPorts->setProperty("showDropIndicator", QVariant(false));
        tblPorts->setSelectionMode(QAbstractItemView::SingleSelection);
        tblPorts->setSelectionBehavior(QAbstractItemView::SelectRows);
        tblPorts->setVerticalScrollMode(QAbstractItemView::ScrollPerPixel);
        tblPorts->setHorizontalScrollMode(QAbstractItemView::ScrollPerPixel);
        tblPorts->setSortingEnabled(false);
        tblPorts->setRowCount(0);
        tblPorts->setColumnCount(2);
        tblPorts->horizontalHeader()->setHighlightSections(false);
        tblPorts->verticalHeader()->setVisible(false);
        tblPorts->verticalHeader()->setHighlightSections(true);
        tblPorts->verticalHeader()->setStretchLastSection(true);
        btnApply = new QPushButton(PortDialog);
        btnApply->setObjectName(QString::fromUtf8("btnApply"));
        btnApply->setGeometry(QRect(0, 310, 75, 23));
        btnCancel = new QPushButton(PortDialog);
        btnCancel->setObjectName(QString::fromUtf8("btnCancel"));
        btnCancel->setGeometry(QRect(225, 310, 75, 23));
        label = new QLabel(PortDialog);
        label->setObjectName(QString::fromUtf8("label"));
        label->setGeometry(QRect(2, 187, 51, 16));
        cbBaudrate = new QComboBox(PortDialog);
        cbBaudrate->setObjectName(QString::fromUtf8("cbBaudrate"));
        cbBaudrate->setGeometry(QRect(195, 185, 106, 22));
        cbBits = new QComboBox(PortDialog);
        cbBits->setObjectName(QString::fromUtf8("cbBits"));
        cbBits->setGeometry(QRect(195, 210, 36, 22));
        label_2 = new QLabel(PortDialog);
        label_2->setObjectName(QString::fromUtf8("label_2"));
        label_2->setGeometry(QRect(2, 215, 111, 16));
        cbFlow = new QComboBox(PortDialog);
        cbFlow->setObjectName(QString::fromUtf8("cbFlow"));
        cbFlow->setGeometry(QRect(195, 235, 86, 22));
        label_3 = new QLabel(PortDialog);
        label_3->setObjectName(QString::fromUtf8("label_3"));
        label_3->setGeometry(QRect(2, 240, 111, 16));
        cbParity = new QComboBox(PortDialog);
        cbParity->setObjectName(QString::fromUtf8("cbParity"));
        cbParity->setGeometry(QRect(195, 260, 86, 22));
        label_4 = new QLabel(PortDialog);
        label_4->setObjectName(QString::fromUtf8("label_4"));
        label_4->setGeometry(QRect(2, 265, 61, 16));
        cbStop = new QComboBox(PortDialog);
        cbStop->setObjectName(QString::fromUtf8("cbStop"));
        cbStop->setGeometry(QRect(195, 285, 36, 22));
        label_5 = new QLabel(PortDialog);
        label_5->setObjectName(QString::fromUtf8("label_5"));
        label_5->setGeometry(QRect(2, 290, 91, 16));

        retranslateUi(PortDialog);

        QMetaObject::connectSlotsByName(PortDialog);
    } // setupUi

    void retranslateUi(QDialog *PortDialog)
    {
        PortDialog->setWindowTitle(QApplication::translate("PortDialog", "\320\235\320\260\321\201\321\202\321\200\320\276\320\271\320\272\320\270 \320\277\320\276\321\200\321\202\320\260", 0, QApplication::UnicodeUTF8));
        gbPort->setTitle(QApplication::translate("PortDialog", "\320\237\320\276\321\200\321\202", 0, QApplication::UnicodeUTF8));
        btnApply->setText(QApplication::translate("PortDialog", "\320\237\321\200\320\270\320\274\320\265\320\275\320\270\321\202\321\214", 0, QApplication::UnicodeUTF8));
        btnCancel->setText(QApplication::translate("PortDialog", "\320\236\321\202\320\274\320\265\320\275\320\260", 0, QApplication::UnicodeUTF8));
        label->setText(QApplication::translate("PortDialog", "\320\241\320\272\320\276\321\200\320\276\321\201\321\202\321\214:", 0, QApplication::UnicodeUTF8));
        label_2->setText(QApplication::translate("PortDialog", "\320\247\320\270\321\201\320\273\320\276 \320\261\320\270\321\202 \320\262 \320\277\320\276\321\201\321\213\320\273\320\272\320\265:", 0, QApplication::UnicodeUTF8));
        label_3->setText(QApplication::translate("PortDialog", "\320\243\320\277\321\200\320\260\320\262\320\273\320\265\320\275\320\270\320\265 \320\277\320\276\321\202\320\276\320\272\320\276\320\274:", 0, QApplication::UnicodeUTF8));
        label_4->setText(QApplication::translate("PortDialog", "\320\247\321\221\321\202\320\275\320\276\321\201\321\202\321\214:", 0, QApplication::UnicodeUTF8));
        label_5->setText(QApplication::translate("PortDialog", "\320\241\321\202\320\276\320\277\320\276\320\262\321\213\320\265 \320\261\320\270\321\202\321\213:", 0, QApplication::UnicodeUTF8));
    } // retranslateUi

};

namespace Ui {
    class PortDialog: public Ui_PortDialog {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_PORTDIALOG_H
