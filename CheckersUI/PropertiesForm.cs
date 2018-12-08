using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PropertiesForm
{
    internal partial class userPropertiesForm : Form
    {
        public userPropertiesForm()
        {
            InitializeComponent();
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (this.player2TextBox.Enabled == true)
            {
                this.player2TextBox.Enabled = false;
                this.player2TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                this.player2TextBox.Text = "Computer";
            }
            else
            {
                this.player2TextBox.Enabled = true;
                this.player2TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
                this.player2TextBox.Text = string.Empty;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updateData();
            this.Close();
        }
    }
}
