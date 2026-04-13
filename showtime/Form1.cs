using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace showtime
{
    public partial class Form1 : Form
    {
        UdpClient U;
        Thread Th;

        public Form1()
        {
            InitializeComponent();
            label3.Enabled = true;
        }

        //監聽副程序
        private void button1_Click(object sender, EventArgs e)
        {
            if (U != null)
            {
                MessageBox.Show("監聽已啟動！");
                return;
            }

            Th = new Thread(Listen);
            Th.IsBackground = true;
            Th.Start();
            button1.Enabled = false;
        }

        private volatile bool listening = false;

        private void Listen()
        {
            if (!int.TryParse(textBox1.Text, out int Port))
            {
                MessageBox.Show("請輸入正確的監聽 Port");
                return;
            }

            try
            {
                U = new UdpClient(Port);
            }
            catch (SocketException)
            {
                MessageBox.Show("Port 已被占用，請換一個");
                return;
            }

            IPEndPoint EP = new IPEndPoint(IPAddress.Any, Port);

            listening = true;

            while (listening)
            {
                try
                {
                    byte[] B = U.Receive(ref EP);
                    string msg = Encoding.UTF8.GetString(B);

                    this.Invoke((MethodInvoker)(() =>
                    {
                        textBox2.AppendText(msg + Environment.NewLine);
                    }));
                }
                catch (ObjectDisposedException)
                {
                    // UdpClient 被關閉時，Receive 會拋此例外，跳出迴圈
                    break;
                }
                catch (SocketException)
                {
                    // 監聽過程中 Socket 例外，可視需求處理
                    break;
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                listening = false;
                U?.Close();
                // 不用 Thread.Abort，等執行緒自然結束
                Th?.Join(1000); // 等待執行緒結束
            }
            catch { }

            base.OnFormClosing(e);
        }

        //發送 UDP 訊息
        private void button2_Click(object sender, EventArgs e)
        {
            string IP = textBox3.Text;
            //設定發送目標 IP
            int Port = int.Parse(textBox4.Text);
            //設定發送目標 Port
            byte[] B = Encoding.UTF8.GetBytes(textBox5.Text);
            //字串翻譯成位元組陣列
            UdpClient S = new UdpClient();
            // 建立 UDP 通訊器
            S.Send(B, B.Length, IP, Port);
            //發送資料到指定位置
            S.Close();
            //關閉通訊器

        }

        // 找出本機的 IPv4 地址
        private string MyIP()
        {
            string hn = Dns.GetHostName(); // 取得本機電腦名稱
            IPAddress[] ip = Dns.GetHostEntry(hn).AddressList; // 取得本機 IP 陣列

            // 遍歷所有的 IP 地址
            foreach (IPAddress it in ip)
            {
                // 檢查是否為 IPv4 地址
                if (it.AddressFamily == AddressFamily.InterNetwork)
                {
                    string addr = it.ToString();

                    //  只要 10.x.x.x
                    if (addr.StartsWith("10."))
                    {
                        return addr; // 找到直接回傳
                    }
                }
            }

            return "No 10.x.x.x IP found"; // 如果沒有
        }

        // 表單載入
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text += " " + MyIP();
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }


        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void 接聽port_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}