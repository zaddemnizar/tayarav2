using ExcelHelperExe;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
        HttpClient client;
        private void Form1_Load(object sender, EventArgs e)
        {
            var httpClientHandler = new HttpClientHandler()
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Add("cookie", File.ReadAllText("ses"));

        }

        private void button2_Click(object sender, EventArgs e)
        {
            _driver.FindElementById("login-tayara-code").SendKeys(textBox1.Text);
            _driver.FindElementById("login-tayara-code").Submit();
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

        private async void button1_Click(object sender, EventArgs e)
        {
            _driver = new ChromeDriver();
            _driver.Navigate().GoToUrl("https://www.tayara.tn/login");
            _driver.FindElementById("login-tayara-phone").SendKeys("52855059");
            _driver.FindElementById("login-tayara-phone").Submit();
            var httpClientHandler = new HttpClientHandler()
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Add("cookie", File.ReadAllText("ses"));
            var json = await client.PostAsync("https://www.tayara.tn/bff/verify-session", null);
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            var annonce = ImportExcel();
            var json = JsonConvert.SerializeObject(annonce);

            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var post = await client.PostAsync("https://www.tayara.tn/graphql", stringContent);
        }

        private static AnnonceImmobilier ImportExcel()
        {
            //using ExcelHelperEx librairie
            var inputs = @"D:\Nizar\Projets\tayarav2\tayarav2\bin\annonce.xlsx".ReadFromExcel<ExcelInput>();
            var input = inputs.First();

            //initialising class AnnonceImmobilier
            var annonce = new AnnonceImmobilier
            {
                operationName = input.operationName,
                variables = new Variables
                {
                    input = new Input
                    {
                        title = input.title,
                        description = input.description,
                        price = input.price,
                        images = input.images,
                        category = input.category,
                        currency = "TND",
                        metadata = new List<Metadata>
                        {
                            new Metadata {key="transactionType",value=input.transactionType},
                            new Metadata {key="rooms",numericValue=input.rooms},
                            new Metadata {key="baathrooms",numericValue=input.bathrooms},
                            new Metadata {key="area",numericValue=input.area},
                        }
                    }
                }
            };
            return annonce;
        }
    }
}
