.include "HAL/m16def.inc"

.set	BootloaderStartAddress		=	0x1E00			; Bootloader starts here

; Set this bit on this port to 1 when lighting-up the LED
.set	LedPort						= PORTB
.set	LedDDR						= DDRB
.set	LedBit						= 4


