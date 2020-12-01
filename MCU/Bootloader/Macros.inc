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

; Macros are here

; First memory-mapped register address
.set	FirstMemoryMappedRegister		= 0x40

; Universal out - uses short/long out depending on IO register address
.macro uout
	.if @0 < FirstMemoryMappedRegister
							out		@0,		@1
	.else
							sts		@0,		@1
	.endif
.endm

; As Universal out, but for in
.macro uin
	.if @1 < 0x40 ; First memory-mapped address
							in		@0,		@1
	.else
							lds		@0,		@1
	.endif
.endm