using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Share_Your_Copy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Thread thr;

        IPEndPoint endpoint;
        int port = 7001;
        Socket sck;
        static bool continueVal = true;
        IPAddress ipAddress;
        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    label1.Text = ip.ToString();
                    ipAddress = ip;
                }
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.ToString().Length < 5 && textBox1.Text.ToString().Length > 0)
            {
                port = Convert.ToInt32(textBox1.Text.ToString());
                Control.CheckForIllegalCrossThreadCalls = false;
                if (thr == null)
                {
                    thr = new Thread(listen);
                    thr.Start();
                }
                else
                {
                    continueVal = true;
                }
            }
            else
            {
                MessageBox.Show("You must write port!");
            }
        }

        private void listen()
        {
            try
            {
                int tempPort = port;
                byte[] data = new byte[1024];
                endpoint = new IPEndPoint(ipAddress, port);
                sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Console.WriteLine("Server started at => " + ipAddress + ":" + port + "...");
                sck.Bind(endpoint);
                richTextBox2.Text = "Server started at => " + ipAddress + ":" + port + "...\n" + richTextBox2.Text.ToString();

                while (true)
                {

                    if (port != tempPort)
                    {
                        sck.Dispose();
                        endpoint = new IPEndPoint(ipAddress, port);
                        sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        Console.WriteLine("Server started at => " + ipAddress + ":" + port + "...");
                        richTextBox2.Text = "Server started at => " + ipAddress + ":" + port + "...\n" + richTextBox2.Text.ToString();
                        sck.Bind(endpoint);
                    }

                    Console.WriteLine("i am listening");
                    EndPoint remote = (EndPoint)endpoint;
                    int recv = sck.ReceiveFrom(data, ref remote);
                    string listenData = Encoding.ASCII.GetString(data, 0, recv).ToString();
                    if (continueVal == true)
                    {
                        String prefix = listenData.Split(new string[] { "/#/" }, StringSplitOptions.None)[0].ToString();
                        String title = listenData.Split(new string[] { "/#/" }, StringSplitOptions.None)[1].ToString();
                        String message = listenData.Split(new string[] { "/#/" }, StringSplitOptions.None)[2].ToString();
                        if (prefix == "LOG")
                        {
                            richTextBox2.Text = "LOG:" + ipAddress + ":" + port + " => "+title +": "+message+ "...\n" + richTextBox2.Text.ToString();

                        }
                        else
                        {
                            listBox1.Items.Insert(0, title + ": " + message);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Server stoped...");
                        richTextBox2.Text += "Server stoped...\n";
                    }
                }

            }


            catch (Exception ex)
            {

                Console.WriteLine("Error: " + ex);
                richTextBox2.Text += "Error:" + ex + "...\n";

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            continueVal = false;

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(listBox1.SelectedItem + "");
        }
    }
}
