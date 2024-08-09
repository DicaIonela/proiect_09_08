using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSM_WOL_Proiect
{
    public class User
    {
        private const char FILE_SEPARATOR = ';';
        public string Name { get; set; }
        public string Number { get; set; }
        public string MACaddress { get; set; }
        private const int NAME = 0;
        private const int NUMBER = 1;
        private const int MACADRESS = 2;
        public User()/*CONSTRUCTOR FARA PARAMETRI*/
        {
            Name = Number = MACaddress = string.Empty;
        }
        public User(string Name, string Number, string MACadress)/*CONSTRUCTOR CU PARAMETRI*/
        {
            this.Name=Name;
            this.Number = Number;
            this.MACaddress = MACadress;
        }
        public User(string FileLine) /*CONSTRUCTOR PENTRU LINIILE FISIERULUI*/
        {
            string[] FileData = FileLine.Split(FILE_SEPARATOR);
            this.Name = FileData[NAME];
            this.Number = FileData[NUMBER];
            this.MACaddress = FileData[MACADRESS];
        }
        //public string Conversie_PentruFisier()/*CONVERTESTE INFORMATIILE UTILIZATORULUI DIN OBIECT IN STRING IN FORMATUL CORESPUNZATOR PENTRU FISIERUL CSV; PENTRU SALVARE IN FISIER*/
        //{
        //    string UtilizatorInFisier = string.Format("{1}{0}{2}{0}{3}",
        //        SEPARATOR_FISIER,
        //        (Nume ?? " NECUNOSCUT "),
        //        (Numar ?? " NECUNOSCUT "),
        //        (AdresaMAC ?? "NECUNOSCUT"));
        //    return UtilizatorInFisier;
        //}
        public string Info()/*FORMAREA UNUI SIR DE CARACTERE CORESPUNZATOR PENTRU AFISAREA IN CONSOLA A INFORMATIILOR UTILIZATORULUI*/
        {
            string info = $"User:\nNume:{Name ?? " UNKNOWN "} \nPhone Number:{Number ?? " UNKNOWN "} \nMAC Address: {MACaddress ?? " UNKNOWN "}";
            return info;
        }
    }
}
