using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace GSM_WOL_Proiect
{
    public class UserFunctions
    {
        public static void ShowUsers(User[] users, int numberofusers)
        {
            if (users.Length > 0)
            {
                //Console.WriteLine("Utilizatorii salvati in fisier gasiti sunt:");
                for (int contor = 0; contor < numberofusers; contor++)/*SE PARCURGE TABLOUL DE OBIECTE SI SE AFISEAZA INFORMATIILE IN FORMATUL CORESPUNZATOR*/
                {
                    string infoUsers = users[contor].Info();
                    Console.WriteLine(infoUsers);
                }
            }
        }
        public static void ShowTB(TestBench[] testBenches, int numberoftbs)
        {
            if (testBenches.Length > 0)
            {
                //Console.WriteLine("Utilizatorii salvati in fisier gasiti sunt:");
                for (int contor = 0; contor < numberoftbs ; contor++)/*SE PARCURGE TABLOUL DE OBIECTE SI SE AFISEAZA INFORMATIILE IN FORMATUL CORESPUNZATOR*/
                {
                    string infoTB = testBenches[contor].Info();
                    Console.WriteLine(infoTB);
                }
            }
            else
                Console.WriteLine("Nu au fost gasite TB-uri.");
        }
        public static string ValidationPhoneNumber(string number)
        {
            if (number.StartsWith("+40"))
            {
                if (number.Length == 13 && number.Substring(3).All(char.IsDigit))/*Verificare daca numarul are exact 13 caractere si sunt doar cifre dupa +40*/
                {
                    return "0040" + number.Substring(3);
                }
                else
                {
                    return null; /*Invalid*/
                }
            }
            if (number.StartsWith("0040"))
            {
                if (number.Length == 13 && number.Substring(3).All(char.IsDigit))/*Verificare daca numarul are exact 13 caractere si sunt doar cifre dupa +40*/
                {
                    return number;
                }
                else
                {
                    return null; /*Invalid*/
                }
            }
            else
            {
                if (number.Length == 10 && number.All(char.IsDigit) && number.StartsWith("0"))/*Verificare daca numarul are exact 10 caractere si sunt doar cifre*/
                {
                    return "004" + number; /*Adaugam prefixul +4 -> 0 fiind deja inclus*/
                }
                else
                {
                    return null; /*Invalid*/
                }
            }
        }
        public static string ValidationMAC(string macaddress)
        {
            var cleanAddress = new string(macaddress
                .Where(c => "0123456789ABCDEF".Contains(char.ToUpper(c)))
                .ToArray());/*Elimina toate caracterele non-hexadecimale si normalizeaza*/
            if (cleanAddress.Length == 12)/*Verificare daca are exact 12 caractere (6 octeti)*/
            {
                return string.Join("-", Enumerable.Range(0, cleanAddress.Length / 2)
                    .Select(i => cleanAddress.Substring(i * 2, 2)));/*Formateaza in formatul 00-11-22-33-44-55*/
            }
            else
            {
                return null; /*Invalid*/
            }
        }
        //public static User CitireUtilizatorTastatura()
        //{
        //    Console.WriteLine("Introduceti datele utilizatorului:");
        //    Console.WriteLine("Nume:");
        //    string nume = Console.ReadLine();
        //    string numarcitit;
        //    do
        //    {
        //        Console.WriteLine("Numar:");
        //        numarcitit = Console.ReadLine();
        //        numarcitit = ValidareSiCorectareNumar(numarcitit);
        //        if (numarcitit == null)
        //        {
        //            Console.WriteLine("Numarul de telefon introdus nu este valid. Te rugam sa incerci din nou.");
        //        }
        //    } while (numarcitit == null);
        //    string adresamac;
        //    do
        //    {
        //        Console.WriteLine("Adresa MAC (format: 00-11-22-33-44-55):");
        //        adresamac = Console.ReadLine();
        //        adresamac = ValidareSiFormatareAdresaMac(adresamac);
        //        if (adresamac == null)
        //        {
        //            Console.WriteLine("Adresa MAC introdusa nu este valida. Te rugam sa incerci din nou.");
        //        }
        //    } while (adresamac == null);
        //    User utilizator = new User(nume, numarcitit, adresamac);
        //    return utilizator;
        //}
        //public static TestBench CitireTBTastatura()
        //{
        //    Console.WriteLine("Introduceti datele TB-ului:");
        //    Console.WriteLine("Nume:");
        //    string Tb = Console.ReadLine();
        //    string adresamac;
        //    do
        //    {
        //        Console.WriteLine("Adresa MAC (format: 00-11-22-33-44-55):");
        //        adresamac = Console.ReadLine();
        //        adresamac = ValidareSiFormatareAdresaMac(adresamac);
        //        if (adresamac == null)
        //        {
        //            Console.WriteLine("Adresa MAC introdusa nu este valida. Te rugam sa incerci din nou.");
        //        }
        //    } while (adresamac == null);
        //    TestBench testBench = new TestBench(Tb, adresamac);
        //    return testBench;
        //}
        public static string GetMacAddress()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();/*Obtine lista de placi de retea*/
            foreach (var networkInterface in networkInterfaces)/*Cautam prima placa de retea activa si obtinem adresa MAC*/
            {
                if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    networkInterface.OperationalStatus == OperationalStatus.Up)/*Verifica daca placa de retea nu este de tip Loopback si este activs*/
                {
                    var macAddress = networkInterface.GetPhysicalAddress();
                    if (macAddress != null)
                    {
                        return string.Join("-", macAddress.GetAddressBytes().Select(b => b.ToString("X2")));/*Formateaza adresa MAC intr-un sir de caractere hexazecimale*/
                    }
                }
            }
            return "Adresa MAC nu a putut fi gasita";
        }
    }
}
