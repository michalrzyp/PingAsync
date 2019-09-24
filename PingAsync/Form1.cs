using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;

namespace PingAsync
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(textBox2.Text!=String.Empty)
            {
                if(textBox2.Text.Trim().Length>0)
                {
                    listBox2.Items.Add(textBox2.Text);
                    textBox2.Clear();
                }
            }
        }
        private string SendPing(string address, int timeout, byte[] buffer, PingOptions option)
        {
            Ping ping = new Ping();
            try
            {
                PingReply answer = ping.Send(address, timeout, buffer, option);
                if (answer.Status == IPStatus.Success)
                {
                    return ("Answer with: " + address + " bajt=" + answer.Buffer.Length
                       + " time=" + answer.RoundtripTime + "ms TTL=" + answer.Options.Ttl);
                }
                else
                {
                    return "Error: " + address + " " + answer.Status.ToString();
                }
            }
            catch (Exception ex)
            {
                return "Error: " + address + " " + ex.Message;
            }
        }
        private void SendPingAsync(string address, int timeout, byte[] buffer, PingOptions option)
        {
            Ping ping = new Ping();
            ping.PingCompleted += new PingCompletedEventHandler(EndPing);
            try
            {
                ping.SendAsync(address, timeout, buffer, option, null);
            }
            catch (Exception ex)
            {
                listBox1.Items.Add("Error: " + address + " " + ex.Message);
            }
        }
        public void EndPing(object sender, PingCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                listBox1.Items.Add("Operation aborted or wrong address");
                ((IDisposable)(Ping)sender).Dispose();
                return;
            }
            PingReply answer = e.Reply;
            if (answer.Status == IPStatus.Success)
            {
                listBox1.Items.Add("Answer with: " + answer.Address.ToString() + " bajt=" + answer.Buffer.Length
                   + " time=" + answer.RoundtripTime + "ms TTL=" + answer.Options.Ttl);
            }
            else
            {
                listBox1.Items.Add("Error: " + answer.Address.ToString() + " " + answer.Status.ToString());
            }
            ((IDisposable)(Ping)sender).Dispose();
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" || listBox2.Items.Count > 0)
            {
                PingOptions option = new PingOptions();
                option.Ttl = (int)numericUpDown2.Value;
                option.DontFragment = true;
                string data = "aaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 120;
                if (textBox1.Text != "")
                {
                    for(int i=0;i<(int)numericUpDown1.Value;i++)
                    {
                        //listBox1.Items.Add(this.SendPing(textBox1.Text, timeout, buffer, option));
                        this.SendPingAsync(textBox1.Text, timeout, buffer, option);
                        listBox1.Items.Add("-------------------");
                    }
                }
                if (listBox2.Items.Count > 0)
                {
                    foreach(string host in listBox2.Items)
                    {
                        for (int i = 0; i < (int)numericUpDown1.Value; i++)
                        {
                           // listBox1.Items.Add(this.SendPing(host, timeout, buffer, option));
                            this.SendPingAsync(host, timeout, buffer, option);
                            listBox1.Items.Add("-------------------");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No addresses provided", "Error");
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            listBox1.Items.Clear();
        }
    }
}
