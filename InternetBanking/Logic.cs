using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace InternetBanking
{
    class Logics
    {
        Mutex mutex = new Mutex(false, "mutex");
        private string cardsPath = @"Files/cards.txt";
        private string pinPath = @"Files/pins.txt";
        private string cashOnCard = @"Files/cash.txt";
        static string[] o; //массив возвращаемых значений из файла static так как его днина используется в нескольких методах!

        public string[] ReadCards()
        {
            if (File.Exists(cardsPath))
            {
                using (Stream s = new FileStream(cardsPath, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(s))
                    {
                        int i = 0;
                        while (!sr.EndOfStream)
                        {
                            sr.ReadLine();
                            i++;
                        }
                        o = new string[i];
                        i = 0;
                        s.Position = 0;
                        while (!sr.EndOfStream)
                        {
                            o[i] = sr.ReadLine();
                            i++;
                        }
                    }
                }
                return o;
            }
            else
                return null;
        }

        public string[] ReadPin()
        {
            if (File.Exists(pinPath))
            {
                using (Stream s = new FileStream(pinPath, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(s))
                    {
                        int i = 0;
                        while (!sr.EndOfStream)
                        {
                            sr.ReadLine();
                            i++;
                        }
                        o = new string[i];
                        i = 0;
                        s.Position = 0;
                        while (!sr.EndOfStream)
                        {
                            o[i] = sr.ReadLine();
                            i++;
                        }
                    }
                }
                return o;
            }
            else
                return null;
        }

        public string[] ReadCash()
        {
            if (File.Exists(cashOnCard))
            {
                using (Stream s = new FileStream(cashOnCard, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(s))
                    {
                        int i = 0;
                        while (!sr.EndOfStream)
                        {
                            sr.ReadLine();
                            i++;
                        }
                        o = new string[i];
                        i = 0;
                        s.Position = 0;
                        while (!sr.EndOfStream)
                        {
                            o[i] = sr.ReadLine();
                            i++;
                        }
                    }
                }
                return o;
            }
            else
                return null;
        }

        public void GetCash(object obj) //снятие налички с карточного счёта
        {
            if (File.Exists(cashOnCard))    //проверка на наличие файла с которым будем работать
            {
                mutex.WaitOne();
                object[] a = obj as object[];
                int line = (int)a[0];
                string[] mass = new string[o.Length];
                using (Stream s = new FileStream(cashOnCard, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(s))
                    {
                        s.Seek(0, SeekOrigin.Begin);
                        int i = 0;
                        while (!sr.EndOfStream)
                        {
                            mass[i] = sr.ReadLine();
                            i++;
                        }
                        mass[line] = Convert.ToString(a[1]);
                    }
                }

                for (int i = 0; i < 10; i++)    //иммитация действий по работе с базой данных(задержка в потока)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }

                using (Stream s = new FileStream(cashOnCard, FileMode.Create))  // файл с наличными проще переписать чем вклинивать в него новые значения
                {                                                               //
                    using (StreamWriter sw = new StreamWriter(s))
                    {
                        s.Seek(0, SeekOrigin.Begin);
                        for (int i = 0; i < mass.Length; i++)
                        {
                            sw.WriteLine(mass[i]);
                        }
                    }
                }
                mutex.ReleaseMutex();
            }
            else
            {
                Console.WriteLine("Error! File {0} does not exist!", cashOnCard);
            }
        }
    }
}
