# SDM3055 Digital Multimeter GUI

This is a Windows Forms (C#) application for communicating with the **Siglent SDM3055** digital multimeter over **Ethernet (TCP/IP)** using SCPI commands.

## 💡 Features

- Connect to SDM3055 via IP and port 5025
- Measure:
  - DC Voltage
  - AC Current
  - Resistance
- View response in real-time
- Plot **Voltage vs Time** using ScottPlot
- Export measurement data to CSV
- Optional: Send manual SCPI commands and view replies

## 📡 Technologies Used

- .NET Framework / WinForms (C#)
- ScottPlot (for graphing)
- TCP/IP sockets (System.Net.Sockets)
- Optional raw Ethernet: SharpPcap + PacketDotNet

## 🧪 How to Use

1. Build the project in **Visual Studio 2022**.
2. Connect your PC and SDM3055 to the same LAN.
3. Enter the multimeter’s IP address and click `Connect`.
4. Use the GUI buttons to:
   - Measure voltage/current/resistance
   - Start/stop auto plotting
   - Send custom SCPI commands
5. View results in text and graph.
6. Export logged data from the table to `.csv`.
