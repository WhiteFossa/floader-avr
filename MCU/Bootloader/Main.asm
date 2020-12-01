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

; Bootloader main file

.include "HAL/ATmega168/Defines.inc"
.include "HAL/ATmega168/Macros.inc"

.include "Macros.inc"
.include "DeviceIdentificationData.inc"

; Private use definitions
.set	IdentificationSequenceLength	= 14

.set	ResultOK						= 0x00
.set	ResultError						= 0x01

.cseg

; Endless loop at main entry point
.org						0x0000
MainEntryPoint:
							rjmp		MainEntryPoint

; Bootloader entry point
.org						BootloaderStartAddress

BootloaderEntryPoint:
							cli
							wdr

							; Setting up stack
							ldi			R16,			high(ramend)
							uout		SPH,			R16
							ldi			R16,			low(ramend)
							uout		SPL,			R16

							; Disabling or slowing down WDT
							call		DisableOrSlowDownWDT

							; Initializing hardware (pre-enter)
							call		InitializeHardwarePreEnter

							; Do we need to enter bootloader
							call		IsEnterBootloader
							tst			R16
							breq		EnterBootloader
							jmp			MainEntryPoint

EnterBootloader:
							; Initializing hardware (post-enter, like UART)
							call InitializeHardwarePostEnter

							; Signalizing that we are in bootloader
							call SignalizeBootloaderMode

							; Command processing loop
MainLoop:
							;ldi			R16,				'T'
							;call		UartSendByte
							call		UartReadByte ; Watchdog is reset here
							
							cpi			R16,			'I' ; Identify
							breq		LblIdentify
							
							cpi			R16,			'Q' ; Quit (reboot into main firmware)
							breq		LblQuit
							
							cpi			R16,			'R' ; Read FLASH page
							breq		LblReadFlashPage
							
							cpi			R16,			'r' ; Read EEPROM
							breq		LblReadEeprom
							
							cpi			R16,			'w' ; Write EEPROM
							breq		LblWriteEeprom
							
							cpi			R16,			'W' ; Write FLASH page
							breq		LblWriteFlashPage

							rjmp		MainLoop

; Identification entry point
LblIdentify:
							call		Identify
							rjmp		MainLoop

; Boot mode quit entry point
LblQuit:
							jmp			Quit ; Quit is not a procedure, there is no return

; Read FLASH page entry point
LblReadFlashPage:
							call		ReadFlashPage
							rjmp		MainLoop

; Read EEPROM entry point
LblReadEeprom:
							call		ReadAllEeprom
							rjmp		MainLoop

; Write EEPROM entry point
LblWriteEeprom:
							call		WriteAllEeprom
							rjmp		MainLoop

; Write FLASH page entry point
LblWriteFlashPage:
							call		WriteFlashPage
							rjmp		MainLoop

							; Must never reach this code
HangForever:
							rjmp		HangForever

; Call this to identify device
Identify:
							push		R16
							push		R17
							push		ZL
							push		ZH
							uin			R16,			SREG
							push		R16

							; Setting start address and counter
							ldi			ZL,				low(IdentificationSequence * 2)
							ldi			ZH,				high(IdentificationSequence * 2)
							clr			R17

IdentifySendNextByte:
							lpm			R16,			Z+
							call		UartSendByte
							inc			R17
							cpi			R17,			IdentificationSequenceLength
							breq		IdentifyExit
							rjmp		IdentifySendNextByte
							

IdentifyExit:
							pop			R16
							uout		SREG,			R16
							pop			ZH
							pop			ZL
							pop			R17
							pop			R16
							ret


; Jump here to quit into main firmware
Quit:
							ldi			R16,			'B' ; Send "BYE" to client software
							call		UartSendByte
							call		UartSendByte	; Second call acts as "wait till previous transmission completed"
							jmp			MainEntryPoint


; Call this to read FLASH page
ReadFlashPage:
							push		R16
							push		R17
							uin			R16,			SREG
							push		R16

							; Reading FLASH page address
							call		UartReadByte
							mov			R17,			R16 ; Copy of address in R17

							; Is this address correct?
							cpi			R16,			FlashPagesTotal
							brlo		ReadFlashPageAddressOK

							; Address incorrect
							ldi			R16,			ResultError
							call		UartSendByte
							rjmp		ReadFlashPageExit

ReadFlashPageAddressOK:
							ldi			R16,			ResultOK
							call		UartSendByte

							; Loading page address (R17) into ZH:ZL and multiplying it by page size
							clr			ZH
							mov			ZL,				R17

							clr			R16 ; Counter
							clr			R17 ; For add with carry

ReadFlashPageMultiplyAddressByTwo:
							clc
							lsl			ZH
							lsl			ZL
							adc			ZH,				R17 ; ZH = ZH + Carry + 0

							inc			R16
							cpi			R16,			FlashPageAddressMultiplier
							brlo		ReadFlashPageMultiplyAddressByTwo

							; We have ZH:ZL pointing to first byte of page, time to send it via UART
							clr			R17 ; counter

ReadFlashPageReadNextByte:
							lpm			R16,			Z+
							call		UartSendByte ; Watchdog is reset here
							inc			R17
							cpi			R17,			FlashPageSize
							brlo		ReadFlashPageReadNextByte
ReadFlashPageExit:
							pop			R16
							uout		SREG,			R16
							pop			R17
							pop			R16
							ret


; Read all EEPROM
ReadAllEeprom:
							push		R16
							push		R17
							push		R18
							push		XL
							push		XH
							uin			R16,			SREG
							push		R16

							; Loading start address
							clr			XL ; Low
							clr			XH ; High

							; And end one
							ldi			R17,			low(EEPROMEND + 1)
							ldi			R18,			high(EEPROMEND + 1)

ReadAllEepromNextByte:
							call		ReadEeprom ; Watchdog is reset here
							call		UartSendByte

							adiw		XH:XL,			1

							; Comparing addresses
							cp			XL,				R17
							cpc			XH,				R18

							brne		ReadAllEepromNextByte


							pop			R16
							uout		SREG,			R16
							pop			XH
							pop			XL
							pop			R18
							pop			R17
							pop			R16
							ret


; Write all EEPROM
WriteAllEeprom:
							push		R16
							push		R17
							push		R18
							push		XL
							push		XH
							uin			R16,			SREG
							push		R16

							; Loading start address
							clr			XL ; Low
							clr			XH ; High

							; End address
							ldi			R17,			low(EEPROMEND + 1)
							ldi			R18,			high(EEPROMEND + 1)

WriteAllEepromNextByte:
							call		UartReadByte
							call		WriteEeprom

							adiw		XH:XL,			1

							; Is finish?
							cp			XL,				R17
							cpc			XH,				R18

							breq		WriteAllEepromFinish

							; Asking for next byte
							ldi			R16,			'n'
							call		UartSendByte
							rjmp		WriteAllEepromNextByte

WriteAllEepromFinish:
							ldi			R16,			'f'
							call		UartSendByte

							pop			R16
							uout		SREG,			R16
							pop			XH
							pop			XL
							pop			R18
							pop			R17
							pop			R16

							ret


; Write FLASH page
WriteFlashPage:
							push		R0
							push		R1
							push		R16
							push		R17
							push		R18
							push		ZL
							push		ZH
							uin			R16,			SREG
							push		R16

							; Getting page number
							call		UartReadByte
							mov			R17,			R16 ; Copy of page number in R17

							cpi			R16,			FlashPagesWriteable
							brlo		WriteFlashPageNumberOK

							; Number is incorrect
							ldi			R16,			ResultError
							call		UartSendByte
							rjmp		WriteFlashPageExit

WriteFlashPageNumberOK:
							ldi			R16,			ResultOK
							call		UartSendByte

							; Loading physical address into ZH:ZL from R17
							call		FlashPageNumberIntoAddress

							clr			R18 ; Words counter

							; Reading word by word and filling page buffer
WriteFlashPageNextWord:
							call		UartReadByte ; WDT is reset here
							mov			R0,				R16 ; Least byte
							call		UartReadByte
							mov			R1,				R16 ; Most byte

							MakeSpmMacro

							; Next word, not byte
							adiw		ZH:ZL,			2
							inc			R18
							cpi			R18,			FlashPageSizeInWords
							brlo		WriteFlashPageNextWord

							; Page buffer is filled now, reloading physical address into ZH:ZL from R17
							call		FlashPageNumberIntoAddress

							; Erase page
							wdr
							MakeSpmErasePageMacro

							; Write page
							wdr
							MakeSpmWritePageMacro

							; Restoring access to RWW FLASH
							wdr
							MakeSpmRestoreRWWAccessMacro

							; Done
							ldi			R16,			ResultOK
							call		UartSendByte

WriteFlashPageExit:
							pop			R16
							uout		SREG,			R16
							pop			ZH
							pop			ZL
							pop			R18
							pop			R17
							pop			R16
							pop			R1
							pop			R0
							ret

; Send this to UART to identify yourself
IdentificationSequence:		.db 'F', 'B', 'L', 0x01, VendorId2, VendorId1, VendorId0, ModelId2, ModelId1, ModelId0, SerialNumber3, SerialNumber2, SerialNumber1, SerialNumber0

.include "HAL/ATmega168/Code.inc"
