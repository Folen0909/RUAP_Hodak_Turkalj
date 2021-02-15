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


namespace Letter_Recognition
{
    public partial class Form1 : Form
    {
        Pen pen;

        Point lastPoint = Point.Empty;

        bool isMouseDown = new Boolean();

        static string letter = "";

        public Form1()
        {
            InitializeComponent();
            pen = new Pen(Color.Black, 20);
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            resultBtn.Enabled = false;
            submitBtn.Enabled = true;
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
            PredictionLabel.Text = "";
            if (canvas.Image != null)
            {
                canvas.Image = null;
                Invalidate();
            }
        }
        private void resultBtn_Click(object sender, EventArgs e)
        {
            PredictionLabel.Text = letter;
            resultBtn.Enabled = false;
            submitBtn.Enabled = true;
        }
        private void submitBtn_Click(object sender, EventArgs e)
        {
            canvas.Image.Save("canvas.bmp");
            submitBtn.Enabled = false;
            Bitmap image = new Bitmap("canvas.bmp");
            Bitmap resized = new Bitmap(image, new Size(image.Width / 20, image.Height / 20));
            resized.Save("resized.bmp");
            int counter = 1;
            List<Dictionary<string, string>> jsonList = new List<Dictionary<string, string>>();
            Dictionary<string, string> word = new Dictionary<string, string>();
            word.Add("Col1", "0");
            counter++;
            for (int x = 0; x < 28; x++)
            {
                for (int y = 0; y < 28; y++)
                {
                    word.Add(("Col" + counter), resized.GetPixel(y, x).A.ToString());
                    counter++;
                }
            }
            jsonList.Add(word);
            image.Dispose();
            InvokeRequestResponseService(jsonList).Wait();
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
                    string[] results = result.Split(',');
                    string[] predictionLine = results[results.Length - 1].Split(':', '}');
                    switch (Convert.ToInt32(predictionLine[1].Replace("\"", "")))
                    {
                        case 0:
                            {
                                letter = "A";
                                break;
                            }
                        case 1:
                            {
                                letter = "B";
                                break;
                            }
                        case 2:
                            {
                                letter = "C";
                                break;
                            }
                        case 3:
                            {
                                letter = "D";
                                break;
                            }
                        case 4:
                            {
                                letter = "E";
                                break;
                            }
                        case 5:
                            {
                                letter = "F";
                                break;
                            }
                        case 6:
                            {
                                letter = "G";
                                break;
                            }
                        case 7:
                            {
                                letter = "H";
                                break;
                            }
                        case 8:
                            {
                                letter = "I";
                                break;
                            }
                        case 9:
                            {
                                letter = "J";
                                break;
                            }
                        case 10:
                            {
                                letter = "K";
                                break;
                            }
                        case 11:
                            {
                                letter = "L";
                                break;
                            }
                        case 12:
                            {
                                letter = "M";
                                break;
                            }
                        case 13:
                            {
                                letter = "N";
                                break;
                            }
                        case 14:
                            {
                                letter = "O";
                                break;
                            }
                        case 15:
                            {
                                letter = "P";
                                break;
                            }
                        case 16:
                            {
                                letter = "Q";
                                break;
                            }
                        case 17:
                            {
                                letter = "R";
                                break;
                            }
                        case 18:
                            {
                                letter = "S";
                                break;
                            }
                        case 19:
                            {
                                letter = "T";
                                break;
                            }
                        case 20:
                            {
                                letter = "U";
                                break;
                            }
                        case 21:
                            {
                                letter = "V";
                                break;
                            }
                        case 22:
                            {
                                letter = "W";
                                break;
                            }
                        case 23:
                            {
                                letter = "X";
                                break;
                            }
                        case 24:
                            {
                                letter = "Y";
                                break;
                            }
                        case 25:
                            {
                                letter = "Z";
                                break;
                            }
                    }

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
    }
}
