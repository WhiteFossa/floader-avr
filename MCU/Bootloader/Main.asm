.include "HAL/ATmega16/Defines.inc"
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
							call		UartReadByte

							cpi			R16,			'I' ; Identify
							breq		LblIdentify

							cpi			R16,			'Q' ; Quit (reboot into main firmware)
							breq		LblQuit

							cpi			R16,			'R' ; Read FLASH page
							breq		LblReadFlashPage

							rjmp		MainLoop

; Identification entry point
LblIdentify:
							call		Identify
							rjmp		MainLoop

; Boot mode quit entry point
LblQuit:
							jmp			Quit ; Quit is not a procedure, there is no return

; Read flash page entry point
LblReadFlashPage:
							call		ReadFlashPage
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
							call		UartSendByte
							inc			R17
							cpi			R17,			FlashPageSize
							brlo		ReadFlashPageReadNextByte
ReadFlashPageExit:
							pop			R16
							uout		SREG,			R16
							pop			R17
							pop			R16
							ret

; Send this to UART to identify yourself
IdentificationSequence:		.db 'F', 'B', 'L', 0x01, VendorId2, VendorId1, VendorId0, ModelId2, ModelId1, ModelId0, SerialNumber3, SerialNumber2, SerialNumber1, SerialNumber0

.include "HAL/ATmega16/Code.inc"
