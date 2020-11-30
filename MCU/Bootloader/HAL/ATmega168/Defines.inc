.include "HAL/ATmega168/m168def.inc"

.set	BootloaderStartAddress		=	0x1E00			; Bootloader starts here

; Set this bit on this port to 1 when lighting-up the LED
.set	LedPort						= PORTC
.set	LedDDR						= DDRC
.set	LedBit						= 1


; Listen this port for bootloader button press (zero level)
.set	ListenPort					= PORTB
.set	ListenDDR					= DDRB
.set	ListenPin					= PINB
.set	ListenBit					= 0

; Clocks
.set	OscFreq						= 4096000
.set	Baudrate					= 38400
.set	BaudrateDivider				= OscFreq / (16 * Baudrate) - 1 ; UART divider

; FLASH
.set	FlashPagesTotal				= 128
.set	FlashPagesWriteable			= 120
.set	FlashPageAddressMultiplier	= 7 ; Multiply page address to this value to get page first byte address
.set	FlashPageSize				= 128
.set	FlashPageSizeInWords		= 64