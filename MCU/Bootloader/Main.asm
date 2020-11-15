.include "HAL/ATmega16/Defines.inc"
.include "Macros.inc"
.include "DeviceIdentificationData.inc"

; Private use definitions
.set	IdentificationSequenceLength	= 14

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

							rjmp		MainLoop

; Identification entry point
LblIdentify:
							call		Identify
							rjmp		MainLoop

; Boot mode quit entry point
LblQuit:
							jmp			Quit ; Quit is not a procedure, there is no return

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


; Send this to UART to identify yourself
IdentificationSequence:		.db 'F', 'B', 'L', 0x01, VendorId2, VendorId1, VendorId0, ModelId2, ModelId1, ModelId0, SerialNumber3, SerialNumber2, SerialNumber1, SerialNumber0

.include "HAL/ATmega16/Code.inc"
