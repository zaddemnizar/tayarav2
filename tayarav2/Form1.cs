using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tayarav2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        ChromeDriver _driver;
        private void Form1_Load(object sender, EventArgs e)
        {
            _driver = new ChromeDriver();
            _driver.Navigate().GoToUrl("https://www.tayara.tn/login");
            _driver.FindElementById("login-tayara-phone").SendKeys("52855059");
            _driver.FindElementById("login-tayara-phone").Submit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _driver.FindElementById("login-tayara-code").SendKeys(textBox1.Text);
            _driver.FindElementById("login-tayara-code").Submit();
            //Thread.Sleep(5000);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            if (_driver.FindElementByXPath("//div[text()='Nizar Zaddem']") != null)
            {
                Console.WriteLine("connected");
            }
            var sb = new StringBuilder();
            foreach (var c in _driver.Manage().Cookies.AllCookies)
            {
                sb.Append($"{c.Name}={c.Value};");
            }
            File.WriteAllText("ses", sb.ToString());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _driver.Quit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var httpClientHandler = new HttpClientHandler() { UseCookies = false };
            var client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Add("cookie", File.ReadAllText("ses"));
            var json=await client.PostAsync()
        }
    }
}
