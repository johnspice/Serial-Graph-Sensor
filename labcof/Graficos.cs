using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.IO.Ports;
using ZedGraph;//libreria usada para los paneles de graficado

using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using System.Media;


namespace LabCOF
{
    public partial class Graficos: Form
    {
        string E0s,E1s,E2s,E3s,E4s,E5s,E6s,E7s,E8s; 
        string[] ParteCadenaOptendato;
        string CadenaRs232;//aqui se guarda la cadena con los datos que envia arduino o algun microcontrolador que envia via serial
        decimal E0D,E1D,E2D,E3D,E4D,E5D,E6D,E7D,E8D;
        // variables para eventos del form
        const int WM_SYSCOMMAND = 0x112;
        //const int SC_MINIMIZE = 0xF020;
        const int SC_RESTORE = 0xF120;
        const int SC_MAXIMIZE = 0xF030;
        const int SC_CLOSE = 0xF060;
             
        double XTiempo = 0;// Tiempo
        int i = 0, j = 0,timerReal=0;//contador para los vectores x,y0
        DataTable Tabla = new DataTable(); //Declaramos una variable de tipo DataTable y a su vez la inicializamos para usarla mas tarde.
        DataRow Renglon;//Esta variable de tipo DataRow solo la declaramos y mas adelante la utilizaremos para agregarsela al dataTable que ya declaramos arriba
        double[] x  = new double[10000];//tiempo
        //las siguientes variables guardan los 9 posibles datos que llegran desde el arduino o microcontrolador
        double[] y0 = new double[10000];//D6
        double[] y1  = new double[10000];//D1
        double[] y2 = new double[10000];//D2
        double[] y3 = new double[10000];//D3
        double[] y4 = new double[10000];//D4
        double[] y5 = new double[10000];//D5
        double[] y6 = new double[10000];//D6  
        double[] y7 = new double[10000];//D7
        double[] y8 = new double[10000];//D7
        //variables de graficas para usar al crear archivo EXCEL solo se guardaran los datos que se estan observando en el panel maximo 6
        double[,] P1G1 = new double[2,10000];//pane1 Grafica1 
        double[,] P1G2 = new double[2, 10000];//panel2 Grafica 2
        double[,] P1G3 = new double[2, 10000];
        double[,] P2G1 = new double[2, 10000];
        double[,] P2G2 = new double[2, 10000];
        double[,] P2G3 = new double[2, 10000];      
        // variables graficas
        public GraphPane myPane,myPane2;//los 2 paneles de graficado
        public LineItem myCurve,myCurve2;//2 curvas una para cada panel
        PointPairList PGraph0;//pares de puntos para cada grafica pueden ser maximo 6 3 por panel de graficado
        PointPairList PGraph1;
        PointPairList PGraph2;
        PointPairList PGraph3;
        PointPairList PGraph4;
        PointPairList PGraph5;
        //dato que se envia cada cierto tiempo
        string DatoAenviar="a";  
        
 



        public Graficos()
        {           
            InitializeComponent();
            /* se obtiene la resolucion de pantalla
            Screen screen = Screen.PrimaryScreen;
            AnchoX = screen.Bounds.Width;
            AltoY = screen.Bounds.Height;*/
            
            button3.Enabled = false;
            zedGraphControl1.Size = new System.Drawing.Size(827, 523);
            zedGraphControl1.GraphPane.CurveList.Clear();// SE BORRA TODO  en el panel 1      
            myPane = zedGraphControl1.GraphPane;
            myPane2 = zedGraphControl2.GraphPane;
            RecargarFondo();
            RecargarFondo2();

            if (Properties.Graficos.Default.Panel2Activo)
            {
                zedGraphControl1.Size = new System.Drawing.Size(827, 262);
                zedGraphControl2.GraphPane.CurveList.Clear();// SE BORRA TODO  en el panel 2 si es que esta activo
                            
            }
           
 
            // se carga el dGVtabla
            //Le agregamos columnas a la variable Tabla que es de tipo DataTable
            Tabla.Columns.Add(new DataColumn("Seg"));
            Tabla.Columns.Add(new DataColumn(comboBox1.Text));
            Tabla.Columns.Add(new DataColumn(comboBox2.Text));

          
          

            //MENSAJE DE ULTIMA CONEXION
            label5.Text = " Ultima conexión a: "; 
            label3.Text=" ->   "+ Properties.Settings.Default.NombrePuerto;// se carga en el label el nombre del ultimo com usado
            label3.ForeColor = Color.Red;

            //VARIABLE PARA ACTIVAR EL CAMBIO DE TAMAÑO DE VENTANA
            //this.ResizeEnd += new EventHandler(SizeVentanaModificado);
        }





       
        private void ConectBluetoothCOM()
        {
            CadenaRs232 = "";
            serialPort1.PortName = Properties.Settings.Default.NombrePuerto;
            if (!serialPort1.IsOpen)
            {
                try { serialPort1.Open();
                      
                      button3.Enabled = true;
                      button2.Text = "Pausar";
                      // se inicializa el contador de tiempo
                      timer1.Interval = Properties.SettingsBluetooth.Default.TimeRcepcion;
                      timer1.Enabled = true;
                      timer2TiempoReal.Enabled = true;
                      timer2TiempoReal.Interval = Properties.SettingsBluetooth.Default.TimeRcepcion;                                      
                    }// ABRE LA CONEXION CON EL PUERTO Y SE CREA UNA EXEPCION
                catch (System.Exception)// aqui puede declararse una variable string "ex" para usar en el mensaje siguiente
                {
                      timer1.Enabled = false;// se desactiva el timer1
                      ErrorDeConexion(); }//SI SE COLOCA "ex.ToString()" Mostrara un cuadro de dialogo normal que muestra el error
            }
        }







        private void Actualizar()
        {
            
            serialPort1.Write(DatoAenviar);   
            // se lee el dato recibido
            CadenaRs232 = serialPort1.ReadExisting();serialPort1.ReadLine();
            if (CadenaRs232 == "") { label3.Text = "No Existen Datos En el Puerto"; }// si no lee nada no hara nada hasta que exista alguna cadena que envie el ARDUINO

            else
            {
                try { ParteCadenaOptendato = SENSORES(CadenaRs232);
                    label5.Text = "Conectado a: " + serialPort1.PortName+" . . . " ; label5.ForeColor = Color.Green; label3.Text = "";
                }
                catch { label3.Text = "Los datos no vienen separados por espacios"; }         
                        //Tiempo
                        double[] XX = new double[i + 1];//tiempo 
                        if (i == 0) { XTiempo = 0; }
                        else { XTiempo = XTiempo + (Convert.ToDouble(Properties.SettingsBluetooth.Default.TimeRcepcion) / 1000); }
                        decimal Tiempo = decimal.Round(Convert.ToDecimal(XTiempo), 2);
                        x[i] = Convert.ToDouble(Tiempo);
                       
                     // se definen nuevas variables a usar localmente, para cada entrada
                                                          double[] YY0 = new double[i + 1];
                        double[] XX1 = new double[i + 1]; double[] YY1 = new double[i + 1];
                        double[] XX2 = new double[i + 1]; double[] YY2 = new double[i + 1];
                        double[] XX3 = new double[i + 1]; double[] YY3 = new double[i + 1];
                        double[] XX4 = new double[i + 1]; double[] YY4 = new double[i + 1];
                        double[] XX5 = new double[i + 1]; double[] YY5 = new double[i + 1];
                        double[] XX6 = new double[i + 1]; double[] YY6 = new double[i + 1];
                        double[] XX7 = new double[i + 1]; double[] YY7 = new double[i + 1];
                        double[] XX8 = new double[i + 1]; double[] YY8 = new double[i + 1];

                //se toma el valor correspondiente del vector cadena "PartreCadenaOptendato" se convierte a decimal para poder redondear
                // luego se convierte a un double asignandolo a "y#" que es variable global  
                try//0
                {
                    E0s = ParteCadenaOptendato[0]; decimal.TryParse(E0s, out E0D); decimal.Round(E0D, 2);
                    //y0[i] =6.6*(Math.Log( Convert.ToDouble(E0D)))-40;
                    y0[i] = Convert.ToDouble(E0D);
                }
                catch {// label3.Text = "Error En la entrada 0, no se recibe cadena de numeros."; label3.ForeColor = Color.Red; }//quiere decir que posiblemente hay texto en la cadena y no numeros por eso no se puede redondear ni convertir a decimal
                }
                    try// 1
                        {
                            E1s = ParteCadenaOptendato[1]; decimal.TryParse(E1s, out E1D); decimal.Round(E1D, 2);
                            y1[i] = Convert.ToDouble(E1D);
                            for (int k = 0; k <= i; k++) { YY1[k] = y1[k]; }
                        }
                        catch { //label3.Text = "Error En la entrada 1, no se recibe cadena de numeros."; label3.ForeColor = Color.Red; 
                }
                        try // 2
                        {
                            E2s = ParteCadenaOptendato[2]; decimal.TryParse(E2s, out E2D); decimal.Round(E2D, 2);
                            y2[i] = Convert.ToDouble(E2D);
                            for (int k = 0; k <= i; k++) { YY2[k] = y2[k]; }
                        }
                        catch {// label3.Text = "Error En la entrada 2, no se recibe cadena de numeros."; label3.ForeColor = Color.Red; 
                }
                        try // 3
                        {
                            E3s = ParteCadenaOptendato[3]; decimal.TryParse(E3s, out E3D); decimal.Round(E3D, 2);
                            y3[i] = Convert.ToDouble(E3D);
                            for (int k = 0; k <= i; k++) { YY3[k] = y3[k]; }
                        }
                        catch {// label3.Text = "Error En la entrada 3, no se recibe cadena de numeros."; label3.ForeColor = Color.Red;
                }
                        try// 4
                        {
                            E4s = ParteCadenaOptendato[4]; decimal.TryParse(E4s, out E4D); decimal.Round(E4D, 2);
                            y4[i] = Convert.ToDouble(E4D);
                            for (int k = 0; k <= i; k++) { YY4[k] = y4[k]; }
                        }
                        catch {// label3.Text = "Error En la entrada 4, no se recibe cadena de numeros."; label3.ForeColor = Color.Red; 
                }
                        try// 5
                        {
                            E5s = ParteCadenaOptendato[7]; decimal.TryParse(E5s, out E5D); decimal.Round(E5D, 2);
                            y5[i] = Convert.ToDouble(E5D);
                            for (int k = 0; k <= i; k++) { YY5[k] = y5[k]; }
                        }
                        catch {// label3.Text = "Error En la entrada 5, no se recibe cadena de numeros."; label3.ForeColor = Color.Red; 
                }
                        try// 6
                        {
                            E6s = ParteCadenaOptendato[6]; decimal.TryParse(E6s, out E6D); decimal.Round(E6D, 2);
                            y6[i] = Convert.ToDouble(E6D);
                            for (int k = 0; k <= i; k++) { YY6[k] = y6[k]; }
                        }
                        catch {// label3.Text = "Error En la entrada 6, no se recibe cadena de numeros."; label3.ForeColor = Color.Red; 
                }

                        try// 7
                        {
                            E7s = ParteCadenaOptendato[7]; decimal.TryParse(E7s, out E7D); decimal.Round(E7D, 2);
                            y7[i] = Convert.ToDouble(E7D);
                            for (int k = 0; k <= i; k++) { YY7[k] = y7[k]; }
                        }
                        catch {// label3.Text = "Error En la entrada 7, no se recibe cadena de numeros."; label3.ForeColor = Color.Red; 
                }
                        try// 8
                        {
                            E8s = ParteCadenaOptendato[8]; decimal.TryParse(E8s, out E8D); decimal.Round(E8D, 2);
                            y8[i] = Convert.ToDouble(E8D);
                            for (int k = 0; k <= i; k++) { YY8[k] = y8[k]; }
                        }
                        catch { //label3.Text = "Error En la entrada 8, no se recibe cadena de numeros."; label3.ForeColor = Color.Red;
                }






                        //__________________________ ELEMENTO 0 ACTIVO

                        if (Properties.Graficos.Default.E0activo)
                          {
                              try
                              {
                                  switch (Properties.Graficos.Default.E0x)
                                  {
                                      case "tiempo":
                                          for (int k = 0; k <= i; k++) { XX[k] = x[k]; P1G1[0, k] = x[k]; }
                                          break;
                                      case "E0":
                                          for (int k = 0; k <= i; k++) { XX[k] = y0[k]; P1G1[0, k] = y0[k]; }
                                          break;
                                      case "E1":
                                          for (int k = 0; k <= i; k++) { XX[k] = y1[k]; P1G1[0, k] = y1[k]; }
                                          break;
                                      case "E2":
                                          for (int k = 0; k <= i; k++) { XX[k] = y2[k]; P1G1[0, k] = y2[k]; }
                                          break;
                                      case "E3":
                                          for (int k = 0; k <= i; k++) { XX[k] = y3[k]; P1G1[0, k] = y3[k]; }
                                          break;
                                      case "E4":
                                          for (int k = 0; k <= i; k++) { XX[k] = y4[k]; P1G1[0, k] = y4[k]; }
                                          break;
                                      case "E5":
                                          for (int k = 0; k <= i; k++) { XX[k] = y5[k]; P1G1[0, k] = y5[k]; }
                                          break;
                                      case "E6":
                                          for (int k = 0; k <= i; k++) { XX[k] = y6[k]; P1G1[0, k] = y6[k]; }
                                          break;
                                      case "E7":
                                          for (int k = 0; k <= i; k++) { XX[k] = y7[k]; P1G1[0, k] = y7[k]; }
                                          break;
                                      case "E8":
                                          for (int k = 0; k <= i; k++) { XX[k] = y8[k]; P1G1[0, k] = y8[k]; }
                                          break;
                                      default:
                                          for (int k = 0; k <= i; k++) { XX[k] = x[k]; P1G1[0, k] =x[k]; }
                                          break;
                                  }
                                  switch (Properties.Graficos.Default.E0y)
                                  {                                    
                                      case "E0":
                                          for (int k = 0; k <= i; k++) { YY0[k] = y0[k]; P1G1[1, k] = y0[k]; }
                                          break;
                                      case "E1":
                                          for (int k = 0; k <= i; k++) { YY0[k] = y1[k]; P1G1[1, k] = y1[k]; }
                                          break;
                                      case "E2":
                                          for (int k = 0; k <= i; k++) { YY0[k] = y2[k]; P1G1[1, k] = y2[k]; }
                                          break;
                                      case "E3":
                                          for (int k = 0; k <= i; k++) { YY0[k] = y3[k]; P1G1[1, k] = y3[k]; }
                                          break;
                                      case "E4":
                                          for (int k = 0; k <= i; k++) { YY0[k] = y4[k]; P1G1[1, k] = y4[k]; }
                                          break;
                                      case "E5":
                                          for (int k = 0; k <= i; k++) { YY0[k] = y5[k]; P1G1[1, k] = y5[k]; }
                                          break;
                                      case "E6":
                                          for (int k = 0; k <= i; k++) { YY0[k] = y6[k]; P1G1[1, k] = y6[k]; }
                                          break;
                                      case "E7":
                                          for (int k = 0; k <= i; k++) { YY0[k] = y7[k]; P1G1[1, k] = y7[k]; }
                                          break;
                                      case "E8":
                                          for (int k = 0; k <= i; k++) { YY0[k] = y8[k]; P1G1[1, k] = y8[k]; }
                                          break;
                                      default:
                                          for (int k = 0; k <= i; k++) { YY0[k] = y0[k]; P1G1[1, k] = y0[k]; }
                                          break;
                                  }
                                  
                                  PGraph0 = new PointPairList(XX, YY0);
                              }
                              catch { label3.Text = "Error P(X,Y) Grafico 1 "; label3.ForeColor = Color.Red; }
                            
                          }
                        //__________________________ ELEMENTO 1 ACTIVO
                        if (Properties.Graficos.Default.E1activo)
                        {                     
                            try
                            {
                                switch (Properties.Graficos.Default.E1x)
                                {
                                    case "tiempo":
                                        for (int k = 0; k <= i; k++) { XX1[k] = x[k]; P1G2[0, k] = x[k]; }
                                        break;
                                    case "E0":
                                        for (int k = 0; k <= i; k++) { XX1[k] = y0[k]; P1G2[0, k] = y0[k]; }
                                        break;
                                    case "E1":
                                        for (int k = 0; k <= i; k++) { XX1[k] = y1[k]; P1G2[0, k] = y1[k]; }
                                        break;
                                    case "E2":
                                        for (int k = 0; k <= i; k++) { XX1[k] = y2[k]; P1G2[0, k] = y2[k]; }
                                        break;
                                    case "E3":
                                        for (int k = 0; k <= i; k++) { XX1[k] = y3[k]; P1G2[0, k] = y3[k]; }
                                        break;
                                    case "E4":
                                        for (int k = 0; k <= i; k++) { XX1[k] = y4[k]; P1G2[0, k] = y4[k]; }
                                        break;
                                    case "E5":
                                        for (int k = 0; k <= i; k++) { XX1[k] = y5[k]; P1G2[0, k] = y5[k]; }
                                        break;
                                    case "E6":
                                        for (int k = 0; k <= i; k++) { XX1[k] = y6[k]; P1G2[0, k] = y6[k]; }
                                        break;
                                    case "E7":
                                        for (int k = 0; k <= i; k++) { XX1[k] = y7[k]; P1G2[0, k] = y7[k]; }
                                        break;
                                    case "E8":
                                        for (int k = 0; k <= i; k++) { XX1[k] = y8[k]; P1G2[0, k] = y8[k]; }
                                        break;
                                    default:
                                        for (int k = 0; k <= i; k++) { XX1[k] = x[k]; P1G2[0, k] = x[k]; }
                                        break;
                                }
                                switch (Properties.Graficos.Default.E1y)
                                {
                                    case "E0":
                                        for (int k = 0; k <= i; k++) { YY1[k] = y0[k]; P1G2[1, k] = y0[k]; }
                                        break;
                                    case "E1":
                                        for (int k = 0; k <= i; k++) { YY1[k] = y1[k]; P1G2[1, k] = y1[k]; }
                                        break;
                                    case "E2":
                                        for (int k = 0; k <= i; k++) { YY1[k] = y2[k]; P1G2[1, k] = y2[k]; }
                                        break;
                                    case "E3":
                                        for (int k = 0; k <= i; k++) { YY1[k] = y3[k]; P1G2[1, k] = y3[k]; }
                                        break;
                                    case "E4":
                                        for (int k = 0; k <= i; k++) { YY1[k] = y4[k]; P1G2[1, k] = y4[k]; }
                                        break;
                                    case "E5":
                                        for (int k = 0; k <= i; k++) { YY1[k] = y5[k]; P1G2[1, k] = y5[k]; }
                                        break;
                                    case "E6":
                                        for (int k = 0; k <= i; k++) { YY1[k] = y6[k]; P1G2[1, k] = y6[k]; }
                                        break;
                                    case "E7":
                                        for (int k = 0; k <= i; k++) { YY1[k] = y7[k]; P1G2[1, k] = y7[k]; }
                                        break;
                                    case "E8":
                                        for (int k = 0; k <= i; k++) { YY1[k] = y8[k]; P1G2[1, k] = y8[k]; }
                                        break;
                                    default:
                                        for (int k = 0; k <= i; k++) { YY1[k] = y1[k]; P1G2[1, k] = y1[k]; }
                                        break;
                                }
                                
                                PGraph1 = new PointPairList(XX1, YY1);
                            }
                            catch { label3.Text = "Error P(X,Y) Grafico 2"; label3.ForeColor = Color.Red; }
                        }

                        //__________________________ ELEMENTO 2 ACTIVO
                        if (Properties.Graficos.Default.E2activo)
                        {      
                            try
                            {
                                switch (Properties.Graficos.Default.E2x)
                                {
                                    case "tiempo":
                                        for (int k = 0; k <= i; k++) { XX2[k] = x[k]; P1G3[0, k] = x[k]; }
                                        break;
                                    case "E0":
                                        for (int k = 0; k <= i; k++) { XX2[k] = y0[k]; P1G3[0, k] = y0[k]; }
                                        break;
                                    case "E1":
                                        for (int k = 0; k <= i; k++) { XX2[k] = y1[k]; P1G3[0, k] = y1[k]; }
                                        break;
                                    case "E2":
                                        for (int k = 0; k <= i; k++) { XX2[k] = y2[k]; P1G3[0, k] = y2[k]; }
                                        break;
                                    case "E3":
                                        for (int k = 0; k <= i; k++) { XX2[k] = y3[k]; P1G3[0, k] = y3[k]; }
                                        break;
                                    case "E4":
                                        for (int k = 0; k <= i; k++) { XX2[k] = y4[k]; P1G3[0, k] = y4[k]; }
                                        break;
                                    case "E5":
                                        for (int k = 0; k <= i; k++) { XX2[k] = y5[k]; P1G3[0, k] = y5[k]; }
                                        break;
                                    case "E6":
                                        for (int k = 0; k <= i; k++) { XX2[k] = y6[k]; P1G3[0, k] = y6[k]; }
                                        break;
                                    case "E7":
                                        for (int k = 0; k <= i; k++) { XX2[k] = y7[k]; P1G3[0, k] = y7[k]; }
                                        break;
                                    case "E8":
                                        for (int k = 0; k <= i; k++) { XX2[k] = y8[k]; P1G3[0, k] = y8[k]; }
                                        break;
                                    default:
                                        for (int k = 0; k <= i; k++) { XX2[k] = x[k]; P1G3[0, k] = x[k]; }
                                        break;
                                }
                                switch (Properties.Graficos.Default.E2y)
                                {
                                    case "E0":
                                        for (int k = 0; k <= i; k++) { YY2[k] = y0[k]; P1G3[1, k] = y0[k]; }
                                        break;
                                    case "E1":
                                        for (int k = 0; k <= i; k++) { YY2[k] = y1[k]; P1G3[1, k] = y1[k]; }
                                        break;
                                    case "E2":
                                        for (int k = 0; k <= i; k++) { YY2[k] = y2[k]; P1G3[1, k] = y2[k]; }
                                        break;
                                    case "E3":
                                        for (int k = 0; k <= i; k++) { YY2[k] = y3[k]; P1G3[1, k] = y3[k]; }
                                        break;
                                    case "E4":
                                        for (int k = 0; k <= i; k++) { YY2[k] = y4[k]; P1G3[1, k] = y4[k]; }
                                        break;
                                    case "E5":
                                        for (int k = 0; k <= i; k++) { YY2[k] = y5[k]; P1G3[1, k] = y5[k]; }
                                        break;
                                    case "E6":
                                        for (int k = 0; k <= i; k++) { YY2[k] = y6[k]; P1G3[1, k] = y6[k]; }
                                        break;
                                    case "E7":
                                        for (int k = 0; k <= i; k++) { YY2[k] = y7[k]; P1G3[1, k] = y7[k]; }
                                        break;
                                    case "E8":
                                        for (int k = 0; k <= i; k++) { YY2[k] = y8[k]; P1G3[1, k] = y8[k]; }
                                        break;
                                    default:
                                        for (int k = 0; k <= i; k++) { YY2[k] = y2[k]; P1G3[1, k] = y2[k]; }
                                        break;
                                }
                                PGraph2 = new PointPairList(XX2, YY2);
                            }
                            catch { label3.Text = "Error P(X,Y) Grafico 3 "; label3.ForeColor = Color.Red; }
                        }

                        //__________________________ ELEMENTO 3 ACTIVO
                        if (Properties.Graficos.Default.E3activo)
                        {                                                    
                            try
                            {
                                switch (Properties.Graficos.Default.E3x)
                                {
                                    case "tiempo":
                                        for (int k = 0; k <= i; k++) { XX3[k] = x[k]; P2G1[0, k] = x[k]; }
                                        break;
                                    case "E0":
                                        for (int k = 0; k <= i; k++) { XX3[k] = y0[k]; P2G1[0, k] = y0[k]; }
                                        break;
                                    case "E1":
                                        for (int k = 0; k <= i; k++) { XX3[k] = y1[k]; P2G1[0, k] = y1[k]; }
                                        break;
                                    case "E2":
                                        for (int k = 0; k <= i; k++) { XX3[k] = y2[k]; P2G1[0, k] = y2[k]; }
                                        break;
                                    case "E3":
                                        for (int k = 0; k <= i; k++) { XX3[k] = y3[k]; P2G1[0, k] = y3[k]; }
                                        break;
                                    case "E4":
                                        for (int k = 0; k <= i; k++) { XX3[k] = y4[k]; P2G1[0, k] = y4[k]; }
                                        break;
                                    case "E5":
                                        for (int k = 0; k <= i; k++) { XX3[k] = y5[k]; P2G1[0, k] = y5[k]; }
                                        break;
                                    case "E6":
                                        for (int k = 0; k <= i; k++) { XX3[k] = y6[k]; P2G1[0, k] = y6[k]; }
                                        break;
                                    case "E7":
                                        for (int k = 0; k <= i; k++) { XX3[k] = y7[k]; P2G1[0, k] = y7[k]; }
                                        break;
                                    case "E8":
                                        for (int k = 0; k <= i; k++) { XX3[k] = y8[k]; P2G1[0, k] = y8[k]; }
                                        break;
                                    default:
                                        for (int k = 0; k <= i; k++) { XX3[k] = x[k]; P2G1[0, k] = x[k]; }
                                        break;
                                }
                                switch (Properties.Graficos.Default.E3y)
                                {
                                    case "E0":
                                        for (int k = 0; k <= i; k++) { YY3[k] = y0[k]; P2G1[1, k] = y0[k]; }
                                        break;
                                    case "E1":
                                        for (int k = 0; k <= i; k++) { YY3[k] = y1[k]; P2G1[1, k] = y1[k]; }
                                        break;
                                    case "E2":
                                        for (int k = 0; k <= i; k++) { YY3[k] = y2[k]; P2G1[1, k] = y2[k]; }
                                        break;
                                    case "E3":
                                        for (int k = 0; k <= i; k++) { YY3[k] = y3[k]; P2G1[1, k] = y3[k]; }
                                        break;
                                    case "E4":
                                        for (int k = 0; k <= i; k++) { YY3[k] = y4[k]; P2G1[1, k] = y4[k]; }
                                        break;
                                    case "E5":
                                        for (int k = 0; k <= i; k++) { YY3[k] = y5[k]; P2G1[1, k] = y5[k]; }
                                        break;
                                    case "E6":
                                        for (int k = 0; k <= i; k++) { YY3[k] = y6[k]; P2G1[1, k] = y6[k]; }
                                        break;
                                    case "E7":
                                        for (int k = 0; k <= i; k++) { YY3[k] = y7[k]; P2G1[1, k] = y7[k]; }
                                        break;
                                    case "E8":
                                        for (int k = 0; k <= i; k++) { YY3[k] = y8[k]; P2G1[1, k] = y8[k]; }
                                        break;
                                    default:
                                        for (int k = 0; k <= i; k++) { YY3[k] = y3[k]; P2G1[1, k] = y3[k]; }
                                        break;
                                }
                                PGraph3 = new PointPairList(XX3, YY3);
                            }
                            catch { label3.Text = "Error P(X,Y) Grafico 4"; label3.ForeColor = Color.Red; }
                        }

                        //__________________________ ELEMENTO 4 ACTIVO 
                        if (Properties.Graficos.Default.E4activo)
                        {
                            try
                            {
                                switch (Properties.Graficos.Default.E4x)
                                {
                                    case "tiempo":
                                        for (int k = 0; k <= i; k++) { XX4[k] = x[k]; P2G2[0, k] = x[k]; }
                                        break;
                                    case "E0":
                                        for (int k = 0; k <= i; k++) { XX4[k] = y0[k]; P2G2[0, k] = y0[k]; }
                                        break;
                                    case "E1":
                                        for (int k = 0; k <= i; k++) { XX4[k] = y1[k]; P2G2[0, k] = y1[k]; }
                                        break;
                                    case "E2":
                                        for (int k = 0; k <= i; k++) { XX4[k] = y2[k]; P2G2[0, k] = y2[k]; }
                                        break;
                                    case "E3":
                                        for (int k = 0; k <= i; k++) { XX4[k] = y3[k]; P2G2[0, k] = y3[k]; }
                                        break;
                                    case "E4":
                                        for (int k = 0; k <= i; k++) { XX4[k] = y4[k]; P2G2[0, k] = y4[k]; }
                                        break;
                                    case "E5":
                                        for (int k = 0; k <= i; k++) { XX4[k] = y5[k]; P2G2[0, k] = y5[k]; }
                                        break;
                                    case "E6":
                                        for (int k = 0; k <= i; k++) { XX4[k] = y6[k]; P2G2[0, k] = y6[k]; }
                                        break;
                                    case "E7":
                                        for (int k = 0; k <= i; k++) { XX4[k] = y7[k]; P2G2[0, k] = y7[k]; }
                                        break;
                                    case "E8":
                                        for (int k = 0; k <= i; k++) { XX4[k] = y8[k]; P2G2[0, k] = y8[k]; }
                                        break;
                                    default:
                                        for (int k = 0; k <= i; k++) { XX4[k] = x[k]; P2G2[0, k] = x[k]; }
                                        break;
                                }
                                switch (Properties.Graficos.Default.E4y)
                                {
                                    case "E0":
                                        for (int k = 0; k <= i; k++) { YY4[k] = y0[k]; P2G2[1, k] = y0[k]; }
                                        break;
                                    case "E1":
                                        for (int k = 0; k <= i; k++) { YY4[k] = y1[k]; P2G2[1, k] = y1[k]; }
                                        break;
                                    case "E2":
                                        for (int k = 0; k <= i; k++) { YY4[k] = y2[k]; P2G2[1, k] = y2[k]; }
                                        break;
                                    case "E3":
                                        for (int k = 0; k <= i; k++) { YY4[k] = y3[k]; P2G2[1, k] = y3[k]; }
                                        break;
                                    case "E4":
                                        for (int k = 0; k <= i; k++) { YY4[k] = y4[k]; P2G2[1, k] = y4[k]; }
                                        break;
                                    case "E5":
                                        for (int k = 0; k <= i; k++) { YY4[k] = y5[k]; P2G2[1, k] = y5[k]; }
                                        break;
                                    case "E6":
                                        for (int k = 0; k <= i; k++) { YY4[k] = y6[k]; P2G2[1, k] = y6[k]; }
                                        break;
                                    case "E7":
                                        for (int k = 0; k <= i; k++) { YY4[k] = y7[k]; P2G2[1, k] = y7[k]; }
                                        break;
                                    case "E8":
                                        for (int k = 0; k <= i; k++) { YY4[k] = y8[k]; P2G2[1, k] = y8[k]; }
                                        break;
                                    default:
                                        for (int k = 0; k <= i; k++) { YY4[k] = y4[k]; P2G2[1, k] = y4[k]; }
                                        break;
                                }
                                PGraph4 = new PointPairList(XX4, YY4);
                            }
                            catch { label3.Text = "Error P(X,Y) Grafico 5"; label3.ForeColor = Color.Red; }
                        }

                        //__________________________ ELEMENTO 5 ACTIVO
                        if (Properties.Graficos.Default.E5activo)
                        {                                                  
                            try
                            {
                                switch (Properties.Graficos.Default.E5x)
                                {
                                    case "tiempo":
                                        for (int k = 0; k <= i; k++) { XX5[k] = x[k]; P2G3[0, k] = x[k]; }
                                        break;
                                    case "E0":
                                        for (int k = 0; k <= i; k++) { XX5[k] = y0[k]; P2G3[0, k] = y0[k]; }
                                        break;
                                    case "E1":
                                        for (int k = 0; k <= i; k++) { XX5[k] = y1[k]; P2G3[0, k] = y1[k]; }
                                        break;
                                    case "E2":
                                        for (int k = 0; k <= i; k++) { XX5[k] = y2[k]; P2G3[0, k] = y2[k]; }
                                        break;
                                    case "E3":
                                        for (int k = 0; k <= i; k++) { XX5[k] = y3[k]; P2G3[0, k] = y3[k]; }
                                        break;
                                    case "E4":
                                        for (int k = 0; k <= i; k++) { XX5[k] = y4[k]; P2G3[0, k] = y4[k]; }
                                        break;
                                    case "E5":
                                        for (int k = 0; k <= i; k++) { XX5[k] = y5[k]; P2G3[0, k] = y5[k]; }
                                        break;
                                    case "E6":
                                        for (int k = 0; k <= i; k++) { XX5[k] = y6[k]; P2G3[0, k] = y6[k]; }
                                        break;
                                    case "E7":
                                        for (int k = 0; k <= i; k++) { XX5[k] = y7[k]; P2G3[0, k] = y7[k]; }
                                        break;
                                    case "E8":
                                        for (int k = 0; k <= i; k++) { XX5[k] = y8[k]; P2G3[0, k] = y8[k]; }
                                        break;
                                    default:
                                        for (int k = 0; k <= i; k++) { XX5[k] = x[k]; P2G3[0, k] = x[k]; }
                                        break;
                                }
                                switch (Properties.Graficos.Default.E5y)
                                {
                                    case "E0":
                                        for (int k = 0; k <= i; k++) { YY5[k] = y0[k]; P2G3[1, k] = y0[k]; }
                                        break;
                                    case "E1":
                                        for (int k = 0; k <= i; k++) { YY5[k] = y1[k]; P2G3[1, k] = y1[k]; }
                                        break;
                                    case "E2":
                                        for (int k = 0; k <= i; k++) { YY5[k] = y2[k]; P2G3[1, k] = y2[k]; }
                                        break;
                                    case "E3":
                                        for (int k = 0; k <= i; k++) { YY5[k] = y3[k]; P2G3[1, k] = y3[k]; }
                                        break;
                                    case "E4":
                                        for (int k = 0; k <= i; k++) { YY5[k] = y4[k]; P2G3[1, k] = y4[k]; }
                                        break;
                                    case "E5":
                                        for (int k = 0; k <= i; k++) { YY5[k] = y5[k]; P2G3[1, k] = y5[k]; }
                                        break;
                                    case "E6":
                                        for (int k = 0; k <= i; k++) { YY5[k] = y6[k]; P2G3[1, k] = y6[k]; }
                                        break;
                                    case "E7":
                                        for (int k = 0; k <= i; k++) { YY5[k] = y7[k]; P2G3[1, k] = y7[k]; }
                                        break;
                                    case "E8":
                                        for (int k = 0; k <= i; k++) { YY5[k] = y8[k]; P2G3[1, k] = y8[k]; }
                                        break;
                                    default:
                                        for (int k = 0; k <= i; k++) { YY5[k] = y5[k]; P2G3[1, k] = y5[k]; }
                                        break;


                                }
                                PGraph5= new PointPairList(XX5, YY5);
                            }
                            catch { label3.Text = "Error P(X,Y) Grafico 6"; label3.ForeColor = Color.Red; }
                        }
 
                        PintarPunto();//SE EJCUTA EL METODO QUE PINTA LAS GRAFICAS EN CADA LLAMADA.
                        MuestraDatosTabla();
                        i = i + 1; timerReal = i;
                        if (i == Properties.SettingsBluetooth.Default.NumDatos+1) { ParoAutomatico(); }
                        else {  }
                
            }// fin else cadena no vacia
        }






        private void PintarPunto()
        {
            zedGraphControl1.GraphPane.CurveList.Clear();// se borra todo lo queeste en el grafico          
           
            if (Properties.Graficos.Default.Panel2Activo)
            {
                zedGraphControl2.GraphPane.CurveList.Clear();
                             
                if (Properties.Graficos.Default.E3activo)
                {
                    myCurve2 = myPane2.AddCurve(Properties.Graficos.Default.E3name, PGraph3, Properties.Graficos.Default.E3color, Properties.Settings.Default.TipoDePunto);
                    myCurve2.Symbol.Fill = new Fill(Properties.Graficos.Default.E3color);// se rellena el circulo
                    myCurve2.Line.IsVisible = Properties.Settings.Default.UnirConLinea;// solo se ven puntos se ha eliminado las rectas q unen los puntos
                    myCurve2.Symbol.Size = Properties.Settings.Default.sizePunto; //tañano de Los puntos
                    myCurve2.Line.Width = 1.0F;// estos numeritos determinan el grueso de linea
                    if (Properties.Graficos.Default.E3mostrar == "Y2") { myCurve2.IsY2Axis = true; }
                }
                if (Properties.Graficos.Default.E4activo)
                {
                    myCurve2 = myPane2.AddCurve(Properties.Graficos.Default.E4name, PGraph4, Properties.Graficos.Default.E4color, Properties.Settings.Default.TipoDePunto);
                    myCurve2.Symbol.Fill = new Fill(Properties.Graficos.Default.E4color);// se rellena el circulo
                    myCurve2.Line.IsVisible = Properties.Settings.Default.UnirConLinea;// solo se ven puntos se ha eliminado las rectas q unen los puntos
                    myCurve2.Symbol.Size = Properties.Settings.Default.sizePunto; //tañano de Los puntos
                    myCurve2.Line.Width = 1.0F;// estos numeritos determinan el grueso de linea
                    if (Properties.Graficos.Default.E4mostrar == "Y2") { myCurve2.IsY2Axis = true; }
                }
                if (Properties.Graficos.Default.E5activo)
                {
                    myCurve2 = myPane2.AddCurve(Properties.Graficos.Default.E5name, PGraph5, Properties.Graficos.Default.E5color, Properties.Settings.Default.TipoDePunto);
                    myCurve2.Symbol.Fill = new Fill(Properties.Graficos.Default.E5color);// se rellena el circulo
                    myCurve2.Line.IsVisible = Properties.Settings.Default.UnirConLinea;// solo se ven puntos se ha eliminado las rectas q unen los puntos
                    myCurve2.Symbol.Size = Properties.Settings.Default.sizePunto; //tañano de Los puntos
                    myCurve2.Line.Width = 1.0F;// estos numeritos determinan el grueso de linea
                    if (Properties.Graficos.Default.E5mostrar == "Y2") { myCurve2.IsY2Axis = true; }
                }             
                zedGraphControl2.AxisChange();
                zedGraphControl2.Invalidate();
                zedGraphControl2.Refresh();
           
            }
           
                      
            if (Properties.Graficos.Default.E0activo)
            {
                myCurve = myPane.AddCurve(Properties.Graficos.Default.E0name, PGraph0, Properties.Graficos.Default.E0color, Properties.Settings.Default.TipoDePunto);
                myCurve.Symbol.Fill = new Fill(Properties.Graficos.Default.E0color);// se rellena el circulo
                myCurve.Line.IsVisible = Properties.Settings.Default.UnirConLinea;// solo se ven puntos se ha eliminado las rectas q unen los puntos
                myCurve.Symbol.Size = Properties.Settings.Default.sizePunto; //tañano de Los puntos
                myCurve.Line.Width = 1.0F;// estos numeritos determinan el grueso de linea
                if (Properties.Graficos.Default.E0mostrar == "Y2") { myCurve.IsY2Axis = true; }
            }
            if (Properties.Graficos.Default.E1activo)
            {
                myCurve = myPane.AddCurve(Properties.Graficos.Default.E1name, PGraph1, Properties.Graficos.Default.E1color, Properties.Settings.Default.TipoDePunto);
                myCurve.Symbol.Fill = new Fill(Properties.Graficos.Default.E1color);// se rellena el circulo
                myCurve.Line.IsVisible = Properties.Settings.Default.UnirConLinea;// solo se ven puntos se ha eliminado las rectas q unen los puntos
                myCurve.Symbol.Size = Properties.Settings.Default.sizePunto; //tañano de Los puntos
                myCurve.Line.Width = 1.0F;// estos numeritos determinan el grueso de linea
                if (Properties.Graficos.Default.E1mostrar == "Y2") { myCurve.IsY2Axis = true; }
              
            }
            if (Properties.Graficos.Default.E2activo)
            {
                myCurve = myPane.AddCurve(Properties.Graficos.Default.E2name, PGraph2, Properties.Graficos.Default.E2color, Properties.Settings.Default.TipoDePunto);
                myCurve.Symbol.Fill = new Fill(Properties.Graficos.Default.E2color);// se rellena el circulo
                myCurve.Line.IsVisible = Properties.Settings.Default.UnirConLinea;// solo se ven puntos se ha eliminado las rectas q unen los puntos
                myCurve.Symbol.Size = Properties.Settings.Default.sizePunto; //tañano de Los puntos
                myCurve.Line.Width = 1.0F;// estos numeritos determinan el grueso de linea
                if (Properties.Graficos.Default.E2mostrar == "Y2") { myCurve.IsY2Axis = true; }
            }
           // myPane.Y2Axis.IsVisible = true;
            // I add all three functions just to be sure it refeshes the plot. 
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();  
        }



      
  

        //boton conectar
        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Conectar")
            {
                ConectBluetoothCOM();
                
               
            }
            else
            {
                if (button2.Text == "Pausar")
                {
                    timer1.Enabled = false;
                    serialPort1.Close();
                    label5.Text = "Desconectado de: " + serialPort1.PortName;
                    label5.ForeColor = Color.Red;
                    button2.Text = "Continuar";
                }
                else
                {
                    ConectBluetoothCOM();
                    timer1.Interval = Properties.SettingsBluetooth.Default.TimeRcepcion;
                    timer1.Enabled = true;
                    button2.Text = "Pausar";
                    XTiempo = XTiempo + (timerReal - i) * (Convert.ToDouble(Properties.SettingsBluetooth.Default.TimeRcepcion) / 1000);
                }
            }           
        }
        //boton para y borrar datos
        private void button3_Click(object sender, EventArgs e)
        {   serialPort1.Close();
        label5.Text = "Desconectado de: " + serialPort1.PortName;
            label5.ForeColor = Color.Red;
            CadenaRs232 = "";
            button2.Text = "Conectar";
            // borran todos los datos almacenado en de cada en trada E0-E6          
            for (j = 0; j < x.Length; j++) {
                x[j] = 0; y0[j] = 0; y1[i] = 0; y2[i] = 0; y3[i] = 0; y4[i] = 0; y5[i] = 0; y6[i] = 0; y7[i] = 0; y8[i] = 0;               
                }
            //se borra pane 1
            zedGraphControl1.GraphPane.CurveList.Clear();
            RecargarFondo();
            zedGraphControl1.Refresh();
            //se borra panel 2
            zedGraphControl2.GraphPane.CurveList.Clear();
            RecargarFondo2();
            zedGraphControl2.Refresh();

            i = 0;
            XTiempo = 0;
            timerReal = 0;
            timer2TiempoReal.Enabled = false;

            //se borra la tabla 
            Tabla.Clear();
            dGVtabla.DataSource = null;
            button3.Enabled = false;// se bloquea el boton
        }


        // TIMERS     
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen) { Actualizar(); }
            else
            {
                label5.Text = "Estado: Desconectado";
                label5.ForeColor = Color.Red;
            }
        }

        private void timer2TiempoReal_Tick(object sender, EventArgs e)
        {
            timerReal = timerReal + 1;
        }

        //boton tiempo de recepcion (aplicar)
        private void TiempoRec_Click(object sender, EventArgs e)
        {
           Properties.SettingsBluetooth.Default.TimeRcepcion = Convert.ToInt32(numericUpDown1.Value);
           Properties.Settings.Default.Save();
           timer1.Interval =Convert.ToInt32( numericUpDown1.Value);
            DatoAenviar = textBoxEnvia.Text;        
        }

        private void ErrorDeConexion()
        {
            MessageBox.Show("NO SE HA PODIDO CONECTAR CON EL PUERTO " + serialPort1.PortName + "\n vuelva a elegir PUERTO COM en Ajustes");
        }

        // MENU TOOLSTRIP
        private void ajustesDeGraficoToolStripMenuItem_Click(object sender, EventArgs e)
        {        
            Ajustes_de_Grafico a = new Ajustes_de_Grafico();
            a.EventAjustarFondosForm1 += new Ajustes_de_Grafico.Delegado1(RecargarFondos12);
            a.ShowDialog();
            a.Dispose();
        }
        

        public void RecargarFondos12() // este metodo se llama desde el form "Ajustes de Grafico"
        {
            RecargarFondo();
           
            if (Properties.Graficos.Default.Panel2Activo)
            {
                RecargarFondo2();
               
            }   
        }

        

        private void ajustesDePuetoCOMToolStripMenuItem_Click(object sender, EventArgs e)
        {        
            Form1 a = new Form1();
            a.EventSegundoPanelForm1 += new Form1.Delegado1(SeActivoPanel2DesdeAjustes);
            a.ShowDialog();
            a.Dispose();
        }
        public void SeActivoPanel2DesdeAjustes(bool Panel2activo)
        {
            if (Panel2activo)
            {
                zedGraphControl1.Size = new System.Drawing.Size(Width - 194, ((Height+20) - 100) / 2);
                zedGraphControl2.Location = new System.Drawing.Point(193, ((Height+20) - 100) / 2);
                zedGraphControl2.Size = new System.Drawing.Size(Width - 194, ((Height+20) - 100) / 2);
                RecargarFondo2();
                RecargarFondo();
            }
            else
            { zedGraphControl1.Size = new System.Drawing.Size(Width - 194,Height - 78); RecargarFondo(); }
        }

        private void factorctoresDeProporcionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //FactoresProporcion b = new FactoresProporcion();
            //b.ShowDialog();
            //b.Dispose();
        }
       
        private void guardarImagenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //la imagen se guarda donde este el ejecutable 
            myPane.GetImage().Save("grafico 1.png");
            if (Properties.Graficos.Default.Panel2Activo)
            {
                myPane2.GetImage().Save("grafico 2.png");
            }
                MessageBox.Show("La imagen Se Guardo En El Directorio Actual del Programa");
        }
      
        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AjustesCom a = new AjustesCom();
            a.ShowDialog();
            a.Dispose();
        }

        private void guardarDatosExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            serialPort1.Close();
            label5.Text = "Desconectado de: " + serialPort1.PortName;
            label5.ForeColor = Color.Red;
            button2.Text = "Continuar";


            if (i == 0) { MessageBox.Show("Aun no hay Datos para Guardar"); }
            else
            {
                Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

                if (xlApp == null)
                {
                    MessageBox.Show("Microsoft Office Excel No esta Instalado Imposible Guardar datos");
                    return;
                }
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                Excel.Range formatRange;//variable para dar color a columnas



                
                
                

                // si la grafica 1 panel 1 esta activa
                if (Properties.Graficos.Default.E0activo)
                {
                    xlWorkSheet.Cells[1, 1] = Properties.Graficos.Default.E0name;
                    xlWorkSheet.Cells[2, 1] = Properties.Graficos.Default.E0x;                  
                    xlWorkSheet.Cells[2, 2] = (Properties.Graficos.Default.E0y) ;
                    formatRange = xlWorkSheet.get_Range("A1", "B" + (i + 2));
                    formatRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Properties.Graficos.Default.E0color);
                   
                   

                    for (int i1 = 0; i1 < i; i1++)
                    {
                        xlWorkSheet.Cells[i1 + 3, 1] = P1G1[0,i1];// tiempo
                        xlWorkSheet.Cells[i1 + 3, 2] = P1G1[1,i1];
                        
                      
                    }
                
                }
                if (Properties.Graficos.Default.E1activo)
                { 
                    xlWorkSheet.Cells[1, 3] = Properties.Graficos.Default.E1name;
                    xlWorkSheet.Cells[2, 3] = Properties.Graficos.Default.E1x;
                    xlWorkSheet.Cells[2, 4] = Properties.Graficos.Default.E1y;
                    formatRange = xlWorkSheet.get_Range("C1", "D" + (i + 2));
                    formatRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Properties.Graficos.Default.E1color);
                    for (int i1 = 0; i1 < i; i1++)
                    {
                        xlWorkSheet.Cells[i1 + 3, 3] = P1G2[0,i1];
                        xlWorkSheet.Cells[i1 + 3, 4] = P1G2[1, i1];
                    }
                
                }
                if (Properties.Graficos.Default.E2activo)
                { 
                    xlWorkSheet.Cells[1, 5] = Properties.Graficos.Default.E2name;
                    xlWorkSheet.Cells[2, 5] = Properties.Graficos.Default.E2x;
                    xlWorkSheet.Cells[2, 6] = Properties.Graficos.Default.E2y;
                    formatRange = xlWorkSheet.get_Range("E1", "F" + (i + 2));
                    formatRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Properties.Graficos.Default.E2color);
                    for (int i1 = 0; i1 < i; i1++)
                    {
                        xlWorkSheet.Cells[i1 + 3, 5] = P1G3[0, i1];
                        xlWorkSheet.Cells[i1 + 3, 6] = P1G3[1, i1];
                    }
                
                }
                if (Properties.Graficos.Default.E3activo)
                {
                    xlWorkSheet.Cells[1, 7] = Properties.Graficos.Default.E3name;
                    xlWorkSheet.Cells[2, 7] = Properties.Graficos.Default.E3x;
                    xlWorkSheet.Cells[2, 8] = Properties.Graficos.Default.E3y;
                    formatRange = xlWorkSheet.get_Range("G1", "H" + (i + 2));
                    formatRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Properties.Graficos.Default.E3color);
                    for (int i1 = 0; i1 < i; i1++)
                    {
                        xlWorkSheet.Cells[i1 + 3, 7] = P2G1[0, i1];
                        xlWorkSheet.Cells[i1 + 3, 8] = P2G1[1, i1];
                    }
                
                }
                if (Properties.Graficos.Default.E4activo) 
                {
                    xlWorkSheet.Cells[1, 9] = Properties.Graficos.Default.E4name;
                    xlWorkSheet.Cells[2, 9] = Properties.Graficos.Default.E4x;
                    xlWorkSheet.Cells[2, 10] = Properties.Graficos.Default.E4y;
                    formatRange = xlWorkSheet.get_Range("I1", "J" + (i + 2));
                    formatRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Properties.Graficos.Default.E4color);
                    for (int i1 = 0; i1 < i; i1++)
                    {
                        xlWorkSheet.Cells[i1 + 3, 9] = P2G2[0, i1];
                        xlWorkSheet.Cells[i1 + 3, 10] = P2G2[1, i1];
                    }
                
                }
                if (Properties.Graficos.Default.E5activo) 
                {
                    xlWorkSheet.Cells[1, 11] = Properties.Graficos.Default.E5name;
                    xlWorkSheet.Cells[2, 11] = Properties.Graficos.Default.E5x;
                    xlWorkSheet.Cells[2, 12] = Properties.Graficos.Default.E5y;
                    formatRange = xlWorkSheet.get_Range("K1", "L" + (i + 2));
                    formatRange.Font.Color = System.Drawing.ColorTranslator.ToOle(Properties.Graficos.Default.E5color);
                    for (int i1 = 0; i1 < i; i1++)
                    {
                        xlWorkSheet.Cells[i1 + 3, 11] = P2G3[0, i1];
                        xlWorkSheet.Cells[i1 + 3, 12] = P2G3[1, i1];
                    }
                }
               // if (Properties.Graficos.Default.E6activo) { xlWorkSheet.Cells[1, 8] = Properties.Graficos.Default.E6name; }
                

               
                try
                {
                    saveFileDialog1.ShowDialog();
                    xlWorkBook.SaveAs(saveFileDialog1.FileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    xlWorkBook.Close(true, misValue, misValue);
                    xlApp.Quit();
                }
                //"d:\\csharp-Excel.xls"
                catch { MessageBox.Show("No se Guardado Ningun Dato"); }
            }
        }

        private void guardarDatostxtOdatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            serialPort1.Close();
            label5.Text = "Desconectado de: " + serialPort1.PortName;
            label5.ForeColor = Color.Red;
            button2.Text = "Continuar";
            string renglon = "";


            if (i == 0) { MessageBox.Show("Aun no hay Datos para Guardar"); }
            else
            {

                if (Properties.Graficos.Default.E5activo)
                {
                   
                    for (int i1 = 0; i1 < i; i1++)
                    {
                     //   xlWorkSheet.Cells[i1 + 3, 11] = P2G3[0, i1];
                       // xlWorkSheet.Cells[i1 + 3, 12] = P2G3[1, i1];
                    }
                }

                try
                {
                    saveFileDialog1.ShowDialog();
                   // xlWorkBook.SaveAs(saveFileDialog1.FileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                   
                }
                //"d:\\csharp-Excel.xls"
                catch { MessageBox.Show("No se Guardado Ningun Dato"); }

            }

         }

        public string[] SENSORES(string cadena)// require de una cadena con espacio al final ("334 4445 444 5.67 ")
        {
            int i, j = 0, k = 0, l = 0;
            // contador de espacios en blanco_________________
           
            cadena = cadena + " ";
            for (i = 0; i < cadena.Length; i++)
            {
                if (cadena.Substring(i, 1) == " ") l = l + 1;
            }//_______________________________________________

            
            string[] Datos = new string[l];
            for (i = 0; i < cadena.Length; i++)
            {
                if (cadena.Substring(i, 1) == " ")
                {
                    Datos[k] = cadena.Substring(j, i - j);
                    j = i + 1; k = k + 1;
                }
            }
            return Datos;
        }

       
        public void MuestraDatosTabla()
        {
            string d1, d2;

            switch (comboBox1.Text)
            {   case "E0":
                d1 = ""+y0[i];
                break;
                case "E1":
                d1 = ""+y1[i];
                break;
                case "E2":
                d1 = ""+y2[i];
                break;
                case "E3":
                d1 = ""+y3[i];
                break;
                case "E4":
                d1 = ""+y4[i];
                break;
                case "E5":
                d1 = ""+y5[i];
                break;
                case "E6":
                d1 =""+ y6[i];
                break;
                case "E7":
                d1 = "" + y7[i];
                break;
                case "E8":
                d1 = "" + y8[i];
                break;
                
                default:
                d1 = "";
                break;
            }
            switch (comboBox2.Text)
            {
                case "E0":
                    d2 = "" + y0[i];
                    break;
                case "E1":
                    d2 = "" + y1[i];
                    break;
                case "E2":
                    d2 = "" + y2[i];
                    break;
                case "E3":
                    d2 = "" + y3[i];
                    break;
                case "E4":
                    d2 = "" + y4[i];
                    break;
                case "E5":
                    d2 = "" + y5[i];
                    break;
                case "E6":
                    d2 = "" + y6[i];
                    break;
                case "E7":
                    d2 = "" + y7[i];
                    break;
                case "E8":
                    d2 = "" + y8[i];
                    break;
                case "off":
                    d2 = "";
                    break;

                default:
                    d2 = "";
                    break;
            }       
                //Aqui es cuando hacemos uso de la variable renglon, la inicializamos diciendole que va a ser un nuevo renglon de la Tabla que es de tipo DataTable
            Renglon = Tabla.NewRow();
                //Aqui simplemente le agregamos el renglon nuevo con los valores que nosotros querramos, para hacer referencia a cada columna podemos utilizar los indices de cada columna
                Renglon[0] = ""+x[i];
                Renglon[1] = d1;
                Renglon[2] = d2;
                //Aqui simplemente le agregamos el renglon nuevo a la tabla
                Tabla.Rows.Add(Renglon);           
            //Aqui le decimos al dataGridView que tome la tabla y la muestre y Fin
            dGVtabla.DataSource = Tabla;
            dGVtabla.Columns[1].HeaderText = comboBox1.Text;
            dGVtabla.Columns[2].HeaderText = comboBox2.Text;
            dGVtabla.FirstDisplayedScrollingRowIndex = dGVtabla.RowCount-1;
        }

       
        // EL SIGUIENTE METODO SE AUTOEJECUTA CUANDO SE MAXIMIZA, SE RESTAURA O SE CIERRA UNA VENTANA LA VENTANA
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SYSCOMMAND)
            {
                if (m.WParam == (IntPtr)SC_MAXIMIZE)
                {
                    // aqui va codigo que se ejecutara al presionar el boton maximizar                   
                }

               if (m.WParam == (IntPtr)SC_RESTORE)
                {
                   // aqui va el codigo que se ejecutara al presionar el boton minimizar
                }

               if (m.WParam == (IntPtr)SC_CLOSE)// se psaran los hilos que se esatnn ejecutando
               {                             
                   serialPort1.Close(); 
               }

              base.WndProc(ref m);
          }
          else    base.WndProc(ref m);            
        }



        private void SizeVentanaModificado(object sender, EventArgs e)
        {

        // aqui va codigo que se ejecutara al cambiar el tamaño de ventana sin usar maximizar o minimizar
            // requiere activar una variable al inicializar form Graficos.
          
        }


        protected override void OnSizeChanged(EventArgs e)// este metodo se ejecuta despues de maximizar la ventana
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                button2.Location = new Point(Width - 152, Height - 120);
                button3.Location = new Point(Width - 360, Height - 120);
                label5.Location = new Point(285, Height - 123);
                label3.Location = new Point(285, Height - 98);
                dGVtabla.Size = new System.Drawing.Size(183, Height - 210);
                TiempoRec.Location = new Point(172, Height - 119);
                numericUpDown1.Location = new Point(4, Height - 115);
                label1.Location = new Point(2, Height - 132);
                textBoxEnvia.Location = new Point(102,Height-117);
          

                if (Properties.Graficos.Default.Panel2Activo)
                {
                    zedGraphControl1.Size = new System.Drawing.Size(Width - 194, (Height - 110) / 2);
                    zedGraphControl2.Location = new System.Drawing.Point(193, (Height - 110) / 2);
                    zedGraphControl2.Size = new System.Drawing.Size(Width - 194, (Height - 140) / 2);
                }
                else
                { zedGraphControl1.Size = new System.Drawing.Size(Width - 194, (Height - 142)); }
              
            }

            if (this.WindowState == FormWindowState.Normal)
            {

                button2.Location = new Point(874, 526);
                button3.Location = new Point(684, 526);
                label5.Location = new Point(292, 532);
                label3.Location = new Point(292, 548);
                dGVtabla.Size = new System.Drawing.Size(183, 456);
                TiempoRec.Location = new Point(162, 541);
                numericUpDown1.Location = new Point(3, 544);
                label1.Location = new Point(0, 526);
                textBoxEnvia.Location = new Point(95,544);
                

                if (Properties.Graficos.Default.Panel2Activo)
                {
                    zedGraphControl2.Location = new System.Drawing.Point(193, 262);
                    zedGraphControl2.Size = new System.Drawing.Size(827, 262);
                    zedGraphControl1.Size = new System.Drawing.Size(827, 262);
                }
                else
                {
                    zedGraphControl1.Size = new System.Drawing.Size(827, 523);
                   
                    zedGraphControl2.Location = new System.Drawing.Point(193, 262);
                    zedGraphControl2.Size = new System.Drawing.Size(827, 262);
                }

            }
            base.OnSizeChanged(e);
        }


        public void RecargarFondo()
        {   //color del fond principal
            myPane.Fill = new Fill(Properties.Settings.Default.fon1Col1, Properties.Settings.Default.fon1Col2, 50f);
            //color del fondo plano xy
            myPane.Chart.Fill = new Fill(Properties.Settings.Default.fon2Col1, Properties.Settings.Default.fon2Col2, 50F);
            // color de fondo de la leyenda___________________________________________________________________________________________         
            myPane.Legend.Fill = new ZedGraph.Fill(Properties.Settings.Default.fon1Col1);
            myPane.Legend.Position = LegendPos.Bottom;
            myPane.Legend.FontSpec.FontColor = Properties.Settings.Default.tituloscolor;
            if (Properties.Graficos.Default.Panel2Activo)
            {
                myPane.Legend.FontSpec.Size = 18.0f;
                myPane.Title.FontSpec.Size = 27.0f;
                myPane.XAxis.Scale.FontSpec.Size = 20.0f;// Tamaño de letra de los numeros  
                myPane.XAxis.Title.FontSpec.Size = 23.0f;
                myPane.YAxis.Scale.FontSpec.Size = 20.0f;
                myPane.YAxis.Title.FontSpec.Size = 23.0f;
                myPane.Y2Axis.Title.FontSpec.Size = 23.0f;
                myPane.Y2Axis.Scale.FontSpec.Size = 20.0f;
            }
            else
            {
                myPane.Legend.FontSpec.Size = 10.0f;
                myPane.Title.FontSpec.Size = 14.0f;
                myPane.XAxis.Scale.FontSpec.Size = 10.0f;// Tamaño de letra de los numeros  
                myPane.XAxis.Title.FontSpec.Size = 12.0f;
                myPane.YAxis.Scale.FontSpec.Size = 10.0f;
                myPane.YAxis.Title.FontSpec.Size = 12.0f;
                myPane.Y2Axis.Title.FontSpec.Size = 12.0f;
                myPane.Y2Axis.Scale.FontSpec.Size = 10.0f;


            }
           
            //agrega imagen de fondo--------------------------------------------------------------------------------------------------
            //Image image = Bitmap.FromFile(@"c:\fondo2.jpeg");
            //TextureBrush texBrush = new TextureBrush(image);
            //myPane.Fill = new Fill(texBrush);
            //myPane.Chart.Fill.IsVisible = false;
            // TITULO PRINCIPAL_______________________________________________________________________________________________________
            myPane.Title.Text = Properties.Settings.Default.tituloP; // se carga el titulo priniupal
            myPane.Title.FontSpec.FontColor = Properties.Settings.Default.tituloscolor;//color de titulo
            

            //CONFIG EJE X _____________________________________________________________________________________________________________ 
            myPane.XAxis.Title.Text = Properties.Settings.Default.tituloX;
            myPane.XAxis.Title.FontSpec.FontColor = Properties.Settings.Default.ColorejeX;
            myPane.XAxis.Scale.FontSpec.FontColor = Properties.Settings.Default.ColorejeX;
           

            // Autoescalado para eje x y eje y
            if (Properties.Settings.Default.AutoEsc)
            {
                myPane.XAxis.Scale.MinAuto = true;
                myPane.XAxis.Scale.MaxAuto = true;
                myPane.YAxis.Scale.MinAuto = true;
                myPane.YAxis.Scale.MaxAuto = true;
            }
            else// no Autoescaladoi para "x" i "y"
            {
                myPane.XAxis.Scale.MinAuto = false;
                myPane.XAxis.Scale.MaxAuto = false;
                myPane.XAxis.Scale.Min = Properties.Settings.Default.minX; // valor minimo visible
                myPane.XAxis.Scale.Max = Properties.Settings.Default.maxX;// valor maximo visible             
                // myPane.XAxis.Scale.MajorStep = 15;// esta linea marca cada cuando se pone una marca o un numero en el eje x
                //myPane.XAxis.Scale.LabelGap = 0.5F; //desplaza las leyendas de los numeros hacia abajo             

                //CONFIG EJE Y   ___________________________________________________________________________________________________________
                //Autoescalado
                myPane.YAxis.Scale.MinAuto = false;
                myPane.YAxis.Scale.MaxAuto = false;
                myPane.YAxis.Scale.Min = Properties.Settings.Default.minY; // valor minimo visible
                myPane.YAxis.Scale.Max = Properties.Settings.Default.maxY;// valor maximo visible
            }
            myPane.YAxis.Title.Text = Properties.Settings.Default.tituloY;
            myPane.YAxis.Title.FontSpec.FontColor = Properties.Settings.Default.ColorPunto;
            myPane.YAxis.Scale.FontSpec.FontColor = Properties.Settings.Default.ColorPunto;
            

            //CONFIG EJE Y2_____________________________________________________________________________________________________________ 
            myPane.Y2Axis.Title.Text = Properties.Settings.Default.tituloY2;
            myPane.Y2Axis.IsVisible = Properties.Graficos.Default.ActivoY2panel1;
           
            // Make the Y2 axis scale blue
            myPane.Y2Axis.Scale.FontSpec.FontColor = Properties.Settings.Default.ColorPunto2;
            myPane.Y2Axis.Title.FontSpec.FontColor = Properties.Settings.Default.ColorPunto2;
            // turn off the opposite tics so the Y2 tics don't show up on the Y axis
            myPane.Y2Axis.MajorTic.IsOpposite = false;
            myPane.Y2Axis.MinorTic.IsOpposite = false;
            // Display the Y2 axis grid lines
            myPane.Y2Axis.MajorGrid.IsVisible = true;
            // Align the Y2 axis labels so they are flush to the axis
            myPane.Y2Axis.Scale.Align = AlignP.Inside;
            myPane.Y2Axis.Scale.MinAuto = true;
            myPane.Y2Axis.Scale.MaxAuto = true;

            //COLOR Y VISIVILIDAD DEL BORDE INTERIOR----------------------------------------------------------------------------------
            myPane.Chart.Border.Color = Color.White;
            myPane.Chart.Border.IsVisible = true;
            myPane.Chart.Border.Width = 1.0f;
            //          BORDE EXTERIOR
            myPane.Border.Color = Color.White;//borde externo
            myPane.BaseDimension = 10; //comprime todo el grafico
            //myPane.Margin.All = 53.5f;//aleja el grafico de los bordes

            //cuadricula menor (true)------------------------------------------------------------------------------------------------
            myPane.XAxis.MinorGrid.IsVisible = true;
            myPane.YAxis.MinorGrid.IsVisible = true;
            myPane.XAxis.MinorGrid.Color = Properties.Settings.Default.cuadCol1;
            myPane.YAxis.MinorGrid.Color = Properties.Settings.Default.cuadCol1;
            myPane.XAxis.MinorGrid.DashOff = 0;
            myPane.YAxis.MinorGrid.DashOff = 0;
            // cuadricula mayor------------------------------------------------------------------------------------------ -----------
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;
            myPane.XAxis.MajorGrid.Color = Properties.Settings.Default.cuadCol2;
            myPane.YAxis.MajorGrid.Color = Properties.Settings.Default.cuadCol2;
            myPane.XAxis.MajorGrid.DashOff = 0;
            myPane.YAxis.MajorGrid.DashOff = 0;

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
            
        }
  
        // FONDO PARA EL PANEL 2 
        public void RecargarFondo2()
        {   //color del fond principal
            myPane2.Fill = new Fill(Properties.Settings.Default.fon1Col1, Properties.Settings.Default.fon1Col2, 50f);
            //color del fondo plano xy
            myPane2.Chart.Fill = new Fill(Properties.Settings.Default.fon2Col1, Properties.Settings.Default.fon2Col2, 50F);
            // color de fondo de la leyenda___________________________________________________________________________________________         
            myPane2.Legend.Fill = new ZedGraph.Fill(Properties.Settings.Default.fon1Col1);
            myPane2.Legend.Position = LegendPos.Bottom;
            myPane2.Legend.FontSpec.FontColor = Properties.Settings.Default.tituloscolor;
            myPane2.Legend.FontSpec.Size = 18.0f;
            //agrega imagen de fondo--------------------------------------------------------------------------------------------------
            //Image image = Bitmap.FromFile(@"c:\fondo2.jpeg");
            //TextureBrush texBrush = new TextureBrush(image);
            //myPane.Fill = new Fill(texBrush);
            //myPane.Chart.Fill.IsVisible = false;
            // TITULO PRINCIPAL_______________________________________________________________________________________________________
            myPane2.Title.Text =Properties.Settings.Default.tituloP2; // se carga el titulo priniupal
            myPane2.Title.FontSpec.FontColor = Properties.Settings.Default.tituloscolor;//color de titulo
            myPane2.Title.FontSpec.Size = 27.0f;
            //CONFIG EJE X _____________________________________________________________________________________________________________
            myPane2.XAxis.Title.Text = Properties.Settings.Default.tituloX2;
            myPane2.XAxis.Title.FontSpec.Size = 23.0f;
            myPane2.XAxis.Title.FontSpec.FontColor = Properties.Settings.Default.ColorejeX;
            myPane2.XAxis.Scale.FontSpec.FontColor = Properties.Settings.Default.ColorejeX;
            myPane2.XAxis.Scale.FontSpec.Size = 20.0f;// Tamaño de letra de los numeros  

            // Autoescalado para eje x y eje y
            if (Properties.Settings.Default.AutoEsc2)
            {
                myPane2.XAxis.Scale.MinAuto = true;
                myPane2.XAxis.Scale.MaxAuto = true;
                myPane2.YAxis.Scale.MinAuto = true;
                myPane2.YAxis.Scale.MaxAuto = true;
            }
            else// no Autoescaladoi para "x" i "y"
            {
                myPane2.XAxis.Scale.MinAuto = false;
                myPane2.XAxis.Scale.MaxAuto = false;
                myPane2.XAxis.Scale.Min = Properties.Settings.Default.minX2; // valor minimo visible
                myPane2.XAxis.Scale.Max = Properties.Settings.Default.maxX2;// valor maximo visible             
                // myPane.XAxis.Scale.MajorStep = 15;// esta linea marca cada cuando se pone una marca o un numero en el eje x
                //myPane.XAxis.Scale.LabelGap = 0.5F; //desplaza las leyendas de los numeros hacia abajo             

                //CONFIG EJE Y   ___________________________________________________________________________________________________________
                //Autoescalado
                myPane2.YAxis.Scale.MinAuto = false;
                myPane2.YAxis.Scale.MaxAuto = false;
                myPane2.YAxis.Scale.Min = Properties.Settings.Default.minY2; // valor minimo visible
                myPane2.YAxis.Scale.Max = Properties.Settings.Default.maxY2;// valor maximo visible
            }
            myPane2.YAxis.Title.Text =Properties.Settings.Default.tituloY22;
            myPane2.YAxis.Title.FontSpec.Size = 23.0f;
            myPane2.YAxis.Title.FontSpec.FontColor = Properties.Settings.Default.ColorPunto;
            myPane2.YAxis.Scale.FontSpec.FontColor = Properties.Settings.Default.ColorPunto;
            myPane2.YAxis.Scale.FontSpec.Size = 20.0f;



            //CONFIG EJE Y2_____________________________________________________________________________________________________________ 
            myPane2.Y2Axis.Title.Text = Properties.Settings.Default.titutloY222;
            myPane2.Y2Axis.IsVisible = Properties.Graficos.Default.ActivoY2panel2;
            myPane2.Y2Axis.Title.FontSpec.Size = 23.0f;
            // Make the Y2 axis scale blue
            myPane2.Y2Axis.Scale.FontSpec.FontColor = Properties.Settings.Default.ColorPunto2;
            myPane2.Y2Axis.Title.FontSpec.FontColor = Properties.Settings.Default.ColorPunto2;
            // turn off the opposite tics so the Y2 tics don't show up on the Y axis
            myPane2.Y2Axis.MajorTic.IsOpposite = false;
            myPane2.Y2Axis.MinorTic.IsOpposite = false;
            // Display the Y2 axis grid lines
            myPane.Y2Axis.MajorGrid.IsVisible = true;
            // Align the Y2 axis labels so they are flush to the axis
            myPane2.Y2Axis.Scale.Align = AlignP.Inside;
            myPane2.Y2Axis.Scale.MinAuto = true;
            myPane2.Y2Axis.Scale.MaxAuto = true;
            myPane2.Y2Axis.Scale.FontSpec.Size = 20.0f;
            //COLOR Y VISIVILIDAD DEL BORDE INTERIOR----------------------------------------------------------------------------------
            myPane2.Chart.Border.Color = Color.White;
            myPane2.Chart.Border.IsVisible = true;
            myPane2.Chart.Border.Width = 1.0f;
            //          BORDE EXTERIOR
            myPane2.Border.Color = Color.White;//borde externo
            myPane2.BaseDimension = 10; //comprime todo el grafico
            //myPane.Margin.All = 53.5f;//aleja el grafico de los bordes
            //cuadricula menor (true)------------------------------------------------------------------------------------------------
            myPane2.XAxis.MinorGrid.IsVisible = true;
            myPane2.YAxis.MinorGrid.IsVisible = true;
            myPane2.XAxis.MinorGrid.Color = Properties.Settings.Default.cuadCol1;
            myPane2.YAxis.MinorGrid.Color = Properties.Settings.Default.cuadCol1;
            myPane2.XAxis.MinorGrid.DashOff = 0;
            myPane2.YAxis.MinorGrid.DashOff = 0;
            // cuadricula mayor------------------------------------------------------------------------------------------ -----------
            myPane2.XAxis.MajorGrid.IsVisible = true;
            myPane2.YAxis.MajorGrid.IsVisible = true;
            myPane2.XAxis.MajorGrid.Color = Properties.Settings.Default.cuadCol2;
            myPane2.YAxis.MajorGrid.Color = Properties.Settings.Default.cuadCol2;
            myPane2.XAxis.MajorGrid.DashOff = 0;
            myPane2.YAxis.MajorGrid.DashOff = 0;

            // las siguientes tres lineas repintan el fondo
            zedGraphControl2.AxisChange();
            zedGraphControl2.Invalidate();
            zedGraphControl2.Refresh();

           
        }

        public void ParoAutomatico()
        {

            timer1.Enabled = false;
            serialPort1.Close();
            label5.Text = "Desconectado de: " + serialPort1.PortName;
            label5.ForeColor = Color.Red;
            button2.Text = "Continuar";

            try
            {
                SoundPlayer mario = new SoundPlayer(LabCOF.Properties.Resources.gameover);
                mario.Play();
            }
            catch { }

          MessageBox.Show("Se han conseguido con Exito los "+Properties.SettingsBluetooth.Default.NumDatos+ "\nDatos Solicitados ya Puedes Guadarlos");
        
        }
        // metodos q no se usan para nada{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{{
        private void Graficos_Load(object sender, EventArgs e)
        { }
        private void textBox1_TextChanged(object sender, EventArgs e)
        { }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        { }
        private void label1_Click(object sender, EventArgs e)
        { }
        private void label3_Click(object sender, EventArgs e)
        { }
        private void zedGraphControl1_Load(object sender, EventArgs e)
        {
        }
        //}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}}
    }
}
