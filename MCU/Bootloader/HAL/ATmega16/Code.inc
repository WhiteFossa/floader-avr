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


; Call this to initialize hardware
InitializeHardware:
							push		R16
							uin			R16,		SREG
							push		R16

							; All ports to input to avoid hardware damage
							clr			R16
							uout		DDRA,		R16
							uout		DDRB,		R16
							uout		DDRC,		R16
							uout		DDRD,		R16

							; Listening pin pull-up enable
							sbi			ListenPort,	ListenBit

							; Signalizing LED
							sbi			LedDDR,		LedBit

							pop			R16
							uout		SREG,		R16
							pop			R16
							ret


; Call this procedure to signalize that device is in bootloader mode, for example
; to light up the LED
SignalizeBootloaderMode:
							sbi			LedPort,	LedBit
							ret


; Checks do we need to enter bootloader. If yes, then R16 is 0, otherwise not 0
IsEnterBootloader:
							push		R17
							uin			R17,		SREG
							push		R17

							uin			R16,		ListenPin
							andi		R16,		1 << ListenBit

							breq		IsEnterBootloaderGoingIn
							ser			R16

IsEnterBootloaderExit:
							pop			R17
							uout		SREG,		R17
							pop			R17
							ret

IsEnterBootloaderGoingIn:
							clr			R16
							rjmp		IsEnterBootloaderExit