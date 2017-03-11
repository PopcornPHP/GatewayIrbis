using System;
using System.Linq;

using ManagedClient;
using WpfTestApplication;
using System.Collections.Generic;

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
                    Console.WriteLine("Database connecting");
                    client.ParseConnectionString("host=127.0.0.1;port=6452;user=USER;password=PASS;");
                    client.Connect();
                    Console.WriteLine("Database connected");

                    Console.WriteLine(new string('-', 60));

                    Console.WriteLine("Start of search");

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
                    Console.WriteLine("End of search");

                    Console.WriteLine(new string('-', 60));

                    // Преобразование списка в массив с удалением дубликатов
                    int[] foundRecords = booksList.Distinct().ToArray();

                    // Сортировка списка ID по возрастанию
                    Array.Sort(foundRecords);

                    int recordsCount = foundRecords.Length;
                    Console.WriteLine("Total records: " + recordsCount);


                    /*
                     * Разбитие массива данных на чанки, и отправка в БД
                     */
                    string domain = "http://localhost/";

                    int offset = 0;
                    int chunk = 100;
                    while (offset < recordsCount)
                    {
                        int chunkLength = (offset + chunk > recordsCount ? (recordsCount - offset) % chunk : chunk);
                        int[] chunkRecords = new int[chunkLength];

                        Array.Copy(foundRecords, offset, chunkRecords, 0, chunkLength);
                        offset += chunk;

                        /*
                         * Подготовка данных перед записью
                         */
                        string books = "";
                        string authors = "";
                        string bookAuthor = "";
                        for (int i = 0; i < chunkLength - 1; i++)
                        {
                            // Формируем чанк данных для переноса
                            IrbisRecord record = client.ReadRecord(chunkRecords[i]);

                            books +=
                                "book[" + i + "][category_id]=" + "1" + "&" +
                                "book[" + i + "][mfn]=" + record.FM("906") + "&" +
                                "book[" + i + "][name]=" + record.FM("200", 'e') + "&" +
                                "book[" + i + "][year]=" + record.FM("210", 'd') + "&" +
                                "book[" + i + "][udk]=" + record.FM("200", 'a') + "&";

                            authors +=
                                "author[" + i + "][sign]=" + record.FM("908") + "&" +
                                "author[" + i + "][surname]=" + record.FM("700", 'a') + "&" +
                                "author[" + i + "][initials]=" + record.FM("700", 'b') + "&";

                            // TODO Как получить список всех авторов для данной книги?
                            bookAuthor +=
                                "attach[mfn]=" + record.FM("906") + "&" +
                                "attach[sign]=" + record.FM("908") + "&";
                        }

                        /*
                         * Методы отправки данных в БД
                         */
                        MyWebRequest createBook = new MyWebRequest(domain + "createBook", "POST", books);
                        Console.WriteLine("createBook: " + createBook.GetResponse());
                        Console.WriteLine(new string('-', 60));

                        MyWebRequest createAuthor = new MyWebRequest(domain + "createAuthor", "POST", authors);
                        Console.WriteLine("createAuthor: " + createAuthor.GetResponse());
                        Console.WriteLine(new string('-', 60));

                        MyWebRequest attachBookAuthor = new MyWebRequest(domain + "attachBookAuthor", "POST", bookAuthor);
                        Console.WriteLine("attachBookAuthor: " + attachBookAuthor.GetResponse());
                        Console.WriteLine(new string('-', 60));
                        Console.WriteLine(new string('-', 60));
                    }

                    Console.WriteLine("Irbris disconnect");
                    client.Disconnect();
                    Console.WriteLine("Irbris disconnected");
                    Console.WriteLine(new string('-', 60));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
