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

#ifndef PORTDIALOG_H
#define PORTDIALOG_H

//Инклюды
#include <QDialog>
#include "qextserialport/qextserialenumerator.h"
#include "qextserialport/qextserialport.h"
#include "qextserialport/qextserialport_global.h"
#include <QtDebug>
#include <QSettings>

//Дефайны
#define PD_PORT_NOT_SELECTED -1 //Порт не выбран

namespace Ui {
    class PortDialog;
}

class PortDialog : public QDialog
{
    Q_OBJECT

public:
    //Публичные свойства
    QextPortInfo selected_port; //Выбранный пользователем порт
    PortSettings port_settings; //Настройки выбранного порта

    //Публичные методы

    //Конструктор
    explicit PortDialog(QWidget *parent = 0);

    //Деструктор
    ~PortDialog();

private:
    //Приватные свойства
    Ui::PortDialog *ui;
    QList<QextPortInfo> ports_list; //Список портов, присутсвующих в системе
    int selected_port_index; //Индекс выбранного порта в ports_list
    QSettings settings; //Класс для хранения настроек

    //Приватные методы

    //Метод перезагружает список портов и устанавливает текущий порт в PD_PORT_NOT_SELECTED
    void reload_ports_list();

    //Метод возвращает пункт списка со скоростями по скорости. Если скорость некорректна, то возвращает -1
    int baudrate_to_index(BaudRateType baudrate);

    //Метод, обратный baudrate_to_index. Возвращает скорость на основе индекса. Если индекс некорректен, выкидывает
    //исключение
    BaudRateType index_to_baudrate(int index);

    //То-же, что baudrate_to_index(), но для числа бит в посылке
    int bits_to_index(DataBitsType bits);

    //То-же, что index_to_baudrate(), но для числа бит в посылке
    DataBitsType index_to_bits(int index);

    //То-же, что baudrate_to_index(), но для режима управления потоком
    int flow_to_index(FlowType flow);

    //То-же, что index_to_baudrate(), но для числа бит в посылке
    FlowType index_to_flow(int index);

    //То-же, что baudrate_to_index(), но для чётности
    int parity_to_index(ParityType parity);

    //То-же, что index_to_baudrate(), но для чётности
    ParityType index_to_parity(int index);

    //То-же, что baudrate_to_index(), но для стоповых битов
    int stop_to_index(StopBitsType stop_bits);

    //То-же, что index_to_baudrate(), но для стоповых битов
    StopBitsType index_to_stop(int index);

private slots:
    //Приватные слоты
    //Слот вызывается при смене выделенной ячейке в таблице портов
    //cr - текущая строка, cc - текущий столбец, pr, pc - предыдущие
    void port_cell_changed(int cr, int cc, int pr, int pc);

    //Слот вызывается, когда происходит нажатие на кнопку "принять"
    void settings_accepted();
};

//Внеклассовые функции

//Функция для сортировки по имени порта
bool sort_by_port_name_func(const QextPortInfo &v1, const QextPortInfo &v2);

#endif // PORTDIALOG_H
