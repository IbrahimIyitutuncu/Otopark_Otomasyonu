using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Otopark
{
    public partial class kayit : Form
    {
        OleDbConnection bag = new OleDbConnection("Provider= Microsoft.ACE.OLEDB.12.0; Data Source=veri.accdb");
        OleDbCommand kmt = new OleDbCommand();
        public kayit()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Her zamanlayıcı olayında, seri porttan gelen veriler okunuyor.
            string sonuc;
            sonuc = serialPort1.ReadExisting();
            if (sonuc != "")
            {
                label9.Text = sonuc;
            }
        }

        private void kayit_Load(object sender, EventArgs e)
        {
            // Form yüklendiğinde seri port ayarları yapılıyor ve bağlantı kontrolü yapılıyor.

            serialPort1.PortName = Form1.port_ismi;
            serialPort1.BaudRate = Convert.ToInt16(Form1.bant_hızı);

            if (serialPort1.IsOpen == false)
            {
                try
                {
                    serialPort1.Open();
                    label10.Text = "Bağlantı Sağlandı";
                    label10.ForeColor = Color.Green;

                }
                catch
                {
                    label10.Text = "Bağlantı Sağlanamadı";

                }
            }
            else
            {
                label10.Text = "Bağlantı Sağlanamadı.";
                label10.ForeColor = Color.Red;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // "Kart Okutmak İçin Tıklayınız." butonuna tıklandığında, giriş alanları ve etiketler sıfırlanıyor.
            timer1.Start();
            label9.Text = "___________________";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            label7.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // "Gözat" butonuna tıklandığında, bir dosya açma iletişim kutusu görüntüleniyor.

            OpenFileDialog dosya = new OpenFileDialog();
            dosya.Filter = "Resim dosyaları (jpg)|*.jpg|Tüm dosyalar |*.*";
            openFileDialog1.InitialDirectory = Application.StartupPath + "\\foto";
            dosya.RestoreDirectory = true;

            if (dosya.ShowDialog() == DialogResult.OK)
            {
                // Seçilen dosyanın adı textbox'a yazılıyor.
                string di = dosya.SafeFileName;
                textBox6.Text = di;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // "Kaydet" butonuna tıklandığında, giriş alanlarındaki veriler veritabanına kaydediliyor.

            if (label9.Text == "__________" || textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || textBox1.Text == "")
            {
                // Gerekli bilgiler eksikse uyarıyor.
                label7.Text = "Eksik bilgi girdiniz.";
                label7.ForeColor = Color.Red;

            }
            else
            {
                try
                {
                    bag.Open();
                    kmt.Connection = bag;

                    // Veriler veritabanına ekleniyor.
                    kmt.CommandText = " INSERT INTO tablo (ID,Isim,Plaka, Model,Tarih,Saat,Resim) VALUES ('" + label9.Text + "', '" + textBox1.Text + "', '" + textBox2.Text + "','" + textBox3.Text + "', '" + textBox4.Text + "', '" + textBox5.Text + "' , '" + textBox6.Text +"')";
                    kmt.ExecuteNonQuery();
                    label7.Text = "Kayıt Yapıldı.";
                    label7.ForeColor = Color.Green;
                    bag.Close();
                }
                catch
                {
                    bag.Close();
                    MessageBox.Show("Bu kart zaten kayıtlı.");
                }
            }
        }

        private void kayit_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Form kapatıldığında seri port durduruluyor ve kapatılıyor.
            timer1.Stop();
            serialPort1.Close();
        }
    }
}
