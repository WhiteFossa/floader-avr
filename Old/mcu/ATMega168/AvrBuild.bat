@ECHO OFF
"C:\Program Files (x86)\Atmel\AVR Tools\AvrAssembler2\avrasm2.exe" -S "E:\Programming\AVR\FLoader\mcu\ATMega168\labels.tmp" -fI -W+ie -C V2E -o "E:\Programming\AVR\FLoader\mcu\ATMega168\bootloader.hex" -d "E:\Programming\AVR\FLoader\mcu\ATMega168\bootloader.obj" -e "E:\Programming\AVR\FLoader\mcu\ATMega168\bootloader.eep" -m "E:\Programming\AVR\FLoader\mcu\ATMega168\bootloader.map" "E:\Programming\AVR\FLoader\mcu\ATMega168\bootloader.asm"
