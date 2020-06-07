/********************************************************************************
** Form generated from reading UI file 'mainwindow.ui'
**
** Created: Sat 13. Oct 16:58:10 2012
**      by: Qt User Interface Compiler version 4.8.1
**
** WARNING! All changes made in this file will be lost when recompiling UI file!
********************************************************************************/

#ifndef UI_MAINWINDOW_H
#define UI_MAINWINDOW_H

#include <QtCore/QVariant>
#include <QtGui/QAction>
#include <QtGui/QApplication>
#include <QtGui/QButtonGroup>
#include <QtGui/QCheckBox>
#include <QtGui/QFrame>
#include <QtGui/QGroupBox>
#include <QtGui/QHeaderView>
#include <QtGui/QLabel>
#include <QtGui/QLineEdit>
#include <QtGui/QMainWindow>
#include <QtGui/QPushButton>
#include <QtGui/QTextEdit>
#include <QtGui/QWidget>

QT_BEGIN_NAMESPACE

class Ui_MainWindow
{
public:
    QWidget *centralWidget;
    QGroupBox *groupBox;
    QLabel *label;
    QLabel *lblPortName;
    QPushButton *btnPortSettings;
    QPushButton *btnQueryDevice;
    QFrame *line;
    QGroupBox *groupBox_2;
    QLineEdit *leFlashFileUpload;
    QPushButton *btnSetFlashFileUpload;
    QLabel *label_2;
    QPushButton *btnSetEepromFileUpload;
    QLineEdit *leEepromUploadFile;
    QCheckBox *cbEepromUpload;
    QGroupBox *groupBox_3;
    QLabel *label_3;
    QLineEdit *leBackupsDir;
    QPushButton *btnSetBackupsDir;
    QLabel *label_4;
    QCheckBox *cb_make_backups;
    QGroupBox *groupBox_4;
    QLabel *label_5;
    QLabel *label_6;
    QLabel *label_7;
    QLabel *lblManufacturer;
    QLabel *lblModel;
    QLabel *lblSerial;
    QGroupBox *groupBox_5;
    QTextEdit *teConsole;
    QGroupBox *groupBox_6;
    QPushButton *btnFlash;
    QPushButton *btnReboot;
    QPushButton *btnReadFlash;
    QPushButton *btnReadEeprom;

    void setupUi(QMainWindow *MainWindow)
    {
        if (MainWindow->objectName().isEmpty())
            MainWindow->setObjectName(QString::fromUtf8("MainWindow"));
        MainWindow->resize(537, 576);
        QSizePolicy sizePolicy(QSizePolicy::Fixed, QSizePolicy::Fixed);
        sizePolicy.setHorizontalStretch(0);
        sizePolicy.setVerticalStretch(0);
        sizePolicy.setHeightForWidth(MainWindow->sizePolicy().hasHeightForWidth());
        MainWindow->setSizePolicy(sizePolicy);
        MainWindow->setMinimumSize(QSize(537, 503));
        MainWindow->setMaximumSize(QSize(537, 600));
        centralWidget = new QWidget(MainWindow);
        centralWidget->setObjectName(QString::fromUtf8("centralWidget"));
        QSizePolicy sizePolicy1(QSizePolicy::Fixed, QSizePolicy::Fixed);
        sizePolicy1.setHorizontalStretch(100);
        sizePolicy1.setVerticalStretch(100);
        sizePolicy1.setHeightForWidth(centralWidget->sizePolicy().hasHeightForWidth());
        centralWidget->setSizePolicy(sizePolicy1);
        groupBox = new QGroupBox(centralWidget);
        groupBox->setObjectName(QString::fromUtf8("groupBox"));
        groupBox->setGeometry(QRect(0, 0, 536, 36));
        label = new QLabel(groupBox);
        label->setObjectName(QString::fromUtf8("label"));
        label->setGeometry(QRect(5, 13, 31, 16));
        lblPortName = new QLabel(groupBox);
        lblPortName->setObjectName(QString::fromUtf8("lblPortName"));
        lblPortName->setGeometry(QRect(40, 13, 286, 16));
        btnPortSettings = new QPushButton(groupBox);
        btnPortSettings->setObjectName(QString::fromUtf8("btnPortSettings"));
        btnPortSettings->setGeometry(QRect(330, 10, 61, 21));
        btnQueryDevice = new QPushButton(groupBox);
        btnQueryDevice->setObjectName(QString::fromUtf8("btnQueryDevice"));
        btnQueryDevice->setGeometry(QRect(405, 10, 126, 21));
        line = new QFrame(groupBox);
        line->setObjectName(QString::fromUtf8("line"));
        line->setGeometry(QRect(393, 8, 10, 25));
        line->setFrameShape(QFrame::VLine);
        line->setFrameShadow(QFrame::Sunken);
        groupBox_2 = new QGroupBox(centralWidget);
        groupBox_2->setObjectName(QString::fromUtf8("groupBox_2"));
        groupBox_2->setGeometry(QRect(0, 110, 536, 71));
        leFlashFileUpload = new QLineEdit(groupBox_2);
        leFlashFileUpload->setObjectName(QString::fromUtf8("leFlashFileUpload"));
        leFlashFileUpload->setGeometry(QRect(45, 19, 461, 20));
        leFlashFileUpload->setReadOnly(true);
        btnSetFlashFileUpload = new QPushButton(groupBox_2);
        btnSetFlashFileUpload->setObjectName(QString::fromUtf8("btnSetFlashFileUpload"));
        btnSetFlashFileUpload->setGeometry(QRect(511, 19, 21, 21));
        label_2 = new QLabel(groupBox_2);
        label_2->setObjectName(QString::fromUtf8("label_2"));
        label_2->setGeometry(QRect(8, 21, 36, 16));
        btnSetEepromFileUpload = new QPushButton(groupBox_2);
        btnSetEepromFileUpload->setObjectName(QString::fromUtf8("btnSetEepromFileUpload"));
        btnSetEepromFileUpload->setGeometry(QRect(511, 44, 21, 21));
        leEepromUploadFile = new QLineEdit(groupBox_2);
        leEepromUploadFile->setObjectName(QString::fromUtf8("leEepromUploadFile"));
        leEepromUploadFile->setGeometry(QRect(90, 44, 416, 20));
        leEepromUploadFile->setReadOnly(true);
        cbEepromUpload = new QCheckBox(groupBox_2);
        cbEepromUpload->setObjectName(QString::fromUtf8("cbEepromUpload"));
        cbEepromUpload->setGeometry(QRect(8, 45, 81, 18));
        groupBox_3 = new QGroupBox(centralWidget);
        groupBox_3->setObjectName(QString::fromUtf8("groupBox_3"));
        groupBox_3->setGeometry(QRect(0, 180, 536, 111));
        label_3 = new QLabel(groupBox_3);
        label_3->setObjectName(QString::fromUtf8("label_3"));
        label_3->setGeometry(QRect(10, 38, 71, 16));
        leBackupsDir = new QLineEdit(groupBox_3);
        leBackupsDir->setObjectName(QString::fromUtf8("leBackupsDir"));
        leBackupsDir->setGeometry(QRect(85, 36, 421, 20));
        leBackupsDir->setReadOnly(true);
        btnSetBackupsDir = new QPushButton(groupBox_3);
        btnSetBackupsDir->setObjectName(QString::fromUtf8("btnSetBackupsDir"));
        btnSetBackupsDir->setGeometry(QRect(511, 36, 21, 21));
        label_4 = new QLabel(groupBox_3);
        label_4->setObjectName(QString::fromUtf8("label_4"));
        label_4->setGeometry(QRect(8, 60, 521, 46));
        label_4->setAlignment(Qt::AlignJustify|Qt::AlignVCenter);
        label_4->setWordWrap(true);
        cb_make_backups = new QCheckBox(groupBox_3);
        cb_make_backups->setObjectName(QString::fromUtf8("cb_make_backups"));
        cb_make_backups->setGeometry(QRect(8, 15, 206, 17));
        groupBox_4 = new QGroupBox(centralWidget);
        groupBox_4->setObjectName(QString::fromUtf8("groupBox_4"));
        groupBox_4->setGeometry(QRect(0, 35, 536, 76));
        label_5 = new QLabel(groupBox_4);
        label_5->setObjectName(QString::fromUtf8("label_5"));
        label_5->setGeometry(QRect(5, 15, 81, 16));
        label_6 = new QLabel(groupBox_4);
        label_6->setObjectName(QString::fromUtf8("label_6"));
        label_6->setGeometry(QRect(5, 35, 56, 16));
        label_7 = new QLabel(groupBox_4);
        label_7->setObjectName(QString::fromUtf8("label_7"));
        label_7->setGeometry(QRect(5, 55, 106, 16));
        lblManufacturer = new QLabel(groupBox_4);
        lblManufacturer->setObjectName(QString::fromUtf8("lblManufacturer"));
        lblManufacturer->setGeometry(QRect(85, 15, 441, 16));
        lblModel = new QLabel(groupBox_4);
        lblModel->setObjectName(QString::fromUtf8("lblModel"));
        lblModel->setGeometry(QRect(60, 35, 471, 16));
        lblSerial = new QLabel(groupBox_4);
        lblSerial->setObjectName(QString::fromUtf8("lblSerial"));
        lblSerial->setGeometry(QRect(115, 55, 411, 16));
        groupBox_5 = new QGroupBox(centralWidget);
        groupBox_5->setObjectName(QString::fromUtf8("groupBox_5"));
        groupBox_5->setGeometry(QRect(0, 290, 536, 206));
        QSizePolicy sizePolicy2(QSizePolicy::Preferred, QSizePolicy::Preferred);
        sizePolicy2.setHorizontalStretch(100);
        sizePolicy2.setVerticalStretch(100);
        sizePolicy2.setHeightForWidth(groupBox_5->sizePolicy().hasHeightForWidth());
        groupBox_5->setSizePolicy(sizePolicy2);
        teConsole = new QTextEdit(groupBox_5);
        teConsole->setObjectName(QString::fromUtf8("teConsole"));
        teConsole->setGeometry(QRect(5, 20, 526, 181));
        QSizePolicy sizePolicy3(QSizePolicy::Expanding, QSizePolicy::Expanding);
        sizePolicy3.setHorizontalStretch(100);
        sizePolicy3.setVerticalStretch(100);
        sizePolicy3.setHeightForWidth(teConsole->sizePolicy().hasHeightForWidth());
        teConsole->setSizePolicy(sizePolicy3);
        groupBox_6 = new QGroupBox(centralWidget);
        groupBox_6->setObjectName(QString::fromUtf8("groupBox_6"));
        groupBox_6->setGeometry(QRect(0, 500, 536, 71));
        btnFlash = new QPushButton(groupBox_6);
        btnFlash->setObjectName(QString::fromUtf8("btnFlash"));
        btnFlash->setEnabled(false);
        btnFlash->setGeometry(QRect(5, 45, 260, 23));
        btnReboot = new QPushButton(groupBox_6);
        btnReboot->setObjectName(QString::fromUtf8("btnReboot"));
        btnReboot->setEnabled(false);
        btnReboot->setGeometry(QRect(272, 45, 260, 23));
        btnReadFlash = new QPushButton(groupBox_6);
        btnReadFlash->setObjectName(QString::fromUtf8("btnReadFlash"));
        btnReadFlash->setEnabled(false);
        btnReadFlash->setGeometry(QRect(5, 20, 260, 23));
        btnReadEeprom = new QPushButton(groupBox_6);
        btnReadEeprom->setObjectName(QString::fromUtf8("btnReadEeprom"));
        btnReadEeprom->setEnabled(false);
        btnReadEeprom->setGeometry(QRect(272, 20, 260, 23));
        MainWindow->setCentralWidget(centralWidget);

        retranslateUi(MainWindow);

        QMetaObject::connectSlotsByName(MainWindow);
    } // setupUi

    void retranslateUi(QMainWindow *MainWindow)
    {
        MainWindow->setWindowTitle(QApplication::translate("MainWindow", "\320\227\320\260\320\263\321\200\321\203\320\267\321\207\320\270\320\272 \320\244\320\276\321\201\321\201\321\213 (\320\272\320\273\320\270\320\265\320\275\321\202\321\201\320\272\320\260\321\217 \321\207\320\260\321\201\321\202\321\214)", 0, QApplication::UnicodeUTF8));
        groupBox->setTitle(QApplication::translate("MainWindow", "\320\237\320\276\321\200\321\202", 0, QApplication::UnicodeUTF8));
        label->setText(QApplication::translate("MainWindow", "\320\237\320\276\321\200\321\202:", 0, QApplication::UnicodeUTF8));
        lblPortName->setText(QApplication::translate("MainWindow", "\320\235\320\265 \320\267\320\260\320\264\320\260\320\275", 0, QApplication::UnicodeUTF8));
        btnPortSettings->setText(QApplication::translate("MainWindow", "\320\222\321\213\320\261\321\200\320\260\321\202\321\214", 0, QApplication::UnicodeUTF8));
        btnQueryDevice->setText(QApplication::translate("MainWindow", "\320\236\320\277\321\200\320\276\321\201\320\270\321\202\321\214 \321\203\321\201\321\202\321\200\320\276\320\271\321\201\321\202\320\262\320\276", 0, QApplication::UnicodeUTF8));
        groupBox_2->setTitle(QApplication::translate("MainWindow", "\320\237\321\200\320\276\321\210\320\270\321\202\321\214", 0, QApplication::UnicodeUTF8));
        btnSetFlashFileUpload->setText(QApplication::translate("MainWindow", "...", 0, QApplication::UnicodeUTF8));
        label_2->setText(QApplication::translate("MainWindow", "Flash:", 0, QApplication::UnicodeUTF8));
        btnSetEepromFileUpload->setText(QApplication::translate("MainWindow", "...", 0, QApplication::UnicodeUTF8));
        cbEepromUpload->setText(QApplication::translate("MainWindow", "EEPROM:", 0, QApplication::UnicodeUTF8));
        groupBox_3->setTitle(QApplication::translate("MainWindow", "\320\221\321\215\320\272\320\260\320\277\321\213", 0, QApplication::UnicodeUTF8));
        label_3->setText(QApplication::translate("MainWindow", "\320\224\320\270\321\200\320\265\320\272\321\202\320\276\321\200\320\270\321\217:", 0, QApplication::UnicodeUTF8));
        btnSetBackupsDir->setText(QApplication::translate("MainWindow", "...", 0, QApplication::UnicodeUTF8));
        label_4->setText(QApplication::translate("MainWindow", "\320\222 \320\264\320\260\320\275\320\275\321\203\321\216 \320\264\320\270\321\200\320\265\320\272\321\202\321\200\320\270\321\216 \321\201\320\276\321\205\321\200\320\260\320\275\321\217\321\216\321\202\321\201\321\217 \321\200\320\265\320\267\320\265\321\200\320\262\320\275\321\213\320\265 \320\272\320\276\320\277\320\270\320\270 \320\277\321\200\320\276\321\210\320\270\320\262\320\272\320\270. \320\235\320\260\320\267\320\262\320\260\320\275\320\270\321\217 \320\272\320\276\320\277\320\270\320\271: bkp_<\321\202\320\270\320\277_\320\277\320\260\320\274\321\217\321\202\320\270>_<\320\264\320\260\321\202\320\260>_<\320\262\321\200\320\265\320\274\321\217>.hex \320\241\321\203\321\211\320\265\321\201\321\202\320\262\321\203\321\216\321\211\320\270\320\265 \321\204\320\260\320\271\320\273\321\213 \320\277\320\265\321\200\320\265\320\267\320\260\320\277\320\270\321\201\321\213\320\262\320\260\321\216\321\202\321\201\321\217 \320\261\320\265\320\267 \320\277\321\200\320\265\320\264\321\203\320\277\321\200\320\265"
                        "\320\266\320\264\320\265\320\275\320\270\321\217!", 0, QApplication::UnicodeUTF8));
        cb_make_backups->setText(QApplication::translate("MainWindow", "\320\222\321\213\320\277\320\276\320\273\320\275\321\217\321\202\321\214 \321\200\320\265\320\267\320\265\321\200\320\262\320\275\320\276\320\265 \320\272\320\276\320\277\320\270\321\200\320\276\320\262\320\260\320\275\320\270\320\265", 0, QApplication::UnicodeUTF8));
        groupBox_4->setTitle(QApplication::translate("MainWindow", "\320\243\321\201\321\202\321\200\320\276\320\271\321\201\321\202\320\262\320\276", 0, QApplication::UnicodeUTF8));
        label_5->setText(QApplication::translate("MainWindow", "\320\240\320\260\320\267\321\200\320\260\320\261\320\276\321\202\321\207\320\270\320\272:", 0, QApplication::UnicodeUTF8));
        label_6->setText(QApplication::translate("MainWindow", "\320\234\320\276\320\264\320\265\320\273\321\214:", 0, QApplication::UnicodeUTF8));
        label_7->setText(QApplication::translate("MainWindow", "\320\241\320\265\321\200\320\270\320\271\320\275\321\213\320\271 \320\275\320\276\320\274\320\265\321\200:", 0, QApplication::UnicodeUTF8));
        lblManufacturer->setText(QApplication::translate("MainWindow", "\320\235\320\265\320\270\320\267\320\262\320\265\321\201\321\202\320\275\320\276", 0, QApplication::UnicodeUTF8));
        lblModel->setText(QApplication::translate("MainWindow", "\320\235\320\265\320\270\320\267\320\262\320\265\321\201\321\202\320\275\320\276", 0, QApplication::UnicodeUTF8));
        lblSerial->setText(QApplication::translate("MainWindow", "\320\235\320\265\320\270\320\267\320\262\320\265\321\201\321\202\320\275\320\276", 0, QApplication::UnicodeUTF8));
        groupBox_5->setTitle(QApplication::translate("MainWindow", "\320\232\320\276\320\275\321\201\320\276\320\273\321\214", 0, QApplication::UnicodeUTF8));
        groupBox_6->setTitle(QApplication::translate("MainWindow", "\320\236\320\277\320\265\321\200\320\260\321\206\320\270\320\270", 0, QApplication::UnicodeUTF8));
        btnFlash->setText(QApplication::translate("MainWindow", "\320\237\321\200\320\276\321\210\320\270\321\202\321\214", 0, QApplication::UnicodeUTF8));
        btnReboot->setText(QApplication::translate("MainWindow", "\320\237\320\265\321\200\320\265\320\272\320\273\321\216\321\207\320\270\321\202\321\214 \321\203\321\201\321\202\321\200\320\276\320\271\321\201\321\202\320\262\320\276 \320\262 \320\276\320\261\321\213\321\207\320\275\321\213\320\271 \321\200\320\265\320\266\320\270\320\274", 0, QApplication::UnicodeUTF8));
        btnReadFlash->setText(QApplication::translate("MainWindow", "\320\247\320\270\321\202\320\260\321\202\321\214 FLASH \320\262 \321\204\320\260\320\271\320\273", 0, QApplication::UnicodeUTF8));
        btnReadEeprom->setText(QApplication::translate("MainWindow", "\320\247\320\270\321\202\320\260\321\202\321\214 EEPROM \320\262 \321\204\320\260\320\271\320\273", 0, QApplication::UnicodeUTF8));
    } // retranslateUi

};

namespace Ui {
    class MainWindow: public Ui_MainWindow {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_MAINWINDOW_H
