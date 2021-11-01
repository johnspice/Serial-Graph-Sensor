/* 
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
+       
+  (CODIGO PRUEBA) PARA "SERIAL SENSOR PLOT 1.0 y 4.0"  "WINDOWS Y ANDROID"       +
+  
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
 */



String envia="";
int i=0;


void setup() {
  // put your setup code here, to run once:
Serial.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
envia.concat(i*i);//set sensor E0 here
envia.concat(" ");
envia.concat(i);//set sensor E1 here
envia.concat(" ");
envia.concat(i+20);//set sensor E2 here
envia.concat(" ");
envia.concat(-i);//set sensor E3 here
envia.concat(" ");
Serial.println(envia);
envia="";
i=i+1;
delay(500);

}

