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
        public string StrExeFilePath { get; set; }
        public string StrWorkPath { get; set; }
        public bool AbfrageErfolgreich { get; set; }

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

            StrExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            StrWorkPath = System.IO.Path.GetDirectoryName(StrExeFilePath);

            AbfrageErfolgreich = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string VAT = tbVATNr.Text;
                VAT = Regex.Replace(VAT, " ", "");
                if (CheckVATregex(VAT))
                {
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

                    AbfrageErfolgreich = true;
                }
                else
                {
                    label2.ForeColor = Color.Red;
                    label2.BackColor = DefaultBackColor;
                    label2.Text = "VAT-Nummernschema nicht gültig!";
                    label3.Text = "";
                    label4.Text = "";
                    tbVATNr.Text = "";

                    AbfrageErfolgreich = false;
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex, "FEHLER");
                label2.ForeColor = Color.Red;
                label2.BackColor = DefaultBackColor;
                label2.Text = "Fehler!";
                label3.Text = "";
                label4.Text = "";
                tbVATNr.Text = "";

                AbfrageErfolgreich = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string pfad = StrWorkPath + @"\VAT-Protokoll.csv";
                bool fileExists = File.Exists(pfad);
                string datum = DateTime.Now.ToString("yyyy-MM-dd");
                string uhrzeit = DateTime.Now.ToString("HH:mm");

                if (!fileExists)
                {
                    string headerFile = "Datum;Uhrzeit;UID;Gueltigkeit;Name;Adresse" + Environment.NewLine;
                    File.WriteAllText(pfad, headerFile);
                }

                if (AbfrageErfolgreich)
                {
                    using (StreamWriter w = File.AppendText(pfad))
                    {
                        w.WriteLine(datum + ";" + uhrzeit + ";" + VATLand + VATNumber + ";" + Gueltigkeit + ";" + NameFirma + ";" + AdresseFirma);
                    }
                }
                else
                {
                    label2.ForeColor = Color.Red;
                    label2.BackColor = DefaultBackColor;
                    label2.Text = "Fehler beim Protokollieren!";
                    label3.Text = "";
                    label4.Text = "";
                    tbVATNr.Text = "";
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex, "FEHLER");
                label2.ForeColor = Color.Red;
                label2.BackColor = DefaultBackColor;
                label2.Text = "Fehler!";
                label3.Text = "";
                label4.Text = "";
                tbVATNr.Text = "";
            }
        }

        public static string ReplaceNewLine(string input, string replace)
        {
            input = Regex.Replace(input, @"\n", replace);
            input = Regex.Replace(input, @"\r", replace);
            input = Regex.Replace(input, @"\t", replace);
            return input;
        }

        public static bool CheckVATregex(string input)
        {
            return Regex.Match(input, @"[a-zA-Z]{2,4}[0-9]{2,12}[a-zA-Z]{0,2}$").Success;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(StrWorkPath + @"\VAT-Protokoll.csv");
        }
    }
}
