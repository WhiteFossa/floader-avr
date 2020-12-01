;
;	                    Fossa's AVR bootloader
; Copyright (C) 2020 White Fossa aka Artyom Vetrov <whitefossa@protonmail.com>
;
; This program is free software: you can redistribute it and/or modify
; it under the terms of the GNU Affero General Public License as published by
; the Free Software Foundation, either version 3 of the License, or
; (at your option) any later version.
;
; This program is distributed in the hope that it will be useful,
; but WITHOUT ANY WARRANTY; without even the implied warranty of
; MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
; GNU Affero General Public License for more details.
;
; You should have received a copy of the GNU Affero General Public License
; along with this program.  If not, see <https://www.gnu.org/licenses/>.
;

; Hardware-specific code for ATmega16

; Disable or slow down WDT procedure
DisableOrSlowDownWDT:
							push		R16
							uin			R16,		SREG
							push		R16

							uin			R16,		WDTCR
							ori			R16,		(1 << WDTOE) | (1 << WDE)
							uout		WDTCR,		R16 ; 4 clocks starts here

							andi		R16,		~(1 << WDE)
							uout		WDTCR,		R16 ; WDE disabled

							pop			R16
							uout		SREG,		R16
							pop			R16
							ret


; Call this to initialize hardware (before bootloader mode enter)
InitializeHardwarePreEnter:
							push		R16
							uin			R16,		SREG
							push		R16

							; All ports to input to avoid hardware damage
							clr			R16
							uout		DDRA,		R16
							uout		DDRB,		R16
							uout		DDRC,		R16
							uout		DDRD,		R16

							; Listening pin pull-up enable
							sbi			ListenPort,	ListenBit

							pop			R16
							uout		SREG,		R16
							pop			R16
							ret


; Call this to initialize hardware (after bootloader mode enter)
InitializeHardwarePostEnter:
							push		R16
							uin			R16,		SREG
							push		R16

							; Signalizing LED pin to output, LED not lit yet
							sbi			LedDDR,		LedBit
							cbi			LedPort,	LedBit

							; UART
							ldi			R16,		low(BaudrateDivider)
							uout		UBRRL,		R16
							ldi			R16,		high(BaudrateDivider)
							uout		UBRRH,		R16

							clr			R16
							uout		UCSRA,		R16 ; Clearing various UART flags

							ldi			R16,		(1 << RXEN) | (1 << TXEN) ; Enabling TX and RX, no interrupts
							uout		UCSRB,		R16

							ldi 		R16, 		(1 << URSEL) | (1 << UCSZ0) | (1 << UCSZ1) ; 8 bit data, 1 stop bit, no parity
							uout		UCSRC,		R16

							pop			R16
							uout		SREG,		R16
							pop			R16
							ret

; Call this procedure to signalize that device is in bootloader mode, for example
; to light up the LED
SignalizeBootloaderMode:
							sbi			LedPort,	LedBit
							ret


; Checks do we need to enter bootloader. If yes, then R16 is 0, otherwise not 0
IsEnterBootloader:
							push		R17
							uin			R17,		SREG
							push		R17

							uin			R16,		ListenPin
							andi		R16,		1 << ListenBit

							breq		IsEnterBootloaderGoingIn
							ser			R16

IsEnterBootloaderExit:
							pop			R17
							uout		SREG,		R17
							pop			R17
							ret

IsEnterBootloaderGoingIn:
							clr			R16
							rjmp		IsEnterBootloaderExit


; Call this to wait for byte from UART. Blocks until byte is received, received byte is stored in R16
UartReadByte:
							push		R17
							uin			R17,		SREG
							push		R17

UartReadByteWait:
							wdr
							uin			R16,		UCSRA
							sbrs		R16,		RXC ; Waiting for byte
							rjmp		UartReadByteWait

							; Byte came
							uin			R16,		UDR

							pop			R17
							uout		SREG,		R17
							pop			R17
							ret


; Call this to send byte over UART. Blocks until UART is ready for transmission. Byte must be in R16.
UartSendByte:
							push		R17
							uin			R17,		SREG
							push		R17

							; Waiting for readyness
UartSendByteWait:
							wdr
							uin			R17,		UCSRA
							sbrs		R17,		UDRE
							rjmp		UartSendByteWait

							uout		UDR,		R16

							pop			R17
							uout		SREG,		R17
							pop			R17
							ret


; Call this to read EEPROM data. Put address into XH:XL and data will be returned in R16.
; Doesn't check address correctness.
ReadEeprom:
							push		R17
							uin			R17,		SREG
							push		R17

							; Waiting wor write complete (if it was initiated before)
ReadEepromWaitForWriteCompletion:
							wdr
							sbic		EECR,		EEWE
							rjmp		ReadEepromWaitForWriteCompletion

							; Set up address
							uout		EEARH,		XH
							uout		EEARL,		XL

							; Reading
							sbi			EECR,		EERE
							uin			R16,		EEDR

							pop			R17
							uout		SREG,		R17
							pop			R17
							ret


; Call this to write EEPROM data. Put address into XH:XL and data into R16.
; Doesn't check address correctness
WriteEeprom:
							push		R17
							uin			R17,		SREG
							push		R17

WriteEepromWaitForWriteCompletion:
							wdr
							sbic		EECR,		EEWE
							rjmp		WriteEepromWaitForWriteCompletion

							; Set up address
							uout		EEARH,		XH
							uout		EEARL,		XL

							; Writing
							uout		EEDR,		R16
							sbi			EECR,		EEMWE
							sbi			EECR,		EEWE

							pop			R17
							uout		SREG,		R17
							pop			R17
							ret


; Converts FLASH page number (like 0-127) into word address in ZH:ZL
; Put page number into R17
FlashPageNumberIntoAddress:
							push		R16
							uin			R16,		SREG
							push		R16

							; Converting page number into physical address in ZH:ZL
							mov			ZL,				R17
							andi		ZL,				0b00000001 ; Selecting least-significant bit and moving it into most significant position of ZL
							bst			ZL,				0
							clr			ZL
							bld			ZL,				7 ; Now we have correct content of ZL

							mov			ZH,				R17
							andi		ZH,				0b01111110 ; Selecting most significant bits
							lsr			ZH

							pop			R16
							uout		SREG,			R16
							pop			R16
							ret


; Makes SPM call. Value to be written into SPMCR must be in R16
; Blocks until previous SPM completed
MakeSPM:
							push		R17
							uin			R17,			SREG
							push		R17

							; Waiting for previous write completion
MakeSPMWait:
							wdr
							uin			R17,			SPMCR
							sbrc		R17,			SPMEN
							rjmp		MakeSPMWait

							uout		SPMCR,			R16
							spm

							pop			R17
							uout		SREG,			R17
							pop			R17
						ret