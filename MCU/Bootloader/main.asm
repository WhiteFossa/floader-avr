.include "HAL/ATmega16/Defines.inc"
.include "Macros.inc"
.include "DeviceIdentificationData.inc"


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

							rjmp		MainLoop

; Identification entry point
LblIdentify:
							call		Identify
							rjmp		MainLoop

							; Must never reach this code
HangForever:
							rjmp		HangForever

; Call this to identify device
Identify:
							push		R16
							uin			R16,			SREG
							push		R16

							; Bootloader signature
							ldi			R16,			'F'
							call		UartSendByte
							ldi			R16,			'B'
							call		UartSendByte
							ldi			R16,			'L'
							call		UartSendByte

							; Version (simple bootloader = 0x01)
							ldi			R16,			0x01
							call		UartSendByte

							; Vendor identifier
							ldi			R16,			VendorId2
							call		UartSendByte
							ldi			R16,			VendorId1
							call		UartSendByte
							ldi			R16,			VendorId0
							call		UartSendByte

							; Model identifier
							ldi			R16,			ModelId2
							call		UartSendByte
							ldi			R16,			ModelId1
							call		UartSendByte
							ldi			R16,			ModelId0
							call		UartSendByte

							; Serial number
							ldi			R16,			SerialNumber3
							call		UartSendByte
							ldi			R16,			SerialNumber2
							call		UartSendByte
							ldi			R16,			SerialNumber1
							call		UartSendByte
							ldi			R16,			SerialNumber0
							call		UartSendByte

							pop			R16
							uout		SREG,			R16
							pop			R16
							ret

.include "HAL/ATmega16/Code.inc"
