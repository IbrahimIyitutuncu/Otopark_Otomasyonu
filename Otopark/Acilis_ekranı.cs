using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Otopark
{
    public partial class Acilis_ekranı : Form
    {
        public Acilis_ekranı()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value<50)
            {
                progressBar1.Value = progressBar1.Value + 1;
            }
            else
            {
                timer1.Enabled = false;
                Form ana_menu= new Form1();
                ana_menu.Show();
                this.Hide();
            }
        }
    }
}
