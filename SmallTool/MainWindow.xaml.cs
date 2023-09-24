using System.Diagnostics;
using System.Windows;
using System.Management;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.Win32;
using System.Windows.Input;
using System.Net;

namespace SmallTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> actoffice_commands = new List<string>
        {
            "cscript //Nologo \"C:\\Program Files\\Microsoft Office\\Office16\\ospp.vbs\" /sethst:kms.03k.org",

            "cscript //Nologo \"C:\\Program Files\\Microsoft Office\\Office16\\ospp.vbs\" /inpkey:FXYTK-NJJ8C-GB6DW-3DYQT-6F7TH",
            // "cscript //Nologo \"C:\\Program Files\\Microsoft Office\\Office16\\ospp.vbs\" /inpkey:KNH8D-FGHT4-T8RK3-CTDYJ-K2HT4",
            // "cscript //Nologo \"C:\\Program Files\\Microsoft Office\\Office16\\ospp.vbs\" /inpkey:FTNWT-C6WBT-8HMGF-K9PRX-QV9H8",
            "cscript //Nologo \"C:\\Program Files\\Microsoft Office\\Office16\\ospp.vbs\" /act",

        };

        List<string> commands = new List<string>
                {
                    "cscript //Nologo %windir%\\system32\\slmgr.vbs /upk",
                    "cscript //Nologo %windir%\\system32\\slmgr.vbs /ipk W269N-WFGWX-YVC9B-4J6C9-T83GX",
                    "cscript //Nologo %windir%\\system32\\slmgr.vbs /skms kms.03k.org",
                    "cscript //Nologo %windir%\\system32\\slmgr.vbs /ato"
                };
        string result = string.Empty;
        public class Node
        {
            public string Name { get; set; }
            public string Ip { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            PCName.Text = Environment.MachineName;
            IP.Text = GetIpV4();
            NetCard.Text = GetNetCard()[1];
            CheckisexistAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            if (checkbox.IsChecked == false)
            {
                ChangeIpAndName();
                MessageBox.Show(GetIpV4() == IP.Text ? "修改成功" : "修改失败");
            }
            else
            {
                List<string> netcardlist = GetNetCard();
                SaveToJson(netcardlist[0], PCName.Text, IP.Text);
                MessageBox.Show($"{netcardlist[0]}已保存");
            }

        }

        private string CmdRun(string command)
        {
            Process process = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            process.StartInfo = startInfo;
            process.Start();
            process.StandardInput.WriteLine(command);
            process.StandardInput.WriteLine("exit");
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            return output;
        }


        private void ChangeIpAndName()
        {
            string ip = IP.Text;
            string host = string.Join(".", ip.Split('.')[0], ip.Split('.')[1], ip.Split('.')[2], "1");
            string gateway = "255.255.255.0";
            string dns1 = "114.114.114.114";
            string dns2 = "8.8.8.8";
            string netcard = GetNetCard()[1];

            string command = $"netsh interface ip set address \"{netcard}\" static {ip} {gateway} {host}";
            CmdRun(command);
            command = $"netsh interface ip set dns \"{netcard}\" static {dns1} primary";
            CmdRun(command);
            command = $"netsh interface ip add dns \"{netcard}\" {dns2} index=2";
            CmdRun(command);

            #region 更改电脑名称
            RegistryKey pregkey;
            pregkey = Registry.LocalMachine.OpenSubKey("SYSTEM\\ControlSet001\\Control\\ComputerName\\ComputerName", true);
            pregkey.SetValue("ComputerName", PCName.Text);
            pregkey = Registry.LocalMachine.OpenSubKey("SYSTEM\\ControlSet001\\Services\\Tcpip\\Parameters", true);
            pregkey.SetValue("NV Hostname", PCName.Text);
            pregkey.SetValue("Hostname", PCName.Text);
            pregkey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ComputerName", true);
            pregkey.SetValue("ComputerName", PCName.Text);
            pregkey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters", true);
            pregkey.SetValue("NV Hostname", PCName.Text);
            #endregion
        }

        private List<string> GetNetCard()
        {
            List<string> NetCardList = new List<string>();

            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter");
            foreach (var obj in searcher.Get())
            {

                //Console.WriteLine(obj["PNPDeviceID"]);

                if (obj["PNPDeviceID"] != null && obj["PNPDeviceID"].ToString().Contains("PCI"))
                {
                    NetCardList.Add(obj["MACAddress"].ToString());
                    NetCardList.Add(obj["NetConnectionID"].ToString());
                }
            }
            return NetCardList;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            btn.Content = "SaveToJson";
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            btn.Content = "ChangeInfo";
        }

        private void SaveToJson(string macadress, string name, string ip)
        {

            #region 读取json并转换成字典
            string filePath = @"Data.json";

            string content = File.ReadAllText(filePath);

            JObject jsonbj = JObject.Parse(content);

            Dictionary<string, Node> node = JsonConvert.DeserializeObject<Dictionary<string, Node>>(jsonbj.ToString());
            #endregion

            node[macadress] = new Node() { Name = name, Ip = ip };

            string jsonstring = JsonConvert.SerializeObject(node, Formatting.Indented);

            System.IO.File.WriteAllText(filePath, jsonstring);

        }

        private string GetIpV4()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                //判断是否为以太网卡
                //Wireless80211         无线网卡    Ppp     宽带连接
                //Ethernet              以太网卡   
                //这里篇幅有限贴几个常用的，其他的返回值大家就自己百度吧！
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    //获取以太网卡网络接口信息
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    //获取单播地址集
                    UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                    foreach (UnicastIPAddressInformation ipadd in ipCollection)
                    {
                        //InterNetwork    IPV4地址      InterNetworkV6        IPV6地址
                        //Max            MAX 位址
                        if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ipadd.Address.ToString();
                        }
                        //判断是否为ipv4                  
                    }
                    return "";
                }
                else
                {
                    return "";
                }

            }
            return "";
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            megbox secwin = new megbox();
            secwin.Owner = this;
            secwin.megboxlbl.Content = "windows激活中...";
            secwin.Show();

            foreach (var command in commands)
            {
                result = CmdRun(command);
            }
            if (result.Contains("成功地激活"))
            {
                secwin.megboxlbl.Content = "激活成功";
                // MessageBox.Show("激活成功");
            }
            else
            {
                secwin.megboxlbl.Content = "激活成失败";
                // MessageBox.Show("激活失败");
            }
        }

        private void ButtonBase_OnClick1(object sender, RoutedEventArgs e)
        {
            megbox secwin = new megbox();
            secwin.Owner = this;
            secwin.megboxlbl.Content = "office激活中...";
            secwin.Show();

            foreach (var command in actoffice_commands)
            {
                result = CmdRun(command);
            }
            if (result.Contains("Product activation successful"))
            {
                secwin.megboxlbl.Content = "激活成功";
                // MessageBox.Show("激活成功");
            }
            else
            {
                secwin.megboxlbl.Content = "激活成失败";
                // MessageBox.Show("激活失败");
            }
        }

        private void CheckisexistAsync()
        {
            string content = "";
            string networkFileUrl = "http://10.0.1.18/DATA.JSON"; // 替换为网络文件的URL
            string localFilePath = "Data.json"; // 本地文件的路径

            try
            {
                WebClient client = new WebClient();
                content = client.DownloadString(networkFileUrl);
            }
            catch (WebException)
            {
                // 如果网络读取失败，尝试从本地读取文件
                try
                {
                    content = File.ReadAllText(localFilePath);

                    // 如果成功从本地读取文件，将内容写入TextBox
                }
                catch (Exception ex)
                {
                    // 处理本地文件读取失败的异常
                    MessageBox.Show("无法读取文件：" + ex.Message);
                }
            }


            JObject jsonstring = JObject.Parse(content);

            Dictionary<string, Node> node =
                JsonConvert.DeserializeObject<Dictionary<string, Node>>(jsonstring.ToString());

            string netcardmac = GetNetCard()[0];

            foreach (var mac in node)
            {
                if (mac.Key == netcardmac)
                {
                    autochangebtn.Visibility = Visibility.Visible;
                    break;
                }
            }
        }



        private void autochangebtn_Click(object sender, RoutedEventArgs e)
        {
            //read json
            string filePath = "Data.json";

            string content = File.ReadAllText(filePath);
            JObject jsonstring = JObject.Parse(content);

            Dictionary<string, Node> node =
                JsonConvert.DeserializeObject<Dictionary<string, Node>>(jsonstring.ToString());

            var machineMac = GetNetCard()[0];

            foreach (var mac in node)
            {
                if (mac.Key == machineMac)
                {
                    var ip = mac.Value.Ip;
                    var pcName = mac.Value.Name;

                    if (ip != GetIpV4() || Environment.MachineName != pcName)
                    {
                        string host = string.Join(".", ip.Split('.')[0], ip.Split('.')[1], ip.Split('.')[2], "1");
                        string gateway = "255.255.255.0";
                        string dns1 = "114.114.114.114";
                        string dns2 = "8.8.8.8";
                        string netcard = GetNetCard()[1];

                        string command = $"netsh interface ip set address \"{netcard}\" static {ip} {gateway} {host}";
                        CmdRun(command);
                        command = $"netsh interface ip set dns \"{netcard}\" static {dns1} primary";
                        CmdRun(command);
                        command = $"netsh interface ip add dns \"{netcard}\" {dns2} index=2";
                        CmdRun(command);

                        #region 更改电脑名称
                        RegistryKey pregkey;
                        pregkey = Registry.LocalMachine.OpenSubKey("SYSTEM\\ControlSet001\\Control\\ComputerName\\ComputerName", true);
                        pregkey.SetValue("ComputerName", pcName);
                        pregkey = Registry.LocalMachine.OpenSubKey("SYSTEM\\ControlSet001\\Services\\Tcpip\\Parameters", true);
                        pregkey.SetValue("NV Hostname", pcName);
                        pregkey.SetValue("Hostname", pcName);
                        pregkey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ComputerName", true);
                        pregkey.SetValue("ComputerName", pcName);
                        pregkey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters", true);
                        pregkey.SetValue("NV Hostname", pcName);
                        #endregion
                        MessageBox.Show("已更改");

                    }
                }
            }
        }
    }
}