using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Nemiro.OAuth;
using Nemiro.OAuth.LoginForms;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Web;
using System.Xml.Serialization;
using System.Xml;
using System.Timers;
using System.Text.RegularExpressions;

namespace StudentsKennen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        string CurrentPath = "/";
        public static string string1;
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.FromArgb(25, 25, 25);
            textBox1.ReadOnly = true;
            Pause.Enabled = false;
            Pause2.Hide();
            button5.Hide();
            webControl3.Hide();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            ProxyS.Hide();
            this.Text = String.Empty;
            p1.Hide();
            p2.Hide();
            butonu.FlatAppearance.BorderSize = 0;
            Pls.FlatAppearance.BorderSize = 0;
            Play.FlatAppearance.BorderSize = 0;
            Pause.FlatAppearance.BorderSize = 0;
            Pause2.FlatAppearance.BorderSize = 0;
            button4.FlatAppearance.BorderSize = 0;
            butonu.FlatStyle = FlatStyle.Flat;
            Pls.FlatStyle = FlatStyle.Flat;
            Play.FlatStyle = FlatStyle.Flat;
            Pause.FlatStyle = FlatStyle.Flat;
            Pause2.FlatStyle = FlatStyle.Flat;
            button4.FlatStyle = FlatStyle.Flat;
            butonu.FlatAppearance.MouseOverBackColor = BackColor;
            Pls.FlatAppearance.MouseOverBackColor = BackColor;
            Play.FlatAppearance.MouseOverBackColor = BackColor;
            Pause.FlatAppearance.MouseOverBackColor = BackColor;
            Pause2.FlatAppearance.MouseOverBackColor = BackColor;
            butonu.FlatAppearance.MouseDownBackColor = BackColor;
            Pls.FlatAppearance.MouseDownBackColor = BackColor;
            Play.FlatAppearance.MouseDownBackColor = BackColor;
            Pause.FlatAppearance.MouseDownBackColor = BackColor;
            Pause2.FlatAppearance.MouseDownBackColor = BackColor;
            butonu.FlatAppearance.BorderColor = Color.DarkRed;
            Pls.FlatAppearance.BorderColor = Color.DarkRed;
            Play.BackColor = Color.Transparent;
            Pause.BackColor = Color.Transparent;
            Pause2.BackColor = Color.Transparent;
            butonu.Hide();

            timer2.Interval = 5000;
            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer2.Enabled = false;
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();
            if (String.IsNullOrEmpty(Properties.Settings.Default.AccessToken))
            {
                this.GetAccessToken();
            }
            else
            {
                this.GetFiles();
            }
        }
        private void GetAccessToken()
        {

            var login = new DropboxLogin("yxk0rl5wlff50vc", "rqcvwt7pmmqubpp");
            login.Owner = this;
            login.ShowDialog();
            if (login.IsSuccessfully)
            {
                Properties.Settings.Default.AccessToken = login.AccessToken.Value;
                Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("error...");
                this.OnFormClosed();

            }
        }

        private void OnFormClosed()
        {
            this.Close();
        }
        private void GetFiles()
        {
            OAuthUtility.GetAsync
            (
              "https://api.dropbox.com/1/metadata/auto/",
                new HttpParameterCollection
                {
                    {"path",this.CurrentPath },
                    {"access_token",Properties.Settings.Default.AccessToken }
                },
                callback: GetFiles_Result
             );
        }
        private void GetFiles_Result(RequestResult result) { }
        private void GetShareLink_Result(RequestResult result)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<RequestResult>(GetShareLink_Result), result);
                string appPath = Path.GetDirectoryName(Application.ExecutablePath);
                var rezultat = result.ToString();
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(rezultat);
                try
                {
                    string url = values["url"];
                    url = url.Remove(url.Length - 1, 1) + "1";
                    TextBox.CheckForIllegalCrossThreadCalls = false;
                    textBox1.Text = url;
                    string1 = url;
                }
                catch (Exception ex) { MessageBox.Show("You are not connected to Dropbox"); }
                XDocument doc = new XDocument(new XElement("body",
                                           new XElement("line", string1)
                                                    )
                                     );
                doc.Save(appPath + "\\" + "Data.xml");
                FileStream upstream = new FileStream((appPath + "\\" + "Data.xml").ToString(), FileMode.Open);
                OAuthUtility.PutAsync
                              (
                              "https://api-content.dropbox.com/1/files_put/auto/",
                              new HttpParameterCollection
                {
                    {"access_token",Properties.Settings.Default.AccessToken},
                    {"path",Path.Combine(Path.GetFileName("Data.xml")).Replace("\\","/")},
                    {"overwrite","false"},
                    {"autorename","false"},
                    {upstream}
                },
                              callback: Upload_Result
                              );
                return;
            }
        }
        private void Upload_Result(RequestResult result)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string appPath = Path.GetDirectoryName(Application.ExecutablePath);
                XmlDocument xDoc = new XmlDocument();
                var web = new WebClient();
                try
                {
                    web.DownloadFile(new Uri(string.Format("https://api-content.dropbox.com/1/files/auto/{0}?access_token={1}", "Data.xml", Properties.Settings.Default.AccessToken)), (appPath + "\\" + "Data.xml").Replace("Data.xml", "Data.xml"));
                    web.DownloadProgressChanged += new DownloadProgressChangedEventHandler(web_DownloadProgressChanged);
                    xDoc.Load(appPath + "\\" + "Data.xml");
                    XmlRootAttribute xRoot = new XmlRootAttribute();
                    xRoot.ElementName = "user";
                    xRoot.IsNullable = true;
                    XmlSerializer sr = new XmlSerializer(typeof(Information));
                    FileStream read = new FileStream("Data.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
                    Information Info = (Information)sr.Deserialize(read);
                    read.Close();
                    textBox1.Text = Info.Url;
                    xDoc.RemoveAll();
                    timer1.Stop();
                    timer2.Start();
                }
                catch
                {
                    timer2.Start();
                    timer1.Stop();
                }
            }
            catch (WebException ex)
            {
   
            }

        }
        void web_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
           
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string load = textBox1.Text.ToString();
            string load2 = Regex.Replace(load, "watch", "");
            string load3 = load2.ToString();
            string load4 = Regex.Replace(load3, "[?]", "");
            string load5 = load4.ToString();
            string load6 = Regex.Replace(load5, "[=]", "/");
            textBox1.Clear();
            textBox1.Text = load6.ToString();
            Play.Show();
            string path = "";
            OAuthUtility.PostAsync
            (
              "https://api.dropbox.com/1/fileops/delete",
                new HttpParameterCollection
                {{ "root", "auto" },
                    {"path",Path.Combine(path,Path.GetFileName(path + "Data.xml")).Replace("\\","/") },
                    {"access_token",Properties.Settings.Default.AccessToken }
                    
                },
                callback: DeleteFile_Result
             );
        }
        private void DeleteFile_Result(RequestResult result)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains("proxfree"))
            {
                var txtLocX = textBox1.Location.X;
                var txtLocY = textBox1.Location.Y;
                txtLocX = 625;
                txtLocY = 23;
                textBox1.Height = txtLocY;
                textBox1.Width = txtLocX;
                textBox1.Location = new System.Drawing.Point(58, 390);
                textBox1.BackColor = Color.FromArgb(25, 25, 25);
                textBox1.BringToFront();
                p1.Show();
                p2.Show();
                ProxyS.BringToFront();
                Vlc.playlist.stop();
                Vlc.Hide();
                webControl3.Source = new Uri(textBox1.Text);
            }
            else
            {
                p1.Hide();
                p2.Hide();
                Vlc.Show();
                ProxyS.Hide();
                ProxyS.Stop();
                Pause.Enabled = true;
                textBox1.Location = new System.Drawing.Point(12, 33);

            Vlc.playlist.add(textBox1.Text.ToString());
            Vlc.playlist.play();
            Pls.Hide();
            butonu.Show();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Vlc.playlist.items.clear();
            
            Play.Show();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Vlc.playlist.togglePause();
            Pause.Hide();
                Pause2.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            Application.Restart();
        }

        private void button4_Click(object sender, EventArgs e)
        {
          
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Pls.Show();
            butonu.Hide();
            Vlc.playlist.items.clear();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            Vlc.playlist.stop();
            Properties.Settings.Default.Reset();
            Application.Restart();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {

            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            button3.Hide();
            button5.Show();

        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void button3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                button5.PerformClick();   
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
                
                timer1.Stop();
                Application.Exit();          
           
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ControlBox = false;
            this.TopMost = false;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Normal;
            button3.Show();
            button5.Hide();
        }

        private void Pause2_Click(object sender, EventArgs e)
        {
            Vlc.playlist.togglePause();
            Pause.Show();
            Pause2.Hide();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            this.Hide();
            Form F1 = new Themes();
            F1.Show();
        }

        private void radMenuButtonItem1_Click(object sender, EventArgs e)
        {

        }

        private void radMenuButtonItem2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Stop();
        }

        private void button2_Click_3(object sender, EventArgs e)
        {
            timer1.Start();
          

            }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
        }               
  }

  

