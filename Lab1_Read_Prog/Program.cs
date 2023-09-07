using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab1_Read_Prog
{
    class Program
    {
        static void Main(string[] args)
        {
            string stegoPath = null;
            string messagePath = null;

            // Парсинг аргументов командной строки
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-s" || args[i] == "--stego")
                {
                    stegoPath = args[++i];
                }
                else if (args[i] == "-m" || args[i] == "--message")
                {
                    messagePath = args[++i];
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

            // Чтение стегоконтейнера
            string[] stegoContainer = ReadStegoContainer(stegoPath);

            if (stegoContainer.Length < 1)
            {
                Console.WriteLine("Ошибка: стегоконтейнер не может быть пустым.");
                return;
            }

            // Извлечение сообщения из стегоконтейнера
            byte[] msgBytes = ExtractMessage(stegoContainer);

            // Сохранение сообщения или вывод в стандартный поток
            if (!string.IsNullOrEmpty(messagePath))
            {
                File.WriteAllBytes(messagePath, msgBytes);
            }
            else
            {
                Console.WriteLine(string.Join(" ", msgBytes.Select(b => $"{b:X2}")));
            }
        }

        static string[] ReadStegoContainer(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                // Чтение стегоконтейнера из стандартного потока ввода
                return Console.In.ReadToEnd().Split('\n');
            }
            else
            {
                // Чтение стегоконтейнера из файла
                return File.ReadAllLines(filePath, Encoding.UTF8);
            }
        }

        static byte[] ExtractMessage(string[] stegoLines)
        {
            int index = stegoLines.Length - 1;
            while (stegoLines[index].Last() != ' ')
            {
                --index;
            }

            BitArray msgBites = new BitArray(index);
            for (int i = 0; i < index - 1; i++)
            {
                msgBites[i] = (stegoLines[i].Last() == ' ') ? true : false;

            }

            byte[] msgBytes = new byte[(msgBites.Length - 1) / 8 + 1];
            msgBites.CopyTo(msgBytes, 0);

            return msgBytes;
        }

        static void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine("Программа восстанавливает сообщение из стегоконтейнера,\n +" +
                "где бит кодируется пробелом на конце каждой строки\n" +
                "Программа консольная и имеет следующий интерфейс: ");
            Console.WriteLine();
            Console.WriteLine("Параметры:");
            Console.WriteLine("-s или --stego:\tПуть к стегоконтейнеру.");
            Console.WriteLine("-m или --message:\tПуть к файлу, в который нужно записать сообщение.");
            Console.WriteLine("-h или --help:\tВывести справку о программе.");
            Console.WriteLine();
            Console.WriteLine("Примеры использования: ");
            Console.WriteLine("get-message -s stegocontainer.txt --message message.txt");
            Console.WriteLine("get-message < stegocontainer.txt > message.txt");
        }
    }

}
