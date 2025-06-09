using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using ScottPlot;

namespace SDM3055
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private Socket rawSocket;
        private List<double> voltageData = new List<double>();
        private List<double> timeData = new List<double>();
        private double timeCounter = 0;
        private string autoLogFilePath = "AutoVoltageLog.csv";

        public Form1()
        {
            InitializeComponent();
            formsPlot1.Visible = false;

            button1.Visible = false;
            button2.Visible = false;

            formsPlot1.Plot.Axes.SetLimitsX(0, 10);
            formsPlot1.Plot.Axes.SetLimitsY(0, 10);
            formsPlot1.Plot.YLabel("Voltage(V)");
            formsPlot1.Plot.XLabel("Time(S)");

            formsPlot1.Refresh();
        }

        private void UpdateGraph(string voltageString)
        {
            if (double.TryParse(voltageString, out double voltage))
            {
                timeData.Add(timeCounter);
                voltageData.Add(voltage);

                dataGridViewLog.Rows.Add(timeCounter, voltage);
                if (dataGridViewLog.Rows.Count > 100)
                {
                    dataGridViewLog.Rows.RemoveAt(0);
                }

                AppendRowToCSV(autoLogFilePath, timeCounter, voltage);
                timeCounter += 1;

                if (timeData.Count > 100)
                {
                    timeData.RemoveAt(0);
                    voltageData.RemoveAt(0);
                }

                formsPlot1.Plot.Clear();
                formsPlot1.Plot.Add.Scatter(timeData.ToArray(), voltageData.ToArray());
                formsPlot1.Refresh();
            }
        }

        private void AppendRowToCSV(string filePath, double time, double voltage)
        {
            bool writeHeader = !File.Exists(filePath);
            using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                if (writeHeader)
                    writer.WriteLine("Time (S),Voltage (V)");

                writer.WriteLine($"{time},{voltage}");
            }
        }

        private void ExecuteCommand(string command)
        {
            if (rawSocket == null || !rawSocket.Connected)
            {
                MessageBox.Show("Not connected to SDM3055!");
                return;
            }
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(command + "\n");
                int totalSent = 0;
                while (totalSent < data.Length)
                {
                    int sent = rawSocket.Send(data, totalSent, data.Length - totalSent, SocketFlags.None);
                    if (sent <= 0) break;
                    totalSent += sent;
                }

                byte[] buffer = new byte[1024];
                int bytesReceived = rawSocket.Receive(buffer);
                string response = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

                txtResponse.Text = response.Trim();
                if (command.StartsWith("MEAS:VOLT"))
                {
                    UpdateGraph(response);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending command: " + ex.Message);
            }
        }

        private void CloseConnection()
        {
            try
            {
                rawSocket?.Shutdown(SocketShutdown.Both);
                rawSocket?.Close();
            }
            catch { }
            finally
            {
                rawSocket = null;
                client = null;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseConnection();
        }

        private void btnConnect_Click_1(object sender, EventArgs e)
        {
            try
            {
                rawSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                rawSocket.Connect(txtIPAddress.Text, 5025);
                MessageBox.Show("Connected to SDM3055!");
            }
            catch
            {
                MessageBox.Show("Connection failed!");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseConnection();
            MessageBox.Show("Disconnected from SDM3055!");
        }

        private void btnVoltage_Click_1(object sender, EventArgs e)
        {
            ExecuteCommand("MEAS:VOLT:DC?");
        }

        private void btnCurrent_Click_1(object sender, EventArgs e)
        { 
            ExecuteCommand("MEAS:CURR:AC?");
        }

        private void btnSend_Click_1(object sender, EventArgs e)
        {
            ExecuteCommand(txtCommand.Text);
        }

        private void btnResistance_Click_1(object sender, EventArgs e)
        {
            ExecuteCommand("MEAS:RES?");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ExecuteCommand("MEAS:VOLT:DC?");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridViewLog.Columns.Add("Time", "Time (S)");
            dataGridViewLog.Columns.Add("Voltage", "Voltage (V)");
        }

        private void btnShowGraph_Click(object sender, EventArgs e)
        {
            formsPlot1.Visible = true;
            button1.Visible = true;
            button2.Visible = true;
        }
    }
}
