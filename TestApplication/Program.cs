using System;
using System.Linq;

using ManagedClient;
using System.Collections.Generic;
using WpfTestApplication;
using System.Net;

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
                    Console.WriteLine("Irbris connecting");
                    client.ParseConnectionString("host=127.0.0.1;port=6452;user=СТА;password=СТА;");
                    client.Connect();
                    Console.WriteLine("Irbris connected");


                    // Делаем переключение на базу MV
                    client.PushDatabase("MV");

                    // Алфавит по которому будем производить циклический поиск
                    string[] alphabet = new string[] {
                        "А","Б","В","Г","Ґ","Д","Е","Є","Ё","Ж","З","И","І","Ї","Й","К","Л","М","Н",
                        "О","П","Р","С","Т","У","Ф","Х","Ц","Ч","Ш","Щ","Ъ","Ы","Ь","Э","Ю","Я",
                        "а","б","в","г","ґ","д","е","є","ё","ж","з","и","і","ї","й","к","л","м","н",
                        "о","п","р","с","т","у","ф","х","ц","ч","ш","щ","ъ","ы","ь","э","ю","я",
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

                    // Сортировка списка ID по возрастанию
                    Array.Sort(foundRecords);

                    int recordsCount = foundRecords.Length;
                    
                    /*
                     * Запись данных в БД
                     */
                    /*for (int i = 0; i < recordsCount - 1; i++)
                    {
                        IrbisRecord record = client.ReadRecord(foundRecords[i]);

                        MyWebRequest myRequest = new MyWebRequest("http://library.local/api/library/create", "POST",
                            "title="        + record.FM("200", 'e') +
                            "&method_id="   + record.FM("906") +
                            "&subject="     + record.FM("200", 'a') +
                            "&authors="     + record.FM("200", 'f') +
                            "&lib_id="      + foundRecords[i]
                        );
                    }*/


                    for (int i = 0; i < recordsCount - 1; i++)
                    {

                        IrbisRecord record = client.ReadRecord(foundRecords[i]);

                        string uriString = "D:/IRBIS64/Datai/MV" + record.FM("951", 'a');
                        string fileName = "http://library.local/api/library/attach";

                        // Create a new WebClient instance.
                        WebClient myWebClient = new WebClient();

                        // Upload the file to the URL using the HTTP 1.0 POST.
                        byte[] responseArray = myWebClient.UploadFile(uriString, "POST", fileName);

                        // Decode and display the response.
                        Console.WriteLine("\nResponse Received.The contents of the file uploaded are:\n{0}",
                            System.Text.Encoding.ASCII.GetString(responseArray));
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
    }
}