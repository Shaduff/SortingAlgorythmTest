using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static AlgorythmSortTest.Program;

namespace AlgorythmSortTest
{
    internal class Program
    {
        // Test settings class
        class Settings
        {
            public int Nmin;
            public int Nmax;
            public int dN;
            public string test_type;
            public string custom_filemane;
        }

        protected string[] test_type = { "_best_", "_average_", "_worst_" };

        static void File_cleaner(string filename)
        {
            StreamWriter cleaner = new StreamWriter(filename, false);
            cleaner.Close();
        }


        // Result outputs to txt
        static void Results_output(int result, string filename)
        {
            StreamWriter fout = new StreamWriter(filename, true);
            fout.WriteLine(result);
            fout.Close();
        }

        static void Results_output(double result, string filename)
        {
            StreamWriter fout = new StreamWriter(filename, true);
            fout.WriteLine($"{result,0:N4}");
            fout.Close();
        }

        static void Out_array(int[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                Console.Write(arr[i] + " ");
            }
            Console.WriteLine();
        }

        static void Swap(ref int var1, ref int var2)
        {
            int temp = var1;
            var1 = var2;
            var2 = temp;
        }

        static Settings Test_setting_menu()
        {
            Settings settings = new Settings();
            Console.Write("Введите изначальный размер массива: ");
            try
            {
                settings.Nmin = int.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                Input_error_message();
            }
            Console.Write("Введите размер шага изменения размера массива: ");
            try
            {
                settings.dN = int.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                Input_error_message();
            }
            Console.Write("Введите максимальный размер массива: ");
            try
            {
                settings.Nmax = int.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                Input_error_message();
            }
            Console.Clear();
            Console.Write("===КАКОЙ ТЕСТ ПРОИЗВЕСТИ?===\n" +
                          "1. Для лучшего случая.\n" +
                          "2. Для среднего случая.\n" +
                          "3. Для худшего случая.\n" +
                          "4. Пользовательский.\n" +
                          "Ввод: ");
            string qst_tt_str = Console.ReadLine();
            if (qst_tt_str == "1" || qst_tt_str == "2" || qst_tt_str == "3" || qst_tt_str == "4")
            {
                settings.test_type = qst_tt_str;
            }
            else
            {
                Input_error_message();
                return null;
            }
            if (qst_tt_str == "4")
            {
            Custom_name_input:
                Console.Clear();
                Console.Write("Введите имя файла с исходными данными (включая расширение .bin): ");
                settings.custom_filemane = Console.ReadLine();
                Console.WriteLine("Вы ввели : " + settings.custom_filemane);
                Console.Write("Для подтверждения нажмите [ENTER].\n" +
                              "Если ввод неверный нажмите любую клавишу чтобы ввести снова." +
                              "Для отмены нажмите [ESC].\n");
                ConsoleKey key = Console.ReadKey().Key;
                switch (key)
                {
                    case ConsoleKey.Enter: { Console.WriteLine(); break; }
                    case ConsoleKey.Escape: { Input_error_message(); return null; }
                    default: goto Custom_name_input;
                }
            }
            return settings;
        }

        static void Comb_sort(ref int[] arr, ref int cp_count, ref int sw_count)
        {
            float factor = 1.3f;
            int step = arr.Length - 1;

            while (step >= 1)
            {
                for (int i = 0; i + step < arr.Length; i++)
                {
                    cp_count++;
                    if (arr[i] > arr[i + step])
                    {
                        Swap(ref arr[i], ref arr[i + step]);
                        sw_count++;
                    }
                }
                step = (int)(step / factor);
                //Console.WriteLine(step);
            }
        }

        static void Merge(ref int[] arr, ref int[] leftArr, int leftS, ref int[] rightArr, int rightS, ref int cp_cnt, ref int sw_cnt)
        {
            {
                int i = 0, j = 0, k = 0;
                while (i < leftS && j < rightS)
                {
                    cp_cnt++;
                    if (rightArr[j] > leftArr[i])
                    {
                        arr[k++] = leftArr[i++];
                    }
                    else
                    {
                        arr[k++] = rightArr[j++];
                        sw_cnt++;
                    }
                }

                while (i < leftS)
                {
                    arr[k++] = leftArr[i++];
                }
                while (j < rightS)
                {
                    arr[k++] = rightArr[j++];
                }
            }
        }

        static void Merge_sort(ref int[] arr, ref int cp_cnt, ref int sw_cnt)
        {
            if (arr.Length < 2) return;

            int size = arr.Length;
            int mid = arr.Length / 2;

            int[] leftArr = new int[mid];
            int[] rightArr = new int[size - mid];

            for (int i = 0; i < mid; i++)
            {
                leftArr[i] = arr[i];
            }

            for (int i = mid; i < size; i++)
            {
                rightArr[i - mid] = arr[i];
            }

            Merge_sort(ref leftArr, ref cp_cnt, ref sw_cnt);
            Merge_sort(ref rightArr, ref cp_cnt, ref sw_cnt);
            Merge(ref arr, ref leftArr, mid, ref rightArr, size - mid, ref cp_cnt, ref sw_cnt);
        }




        static void Test_table_header()
        {
            Console.WriteLine("--------------------------------------------------------------------");
            Console.WriteLine("| Размер массива | Время вып. | Кол. Сравнений | Кол. Перестановок |");
            Console.WriteLine("--------------------------------------------------------------------");
        }

        static void Auto_test_table_header()
        {
            Console.WriteLine("--------------------------------------------------------------------|--------------------------------------------------------------------");
            Console.WriteLine("|                   Comb Sort Algorythm                             |                       Merge Sort Algorythm                        |");
            Console.WriteLine("--------------------------------------------------------------------|--------------------------------------------------------------------");
            Console.WriteLine("| Размер массива | Время вып. | Кол. Сравнений | Кол. Перестановок ||| Размер массива | Время вып. | Кол. Сравнений | Кол. Перестановок |");
            Console.WriteLine("--------------------------------------------------------------------|--------------------------------------------------------------------");
        }

        static void Input_error_message()
        {
            Console.Clear();
            Console.WriteLine("[ОШИБКА ВВОДА]");
            Console.ReadLine();
        }

        static void Array_file_fill(string filemname, int[] arr)
        {
            BinaryReader binaryReader = new BinaryReader(File.Open(filemname, FileMode.Open));
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = binaryReader.ReadInt32();
            }
            binaryReader.Close();
        }

        static void Comb_test(Settings settings)
        {
            Test_table_header();
            int N = settings.Nmin;

            File_cleaner("Comb_Swaps.txt");
            File_cleaner("Comb_Comps.txt");
            File_cleaner("Comb_Times.txt");

            while (N <= settings.Nmax)
            {
                int cp_count = 0, sw_count = 0;
                int[] arr = new int[N];

                if (settings.test_type == "1")
                {
                    Array_file_fill("Best_Input_Data.bin", arr);
                }
                if (settings.test_type == "2")
                {
                    Array_file_fill("Average_Input_Data.bin", arr);
                }
                if (settings.test_type == "3")
                {
                    Array_file_fill("Worst_Input_Data.bin", arr);
                }
                if (settings.test_type == "4")
                {
                    Array_file_fill(settings.custom_filemane, arr);
                }
                //Out_array(arr);
                DateTime start = DateTime.Now;
                Comb_sort(ref arr, ref cp_count, ref sw_count);
                DateTime end = DateTime.Now;
                //Out_array(arr);
                double time = ((double)end.Ticks / TimeSpan.TicksPerSecond) - ((double)start.Ticks / TimeSpan.TicksPerSecond);


                Console.WriteLine($"|{N,16}|{time,12:N3}|{cp_count,16}|{sw_count,19}|");
                if (settings.test_type == "4")
                {
                    Results_output(sw_count, "Custom_Swaps.txt");
                    Results_output(cp_count, "Custom_Comps.txt");
                    Results_output(time, "Custom_Times.txt");
                }
                else
                {
                    Results_output(sw_count, "Comb_Swaps.txt");
                    Results_output(cp_count, "Comb_Comps.txt");
                    Results_output(time, "Comb_Times.txt");
                }
                N += settings.dN;
            }
            Console.WriteLine("--------------------------------------------------------------------");
            Console.ReadLine();
        }

        static void Comb_test_menu()
        {
            Console.Clear();
            Console.WriteLine("==ТЕСТ АЛГОРИТМА РАСЧЕСКОЙ==");
            Settings settings = Test_setting_menu();
            if (settings != null)
            {
                Console.Clear();
                Console.WriteLine("========ТЕСТ АЛГОРИТМА СОРТИРОВКИ РАСЧЕСКОЙ========");
                Comb_test(settings);
            }
        }

        static void Merge_test(Settings settings)
        {
            Test_table_header();
            int N = settings.Nmin;

            File_cleaner("Comb_Swaps.txt");
            File_cleaner("Comb_Comps.txt");
            File_cleaner("Comb_Times.txt");

            while (N <= settings.Nmax)
            {
                int cp_count = 0, sw_count = 0;
                int[] arr = new int[N];

                if (settings.test_type == "1")
                {
                    Array_file_fill("Best_Input_Data.bin", arr);
                }
                else if (settings.test_type == "2")
                {
                    Array_file_fill("Average_Input_Data.bin", arr);
                }
                else if (settings.test_type == "3")
                {
                    Array_file_fill("Worst_Input_Data.bin", arr);
                }
                else if (settings.test_type == "4")
                {
                    Array_file_fill(settings.custom_filemane, arr);
                }
                else { Input_error_message(); }
                //Out_array(arr);
                DateTime start = DateTime.Now;
                //Console.WriteLine("s: " + $"{start.Ticks,10:N5}");
                Merge_sort(ref arr, ref cp_count, ref sw_count);
                DateTime end = DateTime.Now;
                //Console.WriteLine("e: " + $"{end.Ticks,10:N5}");
                //Out_array(arr);
                double time = ((double)end.Ticks / TimeSpan.TicksPerSecond) - ((double)start.Ticks / TimeSpan.TicksPerSecond);


                Console.WriteLine($"|{N,16}|{time,12:N3}|{cp_count,16}|{sw_count,19}|");
                if (settings.test_type == "4")
                {
                    Results_output(sw_count, "Custom_Swaps.txt");
                    Results_output(cp_count, "Custom_Comps.txt");
                    Results_output(time, "Custom_Times.txt");
                }
                else
                {
                    Results_output(sw_count, "Merge_Swaps.txt");
                    Results_output(cp_count, "Merge_Comps.txt");
                    Results_output(time, "Merge_Times.txt");
                }
                N += settings.dN;
            }
            Console.WriteLine("--------------------------------------------------------------------");
            Console.ReadLine();
        }

        static void Merge_test_menu()
        {
            Console.Clear();
            Console.WriteLine("==ТЕСТ АЛГОРИТМА РАСЧЕСКОЙ==");
            Settings settings = Test_setting_menu();
            if (settings != null)
            {
                Console.Clear();
                Console.WriteLine("========ТЕСТ АЛГОРИТМА СОРТИРОВКИ РАСЧЕСКОЙ========");
                Merge_test(settings);
            }
        }

        static void Manual_test_menu()
        {

            Console.Clear();
            Console.WriteLine("==РЕЖИМ РУЧНОГО ТЕСТИРОВАНИЯ==");
            Console.Write("1: Тестирование алг./сорт. расческой.\n" +
                          "2: Тестирование алг./сорт. слияния.\n" +
                          "Ввод: ");
            string menu_qst = Console.ReadLine();
            switch (menu_qst)
            {
                case "1":
                    {
                        Comb_test_menu();
                        break;
                    }
                case "2":
                    {
                        Merge_test_menu();
                        break;
                    }
                default:
                    {
                        Input_error_message();
                        break;
                    }
            }

        }

        static void Auto_test(Settings settings)
        {
            Console.Clear();
            Console.WriteLine("========АВТОМАТИЧЕСКИЙ ТЕСТ АЛГОРИТМОВ СОРТИРОВКИ========");
            Auto_test_table_header();

            for (int i = 1; i <= 3; i++)
            {
                int N = settings.Nmin;
                while (N <= settings.Nmax)
                {
                    int cb_cp_count = 0, cb_sw_count = 0;
                    int mg_cp_count = 0, mg_sw_count = 0;
                    int[] comb_arr = new int[N];
                    int[] merg_arr = new int[N];

                    if (i == 1)
                    {
                        // Console.WriteLine("Test for the best case");
                        Array_file_fill("Best_Input_Data.bin", comb_arr);
                        Array_file_fill("Best_Input_Data.bin", merg_arr);
                    }
                    else if (i == 2)
                    {
                        // Console.WriteLine("Test for average case");
                        Array_file_fill("Average_Input_Data.bin", comb_arr);
                        Array_file_fill("Average_Input_Data.bin", merg_arr);
                    }
                    else if (i == 3)
                    {
                        // Console.WriteLine("Test for the worst case");
                        Array_file_fill("Worst_Input_Data.bin", comb_arr);
                        Array_file_fill("Worst_Input_Data.bin", merg_arr);
                    }

                    DateTime start_cb = DateTime.Now;
                    Comb_sort(ref comb_arr, ref cb_cp_count, ref cb_sw_count);
                    DateTime end_cb = DateTime.Now;

                    DateTime start_mg = DateTime.Now;
                    Merge_sort(ref merg_arr, ref mg_cp_count, ref mg_sw_count);
                    DateTime end_mg = DateTime.Now;


                    double time_cb = ((double)end_cb.Ticks / TimeSpan.TicksPerSecond) - ((double)start_cb.Ticks / TimeSpan.TicksPerSecond);
                    double time_mg = ((double)end_mg.Ticks / TimeSpan.TicksPerSecond) - ((double)start_mg.Ticks / TimeSpan.TicksPerSecond);

                    Console.WriteLine($"|{N,16}|{time_cb,12:N4}|{cb_cp_count,16}|{cb_sw_count,19}|||{N,16}|{time_mg,12:N4}|{mg_cp_count,16}|{mg_sw_count,19}|");


                    N += settings.dN;
                }
                Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------");
            }

            Console.ReadLine();
        }

        static void Auto_test_menu()
        {
            Console.Clear();
            Console.WriteLine("==АВТОМАТИЧЕСКИЙ ТЕСТ АЛГОРИТМОВ СОРТИРОВКИ==");
            Settings auto_test = new Settings
            {
                Nmax = 100000,
                Nmin = 1000,
                dN = 500
            };
            Console.WriteLine("Исходный размер массивов : " + auto_test.Nmin);
            Console.WriteLine("Максимальный размер массивов : " + auto_test.Nmax);
            Console.WriteLine("Шаг роста размера массивов : " + auto_test.dN);
            if (auto_test != null)
            {
                Console.Write("Начать тестирование? [y/n]: ");
                string qst = Console.ReadLine();
                switch (qst)
                {
                    case "y":
                    case "Y":
                        {
                            Auto_test(auto_test);
                            break;
                        }
                    case "n":
                    case "N":
                        {
                            return;
                        }
                    default:
                        {
                            Input_error_message();
                            return;
                        }

                }
            }
        }



        static void Best_file_gen()
        {
            Console.Clear();
            Console.WriteLine("==ГЕНЕРИРОВАНИЕ ИСХОДНЫХ ДАННЫХ:ЛУЧШИЙ СЛУЧАЙ");
            Console.Write("Введите диапазон значений:\n" + "Min: ");
            string min_str = Console.ReadLine();
            int min;
            bool min_check = int.TryParse(min_str, out min);
            if (min_check)
            {
                Console.Write("Max: ");
                string max_str = Console.ReadLine();
                int max;
                bool max_check = int.TryParse(max_str, out max);
                if (max_check && (min < max))
                {
                    Console.Write("Введите желаемое имя файла: ");
                    string filename = Console.ReadLine();
                    Console.Write("[СОЗАНИЕ ФАЙЛА]->");
                    BinaryWriter fout = new BinaryWriter(File.Open(filename + ".bin", FileMode.OpenOrCreate));
                    for (int i = min; i <= max; i++)
                    {
                        fout.Write(i);
                    }
                    fout.Close();
                    Console.Write("[ФАЙЛ СОЗДАН].");
                    Console.ReadLine();
                }
                else
                {
                    Input_error_message();
                    return;
                }
            }
            else
            {
                Input_error_message();
                return;
            }
        }

        static void Average_file_gen()
        {
            Console.Clear();
            Console.WriteLine("==ГЕНЕРИРОВАНИЕ ИСХОДНЫХ ДАННЫХ:СРЕДНИЙ СЛУЧАЙ");
            Console.Write("Введите диапазон значений:\n" + "Min: ");
            string min_str = Console.ReadLine();
            int min;
            bool min_check = int.TryParse(min_str, out min);
            if (min_check)
            {
                Console.Write("Max: ");
                string max_str = Console.ReadLine();
                int max;
                bool max_check = int.TryParse(max_str, out max);
                if (max_check && (min < max))
                {
                    Console.Write("Введите количество значений: ");
                    string N_str = Console.ReadLine();
                    int N;
                    bool N_check = int.TryParse(N_str, out N);
                    if (N_check)
                    {
                        Console.Write("Введите желаемое имя файла: ");
                        string filename = Console.ReadLine();
                        Random rand = new Random();
                        Console.Write("[СОЗАНИЕ ФАЙЛА]->");
                        BinaryWriter fout = new BinaryWriter(File.Open(filename + ".bin", FileMode.OpenOrCreate));
                        for (int i = 0; i <= N; i++)
                        {
                            fout.Write(rand.Next(min, max));
                        }
                        fout.Close();
                        Console.Write("[ФАЙЛ СОЗДАН].");
                        Console.ReadLine();
                    }
                    else
                    {
                        Input_error_message();
                        return;
                    }
                }
                else
                {
                    Input_error_message();
                    return;
                }
            }
            else
            {
                Input_error_message();
                return;
            }
        }

        static void Worst_file_gen()
        {
            Console.Clear();
            Console.WriteLine("==ГЕНЕРИРОВАНИЕ ИСХОДНЫХ ДАННЫХ:ХУДШИЙ СЛУЧАЙ");
            Console.Write("Введите диапазон значений:\n" + "Min: ");
            string min_str = Console.ReadLine();
            int min;
            bool min_check = int.TryParse(min_str, out min);
            if (min_check)
            {
                Console.Write("Max: ");
                string max_str = Console.ReadLine();
                int max;
                bool max_check = int.TryParse(max_str, out max);
                if (max_check && (min < max))
                {
                    Console.Write("Введите желаемое имя файла: ");
                    string filename = Console.ReadLine();
                    Console.Write("[СОЗАНИЕ ФАЙЛА]->");
                    BinaryWriter fout = new BinaryWriter(File.Open(filename + ".bin", FileMode.OpenOrCreate));
                    for (int i = max; i >= min; i--)
                    {
                        fout.Write(i);
                    }
                    fout.Close();
                    Console.Write("[ФАЙЛ СОЗДАН].");
                    Console.ReadLine();
                }
                else
                {
                    Input_error_message();
                    return;
                }
            }
            else
            {
                Input_error_message();
                return;
            }

        }

        static void File_gen_menu()
        {
            Console.Clear();
            Console.WriteLine("==ГЕНЕРИРОВАНИЕ ИСХОДНЫХ ДАННЫХ==");
            Console.Write("1: Для Лучшего случая. (min-max)\n" +
                          "2: Для Среднего случая.(random)\n" +
                          "3: Для Худшего случая. (max-min)\n" +
                          "Ввод: ");
            string menu_qst = Console.ReadLine();
            switch (menu_qst)
            {
                case "1":
                    {
                        Best_file_gen();
                        break;
                    }
                case "2":
                    {
                        Average_file_gen();
                        break;
                    }
                case "3":
                    {
                        Worst_file_gen();
                        break;
                    }
                default:
                    {
                        Input_error_message();
                        break;
                    }
            }
        }

        static void Main_menu()
        {
            Console.Clear();
            Console.WriteLine("==ТЕСТИРОВАНИЕ АЛГОРИТМОВ СОРТИРОВКИ==");
            Console.Write("1: Ручное тестирование.\n" +
                          "2: Автоматическое тестирование.\n" +
                          "3: Формирование файлов с исходными данными.\n" +
                          "Ввод: ");
            string menu_qst = Console.ReadLine();
            switch (menu_qst)
            {
                case "1":
                    {
                        Manual_test_menu();
                        break;
                    }
                case "2":
                    {
                        Auto_test_menu();
                        break;
                    }
                case "3":
                    {
                        File_gen_menu();
                        break;
                    }
                default:
                    {
                        Input_error_message();
                        break;
                    }
            }
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Main_menu();
            }
        }
    }
}
