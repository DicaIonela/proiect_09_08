using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
//using System.Windows.Forms;
using System.Data.SqlTypes;
using System.IO.Ports;
//using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Timers;
using System.Globalization;
using System.Xml.Linq;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;
using GSM_WOL_Proiect;
namespace GSM_WOL_Proiect
{
    public static class Metode
    {
        /*INITIALIZARI PENTRU A PUTEA ASCUNDE CONSOLA LA RULARE*/
        //[DllImport("kernel32.dll")]/*Se importa functii pentru a ascunde/afisa consola*/
        //static extern IntPtr GetConsoleWindow();
        //[DllImport("user32.dll")]
        //static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //const int SW_HIDE = 0;
        //const int SW_SHOW = 5;
        static SerialPort serialPort;
        static StringBuilder dataBuffer = new StringBuilder();
        [STAThread]
        public static void StartCommandPromptMode()
        {
            ListenToSerialPort();
            string option;
            do
            {
                option = Console.ReadLine();
            } while (option.ToUpper() != "X");
            if (option.ToUpper() == "X")
            {
                serialPort.Close();
            }
        }
        static void ListenToSerialPort()
        {
            var serialPortSection = (NameValueCollection)ConfigurationManager.GetSection("serialPortSection");

            if (serialPortSection == null)
            {
                LogError("Sectiunea de configurare pentru portul serial nu a fost gasita.");
                return;
            }
            string portName = serialPortSection["PortName"];
            int baudRate = int.Parse(serialPortSection["BaudRate"]);
            Parity parity = (Parity)Enum.Parse(typeof(Parity), serialPortSection["Parity"]);
            int dataBits = int.Parse(serialPortSection["DataBits"]);
            StopBits stopBits = (StopBits)Enum.Parse(typeof(StopBits), serialPortSection["StopBits"]);
            Handshake handshake = (Handshake)Enum.Parse(typeof(Handshake), serialPortSection["Handshake"]);
            serialPort = new SerialPort(portName);
            serialPort.BaudRate = baudRate;
            serialPort.Parity = parity;
            serialPort.DataBits = dataBits;
            serialPort.StopBits = stopBits;
            serialPort.Handshake = handshake; /*Fara control al fluxului*/
            /*Evenimentul care se declanseaza cand se primesc date*/
            try
            {
                /*Deschide portul serial*/
                serialPort.Open();

                SendCommand("AT");
                System.Threading.Thread.Sleep(500);
                string indata = serialPort.ReadExisting();
                if (!indata.Contains("OK"))
                {
                    LogError("Conexiune nereusita a portului serial.");
                    return;
                }

                Console.WriteLine("Portul serial este deschis. Ascultând date...");
                SendCommand("AT+CLIP=1");
                SendCommand("AT+CNMI=1,1,0,0,0");
                serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            }
            catch (Exception ex)
            {
                LogError("Eroare la deschiderea portului serial: " + ex.Message);
            }
            //serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }
        public static void CloseSerialPort()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                serialPort.Dispose();
                serialPort = null;
                Console.WriteLine("Port inchis.");
            }
        }
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            System.Threading.Thread.Sleep(500);
            string indata = sp.ReadExisting();
            dataBuffer.Append(indata);
            if (indata.Contains("+CMTI:"))
            {
                string[] data = indata.Split(',');
                string index = data[1];
                SendCommand("AT + CMGF = 1");
                System.Threading.Thread.Sleep(500);
                SendCommand("AT+CMGR=" + index);
                System.Threading.Thread.Sleep(500);
            }

            if ((indata.Contains("+CMGR") || indata.Contains("REC UNREAD") || indata.Contains("+CMGL")) && !indata.Contains("CLIP"))
            {
                ProcessMessageBuffer();  // Procesează datele de mesaj
                dataBuffer.Clear();
            }
            else if (indata.Contains("+CLIP"))
            {
                ProcessBufferCall();  // Procesează datele de apel
                dataBuffer.Clear();
                //SendCommand("AT");
            }
        }
        private static readonly string logFilePath = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName).FullName, "erori.log");
        private static void LogError(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true)) // Deschide sau creează fișierul și adaugă la sfârșit
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la scrierea în fișierul de log: {ex.Message}");
            }
        }
        private static void ProcessBufferCall()
        {
            Init.Initialize(out User[] users); /*initializari*/
            string delimiter = "\nRING";/*delimitator pentru a verifica daca buffer ul a stocat toate datele complete*/
            while (dataBuffer.ToString().Contains(delimiter))
            {
                /*Extrage mesajul complet din buffer*/
                int delimiterIndex = dataBuffer.ToString().IndexOf(delimiter) + 5;
                int stopIndex = dataBuffer.ToString().Length - 1;
                string completeMessage = dataBuffer.ToString().Substring(delimiterIndex);
                /*Elimina mesajul complet din buffer*/
                dataBuffer.Remove(0, delimiterIndex + delimiter.Length);
                string cleanedMessage = completeMessage.Trim();
                string callerNumber = ExtractPhoneNumber(cleanedMessage);
                if (cleanedMessage.Contains("+CLIP:"))
                {
                    Console.WriteLine("----Phone Call Received----");
                    Console.WriteLine("Number:" + callerNumber);
                    SendCommand("ATH");
                    for (int i = 0; i < 5; i++)
                    {
                        SendCommand("ATH");
                        //System.Threading.Thread.Sleep(100);
                    }
                    if (ConfigHelper.SearchUsers(callerNumber) != null && callerNumber.Length > 11)
                    {
                        User[] FoundUsers= ConfigHelper.SearchUsers(callerNumber);
                        UserFunctions.ShowUsers(FoundUsers, FoundUsers.Length);
                        if (FoundUsers.Length == 1)
                        {
                            try
                            {
                                // Trimiterea pachetului WoL
                                WakeOnLan.SendWakeOnLan(FoundUsers[0].MACaddress);
                                Console.WriteLine("The WoL packet has been sent.");
                            }
                            catch (Exception ex)
                            {
                                LogError("Eroare la trimiterea pachetului WoL: " + ex.Message);
                            }
                        }
                        else
                            LogError($"Numar '{callerNumber}'. Acces neautorizat.");
                    }
                    SendCommand("AT");
                }
            }
        }
        private static void ProcessMessageBuffer()
        {
            Init.InitializeTB(out TestBench[] testBenches);
            Init.Initialize(out User[] users);
            string delimiter = "+CMGR:"; // Delimitator pentru mesaje
            string data = dataBuffer.ToString();
            string[] separatedData = data.Split(new string[] { delimiter }, StringSplitOptions.None);
            dataBuffer.Clear(); // Resetează buffer-ul pentru a procesa datele viitoare
            if (separatedData[1].Contains("REC UNREAD"))
            {
                Console.WriteLine("----Message Received----");
                string[] parti = separatedData[1].Split('\n');
                string[] parts = parti[0].Split(',');
                string phoneNumber = ExtractPhoneNumberFromParts(parts);
                string message = "";
                Console.WriteLine("Number: " + phoneNumber);
                if (parti.Length > 1)
                {
                    message = parti[1];
                    Console.WriteLine("Message: " + message);
                }
                else
                {
                    LogError($"Preluarea mesajului de la utilizatorul '{phoneNumber}' a intampinat erori.");
                }
                if (ConfigHelper.SearchUsers(phoneNumber).Length > 0 && phoneNumber.Length > 11)
                {
                    TestBench[] FoundTestBenches = ConfigHelper.SearchTB(message);
                    if (FoundTestBenches != null)
                    {
                        UserFunctions.ShowTB(FoundTestBenches, FoundTestBenches.Length);
                        if (FoundTestBenches.Length == 1)
                        {
                            try
                            {
                                // Trimiterea pachetului WoL
                                WakeOnLan.SendWakeOnLan(FoundTestBenches[0].MACaddress);
                                Console.WriteLine("The WoL packet has been sent.");
                            }
                            catch (Exception ex)
                            {
                                LogError("Eroare la trimiterea pachetului WoL: " + ex.Message);
                            }
                        }
                    }
                    else
                    {
                        LogError("Test bench nerecunoscut.");
                    }
                }
                else
                {
                    LogError($"Utilizator '{phoneNumber}' neautorizat. Acces refuzat.");
                }
            }
        }
        private static string ExtractPhoneNumberFromParts(string[] parts)
        {
            // Extrage numărul de telefon din partea de informații (de obicei în formatul "079104770000")
            if (parts.Length >= 3)
            {
                string phoneNumberPart = parts[1].Trim('"'); // Poate fi nevoie să adaptezi indexul
                phoneNumberPart = FormatPhoneNumber(phoneNumberPart);
                return phoneNumberPart;
            }
            return string.Empty;
        }
        private static string FormatPhoneNumber(string number)
        {
            // Înlătură prefixul "+" și transformă în "004"
            if (number.StartsWith("+40"))
            {
                return "0040" + number.Substring(3); // Formatează în "00407"
            }
            else if (number.StartsWith("40"))
            {
                return "0040" + number.Substring(2); // Formatează în "00407"
            }
            else if (number.StartsWith("07") && number.Length == 10)
            {
                return "004" + number; // Adaugă prefixul "0040"
            }
            else
            {
                // Returnează numărul original dacă nu se potrivește niciun format specificat
                return number;
            }
        }
        private static void SendCommand(string command)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    /*Adauga o noua linie dupa comanda AT*/
                    serialPort.WriteLine(command + "\r");
                    //Console.WriteLine("Comandă trimisă: " + command);
                }
                catch (Exception ex)
                {
                    LogError("Eroare la trimiterea comenzii: " + ex.Message);
                }
            }
        }
        private static string ExtractPhoneNumber(string message)
        {
            /*Cauta inceputul si sfarsitul numarului de telefon intre ghilimele*/
            int startIndex = message.IndexOf('"') + 1;
            int endIndex = message.IndexOf('"', startIndex);
            /*Verifica daca indecsii sunt valizi*/
            if (startIndex > 0 && endIndex > startIndex)
            {
                string phoneNumber = message.Substring(startIndex, endIndex - startIndex);
                phoneNumber = FormatPhoneNumber(phoneNumber);
                return phoneNumber;
            }
            return string.Empty; /*Returneaza un sir gol daca numarul nu a fost gasit*/
        }
    }
}