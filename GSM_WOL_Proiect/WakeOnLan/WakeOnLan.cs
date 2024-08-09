using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GSM_WOL_Proiect
{
    public static class WakeOnLan
    {
        public static void SendWakeOnLan(string macAddress)
        {
            /*Convertim adresa MAC intr-un array de octeti*/
            byte[] macBytes = GetMacBytes(macAddress);
            /*Construim pachetul magic WoL*/
            byte[] packet = new byte[102];
            /*Primele 6 octeti trebuie sa fie 0xFF*/
            for (int i = 0; i < 6; i++)
            {
                packet[i] = 0xFF;
            }
            /*Urmatorii 16 * 6 octeti trebuie sa fie adresa MAC repetata de 16 ori*/
            for (int i = 1; i <= 16; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    packet[i * 6 + j] = macBytes[j];
                }
            }
            /*Trimiterea pachetului prin UDP*/
            using (UdpClient client = new UdpClient())
            {
                client.Connect(IPAddress.Broadcast, 9); // Portul 9 este utilizat de obicei pentru WoL
                client.Send(packet, packet.Length);
            }
        }
        static byte[] GetMacBytes(string macAddress)
        {
            string[] macParts = macAddress.Split('-');
            if (macParts.Length != 6)
            {
                throw new ArgumentException("Format de adresa MAC invalid.");
            }
            byte[] macBytes = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                macBytes[i] = Convert.ToByte(macParts[i], 16);
            }
            return macBytes;
        }
    }
}
