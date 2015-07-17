#include <SPI.h>
#include <Nrf2401.h>
#include <Mirf.h>
#include <MirfHardwareSpiDriver.h>
#include <pt.h>

static int sendTimer = 0, receiveTimer = 0;
static struct pt sendPt, receivePt;
static byte data[16];
static int payload = 16;
static bool flag = false;
static byte incomingByte;
static String str = "";
static char tempx[17];

void setup() {
  Serial.begin(9600);
  Mirf.spi = &MirfHardwareSpi;
  Mirf.init();

  Mirf.setRADDR((byte *)"TX_02");
  Mirf.setTADDR((byte *)"TX_01");
  Mirf.payload = 16;
  Mirf.channel = 90;
  Mirf.config();

  PT_INIT(&sendPt);
  PT_INIT(&receivePt);
}

void loop() {
  sendTimer++;
  receiveTimer++;
  sender(&sendPt);
  receiver(&receivePt);
  delay(200);
}

static int sender(struct pt *pt)
{
  PT_BEGIN(pt);
  while (1) {
    PT_WAIT_UNTIL(pt, sendTimer == 2);

    if (Serial.available() > 0) {
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
          memsetTempx();
          str.substring(i * payload, (i + 1) * payload).toCharArray(tempx, payload + 1);
          sends(tempx, payload + 1);
          /*
          Serial.print(" [tempx]:");
          Serial.println(tempx);*/
        }

        str = "";
      }
    }
    sendTimer = 0;
  }
  PT_END(pt);
}

static int receiver(struct pt *pt)
{
  PT_BEGIN(pt);
  while (1) {
    PT_WAIT_UNTIL(pt, receiveTimer == 1);

    if ( !Mirf.isSending() && Mirf.dataReady()) {
      Mirf.getData(data);
      static String Temp;
      for (int i = 0; i < Mirf.payload; i++) {
        Temp += char(data[i]);
      }
      Serial.print(Temp);
    }
    receiveTimer = 0;
  }
  PT_END(pt);
}

static bool isLineBreak(int what)
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

static void sends(char *str, int strLens)
{
  int lens = strLens;
  char msg[lens];
  int i;

  for (i = 0; i < lens; i++) {
    msg[i] = int(str[i]);
  }
  Mirf.send((byte *)&msg);
  while (Mirf.isSending()) {}
}

static void memsetTempx()
{
  for(int i = 0; i < 17; i++){
    tempx[i] = '\0';
  }
}
