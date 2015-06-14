int val = 0;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
  Serial.print(42);
  //nt
  Serial.print("\r");
  Serial.print("\n");
  
  //linux
  //Serial.print("\n");
  
  //mac
  //Serial.print("\r");
  
  //length
  
  //char
  //Serial.print('e');
}
