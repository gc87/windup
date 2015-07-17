#include <SPI.h>
#include <Nrf2401.h>
#include <Mirf.h>
#include <MirfHardwareSpiDriver.h>
#include <pt.h>

byte tempByte;
byte incomingByte;
static struct pt senderPt, receiverPt;
int payload = 16;
String str = "";
bool flag = false;
char tempx[17] = "0000000000000000";

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

void sends(char *str)
{
  int lens;
  lens = strlen(str);
  char msg[lens];
  int i;

  for (i = 0; i < lens; i++) {
    msg[i] = int(str[i]);
  }
  
  Mirf.send((byte *)&msg);
  while (Mirf.isSending()) {}
}

static int sender(struct pt *pt)
{
  PT_BEGIN(pt);
  while (1) {
    PT_WAIT_UNTIL(pt, Serial.available() > 0);
    incomingByte = Serial.read();

    str += char(incomingByte);
    if (isLineBreak(incomingByte)) {
      int len = str.length();
      int div = len / payload;
      int mod = len % payload;
      if (mod > 0) div ++;
      
      Serial.print("[len]:");
      Serial.print(len);
      Serial.print(" [div]:");
      Serial.print(div);
      Serial.print(" [mod]:");
      Serial.print(mod);
      
      for (int i = 0; i < div; i++) {
        str.substring(i * payload, (i + 1) * payload - 1).toCharArray(tempx, payload);
        sends(tempx);0
        
        Serial.print(" [tempx]:");
        Serial.print(tempx);
      }
      
      str = "";
    }
  }
  PT_END(pt);
}

static int receiver(struct pt *pt)
{
  PT_BEGIN(pt);
  while (1) {
    PT_WAIT_UNTIL(pt, Mirf.dataReady());
    Mirf.getData(&tempByte);
    Serial.print(tempByte);
  }
  PT_END(pt);
}

void setup()
{
  Serial.begin(9600);
  Mirf.spi = &MirfHardwareSpi;
  Mirf.init();
  
  Mirf.setRADDR((byte *)"TX_02");
  Mirf.setTADDR((byte *)"TX_01");
  Mirf.payload = 16;
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

