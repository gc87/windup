#include <SCoop.h>
#include <SPI.h>
#include <Nrf2401.h>
#include <Mirf.h>
#include <MirfHardwareSpiDriver.h>
#include "LineBreak.h"

LineBreak lb;
byte incomingByte;
String str;
int payload = 32;
char tempx[33];
int i;
byte data[32];

defineTask(Sender)
void Sender::setup() {
  i = 0;
  memsetTempx();
};
void Sender::loop() {
  if (Serial.available() > 0) {
    incomingByte = Serial.read();
    tempx[i] = incomingByte;
    i++;
    if (lb.isLineBreak4Nt(incomingByte) || 32 <= i) {
      sends(tempx, payload + 1);
      memsetTempx();
      i = 0;
    }
  }
};

defineTask(Receiver)
void Receiver::setup() {
};
void Receiver::loop() {
  if ( !Mirf.isSending() && Mirf.dataReady()) {
    memsetData();
    Mirf.getData(data);
    static String Temp;
    for (int i = 0; i < Mirf.payload; i++) {
      Serial.print(char(data[i]));
    }
  }
};

void setup() {
  Serial.begin(9600);

  Mirf.spi = &MirfHardwareSpi;
  Mirf.init();

  Mirf.setRADDR((byte *)"TX_02");
  Mirf.setTADDR((byte *)"TX_01");
  Mirf.payload = 32;
  Mirf.channel = 90;
  Mirf.config();

  mySCoop.start();
}

void loop() {
  yield();
}

void sends(char *str, int strLens)
{
  int lens = strLens;
  char msg[lens];
  int i;

  for (i = 0; i < lens; i++) {
    msg[i] = int(str[i]);
  }
  Serial.print(msg);
  Mirf.send((byte *)&msg);
  while (Mirf.isSending()) {}
}

void memsetTempx()
{
  for (int i = 0; i < 33; i++) {
    tempx[i] = '\0';
  }
}

void memsetData()
{
  for (int i = 0; i < 32; i++) {
    data[i] = '\0';
  }
}
