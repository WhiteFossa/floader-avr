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


; Call this to initialize hardware (before bootloader mode enter)
InitializeHardwarePreEnter:
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

							pop			R16
							uout		SREG,		R16
							pop			R16
							ret


; Call this to initialize hardware (after bootloader mode enter)
InitializeHardwarePostEnter:
							push		R16
							uin			R16,		SREG
							push		R16

							; Signalizing LED pin to output, LED not lit yet
							sbi			LedDDR,		LedBit
							cbi			LedPort,	LedBit

							; UART
							ldi			R16,		low(BaudrateDivider)
							uout		UBRRL,		R16
							ldi			R16,		high(BaudrateDivider)
							uout		UBRRH,		R16

							clr			R16
							uout		UCSRA,		R16 ; Clearing various UART flags

							ldi			R16,		(1 << RXEN) | (1 << TXEN) ; Enabling TX and RX, no interrupts
							uout		UCSRB,		R16

							ldi 		R16, 		(1 << URSEL) | (1 << UCSZ0) | (1 << UCSZ1) ; 8 bit data, 1 stop bit, no parity
							uout		UCSRC,		R16

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


; Call this to wait for byte from UART. Blocks until byte is received, received byte is stored in R16
UartReadByte:
							push		R17
							uin			R17,		SREG
							push		R17

UartReadByteWait:
							uin			R16,		UCSRA
							sbrs		R16,		RXC ; Skipping loop jump in RXC is set
							rjmp		UartReadByteWait

							; Byte came
							uin			R16,		UDR

							pop			R17
							uout		SREG,		R17
							pop			R17
							ret


; Call this to send byte over UART. Blocks until UART is ready for transmission. Byte must be in R16.
UartSendByte:
							push		R17
							uin			R17,		SREG
							push		R17

							; Waiting for readyness
UartSendByteWait:
							uin			R17,		UCSRA
							sbrs		R17,		UDRE
							rjmp		UartSendByteWait

							uout		UDR,		R16

							pop			R17
							uout		SREG,		R17
							pop			R17
							ret