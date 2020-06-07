/*********************************************************************************/
/*                  Этот файл является частью загрузчика Фоссы                   */
/* Назначение:      Заголовочный файл диалога выбора порта                       */
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

#include "portdialog.h"
#include "ui_portdialog.h"

PortDialog::PortDialog(QWidget *parent) : QDialog(parent), ui(new Ui::PortDialog)
{
    //Настраиваем пользовательский интерфейс
    this->ui->setupUi(this);

    //Связываем сигналы и слоты
    QObject::connect(this->ui->btnApply, SIGNAL(clicked()), this, SLOT(settings_accepted())); //Нажатие на кнопку принятия изменений
    QObject::connect(this->ui->btnCancel, SIGNAL(clicked()), this, SLOT(reject())); //Нажатие на кнопку отмены
    QObject::connect(this->ui->tblPorts, SIGNAL(currentCellChanged(int, int, int, int)), this, SLOT(port_cell_changed(int, int, int, int))); //Выделение новой ячейки

    //Инициализируем всякие вещи
    //Таблица со списком портов
    this->ui->tblPorts->setColumnCount(2);
    QStringList tbl_ports_title;
    tbl_ports_title << QObject::trUtf8("Порт");
    tbl_ports_title << QObject::trUtf8("Описание");
    this->ui->tblPorts->setHorizontalHeaderLabels(tbl_ports_title);
    this->ui->tblPorts->setColumnWidth(0, 50);
    this->ui->tblPorts->setColumnWidth(1, 221);

    //Допустимые скорости (только те, что поддерживаются везде)
    this->ui->cbBaudrate->addItem(QObject::trUtf8("110 бит/сек"));
    this->ui->cbBaudrate->addItem(QObject::trUtf8("300 бит/сек"));
    this->ui->cbBaudrate->addItem(QObject::trUtf8("600 бит/сек"));
    this->ui->cbBaudrate->addItem(QObject::trUtf8("1200 бит/сек"));
    this->ui->cbBaudrate->addItem(QObject::trUtf8("2400 бит/сек"));
    this->ui->cbBaudrate->addItem(QObject::trUtf8("4800 бит/сек"));
    this->ui->cbBaudrate->addItem(QObject::trUtf8("9600 бит/сек"));
    this->ui->cbBaudrate->addItem(QObject::trUtf8("19200 бит/сек"));
    this->ui->cbBaudrate->addItem(QObject::trUtf8("38400 бит/сек"));
    this->ui->cbBaudrate->addItem(QObject::trUtf8("57600 бит/сек"));
    this->ui->cbBaudrate->addItem(QObject::trUtf8("115200 бит/сек"));

    //Число бит в посылке
    this->ui->cbBits->addItem(QObject::trUtf8("5"));
    this->ui->cbBits->addItem(QObject::trUtf8("6"));
    this->ui->cbBits->addItem(QObject::trUtf8("7"));
    this->ui->cbBits->addItem(QObject::trUtf8("8"));

    //Управление потоком
    this->ui->cbFlow->addItem(QObject::trUtf8("Нет"));
    this->ui->cbFlow->addItem(QObject::trUtf8("Аппаратное"));
    this->ui->cbFlow->addItem(QObject::trUtf8("Xon/Xoff"));

    //Четность
    this->ui->cbParity->addItem(QObject::trUtf8("Нет"));
    this->ui->cbParity->addItem(QObject::trUtf8("Нечет"));
    this->ui->cbParity->addItem(QObject::trUtf8("Чёт"));

    //Четность
    this->ui->cbStop->addItem(QObject::trUtf8("1"));
    this->ui->cbStop->addItem(QObject::trUtf8("2"));

    //Загружаем настройки
    this->settings.beginGroup("portsettings");

    int tmp;
    tmp = this->settings.value("speed", this->baudrate_to_index(BAUD57600)).toInt(); //Скорость
    this->port_settings.BaudRate = this->index_to_baudrate(tmp);
    this->ui->cbBaudrate->setCurrentIndex(tmp);

    tmp = this->settings.value("data_bits", this->bits_to_index(DATA_8)).toInt(); //Число бит в посылке
    this->port_settings.DataBits = this->index_to_bits(tmp);
    this->ui->cbBits->setCurrentIndex(tmp);

    tmp = this->settings.value("flow_control", this->flow_to_index(FLOW_OFF)).toInt(); //Управление потоком
    this->port_settings.FlowControl= this->index_to_flow(tmp);
    this->ui->cbFlow->setCurrentIndex(tmp);

    tmp = this->settings.value("parity", this->parity_to_index(PAR_NONE)).toInt(); //Проверка чётности
    this->port_settings.Parity= this->index_to_parity(tmp);
    this->ui->cbParity->setCurrentIndex(tmp);

    tmp = this->settings.value("stop_bits", this->stop_to_index(STOP_1)).toInt(); //Число стоповых битов
    this->port_settings.StopBits= this->index_to_stop(tmp);
    this->ui->cbStop->setCurrentIndex(tmp);

    //А таймаут минимальный всегда
    this->port_settings.Timeout_Millisec = 1;

    //Получаем список портов
    this->reload_ports_list();

    //Если сохранённый порт существует, то выбираем его
    QextPortInfo tmp_port_info;
    tmp_port_info.portName = this->settings.value("port_name", "").toString();
    tmp_port_info.friendName = this->settings.value("friend_name", "").toString();
    tmp_port_info.physName = this->settings.value("phys_name", "").toString();
    tmp_port_info.enumName = this->settings.value("enum_name", "").toString();

    for (int i = 0; i < this->ports_list.size(); i++)
    {
        if ((tmp_port_info.portName == this->ports_list[i].portName) && (tmp_port_info.friendName == this->ports_list[i].friendName) && (tmp_port_info.enumName == this->ports_list[i].enumName))
        {
            //Вот он наш порт
            this->ui->tblPorts->selectRow(i);
            this->selected_port_index = i;
            this->selected_port.portName = this->ports_list[this->selected_port_index].portName;
            this->selected_port.physName = this->ports_list[this->selected_port_index].physName;
            this->selected_port.friendName = this->ports_list[this->selected_port_index].friendName;
            this->selected_port.enumName = this->ports_list[this->selected_port_index].enumName;
            this->ui->btnApply->setEnabled(true);
        }
    }

    this->settings.endGroup();
}

//Метод перезагружает список портов
void PortDialog::reload_ports_list()
{
    this->selected_port_index = PD_PORT_NOT_SELECTED; //Порт не выбран
    this->ui->btnApply->setEnabled(false); //Задавливаем кнопку ОК

    this->ui->tblPorts->clearContents();
    this->ui->tblPorts->clearSelection();
    this->ports_list.clear();
    this->ports_list = QextSerialEnumerator::getPorts();

    //Подчищаем имена
    for (int i = 0; i < this->ports_list.size(); i++)
    {
        this->ports_list[i].portName = QString(this->ports_list.at(i).portName.unicode());
        this->ports_list[i].physName = QString(this->ports_list.at(i).physName.unicode());
        this->ports_list[i].friendName = QString(this->ports_list.at(i).friendName.unicode());
        this->ports_list[i].enumName = QString(this->ports_list.at(i).enumName.unicode());
    }

    //Сортируем по номеру порта
    qSort(this->ports_list.begin(), this->ports_list.end(), sort_by_port_name_func);

    //Выводим список портов
    this->ui->tblPorts->setRowCount(this->ports_list.size());

    for (int i = 0; i < this->ports_list.size(); i++)
    {
        QTableWidgetItem *item_port = new QTableWidgetItem(this->ports_list.at(i).portName);
        QTableWidgetItem *item_descr = new QTableWidgetItem(this->ports_list.at(i).friendName);
        this->ui->tblPorts->setItem(i, 0, item_port);
        this->ui->tblPorts->setItem(i, 1, item_descr);
    }
}

PortDialog::~PortDialog()
{
    delete this->ui; //Удаляем пользовательский интерфейс
}


//Слот вызывается при смене выделенной ячейке в таблице портов
//cr - текущая строка, cc - текущий столбец, pr, pc - предыдущие
void PortDialog::port_cell_changed(int cr, int cc, int pr, int pc)
{
    if (pr == -1) //Таблица только проинитилась
    {
        return;
    }
    this->selected_port_index = cr;
    this->ui->btnApply->setEnabled(true);
}

//Слот вызывается, когда происходит нажатие на кнопку "принять"
void PortDialog::settings_accepted()
{
    //Сохраняем параметры
    this->settings.beginGroup("portsettings");

    //Порт
    this->selected_port.portName = this->ports_list[this->selected_port_index].portName;
    this->selected_port.physName = this->ports_list[this->selected_port_index].physName;
    this->selected_port.friendName = this->ports_list[this->selected_port_index].friendName;
    this->selected_port.enumName = this->ports_list[this->selected_port_index].enumName;
    this->settings.setValue("port_name", this->selected_port.portName);
    this->settings.setValue("phys_name", this->selected_port.physName);
    this->settings.setValue("friend_name", this->selected_port.friendName);
    this->settings.setValue("enum_name", this->selected_port.enumName);

    //Скорость
    this->port_settings.BaudRate = this->index_to_baudrate(this->ui->cbBaudrate->currentIndex());
    this->settings.setValue("speed", this->ui->cbBaudrate->currentIndex());

    //Число бит в посылке
    this->port_settings.DataBits = this->index_to_bits(this->ui->cbBits->currentIndex());
    this->settings.setValue("data_bits", this->ui->cbBits->currentIndex());

    //Управление потоком
    this->port_settings.FlowControl = this->index_to_flow(this->ui->cbFlow->currentIndex());
    this->settings.setValue("flow_control", this->ui->cbFlow->currentIndex());

    //Чётность
    this->port_settings.Parity = this->index_to_parity(this->ui->cbParity->currentIndex());
    this->settings.setValue("parity", this->ui->cbParity->currentIndex());

    //Число стоповых бит
    this->port_settings.StopBits = this->index_to_stop(this->ui->cbStop->currentIndex());
    this->settings.setValue("stop_bits", this->ui->cbStop->currentIndex());

    this->settings.endGroup();

    this->accept();
}

//Метод возвращает пункт списка со скоростями по скорости. Если скорость некорректна, то возвращает -1
int PortDialog::baudrate_to_index(BaudRateType baudrate)
{
    int index;
    switch(baudrate)
    {
        case BAUD110:
            index = 0;
        break;
        case BAUD300:
            index = 1;
        break;
        case BAUD600:
            index = 2;
        break;
        case BAUD1200:
            index = 3;
        break;
        case BAUD2400:
            index = 4;
        break;
        case BAUD4800:
            index = 5;
        break;
        case BAUD9600:
            index = 6;
        break;
        case BAUD19200:
            index = 7;
        break;
        case BAUD38400:
            index = 8;
        break;
        case BAUD57600:
            index = 9;
        break;
        case BAUD115200:
            index = 10;
        break;
        default:
            index = -1;
        break;
    }

    return index;
}

//Метод, обратный baudrate_to_index. Возвращает скорость на основе индекса. Если индекс некорректен, то возвращает
//самую маленькую скорость
BaudRateType PortDialog::index_to_baudrate(int index)
{
    switch(index)
    {
        case 0:
            return BAUD110;
        break;
        case 1:
            return BAUD300;
        break;
        case 2:
            return BAUD600;
        break;
        case 3:
            return BAUD1200;
        break;
        case 4:
            return BAUD2400;
        break;
        case 5:
            return BAUD4800;
        break;
        case 6:
            return BAUD9600;
        break;
        case 7:
            return BAUD19200;
        break;
        case 8:
            return BAUD38400;
        break;
        case 9:
            return BAUD57600;
        break;
        case 10:
            return BAUD115200;
        break;
        default:
            throw; //Возбуждаемся
        break;
    }
}

//То-же, что baudrate_to_index(), но для числа бит в посылке
int PortDialog::bits_to_index(DataBitsType bits)
{
    int index;
    switch(bits)
    {
        case DATA_5:
            index = 0;
        break;
        case DATA_6:
            index = 1;
        break;
        case DATA_7:
            index = 2;
        break;
        case DATA_8:
            index = 3;
        break;
        default:
            index = -1;
        break;
    }

    return index;
}

//То-же, что index_to_baudrate(), но для числа бит в посылке
DataBitsType PortDialog::index_to_bits(int index)
{
    switch(index)
    {
        case 0:
            return DATA_5;
        break;
        case 1:
            return DATA_6;
        break;
        case 2:
            return DATA_7;
        break;
        case 3:
            return DATA_8;
        break;
        default:
            throw; //Возбуждаемся
        break;
    }
}

//То-же, что baudrate_to_index(), но для режима управления потоком
int PortDialog::flow_to_index(FlowType flow)
{
    int index;
    switch(flow)
    {
        case FLOW_OFF:
            index = 0;
        break;
        case FLOW_HARDWARE:
            index = 1;
        break;
        case FLOW_XONXOFF:
            index = 2;
        break;
        default:
            index = -1;
        break;
    }

    return index;
}

//То-же, что index_to_baudrate(), но для числа бит в посылке
FlowType PortDialog::index_to_flow(int index)
{
    switch(index)
    {
        case 0:
            return FLOW_OFF;
        break;
        case 1:
            return FLOW_HARDWARE;
        break;
        case 2:
            return FLOW_XONXOFF;
        break;
        default:
            throw; //Возбуждаемся
        break;
    }
}

//То-же, что baudrate_to_index(), но для чётности
int PortDialog::parity_to_index(ParityType parity)
{
    int index;
    switch(parity)
    {
        case PAR_NONE:
            index = 0;
        break;
        case PAR_ODD:
            index = 1;
        break;
        case PAR_EVEN:
            index = 2;
        break;
        default:
            index = -1;
        break;
    }

    return index;
}

//То-же, что index_to_baudrate(), но для чётности
ParityType PortDialog::index_to_parity(int index)
{
    switch(index)
    {
        case 0:
            return PAR_NONE;
        break;
        case 1:
            return PAR_ODD;
        break;
        case 2:
            return PAR_EVEN;
        break;
        default:
            throw; //Возбуждаемся
        break;
    }
}

//То-же, что baudrate_to_index(), но для стоповых битов
int PortDialog::stop_to_index(StopBitsType stop_bits)
{
    int index;
    switch(stop_bits)
    {
        case STOP_1:
            index = 0;
        break;
        case STOP_2:
            index = 1;
        break;
        default:
            index = -1;
        break;
    }

    return index;
}


//То-же, что index_to_baudrate(), но для стоповых битов
StopBitsType PortDialog::index_to_stop(int index)
{
    switch(index)
    {
        case 0:
            return STOP_1;
        break;
        case 1:
            return STOP_2;
        break;
        default:
            throw; //Возбуждаемся
        break;
    }
}

//Внеклассовые функции

//Функция для сортировки по имени порта
bool sort_by_port_name_func(const QextPortInfo &v1, const QextPortInfo &v2)
{
    return v1.portName.toLower() < v2.portName.toLower();
}
