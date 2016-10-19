using System;
using System.Linq;

using ManagedClient;
using Npgsql;

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
                    string postgresql_connect = "Server=192.168.1.2;User Id=postgres;Password=;Database=library;";
                    string irbis_connect = "host=127.0.0.1;port=6666;user=1;password=1;";

                    /*
                     * Postgresql connect
                     */
                    Console.WriteLine("Postgresql connect");
                    NpgsqlConnection connect = new NpgsqlConnection(postgresql_connect);
                    Console.WriteLine("Postgresql connected");

                    /*
                     * Irbis connect
                     */
                    Console.WriteLine("Irbris connecting");
                    client.ParseConnectionString(irbis_connect);
                    client.Connect();
                    Console.WriteLine("Irbris connected");


/*
                    NpgsqlCommand command = new NpgsqlCommand("select * from books", connect);

                    connect.Open();
                 
                    NpgsqlDataReader reader;
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        try
                        {
                            string result = reader.GetString(1);//Получаем значение из второго столбца! Первый это (0)!
                            Console.WriteLine(result);
                        }
                        catch { }
                    }
                    connect.Close();
*/




                    
/*
                    // Ищем все книги, у которых автор начинается на А (кириллица)
                    int[] foundRecords = client.Search("\"A={0}$\"", "В");

                    int recordsToShow = foundRecords.Length;

                    for (int i = 0; i < recordsToShow; i++)
                    {
                        int thisMfn = foundRecords[i];

                        // Считываем запись
                        IrbisRecord record = client.ReadRecord(thisMfn);

                        // Получаем основное заглавие
                        string mainTitle = record
                            .Fields
                            .GetField("200")
                            .GetSubField('a')
                            .GetSubFieldText()
                            .FirstOrDefault();

                        // Можно было просто написать: 
                        // string mainTitle = record.FM("200", 'a');

                        Console.WriteLine(
                            "MFN={0}, Main title={1}",
                            thisMfn,
                            mainTitle
                        );

                        // Расформатируем запись
                        Console.WriteLine(
                            "BRIEF: {0}",
                            client.FormatRecord("@brief", record)
                        );

                        Console.WriteLine(new string('-', 60));
                    }

                    Console.WriteLine(recordsToShow);
*/
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
