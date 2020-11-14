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

							; Setting up stack
							ldi			R16,			high(ramend)
							uout		SPH,			R16
							ldi			R16,			low(ramend)
							uout		SPL,			R16

							; Disabling or slowing down WDT
							call		DisableOrSlowDownWDT

							; Lighting on LED (test)
							call SignalizeBootloaderMode


							; Must never reach this code
HangForever:
							rjmp		HangForever

.include "HAL/ATmega16/Code.inc"
