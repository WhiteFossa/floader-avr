.include "HAL/ATmega16/m16def.inc"

.set	BootloaderStartAddress		=	0x1E00			; Bootloader starts here

; Set this bit on this port to 1 when lighting-up the LED
.set	LedPort						= PORTB
.set	LedDDR						= DDRB
.set	LedBit						= 4


; Listen this port for bootloader button press (zero level)
.set	ListenPort					= PORTB
.set	ListenDDR					= DDRB
.set	ListenPin					= PINB
.set	ListenBit					= 0

; Clocks
.set	OscFreq						= 16000000
.set	Baudrate					= 57600
.set	BaudrateDivider				= OscFreq / (16 * Baudrate) - 1 ; UART divider

; FLASH
.set	FlashPagesTotal				= 128
.set	FlashPagesWriteable			= 120
.set	FlashPageAddressMultiplier	= 7 ; Multiply page address to this value to get page first byte address
.set	FlashPageSize				= 128
.set	FlashPageSizeInWords		= 64


