.include "HAL/ATmega16/Defines.inc"
.include "macros.inc"


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
		
							; Just echo (for test)
							call		UartReadByte
							call		UartSendByte

							rjmp		MainLoop

							; Must never reach this code
HangForever:
							rjmp		HangForever

.include "HAL/ATmega16/Code.inc"
