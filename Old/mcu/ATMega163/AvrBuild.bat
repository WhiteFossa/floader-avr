@ECHO OFF
"C:\Program Files (x86)\Atmel\AVR Tools\AvrAssembler2\avrasm2.exe" -S "E:\Programming\AVR\FLoader\mcu\ATMega163\labels.tmp" -fI -W+ie -C V2E -o "E:\Programming\AVR\FLoader\mcu\ATMega163\bootloader.hex" -d "E:\Programming\AVR\FLoader\mcu\ATMega163\bootloader.obj" -e "E:\Programming\AVR\FLoader\mcu\ATMega163\bootloader.eep" -m "E:\Programming\AVR\FLoader\mcu\ATMega163\bootloader.map" "E:\Programming\AVR\FLoader\mcu\ATMega163\bootloader.asm"
