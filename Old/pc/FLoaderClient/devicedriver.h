/*********************************************************************************/
/*                  Этот файл является частью загрузчика Фоссы                   */
/* Назначение:      Заголовочный файл для класса драйвера устройства             */
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


#ifndef DEVICEDRIVER_H
#define DEVICEDRIVER_H

//Инклюды
#include <QSqlDatabase>
#include <QSqlQuery>
#include <QSqlRecord>
#include <QSqlError>
#include <QStringList>
#include <QVariant>
#include <QVector>
#include <QApplication>
#include <QCoreApplication>
#include <QTimer>
#include <QMutex>
#include <unistd.h>
#include <QDebug>

//Инклюды библиотеки QExtSerailPort (<http://code.google.com/p/qextserialport/>)
#include "qextserialport/qextserialport.h"
#include "qextserialport/qextserialport_global.h"
#include "qextserialport/qextserialenumerator.h"

//Дефайны
#define DD_BUFFER 1024 //Размер буфера для работы с портом
#define DD_FPAGEBUFFER 1024 //Размер буфера под страницу флеш
#define DD_EEBUFFER 1024 //Размер буфера под EEPROM

#define DD_POLL_WAIT 100 //Сколько микросекунд ждать перед опросом порта на предмет количества доступных байт
#define DD_RX_TIMEOUT 5000 //Если за столько миллисекунд не пришёл очередной байт, то отдаём сколько есть

//Структуры

//Структура с данными идентификации устройства
struct ident_data_struct
{
    //Данные, полученные от устройства
    unsigned int mfid; //Идентификатор производителя
    unsigned int devid; //Идентификатор устройства
    unsigned int sn; //Серийный номер устройства

    //Данные, вытащенные из БД
    unsigned int fpagesall; //Общее число страниц флеш
    unsigned int fpageswrite; //Число страниц флеш, доступных на запись (в загрузчик писать нельзя)
    unsigned int fpagesize; //Размер страницы флеш в байтах
    unsigned int eesize; //Размер EEPROM в байтах
    QString dev_descr; //Описание устройства
    QString man_descr; //Описание производителя
};

//Структура, содержащая одну страницу флеш
struct fpage_struct
{
    unsigned int num; //Номер страницы (от 0)
    unsigned int badrr; //Базовый адрес страницы
    char data[DD_FPAGEBUFFER]; //Байты страницы
};

//Перечисления

//Перечисление с статусом идентификации
enum ident_status_enum
{
    IDENT_NO, //Попытка идентификации не выполнялась
    IDENT_NOFBL, //Загрузчик не ответил
    IDENT_VERINCORR, //Некорректная версия загрузчика
    IDENT_UNKNOWN, //Загрузчик ответил, но мы не знаем о том железе, о котором он сообщил
    IDENT_DBERR, //Ошибка при работе с БД
    IDENT_OK //Идентификация успешна, в this->ident_data лежат данные по устройству
};

//Класс драйвера для работы с прошиваемым железом

class DeviceDriver : public QObject
{
Q_OBJECT
public:
    //Публичные свойства
    ident_status_enum ident_status; //Статус идентификации
    ident_data_struct ident_data; //Данные об устройстве (имеют смысл только в случае корректной идентификации)
    fpage_struct fpage; //Одна страница флеш памяти. Используется методами чтения и записи флеш
    char eebuffer[DD_EEBUFFER]; //Буфер под EEPROM

    //Публичные методы
    //Конструктор
    DeviceDriver();

    //Метод задаёт имя и параметры порта, с которыми будем работать
    void set_port(QextPortInfo name, PortSettings settings);

    //Метод пытается подключиться к устройству и опознать его. В случае успешного опознавания
    //в this->ident_data лежат параметры устройства и порт открыт для работы, в противном случае порт закрывается.
    //Статус идентификации можно прочитать в this->ident_status
    void connect();

    //Метод возвращает базовый адрес страницы флеш по её номеру. В случае ошибки (не получена информация по устройству) или
    //некорректного номера страницы возвращает -1. Счёт страниц с нуля
    int get_fpage_baddr(unsigned int page);

    //Метод считывает заданную страницу флеш. Номер считываемой страницы необходимо поместить в this->fpage.num,
    //в случае успешного чтения метод вернёт true и данные будут лежать this->fpage.data, иначе вернет false
    bool read_fpage();

    //Метод записывает заданную страницу флеш. Номер записываемой страницы необходимо поместить в this->fpage.num,
    //данные - в this->fpage.data. В случае успешной записи метод вернёт true, в случае ошибки - false
    bool write_fpage();

    //Метод считывает EEPROM. После считывания байты лежат в this->eebuffer. Число байт равно this->ident_data.eesize
    //В случае успеха возвращает true, иначе - false
    bool read_eeprom();

    //Метод записывает EEPROM. Данные для записи должны лежать в this->eebuffer. Запись производится до тех пор, пока
    //устройство готово принимать данные (т.е. при отсутствии ошибок записыввается this->ident_data.eesize байт)
    //В случае успеха возвращает true, в противном случае false
    bool write_eeprom();

    //Метод отстегивается от устройства. В случае успеха возвращает true, в противном случае - false. Даже в случае неуспеха
    //закрывает порт и отсоединяется от устройства
    bool disconnect();

    //Деструктор
    ~DeviceDriver();

private:
    //Приватные свойства
    QextSerialPort port; //Порт для обмена
    QextPortInfo portname; //Имя порта
    PortSettings portsettings; //Настройки порта

    bool params_setted; //true, если параметры порта заданы
    bool port_in_use; //true, если порт был открыт, но не закрыт
    char buffer[DD_BUFFER]; //Буфер для приема и передачи данных

    QTimer *rx_timeout_timer; //Таймер для отсчёта таймаута приёма
    bool rx_timeout; //Если true, то за DD_RX_TIMEOUT не было принято новых байт
    QMutex rx_timeout_mutex; //Семафор для rx_timeout

    //Приватные методы
    //Метод ждёт доступности для приёма заданного числа байтов, периодически (DD_POLL_WAIT)
    //опрашивая порт. Если за DD_POLL_CANCEL циклов нужного числа байт не поступило, то выходит
    void wait_for_bytes(unsigned int bytes);

signals:
    //Сигнал высылается при записи очередного байта eeprom и содержит номер байта (от нуля)
    void eeprom_write_byte(int);

private slots:

public slots:
    //Слот вызывается когда таймер приёма дотикал до 0
    void rx_timer_timeout();

};

#endif // DEVICEDRIVER_H
