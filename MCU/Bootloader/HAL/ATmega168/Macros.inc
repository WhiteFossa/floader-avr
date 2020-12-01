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

; Hardware-specific macros for ATmega 168


; Just make SPM
.macro MakeSpmMacro
							ldi			R16,			(1 << SELFPRGEN)
							call		MakeSPM
.endm

; Erase FLASH page
.macro MakeSpmErasePageMacro
							ldi			R16,			(1 << PGERS) | (1 << SELFPRGEN)
							call		MakeSPM
.endm

; Write FLASH page
.macro MakeSpmWritePageMacro
							ldi			R16,			(1 << PGWRT) | (1 << SELFPRGEN)
							call		MakeSPM
.endm

; Restore access to RWW FLASH
.macro MakeSpmRestoreRWWAccessMacro
							ldi			R16,			(1 << RWWSRE) | (1 << SELFPRGEN)
							call		MakeSPM
.endm
