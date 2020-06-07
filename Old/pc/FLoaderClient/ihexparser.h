/*********************************************************************************/
/*                  Этот файл является частью загрузчика Фоссы                   */
/* Назначение:      Заголовочный файл для кодека Intel HEX                       */
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

#ifndef IHEXPARSER_H
#define IHEXPARSER_H

#include <QFile>
#include <QVector>
#include <QByteArray>
#include <QMap>

#define IHP_OK 0
#define IHP_ERR 1

//Типы записей
#define IHP_REC_DATA 0x00 //Данные
#define IHP_REC_EOF 0x01 //Конец файла
#define IHP_REC_ESA 0x02 //Расширенный адрес сегмента
#define IHP_REC_SSA 0x03 //Точка входа внутри сегмента
#define IHP_REC_ELA 0x04 //Расширенный линейный адрес
#define IHP_REC_SLA 0x05 //Точка входа в линейном адресном пространстве

//Максимально возможный адрес
#define IHP_MAX_ADDR 0x10FFEF

//Структура, описывающая одну запись
typedef struct
{
    unsigned char reclen; //Длинна поля данных в записи
    unsigned short int offset; //Смещение данных (по-сути дела - адрес)
    unsigned char rectype; //Тип записи

    QByteArray data; //Данные
}
record_struct;

//Структура с одним блоком данных
typedef struct
{
    unsigned long start_addr; //Начальный и конечный адреса
    unsigned long end_addr;
    QVector<unsigned char> data; //Собственно данные
}
data_block_struct;

//Структура с одним сегментом
typedef struct
{
    unsigned int segment_addr; //Адрес сегмента (4 младших бита убиты, старшие - сдвинуты вправо)
    unsigned int in_segment_start_addr; //Стартовый адрес внутри сегмента

    QByteArray data; //Данные
}
segment_struct;

//Класс для чтения/записи Intel HEX формата
class ihexparser
{
public:
    //Публичные свойства

    //Публичные методы
    //Конструктор
    ihexparser();

    //Метод читает содержимое файла. Возвращает IHP_OK в случае успеха
    int read(QString path);

    //Метод записывает существующие блоки данных в файл. Возвращает IHP_OK в случае успеха
    int write(QString path);

    //Метод возвращает номер строки с ошибкой
    int get_errline();

    //Номер символа в строке
    int get_errchar();

    //Текстовое сообщение об ошибке
    QString get_errmesg();

    //Метод удаляет все существующие блоки данных
    void clear_data_blocks();

    //Метод добавляет очередной байт. Если байт можно добавить к блоку данных, то добавляет байт в блок и расширяет его.
    //Если добавить некуда - создаёт новый блок. Возвращает true в случае успеха и false, если такой байт уже существует
    //или если адрес некошерен
    bool add_byte(unsigned int addr, unsigned char byte);

    //Метод возвращает число присутствующих блоков данных (не важно, были-ли они прочитаны или добавлены)
    int get_num_of_data_blocks();

    //Метод возвращает блок с данными по его индексу. В случае некорректного индекса возвращает false
    bool get_data_block(int index, data_block_struct &block);

private:
    //Приватные свойства
    int errline; //Строка с ошибкой
    int errchar; //Символ строки с ошибкой
    QString errmesg; //Текстовое сообщение об ошибке
    QFile file; //Сам файл
    QVector <QByteArray> raw_lines; //Сырые строки файла
    QVector <QByteArray> byte_lines; //Содержимое строк в виде байтов
    QMap <char, int>symb_to_code; //Таблица преобразования символ->код
    QVector <record_struct> records; //Записи с данными
    QVector <data_block_struct> data_blocks; //Блоки с прочитанными данными
    int segment_addr; //Сегментный и внутрисегментный адреса для построения сегментов из потока байтов
    int in_segment_addr;
    QVector <segment_struct> segments; //Собственно сегменты
    QByteArray content_to_write; //То, что мы будем писать в файл
    QMap <int, char>code_to_symb; //Таблица преобразования код->символ

    //Приватные методы

    //Метод принимает два символа и возвращает код, им соответствующий (в Intel HEX каждый байт кодируется
    //его текстовым представлением). Считается, что некорректного символа не может быть
    unsigned char symb2value(char left, char right);

    //Метод обрывает последовательность байт для преобразования в сегменты и сбрасывает преобразование в начальное положение
    void reset_segment_conversion();

    //Метод находит наиболее подходящий (по запасу внутрисегментного адреса) адрес сегмента по линейному адресу
    //В случае успеха возвращает адрес сегмента, в случае ошибки -1
    int get_initial_segment_addr(unsigned int linear_addr);

    //Метод пытается определить внутрисегментный адрес для данного линейного адреса и сегментного адреса.
    //В случае успеха возвращает этот адрес, в противном случае -1, что указывает на необходимость определения
    //сегментного адреса методом get_initial_segment_addr()
    int get_addr_in_segment(unsigned int linear_addr, unsigned int segment_addr);

    //Метод добавляет очередной байт к содержимому сегментов. В случае успеха - возвращает true. В случае неуспеха
    //надо прекратить преобразование и вызвать reset_segment_conversion()
    bool add_next_segment_byte(unsigned int linear_addr, unsigned char byte);

    //Метод добавляет к переданной записи (в виде QByteArray) контрольную сумму
    QByteArray add_chksum(QByteArray record);

    //Метод принимает стартовый адрес сегмента и формирует сегментную запись. Проверку на корректность адреса не делает.
    //Сегментная запись формируется в бинарном виде, с контрольной суммой
    QByteArray make_esa_record(unsigned short int seg_addr);

    //Метод принимает набор байтов, переводит их в ASCII-представление, дописывает спереди двоеточие, сзади - перенос строки
    //и добавляет полученную строку к this->content_to_write
    void add_bytes_to_content(QByteArray bytes);

    //Метод принимает набор байтов данных, стартовый адрес и формирует запись данных (в бинарном виде, с контрольной суммой)
    //Длину входного массива не проверяет
    QByteArray make_data_record(unsigned short int start_addr, QByteArray bytes);
};

//Внеклассовая функция сравнения двух data_block_struct для сортировки. Условие сортировки: старший адрес меньшего меньше
//младшего адреса большего
bool data_block_struct_less_than(const data_block_struct &bl1, const data_block_struct &bl2);

#endif // IHEXPARSER_H
