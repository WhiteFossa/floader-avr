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

; Hardware-specific defines for ATmega16

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

; Various stuff
.set	SpmFlagName					= SPMEN ; SPM flag have various names in various devices


