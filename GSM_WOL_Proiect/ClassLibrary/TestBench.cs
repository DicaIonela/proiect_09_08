using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSM_WOL_Proiect
{
    public class TestBench
    {
        private const char FILE_SEPARATOR = ';';
        public string Tb { get; set; }
        public string MACaddress { get; set; }
        private const int TB = 0;
        private const int MACADDRESS = 1;
        public TestBench()/*CONSTRUCTOR FARA PARAMETRI*/
        {
            Tb = MACaddress = string.Empty;
        }
        public TestBench(string Tb, string MACadress)/*CONSTRUCTOR CU PARAMETRI*/
        {
            this.Tb = Tb;
            this.MACaddress = MACadress;
        }
        public TestBench(string FileLine) /*CONSTRUCTOR PENTRU LINIILE FISIERULUI*/
        {
            string[] FileData = FileLine.Split(FILE_SEPARATOR);
            this.Tb = FileData[TB];
            this.MACaddress = FileData[MACADDRESS];
        }
        //public string Conversie_PentruFisier()/*CONVERTESTE INFORMATIILE UTILIZATORULUI DIN OBIECT IN STRING IN FORMATUL CORESPUNZATOR PENTRU FISIERUL CSV; PENTRU SALVARE IN FISIER*/
        //{
        //    string UtilizatorInFisier = string.Format("{1}{0}{2}",
        //        SEPARATOR_FISIER,
        //        (Tb ?? " NECUNOSCUT "),
        //        (AdresaMAC ?? "NECUNOSCUT"));
        //    return UtilizatorInFisier;
        //}
        public string Info()/*FORMAREA UNUI SIR DE CARACTERE CORESPUNZATOR PENTRU AFISAREA IN CONSOLA A INFORMATIILOR UTILIZATORULUI*/
        {
            string info = $"Test Bench:\nName:{Tb ?? " UNKNOWN "} \nMAC Address: {MACaddress ?? " UNKNOWN "}";
            return info;
        }
    }
}
