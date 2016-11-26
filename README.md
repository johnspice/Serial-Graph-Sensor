# Serial-Graph-Sensor
Plot Serial Sensor Arduino visual C# "Visual studio"
Creado Para el laboratorio de fenómenos colectivos de la facultad de ciencias UNAM.

Este software es una versión para Windows funcional en Xp,win7,win8,win10 quizá en Xp sea necesario instalar .net framework 4.5. 
Para el guardado en Excel requiere de al menos tener instalado office 2003 o superior. Este proyecto usa las librería “Zedgraph” 
para los paneles de graficado.
Su objetivo es la recepción y graficado de datos vía serial de un arduino (o microchip) con sensores de temperatura presión, campo 
magnético etc…  La intención es que se grafique en tiempo real estos datos. Los datos enviados desde el micro controlador ya sea
arduino o microchip deben venir en una sola cadena separados por espacios. Pueden ser enviados desde 1 hasta 9 datos.
Ejemplo: “3.45 5.0 4.56 23.45” aquí hay 4 datos  que llamamos E0,E1,E2,E3 etc…. Ha funcionado muy bien para lecturas cada 100ms meno a esto provoca errores.
Los datos se envían ya sea por cable USB-serial o por bluetooth usando los módulos HC-05 y Hc-06

//////////   Características de Software:   ///////////
-Ajuste de Colores fondo, cuadricula leyendas
-Títulos de ejes
-escalado automático o fijo
-colores de las graficas
-datos unidos por línea o solo puntos, tamaño de puntos.
-autoguardado de ajustes
-muestra hasta 6 graficas en dos paneles distintos,
-dos ejes Y1, Y2 (derecho izquierdo) configurables.
-Paro automático después de determinados puntos obtenidos, útil para tomar temperaturas o datos por largos tiempos.
- guardado de datos en Excel, guardado de la imagen del gráfico.
-visualización de los datos conforme en tiempo real 

Se anexa un manual rápido en pdf.


