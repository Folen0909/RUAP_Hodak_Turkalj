using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace Letter_Recognition
{
    public partial class HandwritingRecognition : Form
    {
        Pen pen;
        Point lastPoint;

        bool isMouseDown;
        static string letter;
        static List<double> probabilities;

        public HandwritingRecognition()
        {
            InitializeComponent();

            probabilities = new List<double>();
            pen = new Pen(Color.Black, 55);
            lastPoint = new Point();

            dataGridView1.ColumnCount = 26;

            for (int i = 0; i < 26; i++)
            {
                string character = ((char)(i+65)).ToString();
                dataGridView1.Columns[i].Name = character;
            }
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            resultBtn.Enabled = false;
            submitBtn.Enabled = false;
        }
        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = e.Location;
            isMouseDown = true;
        }
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (isMouseDown == true)
            {
                
                if (lastPoint != null)
                {
                    submitBtn.Enabled = true;
                    if (canvas.Image == null)
                    {
                        Bitmap bmp = new Bitmap(canvas.Width, canvas.Height);
                        canvas.Image = bmp;
                    }
                    using (Graphics g = Graphics.FromImage(canvas.Image))
                    {
                        pen.StartCap = pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.DrawLine(pen, lastPoint, e.Location);
                    }
                    
                    canvas.Invalidate();
                    lastPoint = e.Location;
                }
            }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            
            isMouseDown = false;
            lastPoint = Point.Empty;
            
        }
        private void clearBtn_Click(object sender, EventArgs e)
        {
            submitBtn.Enabled = false;
            PredictionLabel.Text = "";
            probabilities.Clear();
            dataGridView1.Rows.Clear();

            if (canvas.Image != null)
            {
                canvas.Image = null;
                Invalidate();
            }
        }
        private void resultBtn_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            PredictionLabel.Text = letter;
            resultBtn.Enabled = false;
            submitBtn.Enabled = false;

            dataGridView1.Rows.Add();

            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Rows[0].Cells[i].Value = probabilities[25-i];
            }
        }
        private void submitBtn_Click(object sender, EventArgs e)
        {
            submitBtn.Enabled = false;
            probabilities.Clear();

            ParseDataToWebService();
            
            resultBtn.Enabled = true;
        }

        static async Task InvokeRequestResponseService(List<Dictionary<string, string>> jsonList)
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, List<Dictionary<string, string>>>() {
                        {
                            "input1",
                            jsonList
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                const string apiKey = "U31tpjuwO73Et6728s4lWdBTE66NBPnDBIVJUy8ZKpcGY0IRMU+6+q0CKMAq3ZDw4yTpDR0Dk8dJ2wvceYczDg==";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/5dca254c248b478aa3953e9b7b25ba3f/services/491546f09a004c2784d9d8eb7a4201cf/execute?api-version=2.0&format=swagger");
                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    GetPredictionValues(result);
                }
                else
                {
                    Debug.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));
                    Debug.WriteLine(response.Headers.ToString());
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(responseContent);
                }
            }
        }

        void ParseDataToWebService()
        {
            int counter = 1;

            canvas.Image.Save("canvas.bmp");

            Bitmap image = new Bitmap("canvas.bmp");
            Bitmap resized = new Bitmap(image, new Size(28, 28));
            resized.Save("resized.bmp");

            List<Dictionary<string, string>> jsonList = new List<Dictionary<string, string>>();
            Dictionary<string, string> word = new Dictionary<string, string>();

            word.Add("Col1", "0");

            for (int x = 0; x < 28; x++)
            {
                for (int y = 0; y < 28; y++)
                {
                    counter++;
                    word.Add(("Col" + counter), resized.GetPixel(y, x).A.ToString());

                }
            }

            jsonList.Add(word);
            image.Dispose();
            InvokeRequestResponseService(jsonList).Wait();
        }
        
        static void GetPredictionValues(string result)
        {
            string[] results = result.Split(',');
            for (int i = results.Length - 2; i > results.Length - 28; i--)
            {
                double probability = Convert.ToDouble(results[i].Split(':')[1].Replace("\"", ""));
                double roundValue = Math.Round(probability, 4);
                probabilities.Add(roundValue);
            }
            string[] predictionLine = results[results.Length - 1].Split(':', '}');
            int asciiValue = Convert.ToInt32(predictionLine[1].Replace("\"", "")) + 65;
            letter = ((char)asciiValue).ToString();
        }


    }
}
