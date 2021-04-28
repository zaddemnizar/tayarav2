using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
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
        private void Form1_Load(object sender, EventArgs e)
        {

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
            var httpClientHandler = new HttpClientHandler()
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var client = new HttpClient(httpClientHandler);

            var annonce = ImportExcel();
            var json = JsonConvert.SerializeObject(annonce);

            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var post = await client.PostAsync("https://www.tayara.tn/graphq1", stringContent);
        }

        private static AnnonceImmobilier ImportExcel()
        {
            //assign the different excel elements
            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;

            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(@"D:\Nizar\Projets\tayarav2\tayarav2\bin\annonce.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets["Feuil1"];

            //Intialiser toutes les class
            var annonce = new AnnonceImmobilier();
            var variables = new Variables();
            var input = new Input();
            var metadata = new List<Metadata>();

            annonce.variables = variables;
            annonce.variables.input = input;
            annonce.variables.input.metadata = metadata;

            //Recherche des différents éléments de l'annonce
            var rowVal = xlWorkSheet.Rows.Find("operationName").Cells.Row;
            annonce.operationName = xlWorkSheet.Cells[rowVal, 2].Text.ToString();

            rowVal = xlWorkSheet.Rows.Find("title").Cells.Row;
            annonce.variables.input.title = xlWorkSheet.Cells[rowVal, 2].Text.ToString();

            rowVal = xlWorkSheet.Rows.Find("description").Cells.Row;
            annonce.variables.input.description = xlWorkSheet.Cells[rowVal, 2].Text.ToString();

            rowVal = xlWorkSheet.Rows.Find("price").Cells.Row;
            annonce.variables.input.price = xlWorkSheet.Cells[rowVal, 2].Text.ToString();

            rowVal = xlWorkSheet.Rows.Find("category").Cells.Row;
            annonce.variables.input.category = xlWorkSheet.Cells[rowVal, 2].Text.ToString();

            rowVal = xlWorkSheet.Rows.Find("subdivisionId").Cells.Row;
            annonce.variables.input.subdivisionId = xlWorkSheet.Cells[rowVal, 2].Text.ToString();

            rowVal = xlWorkSheet.Rows.Find("images").Cells.Row;
            annonce.variables.input.images = xlWorkSheet.Cells[rowVal, 2].Text.ToString();

            var rowValM = new List<int>();
            rowValM.Add(xlWorkSheet.Rows.Find("transactionType").Cells.Row);
            rowValM.Add(xlWorkSheet.Rows.Find("rooms").Cells.Row);
            rowValM.Add(xlWorkSheet.Rows.Find("bathrooms").Cells.Row);
            rowValM.Add(xlWorkSheet.Rows.Find("area").Cells.Row);

            for (int i = 0; i < 4; i++)
            {
                metadata.Add(new Metadata(xlWorkSheet.Cells[rowValM[i], 1].Text.ToString(), xlWorkSheet.Cells[rowValM[i], 2].Text.ToString(), 0));
                //metadata.Add(new Metadata(xlWorkSheet.Cells[rowValM[i], 1].Text.ToString(), xlWorkSheet.Cells[rowValM[i], 2].Text.ToString(), Int32.Parse(xlWorkSheet.Cells[rowValM[i], 2].Text)));
            }

            //rowVal = xlWorkSheet.Rows.Find("transactionType").Cells.Row;
            //annonce.variables.input.metadata[0].key = xlWorkSheet.Cells[rowVal, 1].Text.ToString();
            //annonce.variables.input.metadata[0].value = xlWorkSheet.Cells[rowVal, 2].Text.ToString();

            //rowVal = xlWorkSheet.Rows.Find("rooms").Cells.Row;
            //annonce.variables.input.metadata[1].key = xlWorkSheet.Cells[rowVal, 1].Text.ToString();
            //annonce.variables.input.metadata[1].numericValue = xlWorkSheet.Cells[rowVal, 2].Text;

            //rowVal = xlWorkSheet.Rows.Find("bathrooms").Cells.Row;
            //annonce.variables.input.metadata[2].key = xlWorkSheet.Cells[rowVal, 1].Text.ToString();
            //annonce.variables.input.metadata[2].numericValue = xlWorkSheet.Cells[rowVal, 2].Text;

            //rowVal = xlWorkSheet.Rows.Find("area").Cells.Row;
            //annonce.variables.input.metadata[3].key = xlWorkSheet.Cells[rowVal, 1].Text.ToString();
            //annonce.variables.input.metadata[3].numericValue = xlWorkSheet.Cells[rowVal, 2].Text;

            return annonce;
        }
    }
}
