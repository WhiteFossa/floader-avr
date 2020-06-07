/*********************************************************************************/
/*                  Этот файл является частью загрузчика Фоссы                   */
/* Назначение:      Заголовочный файл главного окна                              */
/* Версия 0.0.3 от 13.01.2012                                                    */
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

#include "mainwindow.h"
#include "ui_mainwindow.h"

MainWindow::MainWindow(QWidget *parent) : QMainWindow(parent), ui(new Ui::MainWindow)
{
    //Настраиваем пользовательский интерфейс
    this->ui->setupUi(this);

    //Соединяем сигналы и слоты
    QObject::connect(this->ui->btnPortSettings, SIGNAL(clicked()), this, SLOT(port_settings_slot())); //Кнопка настройки порта
    QObject::connect(this->ui->btnSetFlashFileUpload, SIGNAL(clicked()), this, SLOT(set_upload_flash())); //Кнопка выбора FLASH файла для прошивки
    QObject::connect(this->ui->btnSetEepromFileUpload, SIGNAL(clicked()), this, SLOT(set_upload_eeprom())); //Кнопка выбора EEPROM файла
    QObject::connect(this->ui->cbEepromUpload, SIGNAL(toggled(bool)), this, SLOT(on_cbEepromUpload_toggled(bool))); //Чекбокс записи в EEPROM
    QObject::connect(this->ui->btnSetBackupsDir, SIGNAL(clicked()), this, SLOT(set_backups_directory_slot())); //Слот выбора директории для бэкапов
    QObject::connect(this->ui->btnQueryDevice, SIGNAL(clicked()), this, SLOT(query_device())); //Слот опроса устройства
    QObject::connect(this->ui->btnFlash, SIGNAL(clicked()), this, SLOT(flash_it())); //Слот запуска прошивки
    QObject::connect(this->ui->btnReboot, SIGNAL(clicked()), this, SLOT(reboot_it())); //Слот перезагрузки устройства
    QObject::connect(this->ui->btnReadFlash, SIGNAL(clicked()), this, SLOT(read_flash_slot())); //Слот чтения FLASH
    QObject::connect(this->ui->btnReadEeprom, SIGNAL(clicked()), this, SLOT(read_eeprom_slot())); //Слот чтения EEPROM
    QObject::connect(this->ui->cb_make_backups, SIGNAL(toggled(bool)), this, SLOT(on_cb_make_backups_toggled(bool))); //Чекбокс создания резервной копии

    //Создаём объекты интерфейса
    this->port_dialog = new PortDialog(this);

    //Начальные настройки
    if (this->port_dialog->selected_port.portName == "")
    {
        //Порт не задан
        this->ui->lblPortName->setText(QObject::trUtf8(MW_PORT_NOT_SPECIFIED));
        this->ui->btnQueryDevice->setEnabled(false);
    }
    else
    {
        this->ui->lblPortName->setText(this->port_dialog->selected_port.portName + QObject::trUtf8(" - ") + this->port_dialog->selected_port.friendName);
        this->ui->btnQueryDevice->setEnabled(true);
    }


    this->ui->cbEepromUpload->setChecked(false); //По-умолчанию в EEPROM не пишем
    this->on_cbEepromUpload_toggled(false);

    //Драйвер устройства
    this->driver = new DeviceDriver();

    //Инициализация переменных
    this->upload_eeprom = false;

    //Находим базовую директорию
    this->base_directory = QApplication::applicationDirPath();

    //Вешаем иконку
    this->setWindowIcon(QIcon(this->base_directory + "/icon.png"));

    //Директория для бэкапов
    this->settings.beginGroup(MW_COMMONSETTINGS);
        this->backups_directory = this->settings.value(MW_BACKUPS_DIR, this->base_directory).toString();
    this->settings.endGroup();

    //Смотрим, существует-ли такая директория
    if (!QDir(this->backups_directory).exists())
    {
        //Нет, используем домашнюю
        this->backups_directory = this->base_directory;
    }

    //Смотрим, включены-ли бэкапы
    this->settings.beginGroup(MW_COMMONSETTINGS);
        this->make_backups = this->settings.value(MW_MAKE_BACKUPS, true).toBool();
    this->settings.endGroup();

    this->ui->cb_make_backups->setChecked(this->make_backups);
    this->ui->leBackupsDir->setEnabled(this->make_backups);
    this->ui->btnSetBackupsDir->setEnabled(this->make_backups);

    this->ui->leBackupsDir->setText(this->backups_directory);
    this->ui->teConsole->append(QObject::trUtf8("Кроссплатформенный клиент загрузчика Фоссы"));
    this->ui->teConsole->append(QObject::trUtf8("v0.0.3 от 13.10.2012"));
    this->ui->teConsole->append(QObject::trUtf8("http://fossa-white.livejournal.com"));
    this->ui->teConsole->append(QObject::trUtf8("http://whitefossa.ru/bootloader/"));
    this->ui->teConsole->append(QObject::trUtf8("mailto://fossa-artem@mail.ru"));
    this->ui->teConsole->append(QObject::trUtf8("Распространяется под GPLv3 или выше"));
}

//Слот настроек порта
void MainWindow::port_settings_slot()
{
    this->ui->btnFlash->setEnabled(false);
    this->ui->btnReboot->setEnabled(false);
    this->ui->btnReadFlash->setEnabled(false);
    this->ui->btnReadEeprom->setEnabled(false);

    //Вызываем диалог настроек
    this->port_dialog->exec();

    if (this->port_dialog->selected_port.portName == "")
    {
        //Порт не задан
        this->ui->lblPortName->setText(QObject::trUtf8(MW_PORT_NOT_SPECIFIED));
        this->ui->btnQueryDevice->setEnabled(false);
        return;
    }

    this->ui->lblPortName->setText(this->port_dialog->selected_port.portName + QObject::trUtf8(" - ") + this->port_dialog->selected_port.friendName);
    this->ui->btnQueryDevice->setEnabled(true);
}

//Слот, вызываемый при нажатии на кнопку опроса устройства
void MainWindow::query_device()
{
    this->ui->teConsole->clear();
    this->ui->teConsole->append(QObject::trUtf8("Порт:"));
    this->ui->teConsole->append(QObject::trUtf8(" Название: ") + this->port_dialog->selected_port.portName);
    this->ui->teConsole->append(QObject::trUtf8(" Описание: ") + this->port_dialog->selected_port.friendName);
    this->ui->teConsole->append(QObject::trUtf8(" Перечислитель: ") + this->port_dialog->selected_port.enumName);
    this->ui->teConsole->append(QObject::trUtf8(" Устройство: ") + this->port_dialog->selected_port.physName);
    this->driver->set_port(this->port_dialog->selected_port, this->port_dialog->port_settings);
    this->driver->connect();

    //Блокируем кнопки операций и сбрасываем информацию об устройстве
    this->ui->lblManufacturer->setText(QObject::trUtf8(MW_UNKNOWN));
    this->ui->lblModel->setText(QObject::trUtf8(MW_UNKNOWN));
    this->ui->lblSerial->setText(QObject::trUtf8(MW_UNKNOWN));
    this->ui->btnFlash->setEnabled(false);
    this->ui->btnReboot->setEnabled(false);
    this->ui->btnReadFlash->setEnabled(false);
    this->ui->btnReadEeprom->setEnabled(false);

    //Выводим данные как будто загрузчик ответил
    this->ui->teConsole->append(QObject::trUtf8("\nИнформация об устройстве:"));
    this->ui->teConsole->append(QObject::trUtf8(" Идентификатор производителя: %1").arg(this->driver->ident_data.mfid));
    this->ui->teConsole->append(QObject::trUtf8(" Идентификатор устройства: %1").arg(this->driver->ident_data.devid));
    this->ui->teConsole->append(QObject::trUtf8(" Серийный номер: %1").arg(this->driver->ident_data.sn));
    this->ui->teConsole->append(QObject::trUtf8(" Общее число страниц FLASH: %1").arg(this->driver->ident_data.fpagesall));
    this->ui->teConsole->append(QObject::trUtf8(" Из них доступны на запись: %1").arg(this->driver->ident_data.fpageswrite));
    this->ui->teConsole->append(QObject::trUtf8(" Размер страницы FLASH в байтах: %1").arg(this->driver->ident_data.fpagesize));
    this->ui->teConsole->append(QObject::trUtf8(" Размер EEPROM: %1").arg(this->driver->ident_data.eesize));
    this->ui->teConsole->append(QObject::trUtf8(" Описание разработчика: %1").arg(this->driver->ident_data.man_descr));
    this->ui->teConsole->append(QObject::trUtf8(" Описание устройства: %1").arg(this->driver->ident_data.dev_descr));


    switch(this->driver->ident_status)
    {
        case IDENT_NO:
            //Идентификация не запрашивалась, интересно, как такое может быть
            QMessageBox::critical(this, QObject::trUtf8("Идентификация не запрашивалась"), QObject::trUtf8("Интересно, как такое может быть? Похоже, где-то серьёзная ошибка"));
            exit(1);
        break;
        case IDENT_NOFBL:
            //Загрузчик не ответил
            this->driver->disconnect();
            this->ui->teConsole->append(QObject::trUtf8("ЗАГРУЗЧИК НЕ ОТВЕТИЛ!"));
            QMessageBox::critical(this, QObject::trUtf8("Загрузчик не отвечает"), QObject::trUtf8("Загрузчик не отвечает! Убедитесь, что:\n-Правильно указан порт и его настройки;\n-Устройство включено;\n-Устройство подключено к компьютеру;\n-Устройство находится в режиме загрузчика"));
            return;
        break;
        case IDENT_VERINCORR:
            //Некорректная версия
            this->driver->disconnect();
            this->ui->teConsole->append(QObject::trUtf8("НЕКОРРЕКТНАЯ ВЕРСИЯ!"));
            QMessageBox::critical(this, QObject::trUtf8("Некорректная версия протокола"), QObject::trUtf8("Загрузчик устройства использует не поддерживаемую данной программой версию протокола обмена данными."));
            return;
        break;
        case IDENT_UNKNOWN:
            //Загрузчик наш, но железо неизвестно
            this->driver->disconnect();
            this->ui->teConsole->append(QObject::trUtf8("НЕИЗВЕСТНОЕ УСТРОЙСТВО!"));
            QMessageBox::critical(this, QObject::trUtf8("Неизвестное устройство"), QObject::trUtf8("В базе данных устройств отсутствует информация об устройстве с данными идентификаторами.\nЕсли вы - разработчик устройства, то добавьте информацию об устройстве в файл ident.sqlite и повторите попытку."));
            return;
        break;
        case IDENT_DBERR:
            //Ошибка при работе с БД
            this->driver->disconnect();
            this->ui->teConsole->append(QObject::trUtf8("ОШИБКА ПРИ РАБОТЕ С БД!"));
            QMessageBox::critical(this, QObject::trUtf8("Ошибка при работе с базой данных"), QObject::trUtf8("Убедитесь, что файл ident.sqlite присутствует и не поврежден"));
            return;
        break;
        case IDENT_OK:
            //Всё успешно
            QMessageBox::information(this, QObject::trUtf8("Устройство успешно опознано"), QObject::trUtf8("Устройство успешно опознано, готов к прошивке."));
        break;
    }

    this->ui->teConsole->append(QObject::trUtf8("\nУстройство успешно опознано, можно прошивать!"));

    this->ui->lblManufacturer->setText(this->driver->ident_data.man_descr);
    this->ui->lblModel->setText(this->driver->ident_data.dev_descr);
    this->ui->lblSerial->setText(QString("%1").arg(this->driver->ident_data.sn));
    this->ui->btnFlash->setEnabled(true);
    this->ui->btnReboot->setEnabled(true);
    this->ui->btnReadFlash->setEnabled(true);
    this->ui->btnReadEeprom->setEnabled(true);
}

//Слот, вызываемый при нажатии на кнопку прошивки
void MainWindow::flash_it()
{
    //Проверяем, какие файлы заданы
    if (this->upload_flash_file_name == "")
    {
        QMessageBox::warning(this, QObject::trUtf8("Задайте имя файла с FLASH-прошивкой"), QObject::trUtf8("Задайте имя файла с FLASH-прошивкой"));
        return;
    }

    if ((this->upload_eeprom) && (this->upload_eeprom_file_name == ""))
    {
        QMessageBox::warning(this, QObject::trUtf8("Задайте имя файла с EEPROM-прошивкой"), QObject::trUtf8("Задайте имя файла с EEPROM-прошивкой"));
        return;
    }

    if (this->upload_flash_file_name == this->upload_eeprom_file_name)
    {
        QMessageBox::warning(this, QObject::trUtf8("Файлы совпадают"), QObject::trUtf8("Один и тот-же файл не может использоваться для прошивки и EEPROM и FLASH"));
        return;
    }

    if (!this->make_backups)
    {
        //Действительно-ли пользователь хочет шиться без бэкапа?
        if (QMessageBox::warning(this, QObject::trUtf8("Внимание!"), QObject::trUtf8("Сохранение резервных копий FLASH и EEPROM перед началом прошивки отключено! Если у вас нет резервных копий, вы не сможете вернуть устройство к состоянию, в котором оно было до прошивки!\nВы действительно хотите продолжить?"), QMessageBox::Yes | QMessageBox::No) != QMessageBox::Yes)
        {
            return;
        }
    }

    this->ui->teConsole->append(QObject::trUtf8("\nНачинаю разбор файла с FLASH прошивкой"));
    ihexparser upload_flash_parser;
    if (upload_flash_parser.read(this->upload_flash_file_name) != IHP_OK)
    {
        this->driver->disconnect();
        this->ui->teConsole->append(QObject::trUtf8("\nОшибка чтения файла с FLASH прошивкой:"));
        this->ui->teConsole->append(QObject::trUtf8(" Сообщение об ошибке: %1").arg(upload_flash_parser.get_errmesg()));
        this->ui->teConsole->append(QObject::trUtf8(" Строка: %1").arg(upload_flash_parser.get_errline()));
        this->ui->teConsole->append(QObject::trUtf8(" Символ: %1").arg(upload_flash_parser.get_errchar()));
        QMessageBox::critical(this, QObject::trUtf8("Ошибка чтения файла"), QObject::trUtf8("Ошибка чтения файла прошивки FLASH"));
        return;
    }

    this->ui->teConsole->append(QObject::trUtf8("Файл с FLASH прошивкой разобран"));

    if (upload_flash_parser.get_num_of_data_blocks() == 0)
    {
        this->driver->disconnect();
        this->ui->teConsole->append(QObject::trUtf8("\nФайл с FLASH прошивкой не содержит данных"));
        QMessageBox::critical(this, QObject::trUtf8("Файл пуст"), QObject::trUtf8("Файл с FLASH прошивкой не содержит данных"));
        return;
    }

    //Максимальный адрес во FLASH
    unsigned int max_flash_addr = this->driver->ident_data.fpagesize * this->driver->ident_data.fpagesall - 1;
    data_block_struct tmp_block;

    upload_flash_parser.get_data_block(upload_flash_parser.get_num_of_data_blocks() - 1, tmp_block);
    if (tmp_block.end_addr > max_flash_addr)
    {
        this->driver->disconnect();
        this->ui->teConsole->append(QObject::trUtf8("\nМаксимальный адрес в файле FLASH прошивки %1, в то время как максимальный адрес для данного устройства %2. Вероятно, файл прошивки не от того устройства").arg(tmp_block.end_addr).arg(max_flash_addr));
        QMessageBox::critical(this, QObject::trUtf8("Некорректный файл"), QObject::trUtf8("Максимальный адрес в файле FLASH прошивки слишком велик!"));
        return;
    }

    unsigned int max_writeable_flash_addr = this->driver->ident_data.fpagesize * this->driver->ident_data.fpageswrite - 1;
    if (tmp_block.end_addr >= max_writeable_flash_addr)
    {
        this->ui->teConsole->append(QObject::trUtf8("\nМаксимальный адрес в файле FLASH прошивки %1 находится в области загрузчика, т.к. максимальный доступный для записи адрес -  %2. Лишние байты будут проигнорированы").arg(tmp_block.end_addr).arg(max_writeable_flash_addr));
        QMessageBox::information(this, QObject::trUtf8("Прошивка содержит загрузчик"), QObject::trUtf8("FLASH прошивка содержит байты в области загрузчика, они будут проигнорированы!"));
    }

    //Создаём массив данных для записи, изначально заполняем его 0xFF
    QByteArray flash_to_write;
    for (unsigned int i = 0; i <= max_writeable_flash_addr; i++)
    {
        flash_to_write.push_back(0xFF);
    }

    //Заполняем реальными данными
    for (int i = 0; i < upload_flash_parser.get_num_of_data_blocks(); i++)
    {
        upload_flash_parser.get_data_block(i, tmp_block);

        for (unsigned int j = tmp_block.start_addr; j <= tmp_block.end_addr; j++)
        {
            if (j > max_writeable_flash_addr)
            {
                break;
            }

            flash_to_write[j] = tmp_block.data.at(j - tmp_block.start_addr);
        }
    }

    //Разбиваем на страницы
    QVector<fpage_struct> flash_pages;
    for (unsigned int i = 0; i < this->driver->ident_data.fpageswrite; i++)
    {
        fpage_struct tmp_page;
        tmp_page.num = i;
        tmp_page.badrr = i * this->driver->ident_data.fpagesize;
        for (unsigned int j = 0; j < this->driver->ident_data.fpagesize; j++)
        {
            tmp_page.data[j] = flash_to_write.at(tmp_page.badrr + j);
        }

        flash_pages.push_back(tmp_page);
    }

    flash_to_write.clear();

    //Начинаем разбирать EEPROM (если нужно)
    QByteArray eeprom_to_write; //Массив для записи в EEPROM
    if (this->upload_eeprom)
    {
        this->ui->teConsole->append(QObject::trUtf8("\nНачинаю разбирать файл прошивки EEPROM"));

        ihexparser upload_eeprom_parser;
        if (upload_eeprom_parser.read(this->upload_eeprom_file_name) != IHP_OK)
        {
            this->driver->disconnect();
            this->ui->teConsole->append(QObject::trUtf8("\nОшибка чтения файла с EEPROM прошивкой:"));
            this->ui->teConsole->append(QObject::trUtf8(" Сообщение об ошибке: %1").arg(upload_eeprom_parser.get_errmesg()));
            this->ui->teConsole->append(QObject::trUtf8(" Строка: %1").arg(upload_eeprom_parser.get_errline()));
            this->ui->teConsole->append(QObject::trUtf8(" Символ: %1").arg(upload_eeprom_parser.get_errchar()));
            QMessageBox::critical(this, QObject::trUtf8("Ошибка чтения файла"), QObject::trUtf8("Ошибка чтения файла прошивки EEPROM"));
            return;
        }

        this->ui->teConsole->append(QObject::trUtf8("Файл с EEPROM прошивкой разобран"));

        if (upload_eeprom_parser.get_num_of_data_blocks() == 0)
        {
            this->driver->disconnect();
            this->ui->teConsole->append(QObject::trUtf8("\nФайл с EEPROM прошивкой не содержит данных"));
            QMessageBox::critical(this, QObject::trUtf8("Файл пуст"), QObject::trUtf8("Файл с EEPROM прошивкой не содержит данных"));
            return;
        }

        //Максимальный адрес
        data_block_struct tmp_block;
        upload_eeprom_parser.get_data_block(upload_eeprom_parser.get_num_of_data_blocks() - 1, tmp_block);
        if (tmp_block.end_addr >= this->driver->ident_data.eesize)
        {
            this->driver->disconnect();
            this->ui->teConsole->append(QObject::trUtf8("\nМаксимальный адрес в файле EEPROM прошивки %1, в то время как максимальный адрес для данного устройства %2. Вероятно, файл прошивки не от того устройства").arg(tmp_block.end_addr).arg(this->driver->ident_data.eesize));
            QMessageBox::critical(this, QObject::trUtf8("Некорректный файл"), QObject::trUtf8("Максимальный адрес в файле EEPROM прошивки слишком велик!"));
            return;
        }

        //Создаём блок данных EEPROM
        for (unsigned int i = 0; i < this->driver->ident_data.eesize; i++)
        {
            eeprom_to_write.push_back(0xFF);
        }

        for (int i = 0; i < upload_eeprom_parser.get_num_of_data_blocks(); i++)
        {
            upload_eeprom_parser.get_data_block(i, tmp_block);

            for (unsigned int j = tmp_block.start_addr; j <= tmp_block.end_addr; j++)
            {
                eeprom_to_write[j] = tmp_block.data.at(j - tmp_block.start_addr);
            }
        }
    }

    this->ui->btnFlash->setEnabled(false); //Блокируем кнопку прошивки
    this->ui->btnReboot->setEnabled(false); //Блокируем кнопку перезагрузки
    this->ui->btnReadFlash->setEnabled(false); //Блокируем кнопки чтения
    this->ui->btnReadEeprom->setEnabled(false);

    //Начинаем резервное копирование (если включено)
    if (this->make_backups)
    {
        //Чтение резервной копии FLASH
        this->ui->teConsole->append(QObject::trUtf8("Создание резервной копии FLASH"));
        ihexparser *flash_bkp_parser = this->read_flash(); //Читаем FLASH
        if (flash_bkp_parser == NULL)
        {
            return;
        }

        //Записываем бэкап FLASH
        QDateTime current_datetime = QDateTime::currentDateTime();
        QString backup_flash_filename;
        backup_flash_filename.sprintf("/bkp_flash_%d%02d%02d_%d%d%d.hex",
                                       current_datetime.date().year(),
                                       current_datetime.date().month(),
                                       current_datetime.date().day(),
                                       current_datetime.time().hour(),
                                       current_datetime.time().minute(),
                                       current_datetime.time().second()
                                       );

        backup_flash_filename = this->backups_directory + backup_flash_filename;

        this->ui->teConsole->append(QObject::trUtf8("Файл резервной копии FLASH: %1").arg(backup_flash_filename));

        if (flash_bkp_parser->write(backup_flash_filename) != IHP_OK)
        {
            delete flash_bkp_parser;
            this->driver->disconnect();
            this->ui->teConsole->append(QObject::trUtf8("Не удалось записать файл резервной копии FLASH!"));
            QMessageBox::critical(this, QObject::trUtf8("Ошибка записи в файл"), QObject::trUtf8("Не удалось записать файл резервной копии FLASH!"));
            return;
        }
        delete flash_bkp_parser;


        this->ui->teConsole->append(QObject::trUtf8("Создание резервной копии EEPROM"));
        ihexparser *eeprom_bkp_parser = this->read_eeprom();
        if (eeprom_bkp_parser == NULL)
        {
            return;
        }

        //Записываем бэкап EEPROM
        current_datetime = QDateTime::currentDateTime();
        QString backup_eeprom_filename;
        backup_eeprom_filename.sprintf("/bkp_eeprom_%d%02d%02d_%d%d%d.hex",
                                       current_datetime.date().year(),
                                       current_datetime.date().month(),
                                       current_datetime.date().day(),
                                       current_datetime.time().hour(),
                                       current_datetime.time().minute(),
                                       current_datetime.time().second()
                                       );

        backup_eeprom_filename = this->backups_directory + backup_eeprom_filename;


        this->ui->teConsole->append(QObject::trUtf8("Файл резервной копии EEPROM: %1").arg(backup_eeprom_filename));

        if (eeprom_bkp_parser->write(backup_eeprom_filename) != IHP_OK)
        {
            delete eeprom_bkp_parser;
            this->driver->disconnect();
            this->ui->teConsole->append(QObject::trUtf8("Не удалось записать файл резервной копии EEPROM!"));
            QMessageBox::critical(this, QObject::trUtf8("Ошибка записи в файл"), QObject::trUtf8("Не удалось записать файл резервной копии EEPROM!"));
            return;
        }
        delete eeprom_bkp_parser;
    }

    QMessageBox::warning(this, QObject::trUtf8("Внимание!"), QObject::trUtf8("Сейчас начнётся процесс прошивки. Не отключайте устройство, компьютер, не перезагружайте их и не закрывайте эту программу!\nВ противном случае проишвка будет повреждена и её придётся восстанавливать через этот загрузчик!"));
    this->ui->teConsole->append(QObject::trUtf8("\nНачинаю прошивку.\nНЕ ОТКЛЮЧАЙТЕ УСТРОЙСТВО И НЕ ПЕРЕЗАГРУЖАЙТЕ КОМПЬЮТЕР ДО ЗАВЕРШЕНИЯ ПРОЦЕССА!"));

    //Страницы готовы, начинаем запись FLASH
    this->ui->teConsole->append(QObject::trUtf8("Записываю во FLASH"));
    for (unsigned int i = 0; i < this->driver->ident_data.fpageswrite; i++)
    {
        QCoreApplication::processEvents();
        this->ui->teConsole->append(QObject::trUtf8("Записываю страницу %1 из %2").arg(i + 1).arg(this->driver->ident_data.fpageswrite));

        this->driver->fpage.num = i;
        for (unsigned int j = 0; j < this->driver->ident_data.fpagesize; j++)
        {
            this->driver->fpage.data[j] = flash_pages.at(i).data[j];
        }

        if (!this->driver->write_fpage())
        {
            this->driver->disconnect();
            this->ui->teConsole->append(QObject::trUtf8("ОШИБКА ЗАПИСИ СТРАНИЦЫ FLASH!"));
            QMessageBox::critical(this, QObject::trUtf8("Ошибка ЗАПИСИ страницы FLASH"), QObject::trUtf8("При записи страницы %1 произошла ошибка! Прошивка в памяти устройства испорчена!\nПовторите процедуру прошивки!").arg(i + 1));
            return;
        }

        //Проверяем свежезаписанную страницу
        this->ui->teConsole->append(QObject::trUtf8("Проверяю страницу %1 из %2").arg(i + 1).arg(this->driver->ident_data.fpageswrite));

        this->driver->fpage.num = i;
        if (!this->driver->read_fpage())
        {
            //Не удалось прочитать страницу
            this->driver->disconnect();
            this->ui->teConsole->append(QObject::trUtf8("ОШИБКА ЧТЕНИЯ СТРАНИЦЫ FLASH!"));
            QMessageBox::critical(this, QObject::trUtf8("Ошибка чтения страницы FLASH"), QObject::trUtf8("Страница %1 не может быть прочитана").arg(i + 1));
            return;
        }

        for (unsigned int j = 0; j < this->driver->ident_data.fpagesize; j++)
        {
            if (this->driver->fpage.data[j] != flash_pages.at(i).data[j])
            {
                //Ошибка проверки
                this->driver->disconnect();
                this->ui->teConsole->append(QObject::trUtf8("ОШИБКА ПРОВЕРКИ СТРАНИЦЫ FLASH!"));
                QMessageBox::critical(this, QObject::trUtf8("Ошибка проверки страницы FLASH"), QObject::trUtf8("Содержимое записанной страницы %1 неверно").arg(i + 1));
                return;
            }
        }
        this->ui->teConsole->append(QObject::trUtf8("ОК"));
    }

    this->ui->teConsole->append(QObject::trUtf8("Запись FLASH завершена успешно"));

    QCoreApplication::processEvents();
    if (this->upload_eeprom)
    {
        this->ui->teConsole->append(QObject::trUtf8("\nЗаписываю EEPROM"));
        for (unsigned int i = 0; i < this->driver->ident_data.eesize; i++)
        {
            this->driver->eebuffer[i] = eeprom_to_write.at(i);
        }

        if (!this->driver->write_eeprom())
        {
            this->driver->disconnect();
            this->ui->teConsole->append(QObject::trUtf8("ОШИБКА ЗАПИСИ EEPROM!"));
            QMessageBox::critical(this, QObject::trUtf8("Ошибка ЗАПИСИ EEPROM"), QObject::trUtf8("При записи EEPROM произошла ошибка. Повторите прошивку!"));
            return;
        }

        QCoreApplication::processEvents();
        //Проверяем EEPROM
        this->ui->teConsole->append(QObject::trUtf8("Проверяю EEPROM"));
        if (!this->driver->read_eeprom())
        {
            this->driver->disconnect();
            this->ui->teConsole->append(QObject::trUtf8("ОШИБКА ЧТЕНИЯ EEPROM!"));
            QMessageBox::critical(this, QObject::trUtf8("Ошибка чтения EEPROM"), QObject::trUtf8("При чтении EEPROM произошла ошибка."));
            return;
        }

        for (unsigned int i = 0; i < this->driver->ident_data.eesize; i++)
        {
            if (eeprom_to_write.at(i) != this->driver->eebuffer[i])
            {
                this->driver->disconnect();
                this->ui->teConsole->append(QObject::trUtf8("ОШИБКА ПРОВЕРКИ EEPROM!"));
                QMessageBox::critical(this, QObject::trUtf8("Ошибка проверки EEPROM"), QObject::trUtf8("Прочитанное содержимое EEPROM не совпадает с записанным!"));
                return;
            }
        }

        QCoreApplication::processEvents();
        this->ui->teConsole->append(QObject::trUtf8("OK"));
    }

    this->ui->teConsole->append(QObject::trUtf8("\nПрошивка успешно завершена."));
    this->reboot_device();

    QMessageBox::information(this, QObject::trUtf8("Успех"), QObject::trUtf8("Прошивка успешно завершена!"));
}

//Слот выбора файла для прошивки во FLASH
void MainWindow::set_upload_flash()
{
    this->settings.beginGroup(MW_COMMONSETTINGS);
        QString saved_dir = this->settings.value(MW_UPLOAD_FLASH_DIR, this->base_directory).toString();
    this->settings.endGroup();

    if (!QDir(saved_dir).exists())
    {
        //Сохранённой директории нет, ставим домашнюю
        saved_dir = this->base_directory;
    }

    QString tmpstr = QFileDialog::getOpenFileName(this, QObject::trUtf8("Файл для прошивки во FLASH"), saved_dir, "*.hex");

    if (tmpstr == "")
    {
        return; //Отмена
    }

    this->ui->leFlashFileUpload->setText(tmpstr);
    this->upload_flash_file_name = tmpstr;
    this->settings.beginGroup(MW_COMMONSETTINGS);
        this->settings.setValue(MW_UPLOAD_FLASH_DIR, QFileInfo(tmpstr).absolutePath());
    this->settings.endGroup();
}

//Слот выбора файла для прошивки в EEPROM
void MainWindow::set_upload_eeprom()
{
    this->settings.beginGroup(MW_COMMONSETTINGS);
        QString saved_dir = this->settings.value(MW_UPLOAD_EEPROM_DIR, this->base_directory).toString();
    this->settings.endGroup();

    if (!QDir(saved_dir).exists())
    {
        //Сохранённой директории нет, ставим домашнюю
        saved_dir = this->base_directory;
    }

    QString tmpstr = QFileDialog::getOpenFileName(this, QObject::trUtf8("Файл для прошивки в EEPROM"), saved_dir, "*.hex");

    if (tmpstr == "")
    {
        return; //Отмена
    }

    this->ui->leEepromUploadFile->setText(tmpstr);
    this->upload_eeprom_file_name = tmpstr;
    this->settings.beginGroup(MW_COMMONSETTINGS);
        this->settings.setValue(MW_UPLOAD_EEPROM_DIR, QFileInfo(tmpstr).absolutePath());
    this->settings.endGroup();
}

 //Слот выбора директории для бэкапов
void MainWindow::set_backups_directory_slot()
{
    //Пытаемся поискать сохранённую директорию
    QString saved_dir;
    this->settings.beginGroup(MW_COMMONSETTINGS);
        saved_dir = this->settings.value(MW_BACKUPS_DIR, this->base_directory).toString();
    this->settings.endGroup();

    //Смотрим, существует-ли такая директория
    if (!QDir(saved_dir).exists())
    {
        //Нет, используем домашнюю
        saved_dir = this->base_directory;
    }

    QString tmp_dir = QFileDialog::getExistingDirectory(this, QObject::trUtf8("Директория для сохранения резервных копий:"), saved_dir, QFileDialog::ShowDirsOnly);
    if (tmp_dir == "")
    {
        return;
    }
    this->backups_directory = tmp_dir;
    this->ui->leBackupsDir->setText(this->backups_directory);
    this->settings.beginGroup(MW_COMMONSETTINGS);
        this->settings.setValue(MW_BACKUPS_DIR, this->backups_directory);
    this->settings.endGroup();
}

//Слот включения/выключения закачки EEPROM
void MainWindow::on_cbEepromUpload_toggled(bool checked)
{
    this->upload_eeprom = checked;

    this->ui->leEepromUploadFile->setEnabled(this->upload_eeprom);
    this->ui->btnSetEepromFileUpload->setEnabled(this->upload_eeprom);
}

//Метод выполняет чтение FLASH и возвращает экземпляр ihexparser с прочитанными данными. В случае ошибки возвращает null
ihexparser* MainWindow::read_flash()
{
    ihexparser* flash_parser = new ihexparser(); //Парсер для содержимого FLASH
    for (unsigned int i = 0; i < this->driver->ident_data.fpagesall; i++)
    {
        QCoreApplication::processEvents();
        this->ui->teConsole->append(QObject::trUtf8("Читаю страницу %1 из %2").arg(i + 1).arg(this->driver->ident_data.fpagesall));
        this->driver->fpage.num = i;
        if (!this->driver->read_fpage())
        {
            //Не удалось прочитать страницу
            delete flash_parser;
            this->driver->disconnect();
            this->ui->teConsole->append(QObject::trUtf8("ОШИБКА ЧТЕНИЯ СТРАНИЦЫ FLASH!"));
            QMessageBox::critical(this, QObject::trUtf8("Ошибка чтения страницы FLASH"), QObject::trUtf8("Страница %1 не может быть прочитана").arg(i + 1));
            return NULL;
        }

        this->ui->teConsole->append(QObject::trUtf8("Добавляю страницу %1 из %2 в файл").arg(i + 1).arg(this->driver->ident_data.fpagesall));
        for (unsigned int j = 0; j < this->driver->ident_data.fpagesize; j++)
        {
            if (!flash_parser->add_byte(this->driver->fpage.badrr + j, this->driver->fpage.data[j]))
            {
                //Не удалось добавить байт
                delete flash_parser;
                this->driver->disconnect();
                this->ui->teConsole->append(QObject::trUtf8("Не удалось добавить байт %1 страницы %2 в файл!").arg(j + 1).arg(i + 1));
                QMessageBox::critical(this, QObject::trUtf8("Ошибка добавления в файл"), QObject::trUtf8("Не удалось добавить байт %1 страницы %2 в файл!").arg(j + 1).arg(i + 1));
                return NULL;
            }
        }
    }

    return flash_parser;
}

//Метод аналогичен read_flash(), но читает EEPROM
ihexparser* MainWindow::read_eeprom()
{
    ihexparser *eeprom_parser = new ihexparser();
    this->ui->teConsole->append(QObject::trUtf8("Чтение EEPROM"));
    if (!this->driver->read_eeprom())
    {
        //Не удалось прочитать
        delete eeprom_parser;
        this->driver->disconnect();
        this->ui->teConsole->append(QObject::trUtf8("Не удалось прочитать EEPROM"));
        QMessageBox::critical(this, QObject::trUtf8("Ошибка чтения EEPROM"), QObject::trUtf8("Не удалось прочитать EEPROM"));
        return NULL;
    }

    for (unsigned int i = 0; i < this->driver->ident_data.eesize; i++)
    {
        if (!eeprom_parser->add_byte(i, this->driver->eebuffer[i]))
        {
            //Не удалось добавить байт
            delete eeprom_parser;
            this->driver->disconnect();
            this->ui->teConsole->append(QObject::trUtf8("Не удалось добавить байт %1 из %2 в файл!").arg(i + 1).arg(this->driver->ident_data.eesize));
            QMessageBox::critical(this, QObject::trUtf8("Ошибка добавления в файл"), QObject::trUtf8("Не удалось добавить байт %1 из %2 в файл!").arg(i + 1).arg(this->driver->ident_data.eesize));
            return NULL;
        }
    }

    return eeprom_parser;
}

//Метод выполняет перезагрузку устройства
void MainWindow::reboot_device()
{
    this->ui->teConsole->append(QObject::trUtf8("Запрашиваю перезагрузку устройства..."));
    if (!this->driver->disconnect())
    {
        this->ui->teConsole->append(QObject::trUtf8("Устройство не подтвердило перезагрузку!"));
        QMessageBox::critical(this, QObject::trUtf8("Устройство не подтвердило перезагрузку"), QObject::trUtf8("Устройство не подтвердило перезагрузку, перезагрузите его вручную!"));
    }
    else
    {
        this->ui->teConsole->append(QObject::trUtf8("Устройство перезагружается"));
    }
}

//Слот, вызываемый при нажатии на кнопку перезагрузки устройства в обычный режим
void MainWindow::reboot_it()
{
    this->ui->btnFlash->setEnabled(false);
    this->ui->btnReboot->setEnabled(false);
    this->ui->btnReadFlash->setEnabled(false);
    this->ui->btnReadEeprom->setEnabled(false);

    this->reboot_device();
}

//Слот, вызываемый при нажатии на кнопку чтения FLASH в файл
void MainWindow::read_flash_slot()
{
    //Пытаемся поискать сохранённую директорию
    QString saved_dir;
    this->settings.beginGroup(MW_COMMONSETTINGS);
        saved_dir = this->settings.value(MW_DOWNLOAD_FLASH_DIR, this->base_directory).toString();
    this->settings.endGroup();

    //Смотрим, существует-ли такая директория
    if (!QDir(saved_dir).exists())
    {
        //Нет, используем домашнюю
        saved_dir = this->base_directory;
    }

    QString tmpstr = QFileDialog::getSaveFileName(this, QObject::trUtf8("Файл, в который будет сохранена FLASH"), saved_dir, QObject::trUtf8("Intel HEX (*.hex *.HEX);;Все файлы (*)"));

    if (tmpstr == "")
    {
        return; //Отмена
    }

    this->settings.beginGroup(MW_COMMONSETTINGS);
        this->settings.setValue(MW_DOWNLOAD_FLASH_DIR, QFileInfo(tmpstr).absolutePath());
    this->settings.endGroup();

    //Чтение FLASH
    this->ui->teConsole->append(QObject::trUtf8("\nЧтение FLASH в файл:"));
    this->ui->teConsole->append(tmpstr + "\n");

    ihexparser *flash_parser = this->read_flash(); //Читаем FLASH
    if (flash_parser == NULL)
    {
        return;
    }

    if (flash_parser->write(tmpstr) != IHP_OK)
    {
        delete flash_parser;
        this->driver->disconnect();
        this->ui->teConsole->append(QObject::trUtf8("Не удалось записать файл с содержимым FLASH!"));
        QMessageBox::critical(this, QObject::trUtf8("Ошибка записи в файл"), QObject::trUtf8("Не удалось записать файл с содержимым FLASH!"));
        return;
    }
    delete flash_parser;

    this->ui->teConsole->append(QObject::trUtf8("\nСодержимое FLASH успешно сохранено в файле:"));
    this->ui->teConsole->append(tmpstr + "\n");
}

//Слот, вызываемый при нажатии на кнопку чтения EEPROM в файл
void MainWindow::read_eeprom_slot()
{
    //Пытаемся поискать сохранённую директорию
    QString saved_dir;
    this->settings.beginGroup(MW_COMMONSETTINGS);
        saved_dir = this->settings.value(MW_DOWNLOAD_EEPROM_DIR, this->base_directory).toString();
    this->settings.endGroup();

    //Смотрим, существует-ли такая директория
    if (!QDir(saved_dir).exists())
    {
        //Нет, используем домашнюю
        saved_dir = this->base_directory;
    }

    QString tmpstr = QFileDialog::getSaveFileName(this, QObject::trUtf8("Файл, в который будет сохранена EEPROM"), saved_dir, QObject::trUtf8("Intel HEX (*.hex *.HEX);;Все файлы (*)"));

    if (tmpstr == "")
    {
        return; //Отмена
    }

    this->settings.beginGroup(MW_COMMONSETTINGS);
        this->settings.setValue(MW_DOWNLOAD_EEPROM_DIR, QFileInfo(tmpstr).absolutePath());
    this->settings.endGroup();

    //Чтение EEPROM
    this->ui->teConsole->append(QObject::trUtf8("\nЧтение EEPROM в файл:"));
    this->ui->teConsole->append(tmpstr + "\n");

    ihexparser *eeprom_parser = this->read_eeprom(); //Читаем EEPROM
    if (eeprom_parser == NULL)
    {
        return;
    }

    if (eeprom_parser->write(tmpstr) != IHP_OK)
    {
        delete eeprom_parser;
        this->driver->disconnect();
        this->ui->teConsole->append(QObject::trUtf8("Не удалось записать файл с содержимым EEPROM!"));
        QMessageBox::critical(this, QObject::trUtf8("Ошибка записи в файл"), QObject::trUtf8("Не удалось записать файл с содержимым EEPROM!"));
        return;
    }
    delete eeprom_parser;

    this->ui->teConsole->append(QObject::trUtf8("\nСодержимое EEPROM успешно сохранено в файле:"));
    this->ui->teConsole->append(tmpstr + "\n");
}

//Слот включения/выключения резервного копирования
void MainWindow::on_cb_make_backups_toggled(bool checked)
{
    qDebug() << "Slot fired";
    this->make_backups = checked;

    this->ui->leBackupsDir->setEnabled(this->make_backups);
    this->ui->btnSetBackupsDir->setEnabled(this->make_backups);

    //Сохраняем настройку
    this->settings.beginGroup(MW_COMMONSETTINGS);
        this->settings.setValue(MW_MAKE_BACKUPS, this->make_backups);
    this->settings.endGroup();
}

MainWindow::~MainWindow()
{
    //Удаляем драйвер
    delete this->driver;

    //Удаляем пользовательский интерфейс
    delete this->ui;
}
