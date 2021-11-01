#include <ESP8266WiFi.h>
 //SET VALUES
const char* ssid = "wifi here";//set name wifi or hospot
const char* password = "pass here";//set password 
const String nameBoard="NodeMCU 0.9 (ESP-12 Module)";//this name show in app android "Serial Sensor Plot" app

String data="";
int ledPin = 2; // (D4 in nodeMCU 1.0) or (pin=2 esp8266) here conect led and turn on or off from "Serial Sensor Plot" app
int i=0;

WiFiServer server(80);

///////////////////////  SETUP  ///////////////// 
void setup() {
  Serial.begin(9600);
  delay(10);
  pinMode(ledPin, OUTPUT);
  digitalWrite(ledPin, LOW);
  // Connect to WiFi network
  Serial.println();
  Serial.println();
  Serial.print("Connecting to: ");
  Serial.println(ssid);
  WiFi.begin(ssid, password);
  
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("");
  Serial.println("WiFi connected");
  // Start the server
  server.begin();
  Serial.println("Server started");
  // Print the IP address
  String ms="URL to connect: http://"+ WiFi.localIP().toString() +"/";
  Serial.println(ms); 
}


///////////////////////  LOOP /////////////////
void loop() {
  // Check if a client has connected
  WiFiClient client = server.available();
  if (!client) {return;}
  // Wait until the client sends some data
  Serial.println("new client");
  while(!client.available()){ delay(1);}
 
  // Read the first line of the request
  String request = client.readStringUntil('\r');
  Serial.println(request);
  client.flush();
 
 
  if (request.indexOf("/option=a") != -1){
    digitalWrite(ledPin, HIGH);
  }
  if (request.indexOf("/option=b") != -1){
    digitalWrite(ledPin, LOW);
  }


  data.concat(i+1);//here put value sensor 1
  data.concat(" ");
  data.concat(i+3);//here put value sensor 2
  data.concat(" ");
  data.concat(i+5);//here put value sensor 3
  data.concat(" ");
  data.concat(-i);//here put value sensor 4
  data.concat(" ");
  data.concat(-i-5);//here put value sensor 5
  data.concat(" ");
  
  // Return Response
  printResponse(client,data,nameBoard);
  data="";
  i=i+1;

  Serial.println("Client disonnected");
  Serial.println("");
  delay(1);
}



/////////////////////////////////////////////////


void printResponse(WiFiClient client,String data,String nB ){
 client.flush();
 String s = "HTTP/1.1 200 OK\r\n";
 s += "Content-Type: application/json\r\n\r\n";
 s += "{\"success\":\"1\",\"values\":\"";//no change "success":"1" used android app 
 s += data;
 s += "\",\"device\":\"";
 s +=nB; 
 s += "\"}\r\n";
 s += "\n";
 // Send the response to the client
 client.print(s);
}

 
