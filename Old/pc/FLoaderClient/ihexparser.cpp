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

#include "ihexparser.h"

//Конструктор
ihexparser::ihexparser()
{
    //Инициализируем таблицу преобразования
    this->symb_to_code['0'] = 0;
    this->symb_to_code['1'] = 1;
    this->symb_to_code['2'] = 2;
    this->symb_to_code['3'] = 3;
    this->symb_to_code['4'] = 4;
    this->symb_to_code['5'] = 5;
    this->symb_to_code['6'] = 6;
    this->symb_to_code['7'] = 7;
    this->symb_to_code['8'] = 8;
    this->symb_to_code['9'] = 9;
    this->symb_to_code['A'] = 10;
    this->symb_to_code['B'] = 11;
    this->symb_to_code['C'] = 12;
    this->symb_to_code['D'] = 13;
    this->symb_to_code['E'] = 14;
    this->symb_to_code['F'] = 15;

    this->code_to_symb[0] = '0';
    this->code_to_symb[1] = '1';
    this->code_to_symb[2] = '2';
    this->code_to_symb[3] = '3';
    this->code_to_symb[4] = '4';
    this->code_to_symb[5] = '5';
    this->code_to_symb[6] = '6';
    this->code_to_symb[7] = '7';
    this->code_to_symb[8] = '8';
    this->code_to_symb[9] = '9';
    this->code_to_symb[10] = 'A';
    this->code_to_symb[11] = 'B';
    this->code_to_symb[12] = 'C';
    this->code_to_symb[13] = 'D';
    this->code_to_symb[14] = 'E';
    this->code_to_symb[15] = 'F';

    this->reset_segment_conversion();

    this->errmesg = QObject::trUtf8("Класс инициализирован");
}

//Метод читает содержимое файла. Возвращает IHP_OK в случае успеха
int ihexparser::read(QString path)
{
    //Сбрасываем различные параметры
    this->errline = 0;
    this->errchar = 0;
    this->errmesg = QObject::trUtf8("Открытие файла на чтение");
    this->raw_lines.clear();
    this->byte_lines.clear();
    this->records.clear();
    this->clear_data_blocks();

    this->file.setFileName(path);
    if (!this->file.open(QIODevice::ReadOnly | QIODevice::Text))
    {
        this->errline = 0;
        this->errchar = 0;
        this->errmesg = QObject::trUtf8(QString("Не удалось открыть файл " + path).toAscii());
        return IHP_ERR;
    }

    //Построчно читаем
    QByteArray tmpline;
    do
    {
        tmpline = this->file.readLine();
        if (tmpline.size() <= 0)
        {
            break;
        }

        //Если последний символ - \n, то отрезаем его
        if (tmpline.at(tmpline.size() - 1) == 0x0A)
        {
            tmpline.remove(tmpline.size() - 1, 1);
        }

        this->raw_lines.push_back(tmpline);
    }
    while (tmpline.size() > 0);

    this->file.close();

    if (this->raw_lines.size() < 2)
    {
        this->errline = 0;
        this->errchar = 0;
        this->errmesg = QObject::trUtf8("Файл должен содержать как минимум 2 строки");
        return IHP_ERR;
    }

    //Проверяем строки
    for (int i = 0; i < this->raw_lines.size(); i++)
    {
        QByteArray *curr_line = &this->raw_lines[i];

        if ((curr_line->size() % 2 != 1) || (curr_line->size() < 3))
        {
            this->errline = i;
            this->errchar = 0;
            this->errmesg = QObject::trUtf8("Строки файла должны содержать нечетное число символов и быть длиннее 2-х символов");
            return IHP_ERR;
        }

        //Подсчитываем двоеточия
        int colon_pos = 0;
        int colons_num = 0;

        for (int j = 0; j < curr_line->size(); j++)
        {
            if (curr_line->at(j) == ':')
            {
                colons_num ++;
                colon_pos = j;
            }
        }

        if ((colons_num != 1) || (colon_pos != 0))
        {
            this->errline = i;
            this->errchar = 0;
            this->errmesg = QObject::trUtf8("Строка файла должна начинаться с символа \":\" и содержать только один такой символ");
            return IHP_ERR;
        }

        //Отрезаем двоеточие
        curr_line->remove(0, 1);

        //Ищем некошерные символы, кошерны 0-9 A-F (0x30-0x39 и 0x41-0x46)
        for (int j = 0; j < curr_line->size(); j++)
        {
            char curr_char = curr_line->at(j);
            if ((curr_char < 0x30) || (curr_char == 0x40) || (curr_char > 0x46))
            {
                this->errline = i;
                this->errchar = j;
                this->errmesg = QObject::trUtf8("Строка содержит недопустимый символ. Допустимые символы: 0-9, A-F");
                return IHP_ERR;
            }
        }

        //Превращаем строку в набор байтов
        QByteArray line_bytes;
        for (int j = 0; j < curr_line->size(); j += 2)
        {
            line_bytes.push_back(this->symb2value(curr_line->at(j), curr_line->at(j + 1)));
        }

        this->byte_lines.push_back(line_bytes);
    }

    this->raw_lines.clear(); //Больше не нужны, пусть память не тратят

    //Начинаем смысловой разбор
    for (int i = 0; i < this->byte_lines.size(); i++)
    {
        QByteArray *curr_line = &this->byte_lines[i];

        if (curr_line->size() < 5)
        {
            this->errline = i;
            this->errchar = 0;
            this->errmesg = QObject::trUtf8("Каждая строка должна содержать как минимум 5 байт");
            return IHP_ERR;
        }

        //Проверяем контрольную сумму
        unsigned char summ = 0; //Именно unsigned char, нам нужно, чтобы переполнялось
        for (int j = 0; j < curr_line->size(); j++)
        {
            summ += curr_line->at(j);
        }

        if (summ != 0)
        {
            this->errline = i;
            this->errchar = 0;
            this->errmesg = QObject::trUtf8("Несовпадение контрольной суммы");
            return IHP_ERR;
        }

        //Вытаскиваем данные
        record_struct tmp_record;
        tmp_record.reclen = (unsigned char)curr_line->at(0);
        tmp_record.offset = ((unsigned char)curr_line->at(1) << 8) + (unsigned char)curr_line->at(2);
        tmp_record.rectype = (unsigned char)curr_line->at(3);

        if (tmp_record.reclen + 5 != curr_line->size())
        {
            this->errline = i;
            this->errchar = 0;
            this->errmesg = QObject::trUtf8("Объём данных в поле RECLEN не совпадает с фактическим");
            return IHP_ERR;
        }

        for (int j = 0; j < tmp_record.reclen; j++)
        {
            tmp_record.data.push_back((unsigned char)curr_line->at(j + 4));
        }

        this->records.push_back(tmp_record);
    }

    //Прибиваем байтовые строки
    this->byte_lines.clear();

    //Анализируем записи, ищем неподдерживаемые
    for (int i = 0; i < this->records.size(); i++)
    {
        record_struct *curr_rec = &this->records[i];

        if ((curr_rec->rectype != IHP_REC_DATA) && (curr_rec->rectype != IHP_REC_EOF) && (curr_rec->rectype != IHP_REC_ESA) && (curr_rec->rectype != IHP_REC_SSA))
        {
            this->errline = i;
            this->errchar = 0;
            this->errmesg = QObject::trUtf8("В строке присутствует неподдерживаемый тип записи");
            return IHP_ERR;
        }
    }

    //Проверяем, заканчивается-ли файл записью EOF
    record_struct *curr_rec = &this->records[this->records.size() - 1];
    if ((curr_rec->rectype != IHP_REC_EOF) || (curr_rec->reclen != 0x00) || (curr_rec->offset != 0x00))
    {
        this->errline = this->records.size() - 1;
        this->errchar = 0;
        this->errmesg = QObject::trUtf8("В конце файла не найдена запись EOF или она некорректна");
        return IHP_ERR;
    }

    //Начинаем вытаскивать данные
    unsigned int base_segment_address = 0x00;
    for (int i = 0; i < this->records.size() - 1; i++)
    {
        record_struct *curr_rec = &this->records[i];

        if (curr_rec->rectype == IHP_REC_EOF)
        {
            this->errline = i;
            this->errchar = 0;
            this->errmesg = QObject::trUtf8("Запись EOF в середине файла");
            return IHP_ERR;
        }
        else if (curr_rec->rectype == IHP_REC_ESA)
        {
            //Идёт базовый адрес сегмента
            if ((curr_rec->reclen != 0x02) || (curr_rec->offset != 0x00))
            {
                this->errline = i;
                this->errchar = 0;
                this->errmesg = QObject::trUtf8("Некорректные параметры записи \"Адрес начала сегмента\"");
                return IHP_ERR;
            }

            base_segment_address = ((unsigned char)curr_rec->data.at(0) << 8) + (unsigned char)curr_rec->data.at(1);
            base_segment_address = base_segment_address << 4; //Выдвигаем на 4 влево (так по стандарту)
        }
        else if (curr_rec->rectype == IHP_REC_SSA)
        {
            //Точка начала исполнения (не имеет смысля для МК, но тупые компиляторы (IAR) ставят её)
            if ((curr_rec->reclen != 0x04) || (curr_rec->offset != 0x00))
            {
                this->errline = i;
                this->errchar = 0;
                this->errmesg = QObject::trUtf8("Некорректные параметры записи \"Точка входа внутри сегмента\"");
                return IHP_ERR;
            }

            //Смело игнорируем содержимое записи
        }
        else
        {
            //Идут данные
            unsigned int real_addr;
            for (int j = 0; j < curr_rec->reclen; j++)
            {
                real_addr = base_segment_address + ((curr_rec->offset + j) & 0xFFFFUL);

                //Добавляем очередной байт
                if (!this->add_byte(real_addr, (unsigned char)curr_rec->data.at(j)))
                {
                    //Байт с таким адресом уже был
                    this->errline = i;
                    this->errchar = 0;
                    this->errmesg = QObject::trUtf8("Байт с адресом, имеющимся в данной записи уже существует");
                    return IHP_ERR;
                }
            }
        }
    }

    //Сортируем блоки по возрастанию адресов
    qSort(this->data_blocks.begin(), this->data_blocks.end(), data_block_struct_less_than);

    return IHP_OK;
}

//Метод принимает два символа и возвращает код, им соответствующий (в Intel HEX каждый байт кодируется
//его текстовым представлением). Считается, что некорректного символа не может быть
unsigned char ihexparser::symb2value(char left, char right)
{
    return (this->symb_to_code[left] << 4) + this->symb_to_code[right];
}

//Метод возвращает номер строки с ошибкой
int ihexparser::get_errline()
{
    return this->errline + 1;
}

//Номер символа в строке
int ihexparser::ihexparser::get_errchar()
{
    return this->errchar + 1;
}

//Текстовое сообщение об ошибке
QString ihexparser::get_errmesg()
{
    return this->errmesg;
}

//Метод возвращает число присутствующих блоков данных (не важно, были-ли они прочитаны или добавлены)
int ihexparser::get_num_of_data_blocks()
{
    return this->data_blocks.size();
}

//Метод возвращает блок с данными по его индексу. В случае некорректного индекса возвращает false
bool ihexparser::get_data_block(int index, data_block_struct &block)
{
    if ((index < 0) || (index >= this->data_blocks.size()))
    {
        return false;
    }

    block = this->data_blocks.at(index);
    return true;
}

//Метод удаляет все существующие блоки данных
void ihexparser::clear_data_blocks()
{
    this->data_blocks.clear();
}

//Метод добавляет очередной байт. Если байт можно добавить к блоку данных, то добавляет байт в блок и расширяет его.
//Если добавить некуда - создаёт новый блок. Возвращает true в случае успеха и false, если такой байт уже существует
bool ihexparser::add_byte(unsigned int addr, unsigned char byte)
{
    if (addr > IHP_MAX_ADDR)
    {
        return false; //Слишком большой адрес
    }

    //Ищем, не попадает-ли он внутрь одного из блоков данных
    for (int h = 0; h < this->data_blocks.size(); h++)
    {
        if ((addr >= this->data_blocks.at(h).start_addr) && (addr <= this->data_blocks.at(h).end_addr))
        {
            //Байт с таким адресом уже был
            return false;
        }
    }

    //Смотрим, можно-ли байт прицепить к какому-либо из блоков
    bool sticked = false;
    for (int h = 0; h < this->data_blocks.size(); h++)
    {
        if (addr == ((long)this->data_blocks.at(h).start_addr - 1))
        {
            //Приклеиваем снизу
            sticked = true;
            this->data_blocks[h].start_addr--;
            this->data_blocks[h].data.push_front(byte);
            break;
        }
        else if (addr == this->data_blocks.at(h).end_addr + 1) //Переполнения сверху никогда не будет
        {
            //Сверху
            sticked = true;
            this->data_blocks[h].end_addr++;
            this->data_blocks[h].data.push_back(byte);
            break;
        }
    }

    if (!sticked)
    {
        //Создаём новый блок
        data_block_struct tmp_block;
        tmp_block.start_addr = addr;
        tmp_block.end_addr = addr;
        tmp_block.data.push_front(byte);
        this->data_blocks.push_back(tmp_block);
    }

    return true;
}

//Метод записывает существующие блоки данных в файл. Возвращает IHP_OK в случае успеха
int ihexparser::write(QString path)
{
    //Сортируем блоки по возрастанию адресов
    qSort(this->data_blocks.begin(), this->data_blocks.end(), data_block_struct_less_than);

    //Генерируем сегменты
    this->reset_segment_conversion();

    for (int i = 0; i < this->data_blocks.size(); i++)
    {
        for (int j = 0; j < this->data_blocks.at(i).data.size(); j++)
        {
            if (!this->add_next_segment_byte(this->data_blocks.at(i).start_addr + j, this->data_blocks.at(i).data.at(j)))
            {
                //Не удалось добавить байт к сегментам
                this->errline = 0;
                this->errchar = 0;
                this->errmesg = QObject::trUtf8("Не удалось добавить байт к сегментам");
                return IHP_ERR;
            }
        }
    }

    //Начинаем формировать записываемые данные
    this->content_to_write.clear();

    //Перебираем сегменты
    for (int i = 0; i < this->segments.size(); i++)
    {
        //Старт сегмента
        this->add_bytes_to_content(this->make_esa_record(this->segments.at(i).segment_addr));

        //Начинаем добавлять записи по 16 байт
        this->in_segment_addr = this->segments.at(i).in_segment_start_addr;

        QByteArray rec_data;
        for (int j = 0; j < this->segments.at(i).data.size(); j++)
        {
            rec_data.push_back(this->segments.at(i).data.at(j));

            if (rec_data.size() == 16)
            {
                //16 байт набралось, делаем запись
                this->add_bytes_to_content(this->make_data_record(this->in_segment_addr, rec_data));
                this->in_segment_addr += 16;
                rec_data.clear();
            }
        }

        if (rec_data.size() > 0)
        {
            this->add_bytes_to_content(this->make_data_record(this->in_segment_addr, rec_data));
        }
    }

    //Добавляем концевую запись
    QByteArray tmp_arr;
    tmp_arr.push_back((char)0x00);
    tmp_arr.push_back((char)0x00);
    tmp_arr.push_back((char)0x00);
    tmp_arr.push_back((char)0x01);
    tmp_arr = this->add_chksum(tmp_arr);
    this->add_bytes_to_content(tmp_arr);

    //Собственно записываем
    this->file.setFileName(path);
    if (!this->file.open(QIODevice::WriteOnly | QIODevice::Text | QIODevice::Truncate))
    {
        this->errline = 0;
        this->errchar = 0;
        this->errmesg = QObject::trUtf8(QString("Не удалось открыть файл " + path).toAscii());
        return IHP_ERR;
    }

    if (this->file.write(this->content_to_write) != this->content_to_write.size())
    {
        this->errline = 0;
        this->errchar = 0;
        this->errmesg = QObject::trUtf8(QString("Ошибка во время записи в файл " + path).toAscii());
        this->file.close();
        return IHP_ERR;
    }

    this->file.close();
    return IHP_OK;
}

//Метод добавляет к переданной записи (в виде QByteArray) контрольную сумму
QByteArray ihexparser::add_chksum(QByteArray record)
{
    unsigned char curr_summ = 0; //Текущая сумма (unsigned char, т.к. нужно, чтобы переполнялось)
    for (int i = 0; i < record.size(); i++)
    {
        curr_summ += record.at(i);
    }

    unsigned char chksumm = 0 - curr_summ;
    record.push_back(chksumm);
    return record;
}

//Метод принимает набор байтов данных, стартовый адрес и формирует запись данных (в бинарном виде, с контрольной суммой)
//Длину входного массива не проверяет
QByteArray ihexparser::make_data_record(unsigned short int start_addr, QByteArray bytes)
{
    QByteArray result;
    result.push_back(bytes.size()); //Объём данных
    result.push_back((start_addr & 0xFF00) >> 8); //Стартовый адрес
    result.push_back(start_addr & 0xFF);
    result.push_back((char)IHP_REC_DATA); //Тип записи
    result.push_back(bytes); //Собственно данные

    result = this->add_chksum(result); //Контрольная сумма
    return result;
}

//Метод находит наиболее подходящий (по запасу внутрисегментного адреса) адрес сегмента по линейному адресу
//В случае успеха возвращает адрес сегмента, в случае ошибки -1
int ihexparser::get_initial_segment_addr(unsigned int linear_addr)
{
    if (linear_addr > IHP_MAX_ADDR)
    {
        return -1;
    }

    int addr_candidate;
    for (int seg_addr = 0xFFFF; seg_addr >= 0x0000; seg_addr--)
    {
        addr_candidate = ((int)linear_addr) - (seg_addr << 4);

        if (addr_candidate >= 0)
        {
            return seg_addr;
        }
    }

    return -1;
}

//Метод пытается определить внутрисегментный адрес для данного линейного адреса и сегментного адреса.
//В случае успеха возвращает этот адрес, в противном случае -1, что указывает на необходимость определения
//сегментного адреса методом get_initial_segment_addr()
int ihexparser::get_addr_in_segment(unsigned int linear_addr, unsigned int segment_addr)
{
    if (linear_addr > IHP_MAX_ADDR)
    {
        return -1;
    }

    int addr_candidate = ((int)linear_addr) - (segment_addr << 4);
    if ((addr_candidate >= 0x0000) && (addr_candidate <= 0xFFFF))
    {
        return addr_candidate;
    }
    else
    {
        return -1;
    }
}

//Метод обрывает последовательность байт для преобразования в сегменты и сбрасывает преобразование в начальное положение
void ihexparser::reset_segment_conversion()
{
    this->segment_addr = -1;
    this->in_segment_addr = -1;
    this->segments.clear();
}

//Метод добавляет очередной байт к содержимому сегментов. В случае успеха - возвращает true. В случае неуспеха
//надо прекратить преобразование и вызвать reset_segment_conversion()
bool ihexparser::add_next_segment_byte(unsigned int linear_addr, unsigned char byte)
{
    if (linear_addr > IHP_MAX_ADDR)
    {
        return false;
    }

    if (this->segments.size() == 0)
    {
        //Ещё нет ни одного сегмента, создаём первый
        this->segment_addr = this->get_initial_segment_addr(linear_addr);

        if (this->segment_addr < 0)
        {
            return false;
        }

        this->in_segment_addr = this->get_addr_in_segment(linear_addr, this->segment_addr);
        if (this->in_segment_addr < 0)
        {
            //В свежесозданном сегменте не удаётся ничего разместить. Интересно
            return false;
        }

        segment_struct tmp_seg_struct;
        tmp_seg_struct.segment_addr = this->segment_addr;
        tmp_seg_struct.in_segment_start_addr = this->in_segment_addr;
        tmp_seg_struct.data.push_back(byte);
        this->segments.push_back(tmp_seg_struct);
        return true;
    }
    else
    {
        //Сегменты уже есть, пробуем добавить к текущему
        int new_addr = this->get_addr_in_segment(linear_addr, this->segment_addr);

        if ((new_addr >= 0) && (new_addr == this->in_segment_addr + 1))
        {
            //У нас есть следующий байт, добавляем его
            this->in_segment_addr = new_addr;
            this->segments[this->segments.size() - 1].data.push_back(byte);
            return true;
        }
        else
        {
            //Находим новый адрес сегмента
            this->segment_addr = this->get_initial_segment_addr(linear_addr);
            if (this->segment_addr < 0)
            {
                return false;
            }

            this->in_segment_addr = this->get_addr_in_segment(linear_addr, this->segment_addr);
            if (this->in_segment_addr < 0)
            {
                return false;
            }

            segment_struct tmp_seg_struct;
            tmp_seg_struct.segment_addr = this->segment_addr;
            tmp_seg_struct.in_segment_start_addr = this->in_segment_addr;
            tmp_seg_struct.data.push_back(byte);
            this->segments.push_back(tmp_seg_struct);
            return true;
        }
    }
}

//Метод принимает стартовый адрес сегмента и формирует сегментную запись. Проверку на корректность адреса не делает.
//Сегментная запись формируется в бинарном виде, с контрольной суммой
QByteArray ihexparser::make_esa_record(unsigned short int seg_addr)
{
    QByteArray result;
    result.push_back((char)0x02); //Длина данных - два байта
    result.push_back((char)0x00); //Адрес - 0x0000
    result.push_back((char)0x00);
    result.push_back((char)IHP_REC_ESA); //Тип записи
    result.push_back((seg_addr & 0xFF00) >> 8); //Собственно адрес
    result.push_back(seg_addr & 0xFF);

    //Добавляем контрольную сумму
    result = this->add_chksum(result);
    return result;
}

//Метод принимает набор байтов, переводит их в ASCII-представление, дописывает спереди двоеточие, сзади - перенос строки
//и добавляет полученную строку к this->content_to_write
void ihexparser::add_bytes_to_content(QByteArray bytes)
{
    //Добавляем двоеточие
    this->content_to_write.push_back(':');

    for (int i = 0; i < bytes.size(); i++)
    {
        this->content_to_write.push_back(this->code_to_symb[(bytes.at(i) & 0xF0) >> 4]);
        this->content_to_write.push_back(this->code_to_symb[bytes.at(i) & 0x0F]);
    }

    this->content_to_write.push_back('\n');
}

//Внеклассовая функция сравнения двух data_block_struct для сортировки. Условие сортировки: старший адрес меньшего меньше
//младшего адреса большего
bool data_block_struct_less_than(const data_block_struct &bl1, const data_block_struct &bl2)
{
    if (bl1.end_addr < bl2.start_addr)
    {
        return true;
    }
    else
    {
        return false;
    }
}
