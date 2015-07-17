#include <SPI.h>
#include <Nrf2401.h>
#include <Mirf.h>
#include <MirfHardwareSpiDriver.h>
#include <pt.h>

byte tempByte;
byte incomingByte;
static struct pt senderPt, receiverPt;
String receiveStr = "";
bool flag = false;
int payload = 16;
byte data[16];

bool isLineBreak(int what)
{
  if (13 == what) {
    flag = true;
    return false;
  }

  if (10 == what && true == flag) {
    flag = false;
    return true;
  }

  flag = false;
  return false;
}

static int sender(struct pt *pt)
{
  PT_BEGIN(pt);
  while (1) {
    PT_WAIT_UNTIL(pt, Serial.available() > 0);
    incomingByte = Serial.read();
    Mirf.send(&incomingByte);
    while (Mirf.isSending());
  }
  PT_END(pt);
}

static int receiver(struct pt *pt)
{
  PT_BEGIN(pt);
  while (1) {
    PT_WAIT_UNTIL(pt, !Mirf.isSending() && Mirf.dataReady());
    Mirf.getData(data);
    int i;
    String Temp;
    for (i = 0; i < Mirf.payload; i++){
      Temp += char(data[i]);
    }
    Serial.print(Temp);
  }
  PT_END(pt);
}

void setup()
{
  Serial.begin(9600);
  Mirf.spi = &MirfHardwareSpi;
  Mirf.init();

  Mirf.setRADDR((byte *)"TX_01");
  Mirf.setTADDR((byte *)"TX_02");
  Mirf.payload = payload;
  Mirf.channel = 90;
  Mirf.config();

  PT_INIT(&senderPt);
  PT_INIT(&receiverPt);
}

void loop()
{
  sender(&senderPt);
  receiver(&receiverPt);
}
