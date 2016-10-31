using System;
using System.Linq;

using ManagedClient;
using Npgsql;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Specialized;

namespace TestApplication
{
    class Program
    {
        private static void Main()
        {
            try
            {
                using (ManagedClient64 client = new ManagedClient64())
                {
                    string postgresql_connect = "Server=127.0.0.1;User Id=postgres;Password=;Database=cyberarmy;";
                    string irbis_connect = "host=127.0.0.1;port=6452;user=СТА;password=СТА;";

                    /*
                     * Postgresql connect
                     */
                    /*Console.WriteLine("Postgresql connect");
                    NpgsqlConnection connect = new NpgsqlConnection(postgresql_connect);
                    connect.Open();
                    Console.WriteLine("Postgresql connected");


                    NpgsqlCommand command = new NpgsqlCommand("select * from php_monitoring_tab_server", connect);
                    NpgsqlDataReader dr = command.ExecuteReader();

                    while (dr.Read())
                    {
                        Console.Write("{0}\t{1}\t{2}\t{3}:{4}\t{5} \n", dr[0], dr[1], dr[2], dr[3], dr[4], dr[5]);
                    }


                    Console.WriteLine("Postgresql disconnect");
                    connect.Close();
                    Console.WriteLine("Postgresql disconnected");

                    /*
                     * Irbis connect
                     */
                    Console.WriteLine("Irbris connecting");
                    client.ParseConnectionString(irbis_connect);
                    client.Connect();
                    Console.WriteLine("Irbris connected");


                    // Делаем переключение на базу MV
                    client.PushDatabase("MV");

                    // Алфавит по которому будем производить циклический поиск
                    string[] alphabet = new string[] {
                        "А","Б","В","Г","Ґ","Д","Е","Є","Ё","Ж",
                        "З","И","І","Ї","Й","К","Л","М","Н","О",
                        "П","Р","С","Т","У","Ф","Х","Ц","Ч","Ш",
                        "Щ","Ъ","Ы","Ь","Э","Ю","Я",
                        "а","б","в","г","ґ","д","е","є","ё","ж",
                        "з","и","і","ї","й","к","л","м","н","о",
                        "п","р","с","т","у","ф","х","ц","ч","ш",
                        "щ","ъ","ы","ь","э","ю","я",
                    };

                    // Осуществляем циклический поиск
                    List<int> booksList = new List<int>();
                    for (int i = 0; i < alphabet.Length; i++)
                    {
                        int[] found = client.Search("\"A={0}$\"", alphabet[i]);

                        for (int j = 0; j < found.Length; j++)
                        {
                            booksList.Add(found[j]);
                        }
                    }

                    // Преобразование списка в массив с удалением дубликатов
                    int[] foundRecords = booksList.Distinct().ToArray();

                    // Сортировка ID по возрастанию
                    Array.Sort(foundRecords);

                    int recordsToShow = foundRecords.Length;
                    
                    for (int i = 0; i < recordsToShow; i++)
                    {
                        int thisMfn = foundRecords[i];
                        IrbisRecord record = client.ReadRecord(thisMfn);

                        string mainSubject = record.FM("200", 'a');
                        string mainTitle = record.FM("200", 'e');
                        string mainAuthors = record.FM("200", 'f');

                        Console.WriteLine(mainAuthors + "\n" + mainTitle + "\n" + mainAuthors + "\n\n");


                        /*
                         * API
                         */
                        //string url = "http://library.local/library/create";

                        //var pars = new NameValueCollection();
                        //pars.Add("subject", mainSubject);
                        //pars.Add("title", mainTitle);
                        //pars.Add("authors", mainAuthors);
                    

                        //var webClient = new WebClient();
                        //var response = webClient.UploadValues(url, pars);
                        //string Answer = POST(url, "subject="+ mainSubject + "&title="+ mainTitle + "&authors"+ mainAuthors);
                    }


                    Console.WriteLine("Irbris disconnect");
                    client.Disconnect();
                    Console.WriteLine("Irbris disconnected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static string POST(string url, string v)
        {
            throw new NotImplementedException();
        }
    }
}