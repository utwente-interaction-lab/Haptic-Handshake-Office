#include <WiFi.h>
#include <ArduinoJson.h>

//for connecting to wifi
const char* ssid     = "TP-LINK_26E8";
const char* password = "60752215";

//for creating a TCP/IP connection
const char* ip = "192.168.0.101";
const int sendPort = 7600;
//const int receivePort = 7501;

WiFiClient client;

//for receiving messages
const int bufsize = 1024;
char buf[bufsize];
int offset = 0;
bool commandReceived;

// setting PWM properties
const int freq = 5000;
const int resolution = 8;

//for the motors
const int m1Channel = 1;
const int m1Pin = 13;
int m1Intensity = 0;

const int m2Channel = 2;
const int m2Pin = 12;
int m2Intensity = 0;

const int m3Channel = 3;
const int m3Pin = 27;
int m3Intensity = 0;

const int m4Channel = 4;
const int m4Pin = 33;
int m4Intensity = 0;

void setup() {
  Serial.begin(115200);
  delay(10);

  ledcSetup(m1Channel, freq, resolution);
  ledcSetup(m2Channel, freq, resolution);
  ledcSetup(m3Channel, freq, resolution);
  ledcSetup(m4Channel, freq, resolution);

  //set the pinouts for our motors
  ledcAttachPin(m1Pin, m1Channel);
  ledcAttachPin(m2Pin, m2Channel);
  ledcAttachPin(m3Pin, m3Channel);
  ledcAttachPin(m4Pin, m4Channel);

  //setup the connections
  connectWIFI();
  connectTCP();

  //let anyone listening know that we've fully connected
  client.println("{\"connection\":\"success\"}");
}

void loop() {
  //try to parse an incoming command
  readCommand();

  //do the stuff with the motors
  if (commandReceived) {
    ledcWrite(m1Channel, m1Intensity);
    ledcWrite(m2Channel, m2Intensity);
    ledcWrite(m3Channel, m3Intensity);
    ledcWrite(m4Channel, m4Intensity);

    //and wait for next command
    commandReceived = false;
  }
}

void readCommand() {
  if (client.available() > 0) {
    //to prevent buffer overflow
    if (offset >= bufsize) {
      offset = 0;
    }

    //read next char and store in buffer
    char c = client.read();
    buf[offset] = c;

    //    Serial.println(buf);
    //are we at the end of the line yet?
    if (offset >= 2 &&  buf[offset] == '\n') {

      //ok we have the end of our line, terminate the string here
      buf[offset - 1] = (char)0;
      Serial.print("Got message: '");
      Serial.print(buf);
      Serial.println("'");

      //prepare to parse the json
      StaticJsonDocument<bufsize * 2> json;

      // Deserialize the JSON document
      DeserializationError error = deserializeJson(json, buf);

      // Test if parsing succeeds.
      if (error) {
        Serial.print(F("deserializeJson() failed: "));
        Serial.println(error.c_str());
        return;
      }

      //save the new values for the motors
      m1Intensity = constrain(json["m1Intensity"], 0, 255);
      m2Intensity = constrain(json["m2Intensity"], 0, 255);
      m3Intensity = constrain(json["m3Intensity"], 0, 255);
      m4Intensity = constrain(json["m4Intensity"], 0, 255);

      client.print("{\"intensity\":" + String(m1Intensity) + "}");
      Serial.println("{\"intensity\":" + String(m1Intensity) + "}");
//      sender.print(m1Intensity);
//      sender.println("}");

      //finally, tell the main loop that we have succesfully received a new command
      commandReceived = true;

      offset = 0;
    } else {
      offset++;
    }
  }
}

void connectWIFI()
{
  Serial.print("Connecting to WIFI network: ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("");
  Serial.println("WiFi connected");
  Serial.print("My IP address is ");
  Serial.println(WiFi.localIP());
}

void connectTCP()
{
  Serial.print("Connecting to server on IP ");
  Serial.print(ip);
  Serial.print(" and port ");
  Serial.println(sendPort);

  // Use WiFiClient class to create TCP connections
  while (!client.connect(ip, sendPort)) {
    Serial.println("Connection failed");
  }

  if (client.connected()) {
    //if succesfully connected, we must now open our own server to accept incoming connection from the other end
    Serial.println("Connected to server");

    
    
//    Serial.print("Waiting for incoming return connection on port ");
//    Serial.println(receivePort);
//
//    WiFiServer server(receivePort);
//    server.begin();
//
//    //listen for incoming connections
//    while (!client) {
//      client = server.available();
//    }
//
//    //yay we now have full duplex connection with other end over two sockets :)
//    if (client && client.connected()) {
//      Serial.println("Incoming connection succesful.");
//    }
  }
}
