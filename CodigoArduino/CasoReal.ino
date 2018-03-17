/* 
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
+       
+  CODIGO (CASO REAL) PARA "SERIAL SENSOR PLOT 1.0"  "WINDOWS Y ANDROID"   +
+  
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
 */
byte   sensorCardiaco = 8;//entrada de arduino donde esta el sensor cardiaco
String envia="";//string (vacio al inicio) que tendra toda la cadena a enviar con cada dato incrustado 
                //y separado por un espacio entre cada dato y al final
void setup()
{
  //definimos como entrada, configuramos el pueto serie a 9600 que es el que usa "Serial Sensor Plot 1.0"
  pinMode(sensorCardiaco,INPUT);
  pinMode(A0,INPUT);
  Serial.begin(9600);
}
void loop()
{  
   //sensor 1--------------------------------
   // se carga el valor analogico leido en A0 (potenciometro) al Strin "envia"
   envia.concat(analogRead(A0));
   //agregamos un espacio al final de la cadena anterior
   envia.concat(" ");
   //sensor 2--------------------------------
   // leemos el pin 8 para ver si hay pulso si lo hay pegamos al Strin "envia" un 1 de lo contrario le pegamos un 0
   if(digitalRead(sensorCardiaco)==HIGH){envia.concat(1);} else {envia.concat(0);}
   //agregamos nuevamente un espacio
   envia.concat(" ");
   //si quisieramos agregar mas datos lo hacemos con "envia.concat(DATO)"; seguido de espacio al final envia.concat(" ");
   //sensor 3------------------------------
   //....
   //....
   //sensor 4------------------------------
  //....
  //....
  //enviamos 
  Serial.println(envia);
  //limpiamos el string "envia" para iniciar nuevamente el loop
  envia="";
  //un retardo aveces si no lo pongo me causa problemas
  delay(500);//el tiempo debe ser el mismo que en Android y Windows
}

