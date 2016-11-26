using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace LabCOF
{
    public partial class Ajustes_de_Grafico : Form
    {
        public SymbolType TipoDePunto = Properties.Settings.Default.TipoDePunto;
        public int origenY1 = Properties.SettingsBluetooth.Default.EjeY1, 
                   origenY2 = Properties.SettingsBluetooth.Default.EjeY2;
       // public bool UnirPuntosConLinea = Properties.Settings.Default.UnirConLinea;
        public delegate void Delegado1();
        public event Delegado1 EventAjustarFondosForm1;

        public Ajustes_de_Grafico()
        {
            InitializeComponent();
            if (Properties.Graficos.Default.Panel2Activo) { groupBox6.Enabled = true; }
            else {groupBox6.Enabled = false;}
            AjustesGuardados(); 
         
        }
        //------------------------------ BOTONES PARA ELEGIR COLORES   --------------------------------------
             
        private void button10_Click_1(object sender, EventArgs e)
        {
            PuntoColor.AllowFullOpen = true;   PuntoColor.ShowDialog();
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            // se abre la opcion para elegir colores
            Fon1color1.AllowFullOpen = true;// cancela la opcion de crear colores personalizados
            Fon1color1.ShowDialog();
        }
        private void button4_Click_1(object sender, EventArgs e)
        {
            Fon2color1.AllowFullOpen = true;// cancela la opcion de crear colores personalizados
            Fon2color1.ShowDialog();
        }
        private void button6_Click_1(object sender, EventArgs e)
        {
            Cuadcolor1.AllowFullOpen = true;         Cuadcolor1.ShowDialog();
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            Fon1color2.AllowFullOpen = true;// cancela la opcion de crear colores personalizados
            Fon1color2.ShowDialog();
        }
        private void button5_Click_1(object sender, EventArgs e)
        {
            Fon2color2.AllowFullOpen = true;// cancela la opcion de crear colores personalizados
            Fon2color2.ShowDialog();
        }
        private void button7_Click_1(object sender, EventArgs e)
        {
            Cuadcolor2.AllowFullOpen = true;    Cuadcolor2.ShowDialog();
        }
        private void button13_Click(object sender, EventArgs e)
        {
            PuntoColor2.AllowFullOpen = true;      PuntoColor2.ShowDialog();
        }
 //---------------------------- BOTON APLICAR AJUSTES            ----------------------------------------------------------------------------------------------------------
        private void button1_Click(object sender, EventArgs e)
        {   // se cargan los ajustes por default
            if (checkBox1.Checked) { AjustesDefault(); }
       
                 //tirulos panel 1
                Properties.Settings.Default.tituloP = Convert.ToString(textBox1.Text);
                Properties.Settings.Default.tituloX = Convert.ToString(textBox2.Text);
                Properties.Settings.Default.tituloY = Convert.ToString(textBox3.Text);
                Properties.Settings.Default.tituloY2 = Convert.ToString(texboxTituloY2.Text);
                //titulos panel  2
                Properties.Settings.Default.tituloP2 = Convert.ToString(textBox11.Text);
                Properties.Settings.Default.tituloX2 = Convert.ToString(textBox10.Text);
                Properties.Settings.Default.tituloY22 = Convert.ToString(textBox9.Text);
                Properties.Settings.Default.titutloY222 = Convert.ToString(textBox4.Text);
             
                Properties.Settings.Default.fon1Col1 = Fon1color1.Color; button2.BackColor = Fon1color1.Color;
                Properties.Settings.Default.fon1Col2 = Fon1color2.Color; button3.BackColor = Fon1color2.Color;
                Properties.Settings.Default.fon2Col1 = Fon2color1.Color; button4.BackColor = Fon2color1.Color;
                Properties.Settings.Default.fon2Col2 = Fon2color2.Color; button5.BackColor = Fon2color2.Color;
                Properties.Settings.Default.cuadCol1 = Cuadcolor1.Color; button6.BackColor = Cuadcolor1.Color;
                Properties.Settings.Default.cuadCol2 = Cuadcolor2.Color; button7.BackColor = Cuadcolor2.Color;
                Properties.Settings.Default.ColorejeX = colorDialogBotX.Color; button11.BackColor = colorDialogBotX.Color;

                Properties.Settings.Default.ColorPunto = PuntoColor.Color; button10.BackColor = PuntoColor.Color;
                Properties.Settings.Default.tituloscolor = titulosColor.Color; button9.BackColor = titulosColor.Color;
                
                Properties.Settings.Default.ColorPunto2 = PuntoColor2.Color; button13.BackColor = PuntoColor2.Color;

                Properties.Settings.Default.TipoDePunto = TPunto(comboBox1.Text);// llama al metodo Tipo de punto  
                Properties.Settings.Default.sizePunto = Convert.ToInt32(numericUpDown1.Value);
                
                //________________________________________________________________________________

                if (comboBox2.Text == "Si") { Properties.Settings.Default.UnirConLinea = true; }
                if (comboBox2.Text == "No") { Properties.Settings.Default.UnirConLinea = false; }

                // settings modificados
                Properties.Settings.Default.SettingsModificados = true;

                //se actualiza las opciones de escalado PANEL1
                if (radioButton1.Checked) { Properties.Settings.Default.AutoEsc = true; }
                if (radioButton2.Checked)
                {
                    Properties.Settings.Default.AutoEsc = false;
                    try
                    {
                        Properties.Settings.Default.minX = Convert.ToDouble(textBox5.Text);
                        Properties.Settings.Default.maxX = Convert.ToDouble(textBox7.Text);
                        Properties.Settings.Default.minY = Convert.ToDouble(textBox6.Text);
                        Properties.Settings.Default.maxY = Convert.ToDouble(textBox8.Text);
                        if ((Convert.ToDouble(textBox5.Text)) >= (Convert.ToDouble(textBox7.Text)) || Convert.ToDouble(textBox6.Text) >= Convert.ToDouble(textBox8.Text))
                        {
                            Properties.Settings.Default.AutoEsc = true;
                            MessageBox.Show("Los valores minimos deben ser mas \npequeños que los Maximos ", "Escalado",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        }
                    }
                    catch
                    {
                        Properties.Settings.Default.AutoEsc = true;
                        MessageBox.Show("llenar todos los espacios en Blanco Del Escalado Panel 1", "Escalado",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    }
                }

                //se actualiza las opciones de escalado PANEL2
                if (radioButton4.Checked) { Properties.Settings.Default.AutoEsc2 = true; }
                if (Properties.Graficos.Default.Panel2Activo)
                {
                    if (radioButton3.Checked)
                  {
                    Properties.Settings.Default.AutoEsc2 = false;
                    try
                    {
                        Properties.Settings.Default.minX = Convert.ToDouble(textBox15.Text);
                        Properties.Settings.Default.maxX = Convert.ToDouble(textBox13.Text);
                        Properties.Settings.Default.minY = Convert.ToDouble(textBox14.Text);
                        Properties.Settings.Default.maxY = Convert.ToDouble(textBox12.Text);
                        if ((Convert.ToDouble(textBox15.Text)) >= (Convert.ToDouble(textBox13.Text)) || Convert.ToDouble(textBox14.Text) >= Convert.ToDouble(textBox12.Text))
                        {
                            Properties.Settings.Default.AutoEsc2 = true;
                            MessageBox.Show("Los valores minimos deben ser mas \npequeños que los Maximos ", "Escalado",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        }
                    }
                    catch
                    {
                        Properties.Settings.Default.AutoEsc2 = true;
                        MessageBox.Show("llenar todos los espacios en Blanco Del Escalado Panel 2", "Escalado",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    }
                  }
                }

                Properties.Settings.Default.Save();
                EventAjustarFondosForm1();//evento que pasa la accion al delegado1 para actualizar los fondos Form1
        }//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // boton cerrar
        private void button8_Click(object sender, EventArgs e)
        {
            Close();
                    
        }
        private void Ajustes_de_Grafico_Load(object sender, EventArgs e)
        {
        }

        private SymbolType TPunto(string Tipopunto)
        {
            switch (Tipopunto)
            {
                case "Circulo":          TipoDePunto = SymbolType.Circle;       break;
                case "Diamante":         TipoDePunto = SymbolType.Diamond;      break;
                case "Cuadrado":         TipoDePunto = SymbolType.Square;       break;
                case "Triangulo":        TipoDePunto = SymbolType.Triangle;     break;
                case "Estrella":         TipoDePunto = SymbolType.Star;         break;
                case "TrianguloInvertido": TipoDePunto = SymbolType.TriangleDown;break;
                case "X":                TipoDePunto = SymbolType.XCross;       break;
                case "Ninguno":          TipoDePunto = SymbolType.None;         break;
                default:                 TipoDePunto=SymbolType.Circle;         break;
            }
            return TipoDePunto;
        }

        private int Origeny1(string Dato)
        {
            switch (Dato)
            {
                case "Dato 1": origenY1 = 1; break;
                case "Dato 2": origenY1 = 2; break;
                case "Dato 3": origenY1 = 3; break;
                case "Dato 4": origenY1 = 4; break;
                case "Dato 5": origenY1 = 5; break;
                case "Dato 6": origenY1 = 6; break;
                case "Dato 7": origenY1 = 7; break;
                default: origenY1 = 1; break;// faltra ocion desabilitar
            }

            return origenY1;
        }

        private int Origeny2(string Dato)
        {
            switch (Dato)
            {
                case "Dato 1": origenY2 = 1; break;
                case "Dato 2": origenY2 = 2; break;
                case "Dato 3": origenY2 = 3; break;
                case "Dato 4": origenY2 = 4; break;
                case "Dato 5": origenY2 = 5; break;
                case "Dato 6": origenY2 = 6; break;
                case "Dato 7": origenY2 = 7; break;
                default: origenY2 = 7; break;// falta ociuon desabilitar
            }

            return origenY2;
        }

        //boton de color de titulos
        private void button9_Click(object sender, EventArgs e)
        {
            titulosColor.AllowFullOpen = true;   titulosColor.ShowDialog();
        }
        //boton color leyenda
        private void button11_Click(object sender, EventArgs e)
        {
            leyendaColor.AllowFullOpen=true;
            leyendaColor.ShowDialog();
        }
        //boton color fondo leyenda
        private void button12_Click(object sender, EventArgs e)
        {
            leyendaFondo.AllowFullOpen = true;    leyendaFondo.ShowDialog();
        }
        // escalado definido
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox5.Enabled = true; textBox6.Enabled = true; textBox7.Enabled = true; textBox8.Enabled = true;
            label14.Enabled = true; label15.Enabled = true; label16.Enabled = true; label17.Enabled = true;
        }
        // escalado automatico
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox5.Enabled = false; textBox6.Enabled = false; textBox7.Enabled = false; textBox8.Enabled = false;
            label14.Enabled = false; label15.Enabled = false; label16.Enabled = false; label17.Enabled = false;
        }
        private void AjustesGuardados()

        {
            radioButton1.Checked = true; textBox5.Enabled = false; textBox6.Enabled = false; textBox7.Enabled = false; textBox8.Enabled = false;
            label14.Enabled = false; label15.Enabled = false; label16.Enabled = false; label17.Enabled = false;

            // AL ABRIR LOS AJUSTES SE CARGAN LOS SIGUIENTES CONTENIDOS--------------------------------------------------------------------------------------------
            textBox1.Text = Convert.ToString(Properties.Settings.Default.tituloP);
            textBox2.Text = Convert.ToString(Properties.Settings.Default.tituloX);
            textBox3.Text = Convert.ToString(Properties.Settings.Default.tituloY);
            texboxTituloY2.Text = Properties.Settings.Default.tituloY2;
            comboBox1.Text = Convert.ToString(TipoDePunto);
            //titulos panel2
            textBox11.Text = Convert.ToString(Properties.Settings.Default.tituloP2);
            textBox10.Text = Convert.ToString(Properties.Settings.Default.tituloX2);
            textBox9.Text = Convert.ToString(Properties.Settings.Default.tituloY22);
            textBox4.Text = Convert.ToString(Properties.Settings.Default.titutloY222);  
           

            numericUpDown1.Value = Convert.ToDecimal(Properties.Settings.Default.sizePunto);

            
            //_____________________________________________________________________________________________________________________________________________________

            // los botones y los cuadros de dialogo de color se cargan con los del settings
            button2.BackColor = Properties.Settings.Default.fon1Col1; Fon1color1.Color = Properties.Settings.Default.fon1Col1;
            button3.BackColor = Properties.Settings.Default.fon1Col2; Fon1color2.Color = Properties.Settings.Default.fon1Col2;
            button4.BackColor = Properties.Settings.Default.fon2Col1; Fon2color1.Color = Properties.Settings.Default.fon2Col1;
            button5.BackColor = Properties.Settings.Default.fon2Col2; Fon2color2.Color = Properties.Settings.Default.fon2Col2;
            button6.BackColor = Properties.Settings.Default.cuadCol1; Cuadcolor1.Color = Properties.Settings.Default.cuadCol1;
            button7.BackColor = Properties.Settings.Default.cuadCol2; Cuadcolor2.Color = Properties.Settings.Default.cuadCol2;
            button10.BackColor = Properties.Settings.Default.ColorPunto; PuntoColor.Color = Properties.Settings.Default.ColorPunto;
            button9.BackColor = Properties.Settings.Default.tituloscolor; titulosColor.Color = Properties.Settings.Default.tituloscolor;
            button11.BackColor = Properties.Settings.Default.ColorejeX; colorDialogBotX.Color = Properties.Settings.Default.ColorejeX;

            button13.BackColor = Properties.Settings.Default.ColorPunto2; PuntoColor2.Color = Properties.Settings.Default.ColorPunto2;

            //ESCALADO pane1
            if (Properties.Settings.Default.AutoEsc) { radioButton1.Checked = true; }
            else
            {
                radioButton2.Checked = true;
                textBox5.Text = Convert.ToString(Properties.Settings.Default.minX);
                textBox7.Text = Convert.ToString(Properties.Settings.Default.maxX);
                textBox6.Text = Convert.ToString(Properties.Settings.Default.minY);
                textBox8.Text = Convert.ToString(Properties.Settings.Default.maxY);
            }
            //ESCALADO panel2
            if (Properties.Settings.Default.AutoEsc2) { radioButton4.Checked = true; }
            else
            {
                radioButton3.Checked = true;
                textBox12.Text = Convert.ToString(Properties.Settings.Default.maxY2);
                textBox13.Text = Convert.ToString(Properties.Settings.Default.maxX2);
                textBox14.Text = Convert.ToString(Properties.Settings.Default.minY2);
                textBox15.Text = Convert.ToString(Properties.Settings.Default.minX2);
            }

            //UNIR CON LINEA
            if (Properties.Settings.Default.UnirConLinea) { comboBox2.Text = "Si"; } else { comboBox2.Text = "No"; }
            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        }

        private void AjustesDefault()
        {
            radioButton1.Checked = true; textBox5.Enabled = false; textBox6.Enabled = false; textBox7.Enabled = false; textBox8.Enabled = false;
            label14.Enabled = false; label15.Enabled = false; label16.Enabled = false; label17.Enabled = false;

            // AL ABRIR LOS AJUSTES SE CARGAN LOS SIGUIENTES CONTENIDOS--------------------------------------------------------------------------------------------
            textBox1.Text = " X vs Y";
            textBox2.Text = "X";
            textBox3.Text = "Y";
            texboxTituloY2.Text="Y2";
            comboBox1.Text = "Circulo";

            textBox11.Text = " X vs Y";
            textBox10.Text = "X";
            textBox9.Text = "Y";
            textBox4.Text = "Y2";
           

            numericUpDown1.Value = Convert.ToDecimal(2);

            // los botones y los cuadros de dialogo de color se cargan por defalt con los siguientes colores
            button2.BackColor = Color.Black;                 Fon1color1.Color = Color.Black;
            button3.BackColor = Color.Black;                 Fon1color2.Color = Color.Black;
            button4.BackColor = Color.FromArgb(30, 30, 30);  Fon2color1.Color = Color.FromArgb(30, 30, 30);
            button5.BackColor = Color.FromArgb(30, 30, 30);  Fon2color2.Color = Color.FromArgb(30, 30, 30);
            button6.BackColor = Color.FromArgb(35, 35, 35);  Cuadcolor1.Color = Color.FromArgb(35, 35, 35);
            button7.BackColor = Color.FromArgb(41, 41, 41);  Cuadcolor2.Color = Color.FromArgb(41, 41, 41);
            button10.BackColor = Color.White;                PuntoColor.Color = Color.White;
            button9.BackColor = Color.White;                 titulosColor.Color = Color.White;
            
            button13.BackColor = Color.White;                PuntoColor2.Color = Color.White;
            button11.BackColor = Color.White;                colorDialogBotX.Color = Color.White;

            //ESCALADO
            radioButton1.Checked = true;
            radioButton4.Checked = true;
            //UNIR CON LINEA
            comboBox2.Text = "Si";
            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            checkBox1.Checked = false;
            //----------------------------------------------------
          
        }


        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            textBox12.Enabled = false; textBox13.Enabled =false; textBox14.Enabled = false; textBox15.Enabled = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            textBox12.Enabled = true; textBox13.Enabled = true; textBox14.Enabled = true; textBox15.Enabled = true;
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            //PuntoColor2.AllowFullOpen = true; PuntoColor2.ShowDialog();
            colorDialogBotX.AllowFullOpen = true; colorDialogBotX.ShowDialog();
        }


    }
}


