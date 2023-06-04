using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Data.OleDb;
using System.Security.Permissions;
using System.Reflection.Emit;

namespace Otopark
{
    public partial class Form1 : Form
    {
        // OleDbConnection ve OleDbCommand nesneleri tanımlanıyor.
        OleDbConnection bag = new OleDbConnection("Provider= Microsoft.ACE.OLEDB.12.0; Data Source=veri.accdb");
        OleDbCommand kmt = new OleDbCommand();

        // Port ismi, bant hızı ve port listesi için değişkenler tanımlanıyor.
        public static string port_ismi, bant_hızı;
        string[] ports = SerialPort.GetPortNames();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Serial port bağlantısı açılıyor.
            timer1.Start();
            port_ismi = comboBox1.Text;
            bant_hızı = comboBox2.Text;

            try
            {
                serialPort1.PortName = port_ismi;
                serialPort1.BaudRate = Convert.ToInt16(bant_hızı);

                serialPort1.Open();
                label1.Text = "Bağlantı Sağlandı";
                label1.ForeColor = Color.Green;
            }
            catch
            {
                serialPort1.Close();
                serialPort1.Open();
                MessageBox.Show("Bağlantı zaten açık.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Serial port bağlantısı kapatılıyor.
            timer1.Stop();
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
                label1.Text = "Bağlantı kesildi";
                label1.ForeColor = Color.Red;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Form kapatıldığında serial port bağlantısı kontrol edilip kapatılıyor.
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
            }
        }

        private bool kartOkutuldu = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Timer her tetiklendiğinde seri porttan gelen veri okunuyor.
            string sonuc = serialPort1.ReadExisting();
            if (!string.IsNullOrEmpty(sonuc))
            {
                label2.Text = sonuc;

                if (!kartOkutuldu)
                {
                    // Veritabanında kart ID'sine göre arama yapılıyor.
                    bag.Open();
                    kmt.Connection = bag;
                    kmt.CommandText = "SELECT * FROM tablo WHERE ID='" + sonuc + "'";
                    OleDbDataReader oku = kmt.ExecuteReader();

                    if (oku.Read())
                    {
                        // Kart kayıtlı ise bilgiler getiriliyor ve gerekli işlemler yapılıyor.
                        serialPort1.WriteLine("1");
                        DateTime bugun = DateTime.Now;
                        pictureBox1.Image = Image.FromFile("foto\\" + oku["Resim"].ToString());
                        label8.Text = oku["Isim"].ToString();
                        label9.Text = oku["Plaka"].ToString();
                        label10.Text = bugun.ToShortDateString();
                        label11.Text = bugun.ToLongTimeString();
                        bag.Close();

                        bag.Open();
                        kmt.CommandText = "INSERT INTO zaman (Plaka,Tarih,Saat) VALUES ('" + label9.Text + "', '" + label10.Text + "', '" + label11.Text + "')";
                        kmt.ExecuteReader();
                        bag.Close();
                    }
                    else
                    {
                        // Kart kayıtlı değilse gerekli işlemler yapılıyor.
                        bag.Close();
                        serialPort1.WriteLine("2");
                        pictureBox1.Image = Image.FromFile("foto\\kayitsiz.jpg");
                        label2.Text = "Kart Kayıtlı Değil";
                        label8.Text = "___________________";
                        label9.Text = "___________________";
                        label10.Text = "___________________";
                        label11.Text = "___________________";
                    }
                }
            }
            else
            {
                kartOkutuldu = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Serial port üzerinden '3' komutu gönderiliyor.
            if (serialPort1.IsOpen == true)
            {
                serialPort1.WriteLine("3");
            }
            else
            {
                MessageBox.Show("Lütfen önce bağlanın.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Serial port üzerinden '4' komutu gönderiliyor.
            if (serialPort1.IsOpen == true)
            {
                serialPort1.WriteLine("4");
            }
            else
            {
                MessageBox.Show("Lütfen önce bağlanın.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Eğer port ismi veya bant hızı belirtilmemişse uyarı mesajı gösteriliyor.
            if (port_ismi == null || bant_hızı == null)
            {
                MessageBox.Show("Bağlantını kontrol et.");
            }
            else
            {
                // Timer durduruluyor ve serial port bağlantısı kapatılıyor.
                timer1.Stop();
                serialPort1.Close();
                label1.Text = "Bağlantı Kapalı.";
                label1.ForeColor = Color.Red;

                // Kayıt formu açılıyor.
                kayit kyt = new kayit();
                kyt.ShowDialog();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // ComboBox2'ye bant hızı değerleri ekleniyor.
            comboBox2.Items.Add("2400");
            comboBox2.Items.Add("4800");
            comboBox2.Items.Add("9600");
            comboBox2.Items.Add("19200");
            comboBox2.Items.Add("115200");

            // Port listesi kontrol ediliyor.
            if (ports.Length == 0)
            {
                MessageBox.Show("Herhangi bir port takılı değil.Lütfen portunuzu taktıktan sonra kapatıp tekrar açınız.");
            }
            if (ports.Length != 0)
            {
                foreach (string port in ports)
                {
                    comboBox1.Items.Add(port);
                }

                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 2;
            }
        }
    }
}


