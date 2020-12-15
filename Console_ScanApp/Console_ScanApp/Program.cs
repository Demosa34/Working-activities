using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Console_ScanApp
{
    class Program
    {

        public static class FileHelper
        {
            public static void GetAllFiles(string rootDirectory, string fileExtension, List<string> files)
            {
                string[] directories = Directory.GetDirectories(rootDirectory);
                files.AddRange(Directory.GetFiles(rootDirectory, fileExtension));

                foreach (string path in directories)
                    GetAllFiles(path, fileExtension, files);
            }
        }


        static void Main(string[] args)
        {
            // устанавливаем метод обратного вызова
            TimerCallback tm = new TimerCallback(SeachFileInCatalog);//вызывает метод поиска файлов в каталоге по таймеру 
            // создаем таймер
            int num = 0;
            Timer timer = new Timer(tm, num, 0, 20000);  
            Console.ReadKey();
        }

        public static void SeachFileInCatalog(object obj)
        {
            string dirName = "C:\\Catalog";//Расположение каталога с .DBF файлами
            DirectoryInfo dirInfo = new DirectoryInfo(dirName);

            Console.WriteLine($"Название каталога: {dirInfo.Name}");
            Console.WriteLine($"Полное название каталога: {dirInfo.FullName}");
            Console.WriteLine($"Время создания каталога: {dirInfo.CreationTime}");
            Console.WriteLine($"Корневой каталог: {dirInfo.Root}");
            Console.WriteLine($"Сканирование папки Catalog...");


            //Получение списка файлов  >>>
            if (Directory.Exists(dirName))
            {
                Console.WriteLine();
                Console.WriteLine("Найденые файлы:");

                string[] files1 = Directory.GetFiles(dirName, "*.DBF", SearchOption.AllDirectories);
                foreach (string s in files1)
                {
                    Console.WriteLine(s); //выводит в кансоль найденые в каталоге файлы            
                    Console.WriteLine();
                }              
            }
            //Получение списка файлов <<<
        }

        public static string StartLoad()
        {
            string result;
            OleDbConnection conn = new OleDbConnection();
            DataTable dt = new DataTable();
            DataRow row;
            DataColumn column = new DataColumn();
            try
            {
                conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\\;Extended Properties=dBASE IV;User ID=;Password=;";
                conn.Open();
                OleDbCommand comm = conn.CreateCommand();
                comm.CommandText = @"SELECT * FROM  www";

                dt.Load(comm.ExecuteReader());
                column = dt.Columns[0];
                row = dt.Rows[0];
                result = row[column].ToString();
                ReadTable(dt);
            }

            catch (Exception e)
            {
                result = e.ToString();
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        static void ReadTable(DataTable dt)
        {
            // Создание объекта DataTableReader
            DataTableReader dtReader = dt.CreateDataReader();

            while (dtReader.Read())
            {
                for (int i = 0; i < dtReader.FieldCount; i++)
                    Console.Write("{0}\t", dtReader.GetValue(i).ToString().Trim());
                Console.WriteLine();
            }
            dtReader.Close();
        }

    }
}
