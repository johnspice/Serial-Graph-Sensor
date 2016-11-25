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

namespace LabCOF
{
    public partial class Form1 : Form
    {

        string NombrePuerto;
        byte Enviar0, Enviar1, Enviar2;
        public delegate void Delegado1(bool Panel2Activo);
        public event Delegado1 EventSegundoPanelForm1;
        

        public Form1()
        {
            InitializeComponent();
            AjustesGuardados();            
        }


        private void button1_Click(object sender, EventArgs e)
        {
            int i;          
            string[] ListaPuertos = SerialPort.GetPortNames();

            comboBox1.Items.Clear();
                for (i = 0; i < ListaPuertos.Length; i++)
                {
                    comboBox1.Items.Add(ListaPuertos[i]);
                }
            button2.Enabled =true;          
        }



        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "")
            {
                MessageBox.Show(this, "No se ha seleccionado dispositivo de la lista ", "No se ha elegido Dispositivo", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
           
            }

            else
            {
                NombrePuerto = comboBox1.Text;
                Properties.Settings.Default.NombrePuerto = NombrePuerto;
                Properties.Settings.Default.Save();
                //Close();
               
            }
           
        }


        //   ------------------------------------------------------------   RADIOBUTTONS  ------------------------------------------------------------------------------------------------------------
        //Entrada 0 ________________________________________________________
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = true; comboBox2.Enabled = true; comboBox3.Enabled = true; comboBox16.Enabled = true; buttonColorE0.Enabled = true;
        }
        private void RbEntrada0off_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = false; comboBox2.Enabled = false; comboBox3.Enabled = false; comboBox16.Enabled = false; buttonColorE0.Enabled = false;
        }

        //Entrada 1  _______________________________________________________
        private void RbEntrada1On_CheckedChanged(object sender, EventArgs e)
        {
            textBox3.Enabled = true; comboBox5.Enabled = true; comboBox4.Enabled = true; comboBox17.Enabled = true; buttonColorE1.Enabled = true;
        }
        private void RbEntrada1off_CheckedChanged(object sender, EventArgs e)
        {
            comboBox17.Text = "Y1";
            textBox3.Enabled = false; comboBox5.Enabled = false; comboBox4.Enabled = false; comboBox17.Enabled = false; buttonColorE1.Enabled = false;
        }

        //Entrada 2 ________________________________________________________
        private void RbEntrada2on_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.Enabled = true; comboBox7.Enabled =true; comboBox6.Enabled = true; comboBox18.Enabled = true; buttonColorE2.Enabled = true;
        }
        private void RbEntrada2off_CheckedChanged(object sender, EventArgs e)
        {
            comboBox18.Text = "Y1";
            textBox4.Enabled = false; comboBox7.Enabled = false; comboBox6.Enabled = false; comboBox18.Enabled = false; buttonColorE2.Enabled = false;
        }

        //Entrada 3 ________________________________________________________
        private void RbEntrada3on_CheckedChanged(object sender, EventArgs e)
        {
            textBox5.Enabled = true; comboBox9.Enabled = true; comboBox8.Enabled = true; comboBox19.Enabled =true; buttonColorE3.Enabled = true;
        }
        private void RbEntrada3off_CheckedChanged(object sender, EventArgs e)
        {
            comboBox19.Text = "Y1";
            textBox5.Enabled = false; comboBox9.Enabled = false; comboBox8.Enabled = false; comboBox19.Enabled = false; buttonColorE3.Enabled = false;
        }

        //Entrada 4 _________________________________________________________
        private void RbEntrada4on_CheckedChanged(object sender, EventArgs e)
        {
            textBox6.Enabled = true; comboBox11.Enabled = true; comboBox10.Enabled = true; comboBox20.Enabled = true; buttonColorE4.Enabled = true;
        }
        private void RbEntrada4off_CheckedChanged(object sender, EventArgs e)
        {
            comboBox20.Text = "Y1";
         textBox6.Enabled = false; comboBox11.Enabled = false; comboBox10.Enabled = false; comboBox20.Enabled = false; buttonColorE4.Enabled = false;
        }

        //Entrada 5 ________________________________________________________
        private void RbEntrada5on_CheckedChanged(object sender, EventArgs e)
        {
            textBox7.Enabled = true; comboBox13.Enabled = true; comboBox12.Enabled = true; comboBox21.Enabled = true; buttonColorE5.Enabled = true;
        }
        private void RbEntrada5off_CheckedChanged(object sender, EventArgs e)
        {
            comboBox21.Text = "Y1";
            textBox7.Enabled = false; comboBox13.Enabled = false; comboBox12.Enabled = false; comboBox21.Enabled = false; buttonColorE5.Enabled = false;
        }

        // Entrada 6 ______________________________________________________
        private void RbEntrada6on_CheckedChanged(object sender, EventArgs e)
        {
           
        }
        private void RbEntrada6off_CheckedChanged(object sender, EventArgs e)
        {
             
        }


        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        
       

        // Ajustes Por defaul
        private void AjustesGuardados()
        {
            Enviar0 = Properties.SettingsBluetooth.Default.DatoEnviado0;
            Enviar1 = Properties.SettingsBluetooth.Default.DatoEnviado0;
            Enviar2 = Properties.SettingsBluetooth.Default.DatoEnviado0;

            numericUpDown1.Value = Properties.SettingsBluetooth.Default.NumDatos;

            // Entrada0
            textBox1.Text = Properties.Graficos.Default.E0name;
            comboBox2.Text = Properties.Graficos.Default.E0x;
            comboBox3.Text = Properties.Graficos.Default.E0y;
            comboBox16.Text = Properties.Graficos.Default.E0mostrar;
            buttonColorE0.BackColor = Properties.Graficos.Default.E0color;
            if (Properties.Graficos.Default.E0activo) {RbEntrada0on.Checked = true; RbEntrada0off.Checked = false; }
            else{RbEntrada0on.Checked = false; RbEntrada0off.Checked = true; textBox1.Enabled = false; comboBox2.Enabled = false; comboBox3.Enabled = false; comboBox16.Enabled = false; buttonColorE0.Enabled = false; }
            colorDialogE0.Color = Properties.Graficos.Default.E0color;

            //Entrada1        
            textBox3.Text = Properties.Graficos.Default.E1name;
            comboBox5.Text = Properties.Graficos.Default.E1x;
            comboBox4.Text = Properties.Graficos.Default.E1y;
            comboBox17.Text = Properties.Graficos.Default.E1mostrar;
            buttonColorE1.BackColor = Properties.Graficos.Default.E1color;
            if (Properties.Graficos.Default.E1activo)  {RbEntrada1On.Checked = true; RbEntrada1off.Checked = false;}
            else { RbEntrada1On.Checked = false; RbEntrada1off.Checked = true; textBox3.Enabled = false; comboBox5.Enabled = false; comboBox4.Enabled = false; comboBox17.Enabled = false; buttonColorE1.Enabled = false; }
            colorDialogE1.Color = Properties.Graficos.Default.E1color;


            //Entrada2        
            textBox4.Text = Properties.Graficos.Default.E2name;
            comboBox7.Text = Properties.Graficos.Default.E2x;
            comboBox6.Text = Properties.Graficos.Default.E2y;
            comboBox18.Text = Properties.Graficos.Default.E2mostrar;
            buttonColorE2.BackColor = Properties.Graficos.Default.E2color;
            if (Properties.Graficos.Default.E2activo) { RbEntrada2on.Checked = true; RbEntrada2off.Checked = false; }
            else { RbEntrada2on.Checked = false; RbEntrada2off.Checked = true; textBox4.Enabled = false; comboBox7.Enabled = false; comboBox6.Enabled = false; comboBox18.Enabled = false; buttonColorE2.Enabled = false; }
            colorDialogE2.Color = Properties.Graficos.Default.E2color;

            //Etrada 3      
            textBox5.Text = Properties.Graficos.Default.E3name;
            comboBox9.Text = Properties.Graficos.Default.E3x;
            comboBox8.Text = Properties.Graficos.Default.E3y;
            comboBox19.Text = Properties.Graficos.Default.E3mostrar;
            buttonColorE3.BackColor = Properties.Graficos.Default.E3color;
            if (Properties.Graficos.Default.E3activo) { RbEntrada3on.Checked = true; RbEntrada3off.Checked = false; }
            else { RbEntrada3on.Checked = false; RbEntrada3off.Checked = true; textBox5.Enabled = false; comboBox9.Enabled = false; comboBox8.Enabled = false; comboBox19.Enabled = false; buttonColorE3.Enabled = false; }
            colorDialogE3.Color = Properties.Graficos.Default.E3color;


            //Entrada 4  
            textBox6.Text = Properties.Graficos.Default.E4name;
            comboBox11.Text = Properties.Graficos.Default.E4x;
            comboBox10.Text = Properties.Graficos.Default.E4y;
            comboBox20.Text = Properties.Graficos.Default.E4mostrar;
            buttonColorE4.BackColor = Properties.Graficos.Default.E4color;
            if (Properties.Graficos.Default.E4activo) { RbEntrada4on.Checked = true; RbEntrada4off.Checked = false; }
            else { RbEntrada4on.Checked = false; RbEntrada4off.Checked = true; textBox6.Enabled = false; comboBox11.Enabled = false; comboBox10.Enabled = false; comboBox20.Enabled = false; buttonColorE4.Enabled = false; }
            colorDialogE4.Color = Properties.Graficos.Default.E4color;


            //Entrada 5  
            textBox7.Text = Properties.Graficos.Default.E5name;
            comboBox13.Text = Properties.Graficos.Default.E5x;
            comboBox12.Text = Properties.Graficos.Default.E5y;
            comboBox21.Text = Properties.Graficos.Default.E5mostrar;
            buttonColorE5.BackColor = Properties.Graficos.Default.E5color;
            if (Properties.Graficos.Default.E5activo) { RbEntrada5on.Checked = true; RbEntrada5off.Checked = false; }
            else { RbEntrada5on.Checked = false; RbEntrada5off.Checked = true; textBox7.Enabled = false; comboBox13.Enabled = false; comboBox12.Enabled = false; comboBox21.Enabled = false; buttonColorE5.Enabled = false; }
            colorDialogE5.Color = Properties.Graficos.Default.E5color;
            //Panel 2

            if (Properties.Graficos.Default.Panel2Activo) { groupBox10.Enabled = true; radioButton1.Checked = true; }
            else { groupBox10.Enabled = false; radioButton2.Checked = true; }


        }


        //IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII          APLICAR AJUSTES     IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII
        private void buttonAplicar_Click(object sender, EventArgs e)
        {
            if (RbEntrada0on.Checked) {Properties.Graficos.Default.E0activo=true ;} else {Properties.Graficos.Default.E0activo=false;}
            if (RbEntrada1On.Checked) {Properties.Graficos.Default.E1activo=true ;} else {Properties.Graficos.Default.E1activo=false;}
            if (RbEntrada2on.Checked) {Properties.Graficos.Default.E2activo=true ;} else {Properties.Graficos.Default.E2activo=false;}
            if (RbEntrada3on.Checked) {Properties.Graficos.Default.E3activo=true ;} else {Properties.Graficos.Default.E3activo=false;}
            if (RbEntrada4on.Checked) {Properties.Graficos.Default.E4activo=true ;} else {Properties.Graficos.Default.E4activo=false;}
            if (RbEntrada5on.Checked) {Properties.Graficos.Default.E5activo=true ;} else {Properties.Graficos.Default.E5activo=false;}
            
            // nombres de cada grafico
            Properties.Graficos.Default.E0name=textBox1.Text;
            Properties.Graficos.Default.E1name=textBox3.Text;
            Properties.Graficos.Default.E2name=textBox4.Text;
            Properties.Graficos.Default.E3name=textBox5.Text;
            Properties.Graficos.Default.E4name=textBox6.Text;
            Properties.Graficos.Default.E5name=textBox7.Text;
            

            //eje X de cada grafica
            Properties.Graficos.Default.E0x = comboBox2.Text;
            Properties.Graficos.Default.E1x = comboBox5.Text;
            Properties.Graficos.Default.E2x = comboBox7.Text;
            Properties.Graficos.Default.E3x = comboBox9.Text;
            Properties.Graficos.Default.E4x = comboBox11.Text;
            Properties.Graficos.Default.E5x = comboBox13.Text;
            
            //eje y de cada grafico
            Properties.Graficos.Default.E0y = comboBox3.Text;
            Properties.Graficos.Default.E1y = comboBox4.Text;
            Properties.Graficos.Default.E2y = comboBox6.Text;
            Properties.Graficos.Default.E3y = comboBox8.Text;
            Properties.Graficos.Default.E4y = comboBox10.Text;
            Properties.Graficos.Default.E5y = comboBox12.Text;
            
            // eje donde se mostrara
            Properties.Graficos.Default.E0mostrar = comboBox16.Text;
            Properties.Graficos.Default.E1mostrar = comboBox17.Text;
            Properties.Graficos.Default.E2mostrar = comboBox18.Text;
            Properties.Graficos.Default.E3mostrar = comboBox19.Text;
            Properties.Graficos.Default.E4mostrar = comboBox20.Text;
            Properties.Graficos.Default.E5mostrar = comboBox21.Text;
          
            // COLORES De cada Grafico
            Properties.Graficos.Default.E0color = colorDialogE0.Color; buttonColorE0.BackColor = colorDialogE0.Color;
            Properties.Graficos.Default.E1color = colorDialogE1.Color; buttonColorE1.BackColor = colorDialogE1.Color;
            Properties.Graficos.Default.E2color = colorDialogE2.Color; buttonColorE2.BackColor = colorDialogE2.Color;
            Properties.Graficos.Default.E3color = colorDialogE3.Color; buttonColorE3.BackColor = colorDialogE3.Color;
            Properties.Graficos.Default.E4color = colorDialogE4.Color; buttonColorE4.BackColor = colorDialogE4.Color;
            Properties.Graficos.Default.E5color = colorDialogE5.Color; buttonColorE5.BackColor = colorDialogE5.Color;
            
            //
            if (comboBox16.Text == "Y2" || comboBox17.Text == "Y2" || comboBox18.Text == "Y2")
            {
                Properties.Graficos.Default.ActivoY2panel1 = true;
            }
            else { Properties.Graficos.Default.ActivoY2panel1 = false; }
            if (comboBox19.Text == "Y2" || comboBox20.Text == "Y2" || comboBox21.Text == "Y2")
            {
                Properties.Graficos.Default.ActivoY2panel2 = true;
            }
            else { Properties.Graficos.Default.ActivoY2panel2 = false; }
            
            //PANEL 2 ACTIVO
            if (radioButton1.Checked) { Properties.Graficos.Default.Panel2Activo = true; EventSegundoPanelForm1(true); }
            else { Properties.Graficos.Default.Panel2Activo = false; EventSegundoPanelForm1(false); }

            //numericupdown
            try { Properties.SettingsBluetooth.Default.NumDatos = Convert.ToInt32(numericUpDown1.Value); }
            catch { Properties.SettingsBluetooth.Default.NumDatos = 9999; }

            //salvando ajustes
            Properties.Graficos.Default.Save();
            
        }

        //IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII
       

        // BOTONES DE LOS COLORES
        private void buttonColorE0_Click(object sender, EventArgs e)
        {
           colorDialogE0.AllowFullOpen = true; colorDialogE0.ShowDialog();
        }

        private void buttonColorE1_Click(object sender, EventArgs e)
        {
            colorDialogE1.AllowFullOpen = true; colorDialogE1.ShowDialog();
        }

        private void buttonColorE2_Click(object sender, EventArgs e)
        {
            colorDialogE2.AllowFullOpen = true; colorDialogE2.ShowDialog();
        }

        private void buttonColorE3_Click(object sender, EventArgs e)
        {
            colorDialogE3.AllowFullOpen = true; colorDialogE3.ShowDialog();
        }

        private void buttonColorE4_Click(object sender, EventArgs e)
        {
            colorDialogE4.AllowFullOpen = true; colorDialogE4.ShowDialog();
        }

        private void buttonColorE5_Click(object sender, EventArgs e)
        {
            colorDialogE5.AllowFullOpen = true; colorDialogE5.ShowDialog();
        }

        private void buttonColorE6_Click(object sender, EventArgs e)
        {
            colorDialogE6.AllowFullOpen = true; colorDialogE6.ShowDialog();
        }

        private void buttonCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            groupBox10.Enabled = true;
           
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            RbEntrada3off.Checked = true;
            RbEntrada4off.Checked = true;
            RbEntrada5off.Checked = true;         
            groupBox10.Enabled = false;
            comboBox19.Text = "Y1";
            comboBox20.Text = "Y1";
            comboBox21.Text = "Y1";



        }

       
       
       
       


       

    }
}
