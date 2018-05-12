/*Sensor de Temperatura contra agua NTC 10K */


#include <math.h><br>

String envia="";
        
void setup() {            
Serial.begin(9600); 
} 


double Thermister(int RawADC) {  
double Temp;
Temp = log(((10240000/RawADC) - 10000));
Temp = 1 / (0.001129148 + (0.000234125 + (0.0000000876741 * Temp * Temp ))* Temp );
Temp = Temp - 273.15;// Converierte de Kelvin a Celsius
//Para convertir Celsius a Farenheith esriba en esta linea: Temp = (Temp * 9.0)/ 5.0 + 32.0; 
return Temp;
}

 
void loop() {             
int val;
double temp;
val=analogRead(0);//Lee el valor del pin analogo 0 y lo mantiene como val
temp=Thermister(val);//Realiza la conversión del valor analogo a grados Celsius
envia.concat(temp);
envia.concat(" ");//espacio al final para el software "serial sensor plot"
Serial.println(envia);
envia="";//limpiamos
delay(1000);//Este tiempo debe ser el mismo en Android o Windows
}
