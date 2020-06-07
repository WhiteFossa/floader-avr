/*********************************************************************************/
/*                  Этот файл является частью загрузчика Фоссы                   */
/* Назначение:      Заголовочный файл главного окна                              */
/* Версия 0.0.3 от 15.01.2012                                                    */
/* Copyright 2012 Фосса aka Артём Ветров                                         */
/*                                                                               */
/*    This program is free software: you can redistribute it and/or modify       */
/*    it under the terms of the GNU General Public License as published by       */
/*    the Free Software Foundation, either version 3 of the License, or          */
/*    (at your option) any later version.                                        */
/*                                                                               */
/*    This program is distributed in the hope that it will be useful,            */
/*    but WITHOUT ANY WARRANTY; without even the implied warranty of             */
/*    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the              */
/*    GNU General Public License for more details.                               */
/*                                                                               */
/*    You should have received a copy of the GNU General Public License          */
/*    along with this program.  If not, see <http://www.gnu.org/licenses/>.      */
/*                                                                               */
/*    (Это свободная программа: вы можете перераспространять ее и/или изменять   */
/*    ее на условиях Стандартной общественной лицензии GNU в том виде, в каком   */
/*    она была опубликована Фондом свободного программного обеспечения; либо     */
/*    версии 3 лицензии, либо (по вашему выбору) любой более поздней версии.     */
/*                                                                               */
/*    Эта программа распространяется в надежде, что она будет полезной,          */
/*    но БЕЗО ВСЯКИХ ГАРАНТИЙ; даже без неявной гарантии ТОВАРНОГО ВИДА          */
/*    или ПРИГОДНОСТИ ДЛЯ ОПРЕДЕЛЕННЫХ ЦЕЛЕЙ. Подробнее см. в Стандартной        */
/*    общественной лицензии GNU.                                                 */
/*                                                                               */
/*    Вы должны были получить копию Стандартной общественной лицензии GNU        */
/*    вместе с этой программой. Если это не так, см.                             */
/*    <http://www.gnu.org/licenses/>.)                                           */
/*********************************************************************************/

#ifndef MAINWINDOW_H
#define MAINWINDOW_H

//Инклюды
#include <QMainWindow>
#include "portdialog.h" //Диалог выбора параметров порта
#include <QTextCodec>
#include "devicedriver.h" //Собственно драйвер работы с железом
#include <QFileDialog>
#include <QSettings>
#include <QFileInfo>
#include "ihexparser.h" //Парсер файлов Intel HEX
#include <QDir>
#include <QMessageBox>
#include <QDateTime>
#include <QIcon>
#include <QLatin1Char>

//Названия секций и параметров для хранения данных
#define MW_COMMONSETTINGS "commonsettings"
#define MW_BACKUPS_DIR "backups_dir"
#define MW_UPLOAD_FLASH_DIR "upload_flash_file_dir"
#define MW_UPLOAD_EEPROM_DIR "upload_eeprom_file_dir"
#define MW_DOWNLOAD_FLASH_DIR "download_flash_file_dir"
#define MW_DOWNLOAD_EEPROM_DIR "download_eeprom_file_dir"
#define MW_MAKE_BACKUPS "make_backups"

//Различные текстовые константы
#define MW_PORT_NOT_SPECIFIED "Не задан" //Порт не задан
#define MW_UNKNOWN "Неизвестно" //Для полей с информацией по устройству

namespace Ui {
    class MainWindow;
}

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    //Публичные свойства

    //Публичные методы

    //Конструктор
    explicit MainWindow(QWidget *parent = 0);

    //Деструктор
    ~MainWindow();

private:
    //Приватные свойства
    Ui::MainWindow *ui;
    PortDialog *port_dialog; //Диалог выбора порта
    DeviceDriver *driver; //Драйвер работы с устройством
    QString upload_flash_file_name; //Имя файла FLASH для прошивки
    QString upload_eeprom_file_name; //Аналогично, но для EEPROM
    bool upload_eeprom; //Если true, то надо прошить и eeprom
    QSettings settings; //Класс для хранения настроек
    QString base_directory; //Директория, откуда был запущен проишвальщик
    QString backups_directory; //Директория, куда будем писать бэкапы
    bool make_backups; //true, если будем делать бэкапы

    //Приватные методы

    //Метод выполняет чтение FLASH и возвращает экземпляр ihexparser с прочитанными данными. В случае ошибки возвращает null
    ihexparser* read_flash();

    //Метод аналогичен read_flash(), но читает EEPROM
    ihexparser* read_eeprom();

    //Метод выполняет перезагрузку устройства
    void reboot_device();

public slots:
    //Публичные слоты

private slots:
    //Приватные слоты
    void port_settings_slot(); //Слот настроек порта
    void set_upload_flash(); //Слот выбора файла для прошивки во FLASH
    void set_upload_eeprom(); //Слот выбора файла для прошивки в EEPROM
    void set_backups_directory_slot(); //Слот выбора директории для бэкапов
    void on_cbEepromUpload_toggled(bool checked); //Слот включения/выключения закачки EEPROM
    void query_device(); //Слот, вызываемый при нажатии на кнопку опроса устройства

    void flash_it(); //Слот, вызываемый при нажатии на кнопку прошивки
    void reboot_it(); //Слот, вызываемый при нажатии на кнопку перезагрузки устройства в обычный режим

    void read_flash_slot(); //Слот, вызываемый при нажатии на кнопку чтения FLASH в файл
    void read_eeprom_slot(); //Слот, вызываемый при нажатии на кнопку чтения EEPROM в файл

    void on_cb_make_backups_toggled(bool checked); //Слот включения/выключения резервного копирования
};

#endif // MAINWINDOW_H
