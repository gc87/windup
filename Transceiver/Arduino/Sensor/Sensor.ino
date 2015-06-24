#include <SPI.h>
#include <Nrf2401.h>
#include <Mirf.h>
#include <MirfHardwareSpiDriver.h>

byte tempByte;
byte incomingByte;

void setup()
{
  Serial.begin(9600);
  Mirf.spi = &MirfHardwareSpi;
  Mirf.init();
  Mirf.payload = 1;
  Mirf.channel = 90;
  Mirf.config();
  Mirf.configRegister(RF_SETUP, 0x06);
  Mirf.setTADDR((byte *)"TX_01");
  Mirf.setRADDR((byte *)"TX_02");
}

void loop()
{
  if (Serial.available() > 0) {
    incomingByte = Serial.read();
    Mirf.send(&incomingByte);
    while (Mirf.isSending());
  }

  if (Mirf.dataReady())
  {
    Mirf.getData(&tempByte);
    Serial.print(tempByte);
  }
}
