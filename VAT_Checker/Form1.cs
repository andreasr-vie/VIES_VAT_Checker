using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VAT_Checker.Service.VIES;

namespace VAT_Checker
{
    public partial class Form1 : Form
    {
        public string VATLand { get; set; }
        public string VATNumber { get; set; }
        public string NameFirma { get; set; }
        public string AdresseFirma { get; set; }
        public string Gueltigkeit { get; set; }
        public string strExeFilePath { get; set; }
        public string strWorkPath { get; set; }


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            label2.ForeColor = Color.Gray;
            label3.ForeColor = Color.Gray;
            label4.ForeColor = Color.Gray;
            label2.Text = "GÜLTIGKEIT";
            label3.Text = "Firmenname";
            label4.Text = "Adresse";

            strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);

        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string VAT = tbVATNr.Text;
            VAT = Regex.Replace(VAT, " ", "");
            tbVATNr.Text = VAT;
            int anzZeichen = VAT.Length;
            int zeichenOhneLand = anzZeichen - 2;

            string Land = VAT.Remove(2, zeichenOhneLand);
            string VATNr = VAT.Substring(2);
            VATNumber = VATNr;

            checkVatPortTypeClient viesVATchecker = new checkVatPortTypeClient();
            viesVATchecker.checkVat(ref Land, ref VATNr, out bool valide, out string Name, out string Adresse);

            VATLand = Land;
            
            if (valide == true)
            {
                label2.ForeColor = Color.Black;
                label2.BackColor = Color.LimeGreen;
                label2.Text = "GÜLTIG";
                Gueltigkeit = "GUELTIG";
            }
            else
            {
                label2.ForeColor = Color.Black; 
                label2.BackColor = Color.Red;
                label2.Text = "UNGÜLTIG";
                Gueltigkeit = "UNGUELTIG";
            }

            label3.ForeColor = Color.Black;
            NameFirma = (ReplaceNewLine(Name, ", "));
            label3.Text = NameFirma;

            label4.ForeColor = Color.Black;
            AdresseFirma = (ReplaceNewLine(Adresse, ", "));
            AdresseFirma = AdresseFirma.Replace(@"//", @"/");
            label4.Text = AdresseFirma;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string pfad = strWorkPath + @"\VATcheck.txt";
            bool fileExists = File.Exists(pfad);
            string datum = DateTime.Now.ToString("yyyy-MM-dd");
            string uhrzeit = DateTime.Now.ToString("HH:mm");

            if (!fileExists)
            {
                string headerFile = "Datum;Uhrzeit;UID;Gueltigkeit;Name;Adresse" + Environment.NewLine;
                File.WriteAllText(pfad, headerFile);
            }

            using (StreamWriter w = File.AppendText(pfad))
            {
                w.WriteLine(datum + ";" + uhrzeit + ";" + VATLand + VATNumber + ";" + Gueltigkeit + ";" + NameFirma + ";" + AdresseFirma);
            }

        }

        public static string ReplaceNewLine(string input, string replace)
        {
            input = Regex.Replace(input, @"\n", replace);
            input = Regex.Replace(input, @"\r", replace);
            input = Regex.Replace(input, @"\t", replace);
            return input;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
