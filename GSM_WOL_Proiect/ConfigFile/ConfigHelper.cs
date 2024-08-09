using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace GSM_WOL_Proiect
{
    public class ConfigHelper
    {
        //private const int NR_MAX = 50;
        //private string numeFisier;
        public static User[] GetUsers()
        {
            NameValueCollection usersSection = (NameValueCollection)ConfigurationManager.GetSection("usersSection");
            User[] users = new User[usersSection.Count];
            int index = 0;
            foreach (string key in usersSection)
            {
                string[] values = usersSection[key].Split(';');
                users[index++] = new User
                {
                    Name = key,
                    Number = values[0],
                    MACaddress = values[1]
                };
            }
            return users;
        }
        public static User[] SearchUsers(string parameter)
        {
            //int nrUtilizatori = 0;
            User[] users = GetUsers();
            List<User> FoundUsers = new List<User>();/*se creeaza o lista pentru utilizatorii gasiti*/
            foreach (User user in users)/*se parcurge tabloul de obiecte*/
            {
                if (user != null && (user.Name.Contains(parameter) || (user.Number.Contains(parameter))))/*se verifica daca numele contine caracterele introduse/numarul*/
                {
                    FoundUsers.Add(user);/*daca numele a indeplinit conditia, se va adauga obiectul la lista de utilizatori gasiti*/
                }
            }
            return FoundUsers.ToArray();
        }
        public static TestBench[] Gettb()
        {
            NameValueCollection tbSection = (NameValueCollection)ConfigurationManager.GetSection("testBenchSection");
            TestBench[] testBenches = new TestBench[tbSection.Count];
            int index = 0;
            foreach (string key in tbSection)
            {
                testBenches[index++] = new TestBench
                {
                    Tb = key,
                    MACaddress = tbSection[key]
                };
            }
            return testBenches;
        }
        public static TestBench[] SearchTB(string parameter)
        {
            TestBench[] testBenches = Gettb();
            List<TestBench> tbFound = new List<TestBench>();/*se creeaza o lista pentru TB-URILE gasiti*/
            foreach (TestBench testBench in testBenches)/*se parcurge tabloul de obiecte*/
            {
                if (testBench.Tb.Trim().Equals(parameter.Trim(), StringComparison.OrdinalIgnoreCase))/*se verifica daca numele contine caracterele introduse*/
                {
                    tbFound.Add(testBench);/*daca numele a indeplinit conditia, se va adauga obiectul la lista de TB-URI gasite*/
                }
            }
            return tbFound.ToArray();
        }
    }
}
