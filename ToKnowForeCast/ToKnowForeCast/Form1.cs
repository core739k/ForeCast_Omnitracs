using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ToKnowForeCast
{
    public partial class frmForeCast : Form
    {
        private string apiKey = "5f6ea3c25680c72cc5f238fb8ac321ea";
        string state = "Dallas";
        string country = "usa";
        private string PathLog = @"C:\openweather\";
        private string fileForeCast = "forecast.csv";
        string url = "http://api.openweathermap.org/data/2.5/weather?q=##State,##Country&APPID=##API";
        public frmForeCast()
        {
            InitializeComponent();
        }

        private void frmForeCast_Load(object sender, EventArgs e)
        {
            DirectoryInfo FL_DIR = new DirectoryInfo(PathLog);
            if (!FL_DIR.Exists)
            {
                FL_DIR.Create();
            }
            string file = PathLog + fileForeCast;
            REG_LOG(file, "*** New log " + DateTime.Now.ToString() + " ***", true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string urlForeCast = "";
            urlForeCast = url.Replace("##State", state);
            urlForeCast = urlForeCast.Replace("##Country", country);
            urlForeCast = urlForeCast.Replace("##API", apiKey);

            using (WebClient client = new WebClient())
            {
                bool next = false;
                int fiveMinutes = 60000 * 5;
                try
                {
                    do
                    {
                        string fc = client.DownloadString(urlForeCast);
                        var clima = JObject.Parse(fc);
                        string cl = clima["main"].ToString();
                        var details = JObject.Parse(cl);
                        string temp = details["temp"].ToString();
                        // Farenheith is the default measure for OpenWeather API 
                        string rainfall = clima["clouds"].ToString();
                        var rainPossibility = JObject.Parse(rainfall);
                        string rainP = rainPossibility["all"].ToString();
                        string file = PathLog + fileForeCast;
                        REG_LOG(file, temp + ",Farenheith," + rainP + "%", false);
                        Thread.Sleep(fiveMinutes);
                        next = true;
                    } while (next);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unknown error: \n" + ex.Message);
                }
            }
        }
        private void REG_LOG(string Archivo, string Linea, bool Nuevo)
        {
            if (Nuevo)
            {
                StreamWriter LOG = new StreamWriter(Archivo);
                LOG.WriteLine(Linea);
                LOG.Close();
            }
            else
            {
                StreamWriter LOG = new StreamWriter(Archivo, append: true);
                LOG.WriteLine(Linea);
                LOG.Close();
            }
        }
    }
}
