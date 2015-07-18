#include <SoftwareSerial.h>
#include <SCoop.h>
#include "LineBreak.h"

const int RX_BT = 10;
const int TX_BT = 11;
SoftwareSerial btSerial(RX_BT, TX_BT);
LineBreak lb;

defineTask(Sender)
void Sender::setup(){
};
void Sender::loop(){
  if (Serial.available() > 0) {
    btSerial.print((char)Serial.read());
  }
};

defineTask(Receiver)
void Receiver::setup() {
};
void Receiver::loop() {
   if(btSerial.available()){
     Serial.print((char)btSerial.read());
   }
};

void setup() {
  Serial.begin(9600);
  //Serial.println("USB Connected.");
  btSerial.begin(9600);
  //btSerial.println("BT Connected.");
  mySCoop.start();
}

void loop() {
  yield();
}
