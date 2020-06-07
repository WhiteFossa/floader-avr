/*********************************************************************************/
/*                  Этот файл является частью загрузчика Фоссы                   */
/* Назначение:      Файл реализации класса драйвера устройства                   */
/* Версия 0.0.3 от 13.10.2012                                                    */
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

#include "devicedriver.h"

//Конструктор
DeviceDriver::DeviceDriver()
{
    this->params_setted = false; //Параметры порта ещё не заданы
    this->port_in_use = false; //Порт ещё не используется
    this->ident_status = IDENT_NO; //Мы ещё не пытались идентифицироваться

    //Создаём и настраиваем таймер таймаута приёма
    this->rx_timeout_timer = new QTimer(this);
    this->rx_timeout_timer->setSingleShot(true); //Однократное срабатывание
    this->rx_timeout_timer->setInterval(DD_RX_TIMEOUT); //Интервал

    QObject::connect(this->rx_timeout_timer, SIGNAL(timeout()), this, SLOT(rx_timer_timeout()));
}

//Метод задаёт имя и параметры порта, с которыми будем работать
void DeviceDriver::set_port(QextPortInfo name, PortSettings settings)
{
    if (this->port_in_use)
    {
        this->port.close();
        this->port_in_use = false;
        this->ident_status = IDENT_NO; //Порт сменился
    }

    this->portname = name;
    this->portsettings = settings;
    this->params_setted = true;
}

//Метод пытается подключиться к устройству и опознать его. В случае успешного опознавания
//в this->ident_data лежат параметры устройства и порт открыт для работы, в противном случае порт закрывается.
//Статус идентификации можно прочитать в this->ident_status
void DeviceDriver::connect()
{
    if ((!this->params_setted) || (this->port_in_use))
    {
        return;
    }

#if defined(Q_WS_WIN)
    this->port.setPortName(this->portname.portName); //Название порта
#else
    this->port.setPortName(this->portname.physName);
#endif

    //Открываем порт
    this->port.open(QIODevice::ReadWrite);
    this->port_in_use = true;

    this->port.setBaudRate(this->portsettings.BaudRate); //Параметры порта
    this->port.setDataBits(this->portsettings.DataBits);
    this->port.setFlowControl(this->portsettings.FlowControl);
    this->port.setParity(this->portsettings.Parity);
    this->port.setStopBits(this->portsettings.StopBits);
    this->port.setTimeout(this->portsettings.Timeout_Millisec);

    //Запрашиваем идентификацию
    this->buffer[0] = 'I'; //Посылаем 1 байт - 0x49(I)
    this->port.write(this->buffer, 1);

    //Принимаем ответ
    this->wait_for_bytes(14);
    int rd_cnt; //Число прочитанных байт
    rd_cnt = this->port.read(this->buffer, DD_BUFFER);

    if (rd_cnt != 14)
    {
        //Длина не равна идентификационной записи
        this->ident_status = IDENT_NOFBL;
        this->port.close();
        this->port_in_use = false;
        return;
    }

    if ((this->buffer[0] != 'F') || (this->buffer[1] != 'B') || (this->buffer[2] != 'L'))
    {
        //Сигнатура не совпадает
        this->ident_status = IDENT_NOFBL;
        this->port.close();
        this->port_in_use = false;
        return;
    }

    //Проверяем версию (пока-что можем работать только с версией 1)
    if (this->buffer[3] != 0x01)
    {
        this->ident_status = IDENT_VERINCORR;
        this->port.close();
        this->port_in_use = false;
        return;
    }

    //Выделяем идентификатор производителя
    this->ident_data.mfid = 0x10000 * this->buffer[4] + 0x100 * this->buffer[5] + this->buffer[6];

    //Идентификатор устройства
    this->ident_data.devid = 0x10000 * this->buffer[7] + 0x100 * this->buffer[8] + this->buffer[9];

    //Серйиный номер
    this->ident_data.sn = 0x1000000 * this->buffer[10] + 0x10000 * this->buffer[11] + 0x100 * this->buffer[12] + this->buffer[13];

    //Данные от устройства получены, ищем их в нашей БД
    QSqlDatabase dev_db; //База данных с данными об устройствах

    dev_db = QSqlDatabase::addDatabase("QSQLITE");
    dev_db.setDatabaseName(QApplication::applicationDirPath() + "/ident.sqlite");

    if (!dev_db.open())
    {
        //Не удалось прицепиться к БД
        this->ident_status = IDENT_DBERR;
        this->port.close();
        this->port_in_use = false;
        return;
    }

    QSqlQuery dev_query(dev_db); //Объект запроса для БД (создаём строго после открытия соединения!)

    //Запрашиваем идентификацию из БД
    QString sql_string = QString("SELECT count(*), devices.fpagesall, devices.fpageswrite, devices.fpagesize, devices.eesize, devices.description as dev_descr, manufacturers.description as man_descr FROM devices, manufacturers WHERE manufacturers.mfid = devices.mfid AND devices.devid = %1 AND devices.mfid = %2").arg(this->ident_data.devid).arg(this->ident_data.mfid);
    if (!dev_query.exec(sql_string))
    {
        this->ident_status = IDENT_DBERR;
        dev_db.close();
        this->port.close();
        this->port_in_use = false;
        return;
    }

    QSqlRecord dev_record = dev_query.record(); //Запись информацией о результатах
    dev_query.first(); //Переходим к первой строке результата (она всегда есть, бо count(*))

    int num_of_rows; //Число возвращённых строк
    num_of_rows = dev_query.value(dev_record.indexOf("count(*)")).toInt();
    if (num_of_rows != 1)
    {
        //Мы не знаем о таком железе
        this->ident_status = IDENT_UNKNOWN;
        dev_db.close();
        this->port.close();
        this->port_in_use = false;
        return;
    }

    //Параметры устройства
    this->ident_data.fpagesall = dev_query.value(dev_record.indexOf("fpagesall")).toInt();
    this->ident_data.fpageswrite = dev_query.value(dev_record.indexOf("fpageswrite")).toInt();
    this->ident_data.fpagesize = dev_query.value(dev_record.indexOf("fpagesize")).toInt();
    this->ident_data.eesize = dev_query.value(dev_record.indexOf("eesize")).toInt();
    this->ident_data.fpagesall = dev_query.value(dev_record.indexOf("fpagesall")).toInt();
    this->ident_data.dev_descr = dev_query.value(dev_record.indexOf("dev_descr")).toString();
    this->ident_data.man_descr = dev_query.value(dev_record.indexOf("man_descr")).toString();

    //Отцепляемся от БД
    dev_db.close();

    //Идентификация успешна
    this->ident_status = IDENT_OK;
}

//Метод возвращает базовый адрес страницы флеш по её номеру. В случае ошибки (не получена информация по устройству) или
//некорректного номера страницы возвращает -1. Счёт страниц с нуля
int DeviceDriver::get_fpage_baddr(unsigned int page)
{
    if (!this->port_in_use)
    {
        return -1;
    }

    if (this->ident_status != IDENT_OK)
    {
        return -1;
    }

    if (page >= this->ident_data.fpagesall)
    {
        return -1;
    }

    return page * this->ident_data.fpagesize;
}

//Метод считывает заданную страницу флеш. Номер считываемой страницы необходимо поместить в this->fpage.num,
//в случае успешного чтения метод вернёт true и данные будут лежать this->fpage.data, иначе вернет false
bool DeviceDriver::read_fpage()
{
    if (!this->port_in_use)
    {
        return false;
    }

    if (this->ident_status != IDENT_OK)
    {
        return false;
    }

    if (this->fpage.num >= this->ident_data.fpagesall)
    {
        return false;
    }

    //Берем базовй адрес
    this->fpage.badrr = this->get_fpage_baddr(this->fpage.num);

    //Собственно читаем
    this->buffer[0] = 'R'; //Команда чтения
    this->buffer[1] = this->fpage.num; //Номер страницы
    this->port.write(this->buffer, 2);

    //Принимаем ответ
    this->wait_for_bytes(this->ident_data.fpagesize + 1);
    unsigned int rd_cnt; //Число прочитанных байт
    rd_cnt = this->port.read(this->buffer, DD_BUFFER);

    if ((rd_cnt != 1) && (rd_cnt != this->ident_data.fpagesize + 1))
    {
        return false; //Что-то левое пришло
    }

    if (this->buffer[0] != 0x00)
    {
        //Устройство говорит, мол ошибка чтения
        return false;
    }
    else
    {
        //Страница прочитана успешно
        for (unsigned int i = 0; i < this->ident_data.fpagesize; i++)
        {
            this->fpage.data[i] = this->buffer[i + 1];
        }

        return true;
    }
}

//Метод записывает заданную страницу флеш. Номер считываемой страницы необходимо поместить в this->fpage.num,
//данные - в this->fpage.data. В случае успешной записи метод вернёт true, в случае ошибки - false
bool DeviceDriver::write_fpage()
{
    if (!this->port_in_use)
    {
        return false;
    }

    if (this->ident_status != IDENT_OK)
    {
        return false;
    }

    if (this->fpage.num >= this->ident_data.fpagesall)
    {
        return false;
    }

    //Берем базовй адрес
    this->fpage.badrr = this->get_fpage_baddr(this->fpage.num);

    //Сообщаем о намерении писать страницу
    this->buffer[0] = 'W'; //Команда чтения
    this->buffer[1] = this->fpage.num; //Номер страницы
    this->port.write(this->buffer, 2);

    //Принимаем ответ
    this->wait_for_bytes(1);
    int rd_cnt; //Число прочитанных байт
    rd_cnt = this->port.read(this->buffer, DD_BUFFER);
    if (rd_cnt != 1)
    {
        return false; //Пришло что-то левое
    }

    if (this->buffer[0] != 0x00)
    {
        return false; //Устройство отказывается писать по этому адресу, возможно попытка записи в загрузчик
    }

    //Загружаем буфер для записи
    for (unsigned int i = 0; i < this->ident_data.fpagesize; i++)
    {
        this->buffer[i] = this->fpage.data[i];
    }

    //Пишем
    this->port.write(this->buffer, this->ident_data.fpagesize);

    //Проверяем результат
    this->wait_for_bytes(1);
    rd_cnt = this->port.read(this->buffer, DD_BUFFER);
    if (rd_cnt != 1)
    {
        return false; //Пришло что-то левое
    }

    if (this->buffer[0] != 0x00)
    {
        return false; //Код ошибки после записи не 0
    }

    //Успешно записали
    return true;
}

//Метод считывает EEPROM. После считывания байты лежат в this->eebuffer. Число байт равно this->ident_data.eesize
//В случае успеха возвращает true, иначе - false
bool DeviceDriver::read_eeprom()
{
    if (!this->port_in_use)
    {
        return false;
    }

    if (this->ident_status != IDENT_OK)
    {
        return false;
    }

    //Запрашиваем чтение EEPROM
    this->buffer[0] = 'r'; //Команда чтения
    this->port.write(this->buffer, 1);

    //Принимаем ответ
    this->wait_for_bytes(this->ident_data.eesize);
    unsigned int rd_cnt; //Число прочитанных байт
    rd_cnt = this->port.read(this->buffer, DD_BUFFER);
    if (rd_cnt != this->ident_data.eesize)
    {
        return false; //Число принятых байт не равно размеру EEPROM
    }

    for (unsigned int i = 0; i < this->ident_data.eesize; i++)
    {
        this->eebuffer[i] = this->buffer[i];
    }

    return true;
}

//Метод записывает EEPROM. Данные для записи должны лежать в this->eebuffer. Запись производится до тех пор, пока
//устройство готово принимать данные (т.е. при отсутствии ошибок записыввается this->ident_data.eesize байт)
//В случае успеха возвращает true, в противном случае false
bool DeviceDriver::write_eeprom()
{
    if (!this->port_in_use)
    {
        return false;
    }

    if (this->ident_status != IDENT_OK)
    {
        return false;
    }

    unsigned int i = 0; //Индекс байта
    int rd_cnt; //Число прочитанных байт

    this->buffer[0] = 'w'; //Начинаем запись
    this->port.write(this->buffer, 1);

    do
    {
        QCoreApplication::processEvents();
        //Проверяем кошерность индекса
        if (i >= this->ident_data.eesize)
        {
            return false; //Вылезли за размер EEPROM
        }

        if (i >= DD_EEBUFFER)
        {
            return false; //Непонятно каким хвостом вылезли за буфер
        }

        this->buffer[0] = this->eebuffer[i];
        this->port.write(this->buffer, 1); //Передаем байт
        emit this->eeprom_write_byte(i);

        this->wait_for_bytes(1);
        rd_cnt = this->port.read(this->buffer, DD_BUFFER);
        if (rd_cnt != 1)
        {
            return false; //Что-то непонятное пришло
        }

        if ((this->buffer[0] != 'n') && (this->buffer[0] != 'f'))
        {
            //Непонятный ответ
            return false;
        }

        if (this->buffer[0] == 'f')
        {
            //Больше байтов не надо
            if (i == this->ident_data.eesize - 1)
            {
                //Закончили на нужном байте
                return true;
            }
            else
            {
                return false;
            }
        }

        i++;
    }
    while(true);
}

//Метод отстегивается от устройства. В случае успеха возвращает true, в противном случае - false
bool DeviceDriver::disconnect()
{
    if (!this->port_in_use)
    {
        return false;
    }

    if (this->ident_status != IDENT_OK)
    {
        this->port.close();
        this->port_in_use = false;
        this->ident_status = IDENT_NO;
        return false;
    }

    this->buffer[0] = 'Q';
    this->port.write(this->buffer, 1); //Передаем байт

    this->wait_for_bytes(1);
    int rd_cnt = this->port.read(this->buffer, DD_BUFFER);
    if (rd_cnt != 1)
    {
        this->port.close();
        this->port_in_use = false;
        this->ident_status = IDENT_NO;
        return false; //Что-то непонятное пришло
    }

    if (this->buffer[0] != 'B')
    {
        //Устройство не попрощалось, похоже что-то не так, но всё равно отключаемся
        this->port.close();
        this->port_in_use = false;
        this->ident_status = IDENT_NO;
        return false;
    }

    this->port.close();
    this->port_in_use = false;
    this->ident_status = IDENT_NO;
    return true;
}

//Метод ждёт доступности для приёма заданного числа байтов, периодически (DD_POLL_WAIT)
//опрашивая порт. Если за DD_BYTE_TIMEOUT не поступил очередной байт, то выходит и отдаёт сколько есть.
void DeviceDriver::wait_for_bytes(unsigned int bytes)
{
    //Пока-что таймаута нет
    this->rx_timeout_mutex.lock();
    this->rx_timeout = false;
    this->rx_timeout_mutex.unlock();

    this->rx_timeout_timer->start(); //Запускаем таймер (перезапускаем, если он уже был запущен)

    unsigned int recieved_bytes = 0; //Счётчик принятых байт

    while(true)
    {
        bool is_timeout;
        this->rx_timeout_mutex.lock();
        is_timeout = this->rx_timeout;
        this->rx_timeout_mutex.unlock();

        if (is_timeout)
        {
            //Выходим с тем, что есть; таймер уже стоит
            return;
        }

        unsigned int available = this->port.bytesAvailable();

        if (available > recieved_bytes)
        {
            //Есть новые байты
            recieved_bytes = available;

            if (available >= bytes)
            {
                //Приняли всё, что надо
                this->rx_timeout_timer->stop();
                return;
            }

            //Нужны ещё байты
            this->rx_timeout_timer->start(); //Перезапускаем таймер
        }

        usleep(DD_POLL_WAIT);
        QCoreApplication::processEvents();
    }
}

//Слот вызывается когда таймер приёма дотикал до 0
void DeviceDriver::rx_timer_timeout()
{
    this->rx_timeout_mutex.lock();
    this->rx_timeout = true;
    this->rx_timeout_mutex.unlock();
}

//Деструктор
DeviceDriver::~DeviceDriver()
{
    //Удаляем таймер приёма
    this->rx_timeout_timer->stop();
    delete this->rx_timeout_timer;
}
