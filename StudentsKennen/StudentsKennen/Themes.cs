using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace StudentsKennen
{
    public partial class Themes : Form
    {
        public Themes()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form F1 = new Form1();
            F1.BackColor = Color.FromArgb(25, 25, 25);
            button1.BackColor = Color.FromArgb(192, 0, 0);
            button2.BackColor = Color.FromArgb(14, 17, 29);
            button4.BackColor = Color.FromArgb(14, 17, 29);
            button3.BackColor = Color.FromArgb(17, 17, 29);
            
        }
    }
}
