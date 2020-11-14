; Disable or slow down WDT procedure
DisableOrSlowDownWDT:
							push		R16
							uin			R16,		SREG
							push		R16

							uin			R16,		WDTCR
							ori			R16,		(1 << WDTOE) | (1 << WDE)
							uout		WDTCR,		R16 ; 4 clocks starts here

							andi		R16,		~(1 << WDE)
							uout		WDTCR,		R16 ; WDE disabled

							pop			R16
							uout		SREG,		R16
							pop			R16
							ret


; Call this procedure to signalize that device is in bootloader mode, for example
; to light up the LED
SignalizeBootloaderMode:
							sbi			LedDDR,		LedBit
							sbi			LedPort,	LedBit
							ret