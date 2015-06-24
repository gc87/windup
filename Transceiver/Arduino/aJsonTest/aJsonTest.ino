#include <aJSON.h>

aJsonObject *root, *fmt;

void setup() {
  Serial.begin(9600);
}

void loop() {
  root = aJson.createObject();
  aJson.addItemToObject(root, "name", aJson.createItem("Jack (\"Bee\") Nimble"));
  aJson.addItemToObject(root, "format", fmt = aJson.createObject());
  aJson.addStringToObject(fmt, "type",     "rect");
  aJson.addNumberToObject(fmt, "width",        1920);
  aJson.addNumberToObject(fmt, "height",       1080);
  aJson.addBooleanToObject (fmt, "interlace", false);
  aJson.addNumberToObject(fmt, "frame rate",   24);
  
  Serial.println(aJson.print(root));
  delay(1000);
}
