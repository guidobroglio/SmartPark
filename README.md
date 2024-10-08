# AA23-24-Gruppo01

## Istruzioni per l'avvio
Prima di poter eseguire l'applicazione SmartPark, è necessario attivare i seguenti servizi:
* il broker Mosquitto.
* l'Emulatore per le lampadine Philips Hue, eseguendo il file `HueEmulator-v0.8.jar` ed importando la configurazione presente in `hue_smartpark.json` (entrambi contenuti nel folder *EmulatorePhilipsHue*). L'emulatore deve essere attivato sulla porta 8000.

Per avviare SmartPark è necessario eseguire i seguenti progetti della soluzione .NET ProgettoPissir:
* SmartParkWebAPI, che avvia l'API REST.
* SmartParkWebApp, che avvia l'applicazione web che utilizza l'API REST.

I progetti possono essere attivati contemporaneamente da Visual Studio (configurando "Progetti di avvio multipli" nelle proprietà della soluzione), oppure lanciando il comando `dotnet run` rispettivamente
per SmartParkWebAPI e SmartParkWebApp.
