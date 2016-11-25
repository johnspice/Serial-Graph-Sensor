using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LabCOF
{
    public partial class AjustesCom : Form
    {
        public AjustesCom()
        {
            InitializeComponent();
            label5.Text = "Por: Juan G.L.H. 2015.";
            label2.Text = "Funciona bien bajo Windows 7, 8, 8.1, 10 Para versiones anteriores XP o Vista \nes posible que sea necesario Instalar Microsoft .NET Framework 4.0.";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            System.Diagnostics.Process.Start("https://www.microsoft.com/es-mx/download/details.aspx?id=17851");
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
