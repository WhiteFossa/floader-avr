/*********************************************************************************/
/*                  ���� ���� �������� ������ ���������� �����                   */
/* ����������:      ���� ��������                                                */
/*                         �������� � AVR ASM 2                                  */   
/* ������ 0.0.3  �� 05.09.2012                                                   */
/* Copyright 2012 ����� aka ���� ������                                         */
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
/*    (��� ��������� ���������: �� ������ ������������������ �� �/��� ��������   */
/*    �� �� �������� ����������� ������������ �������� GNU � ��� ����, � �����   */
/*    ��� ���� ������������ ������ ���������� ������������ �����������; ����     */
/*    ������ 3 ��������, ���� (�� ������ ������) ����� ����� ������� ������.     */
/*                                                                               */
/*    ��� ��������� ���������������� � �������, ��� ��� ����� ��������,          */
/*    �� ���� ������ ��������; ���� ��� ������� �������� ��������� ����          */
/*    ��� ����������� ��� ������������ �����. ��������� ��. � �����������        */
/*    ������������ �������� GNU.                                                 */
/*                                                                               */
/*    �� ������ ���� �������� ����� ����������� ������������ �������� GNU        */
/*    ������ � ���� ����������. ���� ��� �� ���, ��.                             */
/*    <http://www.gnu.org/licenses/>.)                                           */
/*********************************************************************************/

//�������
.include "m16def.inc"


//�������
.equ		START_ADDR		=		0x1E00					//��������� ����� ����������

.equ		LISTEN_PORT		=		PORTB					//�� ����� ����� �������
.equ		LISTEN_DDR		=		DDRB					//���� �����������
.equ		LISTEN_PIN		=		PINB					//���� �����
.equ		LISTEN_BIT		=		0						//����� ��� �������

.equ		LED_PORT		=		PORTB					//�� ����� ����� �������� ���������
.equ		LED_DDR			=		DDRB					//���� �����������
.equ		LED_BIT			=		4						//����� ��� ���������� � 1

.equ		OSC_FREQ		=		16000000				//������� ������
.equ		BAUDRATE		=		57600					//�������� UART
.equ		BAUDDIVIDER		=		OSC_FREQ / (16 * BAUDRATE) - 1	//�������� ��� UART

.equ		ERR_OK			=		0x00					//��� ������
.equ		ERR_FAIL		=		0x01					//������

//������� ���������
.def		PGA				=		R20						//����� ��������

.def		EPL				=		R24						//������� � ������� ����� �������� ������ EEPROM
.def		EPH				=		R25

//����������-������������� �������
.equ		VER				=		0x01					//������ ����������

.equ		MFID2			=		0x00					//������� ���� �������������� ������������
.equ		MFID1			=		0x00					//�������
.equ		MFID0			=		0x00					//�������

.equ		DEVID2			=		0x00					//��� ����� �������������� ����������
.equ		DEVID1			=		0x00
.equ		DEVID0			=		0x01

.equ		SN3				=		0x00					//������ ����� ��������� ������ ����������
.equ		SN2				=		0x00
.equ		SN1				=		0x00
.equ		SN0				=		0x01



//��-������������� ������� (ATmega16A)
.equ		NPT				=		0x80					//����� ����� ������� FLASH

.equ		NPW				=		0x78					//����� �������, ��������� �� ������

.equ		PS				=		0x80					//������ �������� FLASH (� ������)

.equ		PSS				=		0x07					//������� ��� ���� �������� ����� ����� ��������,
//����� ������� � ������

.equ		PSW				=		0x40					//������ �������� � ������


//������� ������
.DSEG


//������� ����
.CSEG
.org		0x0000											//����� ����� � �������� ���������
RESET:
jmp			RESET


//����� ����� � ���������
.org		START_ADDR

//���������� ��������� ���������� ������
cli															//��������� ����������
wdr															//���������� �� ������ ������

//������� WDTOE � WDE � ����������� ������������
in			R16,			WDTCR
ori			R16,			(1<<WDTOE) | (1<<WDE)
out			WDTCR,			R16

//�������� � ���� WDE
andi		R16,			~(1<<WDE)
out			WDTCR,			R16

//����������� ����
ldi			R16,			high(ramend)
out			SPH,			R16
ldi			R16,			low(ramend)
out			SPL,			R16


//��������� ������� �����
cbi			LISTEN_DDR,		LISTEN_BIT						//��������� ���� - �� ����
sbi			LISTEN_PORT,	LISTEN_BIT						//�������� ������������� ��������
sbic		LISTEN_PIN,		LISTEN_BIT
jmp			RESET											//�� ���� ������� �������, ������ �� �������� ���������


//���� � ���������

//�������� ���������
sbi			LED_DDR,		LED_BIT
sbi			LED_PORT,		LED_BIT

//����������� UART
ldi			R16,			low(BAUDDIVIDER)
out			UBRRL,			R16
ldi			R16,			high(BAUDDIVIDER)
out			UBRRH,			R16
clr			R16
out			UCSRA,			R16
ldi 		R16, 			(1<<RXEN)|(1<<TXEN) //�������� ���� � ��������
out			UCSRB,			R16
ldi 		R16, 			(1<<URSEL)|(1<<UCSZ0)|(1<<UCSZ1) //���� �������� ���, 8 ��� ������, ��� �������� ��������
out			UCSRC,			R16

//������� ����������� ������
COMMAND_WAIT:
call		UART_READ_BYTE									//��������� ����
cpi			R16,			'I'								//�������������
breq		IDENTIFY

cpi			R16,			'W'								//������ �������� FLASH
breq		WRITE_FLASH

cpi			R16,			'R'								//������ �������� FLASH
breq		READ_FLASH

cpi			R16,			'w'								//������ �������� EEPROM
breq		WRITE_EEPROM

cpi			R16,			'r'								//������ �������� EEPROM
breq		READ_EEPROM

cpi			R16,			'Q'								//���������� ��������
breq		QUIT

COMMAND_PROCESSED:											//��������� ��������� ���������
jmp			COMMAND_WAIT


//����������� ������

//�������������
IDENTIFY:
jmp			IDENTIFY_P


//������ �������� ����
WRITE_FLASH:
jmp			WRITE_FLASH_P


//������ �������� ����
READ_FLASH:
jmp			READ_FLASH_P


//������ EEPROM
WRITE_EEPROM:
jmp			WRITE_EEPROM_P

//������ EEPROM
READ_EEPROM:
jmp			READ_EEPROM_P

//���������� ��������
QUIT:
ldi			R16,			'B' 								//�������� 'B'
call		UART_SEND_BYTE

//��� ���������� �������� (TXC, �� UDRE)
TX_NOT_COMPLETED:
sbis		UCSRA,			TXC
rjmp		TX_NOT_COMPLETED

//����� ��������� � ���������� ����� � �������� ���������
cbi			LED_PORT,		LED_BIT
cbi			LED_DDR,		LED_BIT
cbi			LISTEN_PORT,	LISTEN_BIT							//�������� ������������� ��������


jmp			RESET												//������ � �������� ���������		


//��������� �������������
IDENTIFY_P:
//���������� ��������� ����������
ldi			R16,			'F'
call		UART_SEND_BYTE
ldi			R16,			'B'
call		UART_SEND_BYTE
ldi			R16,			'L'
call		UART_SEND_BYTE

//������ ����������
ldi			R16,			VER
call		UART_SEND_BYTE

//������������� ������������
ldi			R16,			MFID2
call		UART_SEND_BYTE
ldi			R16,			MFID1
call		UART_SEND_BYTE
ldi			R16,			MFID0
call		UART_SEND_BYTE

//������������� ����������
ldi			R16,			DEVID2
call		UART_SEND_BYTE
ldi			R16,			DEVID1
call		UART_SEND_BYTE
ldi			R16,			DEVID0
call		UART_SEND_BYTE

//�������� �����
ldi			R16,			SN3
call		UART_SEND_BYTE
ldi			R16,			SN2
call		UART_SEND_BYTE
ldi			R16,			SN1
call		UART_SEND_BYTE
ldi			R16,			SN0
call		UART_SEND_BYTE

jmp			COMMAND_PROCESSED


//������ �������� FLASH
READ_FLASH_P:
//������ ����� ��������
call		UART_READ_BYTE
mov			PGA,			R16

//��������� ����� ��������
ldi			R22,			NPT
call		CHECK_PG_ADDR

call		UART_SEND_BYTE //�������� ��������� �������� ������

cpi			R16,			ERR_FAIL
breq		READ_FLASH_P_EXIT //����� �����������, ������� �� �������


//�������� � ZH:ZL ����� ������
clr			R16
clr			R17
clr			ZH
mov			ZL,				PGA

READ_FLASH_P_NEXT_SHIFT:
//�������� ����� ����� �� 1
clc //���������� ���� ��������
lsl			ZH //�������� ������� ����
lsl			ZL //�������� ������� ����
adc			ZH,				R16 //���������� ������� � ��������

inc			R17
cpi			R17,			PSS
brlo		READ_FLASH_P_NEXT_SHIFT

//������ ������ �������� � ���������� �� �� UART
clr			R17

//������ ���������� ����� ����
READ_FLASH_P_NEXT_BYTE:
lpm			R16,			Z+
call		UART_SEND_BYTE

inc			R17
cpi			R17,			PS
brlo		READ_FLASH_P_NEXT_BYTE

READ_FLASH_P_EXIT: //����� �� ������ ��������
jmp			COMMAND_PROCESSED


//������ �������� ����
WRITE_FLASH_P:
//������ ����� ��������
call		UART_READ_BYTE
mov			PGA,			R16

//��������� ����� ��������
ldi			R22,			NPW //� ��������� ������ ������
call		CHECK_PG_ADDR

//������� ��������� �������� ������
call		UART_SEND_BYTE

cpi			R16,			ERR_FAIL
breq		WRITE_FLASH_P_EXIT //����� �����������, ������� �� �������

//����� ����� �������� � Z
call		SET_PG_ADDR

//��������� ���� ������� � ����� �� � R1:R0, ������� �������� � ������
clr			R17

WRITE_FLASH_P_READ_WORD: //��������� ��������� �����
call		UART_READ_BYTE //������� ����
mov			R0,				R16
call		UART_READ_BYTE //������� ����
mov			R1,				R16

//����� ����� � �����
ldi			R16,			(1<<SPMEN)
call		MAKE_SPM

//����������� ����� ����� � Z
adiw		ZH:ZL,			0x02 //���������� 2, ����� ������� ��� ������ ��������� ����� 0

inc			R17
cpi			R17,			PSW
brlo		WRITE_FLASH_P_READ_WORD

call		SET_PG_ADDR //������������� �����

//������� ��������
ldi			R16,			(1<<PGERS)|(1<<SPMEN)
call		MAKE_SPM

//����� ��������
ldi			R16,			(1<<PGWRT)|(1<<SPMEN)
call		MAKE_SPM

//��������������� ������ � RWW
ldi			R16,			(1<<RWWSRE)|(1<<SPMEN)
call		MAKE_SPM

//�� �������
ldi			R16,			ERR_OK

WRITE_FLASH_P_EXIT: //����� �� �������� ��������
call		UART_SEND_BYTE //�������� ���������
jmp			COMMAND_PROCESSED


//������ EEPROM
READ_EEPROM_P:

//��� ���������� ������ (���� ����)
sbic		EECR,			EEWE
rjmp		READ_EEPROM_P

//���������� �����
clr			XL
clr			XH

//��������� � ������� ����� ����� ���� EEPROM
ldi			EPH,			high(EEPROMEND)
ldi			EPL,			low(EEPROMEND)


READ_EEPROM_P_NEXT_BYTE: //������ ��������� ����

out			EEARH,			XH
out			EEARL,			XL
sbi			EECR,			EERE //���������� ������
in			R16,			EEDR
call		UART_SEND_BYTE //�������� ���������� ����

//����������� ����� � ����������� �������
adiw		XH:XL,			0x01
sbiw		EPH:EPL,		0x01
brsh		READ_EEPROM_P_NEXT_BYTE

jmp			COMMAND_PROCESSED


//������ EEPROM
WRITE_EEPROM_P:
//���������� �����
clr			XL
clr			XH

//��������� � ������� ����� ����� ���� EEPROM
ldi			EPH,			high(EEPROMEND)
ldi			EPL,			low(EEPROMEND)


WRITE_EEPROM_P_NEXT_BYTE: //����� ��������� ����
//��������� ����
call		UART_READ_BYTE

//��� ���������� ���������� ������
WRITE_EEPROM_P_WAIT:
sbic		EECR,			EEWE
rjmp		WRITE_EEPROM_P_WAIT

//�������� ������
out			EEARH,			XH
out			EEARL,			XL
out			EEDR,			R16
sbi			EECR,			EEMWE
sbi			EECR,			EEWE

//����������� ����� � ����������� �������
adiw		XH:XL,			0x01
sbiw		EPH:EPL,		0x01
brsh		WRITE_EEPROM_P_WANT_NEXT

//��� ����� ��������
ldi			R16,			'f'
call		UART_SEND_BYTE
jmp			COMMAND_PROCESSED

//����� ��� ����
WRITE_EEPROM_P_WANT_NEXT:
ldi			R16,			'n'
call		UART_SEND_BYTE
rjmp		WRITE_EEPROM_P_NEXT_BYTE


//���������

//��������� ���������� ���� �� UART. ���� ������ ������ � R16
UART_SEND_BYTE:
	push		R17
	in			R17,			SREG
	push		R17

	UART_SEND_BYTE_WAIT_READY:
	sbis		UCSRA,			UDRE
	rjmp		UART_SEND_BYTE_WAIT_READY

	out			UDR,			R16

	pop			R17
	out			SREG,			R17
	pop			R17
ret

//��������� ��������� ���� �� UART. ����� ������ ���� ���� �� ����� ������ ����. ���������� ����
//������� � R16
UART_READ_BYTE:
	push		R17
	in			R17,			SREG
	push		R17

	UART_READ_BYTE_WAIT:
	sbis		UCSRA,			RXC
	rjmp		UART_READ_BYTE_WAIT
	in			R16,			UDR

////������� ���� � 0x00 � ��������� ��������
//ldi		R17,	'/'
//cpse	R16,	R17
//rjmp	UART_READ_BYTE_EXIT
//clr		R16

UART_READ_BYTE_EXIT:
	pop			R17
	out			SREG,			R17
	pop			R17
ret


//��������� ��������� ����� �������� �� ������������. ����� �������� ������ ������ � PGA,
//����� ������� - � R22. ����� �������� ������ ���� ������ ����� �������. ���� ��� ��� - ����������
//� R16 ERR_OK, ����� ERR_FAIL
CHECK_PG_ADDR:
	push		R17
	in			R17,			SREG
	push		R17

	//���������� ������� ����� ������
	cp			PGA,			R22
	brlo		CHECK_PG_ADDR_OK //PGAH < R22, ����� ���������

	//����� �����������
	ldi			R16,			ERR_FAIL
	rjmp		CHECK_PG_ADDR_EXIT

CHECK_PG_ADDR_OK: //����� ���������
	ldi			R16,			ERR_OK
	rjmp		CHECK_PG_ADDR_EXIT

CHECK_PG_ADDR_EXIT: //����� �� ���������
	pop			R17
	out			SREG,			R17
	pop			R17
ret

//��������� ��������� ������� SPM. ��������, ������������ � SPMCR, ������ ������ � R16.
//������ ��������� ��� ���������� ���������� ������� SPM, �� ���������� ���������� ���� �������������.
MAKE_SPM:
	push		R17
	in			R17,			SREG
	push		R17

MAKE_SPM_WAIT1: //��� ���������� ���������� ������� SPM
	in			R17,			SPMCR
	sbrc		R17,			SPMEN
	rjmp		MAKE_SPM_WAIT1

	//���������� ��������� �������
	out			SPMCR,			R16
	spm

	pop			R17
	out			SREG,			R17
	pop			R17
ret

//��������� ����������� ����� �������� (������� � PGA) � ����� ��������. ����� ������� �
//Z13:Z7, ��� ��������� ���� �������� Z ����������
SET_PG_ADDR:
	push		R16
	in			R16,			SREG
	push		R16

	mov			ZL,				PGA
	andi		ZL,				0b00000001 //�������� ������� ��� ������
	bst			ZL,				0x00 //������������� ��� � ������� ���
	clr			ZL
	bld			ZL,				0x07

	mov			ZH,				PGA
	andi		ZH,				0b01111110 //�������� ������� ���� ������
	lsr			ZH

	pop			R16
	out			SREG,			R16
	pop			R16
ret
