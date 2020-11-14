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