#include <Wire.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BNO055.h>
#include <utility/imumaths.h>
float x;
float y;
float z;
float w;
int s;
#include "BluetoothSerial.h"

#if !defined(CONFIG_BT_ENABLED) || !defined(CONFIG_BLUEDROID_ENABLED)
#error Bluetooth is not enabled! Please run `make menuconfig` to and enable it
#endif  
#define I2C_SDA 21
#define I2C_SCL 22

uint16_t BNO055_SAMPLERATE_DELAY_MS = 100;

// Check I2C device address and correct line below (by default address is 0x29 or 0x28)

//                                   id, address
Adafruit_BNO055 bno = Adafruit_BNO055(55, 0x29);
BluetoothSerial SerialBT;
  

void setup() {
  pinMode(15,INPUT);
  SerialBT.begin("ESP32test"); //Bluetooth device name

  
  // put your setup code here, to run once:
  Serial.begin(115200);
  if (!bno.begin())
  {
    /* There was a problem detecting the BNO055 ... check your connections */
    Serial.print("Ooops, no BNO055 detected ... Check your wiring or I2C ADDR!");
    while (1);
  }
  bno.setExtCrystalUse(true);
  delay(1000);
}

void loop() {

  // put your main code here, to run repeatedly:
//   imu::Quaternion quat = bno.getQuat();  
imu::Vector<3> euler = bno.getVector(Adafruit_BNO055::VECTOR_EULER);
  /* Display the floating point data */
  if (digitalRead(15)==LOW){
    s = 1;
  }
  else
  {
    s=0;
  }
  x=euler.x();
  y=euler.y();
  z=euler.z();
//  x=quat.x();
// 
//  y=quat.y();
// 
//  z=quat.z();
//  w=quat.w();
  //////////////////////////////////////////////////
  
       

  
  String temp = "";
        temp += x;
        temp += ",";
        temp += y;
        temp += ",";
        temp += z;
//        temp += ",";
//        temp += w;
//        temp += ",";
//        temp += s;
        
  //SerialBT.flush();
  Serial.println(temp);
  SerialBT.println(temp);
  
 
  //SerialBT.flush();
    

}
void printEvent(sensors_event_t* event) {
  double x = -1000000, y = -1000000 , z = -1000000; //dumb values, easy to spot problem
  if (event->type == SENSOR_TYPE_ACCELEROMETER) {
    Serial.print("Accl:");
    x = event->acceleration.x;
    y = event->acceleration.y;
    z = event->acceleration.z;
  }
  else if (event->type == SENSOR_TYPE_ORIENTATION) {
    Serial.print("Orient:");
    x = event->orientation.x;
    y = event->orientation.y;
    z = event->orientation.z;
  }
  else if (event->type == SENSOR_TYPE_MAGNETIC_FIELD) {
    Serial.print("Mag:");
    x = event->magnetic.x;
    y = event->magnetic.y;
    z = event->magnetic.z;
  }
  else if (event->type == SENSOR_TYPE_GYROSCOPE) {
    Serial.print("Gyro:");
    x = event->gyro.x;
    y = event->gyro.y;
    z = event->gyro.z;
  }
  else if (event->type == SENSOR_TYPE_ROTATION_VECTOR) {
    Serial.print("Rot:");
    x = event->gyro.x;
    y = event->gyro.y;
    z = event->gyro.z;
  }
  else if (event->type == SENSOR_TYPE_LINEAR_ACCELERATION) {
    Serial.print("Linear:");
    x = event->acceleration.x;
    y = event->acceleration.y;
    z = event->acceleration.z;
  }
  else {
    Serial.print("Unk:");
  }

  Serial.print("\tx= ");
  Serial.print(x);
  Serial.print(" |\ty= ");
  Serial.print(y);
  Serial.print(" |\tz= ");
  Serial.println(z);
}
