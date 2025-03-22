using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace CezarEnc
{
    public partial class Form1 : Form
    {
        private Core core = new Core();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            core.text = text.Text;
            core.set_key(key.Text);

            core.encrypted();

            if (!core.error_status)
            {
                encrText.Text = core.encryp_Text;
            }
            message.Text = core.status_message;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            core.encryp_Text = encrText.Text;
            core.set_key(key.Text);

            core.decrypted();

            if (!core.error_status)
            {
                text.Text = core.text;
            }
            message.Text = core.status_message;

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Checked)
            {
                core.lang_gl = "ru";
            }
        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Checked)
            {
                core.lang_gl = "eng";
            }
        }
    }
}
