using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VAT_Checker.Service.VIES;

namespace VAT_Checker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label2.ForeColor = Color.Gray;
            label3.ForeColor = Color.Gray;
            label4.ForeColor = Color.Gray;
            label2.Text = "GÜLTIGKEIT";
            label3.Text = "Firmenname";
            label4.Text = "Adresse";

        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string VAT = tbVATNr.Text;
            int anzZeichen = VAT.Length;
            int zeichenOhneLand = anzZeichen - 2;

            string Land = VAT.Remove(2, zeichenOhneLand);
            string VATNr = VAT.Substring(2);

            checkVatPortTypeClient testPortTypeClient = new checkVatPortTypeClient();
            testPortTypeClient.checkVat(ref Land, ref VATNr, out bool valide, out string Name, out string Adresse);

            if (valide == true)
            {
                label2.ForeColor = Color.Black;
                label2.BackColor = Color.LimeGreen;
                label2.Text = "GÜLTIG";
            }
            else
            {
                label2.ForeColor = Color.Black; 
                label2.BackColor = Color.Red;
                label2.Text = "UNGÜLTIG";
            }

            label3.ForeColor = Color.Black;
            label3.Text = (Name);
            
            label4.ForeColor = Color.Black; 
            label4.Text = (Adresse);

        }

    }
}
