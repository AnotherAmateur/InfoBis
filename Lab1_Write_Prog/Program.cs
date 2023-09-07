using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Lab1_Write_Prog
{
    class Program
    {
        static void Main(string[] args)
        {
            string messagePath = null;
            string stegoPath = null;
            string containerPath = null;

            // Парсинг аргументов командной строки
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-m" || args[i] == "--message")
                {
                    messagePath = args[++i];
                }
                else if (args[i] == "-s" || args[i] == "--stego")
                {
                    stegoPath = args[++i];
                }
                else if (args[i] == "-c" || args[i] == "--container")
                {
                    containerPath = args[++i];
                }
                else if (args[i] == "-h" || args[i] == "--help")
                {
                    ShowHelp();
                    return;
                }
                else
                {
                    Console.WriteLine("Некорректный ввод");
                    return;
                }
            }

            // Проверка обязательного параметра -c
            if (string.IsNullOrEmpty(containerPath))
            {
                Console.WriteLine("Ошибка: не указан путь к файлу-контейнеру (-c).");
                return;
            }

            try
            {
                // Чтение сообщения
                BitArray message = ReadMessage(messagePath);

                if (message.Length < 1)
                {
                    Console.WriteLine("Ошибка: сообщение не может быть пустым.");
                    return;
                }

                // Встраивание сообщения в контейнер
                string[] stegoContainer = EmbedMessage(containerPath, message);

                // Сохранение стегоконтейнера или вывод в стандартный поток
                if (!string.IsNullOrEmpty(stegoPath))
                {
                    File.WriteAllLines(stegoPath, stegoContainer, Encoding.UTF8);
                }
                else
                {
                    Console.WriteLine(stegoContainer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        static BitArray ReadMessage(string filePath)
        {
            byte[] bytes;
            if (string.IsNullOrEmpty(filePath))
            {
                // Чтение сообщения из стандартного потока ввода
                bytes = Console.ReadLine().Split().Select(x => Convert.ToByte(x, 16)).ToArray();
            }
            else
            {
                bytes = File.ReadAllBytes(filePath);              
            }

            return new BitArray(bytes);
        }

        static string[] EmbedMessage(string containerPath, BitArray message)
        {
            string[] stegoContainer = File.ReadAllLines(containerPath);

            if (stegoContainer.Length < message.Length)
            {
                Console.WriteLine("В контейнере недостаточно строк для записи сообщения");
            }

            for (int i = 0; i < stegoContainer.Length; i++)
            {
                stegoContainer[i] = stegoContainer[i].TrimEnd();

                if (i < message.Length)
                {
                    stegoContainer[i] += message[i] ? " " : "";
                }
                else if (i == message.Length)
                {
                    stegoContainer[i] += " ";
                }
            }

            return stegoContainer;
        }


        static void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine("Программа встраивает сообщение в контейнер, где бит кодируется\n" +
                "пробелом на конце строки и в конце текста, и создаёт стегоконтейнер.\n" +
                "Программа консольная и имеет следующий интерфейс: ");
            Console.WriteLine();
            Console.WriteLine("Параметры:");
            Console.WriteLine("-m или --message:\tПуть к файлу, содержащему сообщение.");
            Console.WriteLine("-s или --stego:\tПуть по которому нужно записать стегоконтейнер.");
            Console.WriteLine("-c или --container:\tПуть к файлу-контейнеру (обязательный параметр).");
            Console.WriteLine("-h или --help:\tВывести справку о программе.");
            Console.WriteLine();
            Console.WriteLine("Примеры использования: ");
            Console.WriteLine("put-message -m message.txt -s stegocontainer.txt -c container.txt");
            Console.WriteLine("put-message -c container.txt < message.txt > stegocontainer.txt");
        }
    }

}
