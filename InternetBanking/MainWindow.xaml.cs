using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;

namespace InternetBanking
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    delegate void CashMachineStart();
    public partial class MainWindow : Window
    {
        //все компоненты формы MainWindow имеют в своём названии окончание IB(InternetBanking)
        string[] cards;                         //значение всех номеров карт     
        Logics lObj = new Logics();
         
        
        public MainWindow()
        {
            InitializeComponent();
            this.Height = 350;
            this.Width = 500; 

            Thread CMStart = new Thread(() => //запуск формы CashMachine будет в новом потоке
            {
                CashMachine CM = new CashMachine();   //создаём новый обЪект CashMachine в новом потоке
                CM.Show();
                CM.Height = 350;        
                CM.Width = 500;
                System.Windows.Threading.Dispatcher.Run();  //работа с Диспетчером
            });
            CMStart.IsBackground = true;    //!!!!ЭТО РЕШЕНИЕ ДЛЯ ТОГО ЧТОБЫ ПРОГРАММА НОРИАЛЬНО ЗАВЕРШАЛАСЬ (HELP!!!)
            CMStart.SetApartmentState(ApartmentState.STA);  // Устанавливаем AppartmentState в STA для работы с компонентами формы
            CMStart.Start();    //старт CashMachine

            lbPinIB.Visibility = System.Windows.Visibility.Hidden;  //часть компонентов формы при старте скрыта или неактивна
            tbPinIB.Visibility = System.Windows.Visibility.Hidden;
            btnOkIB.Visibility = System.Windows.Visibility.Hidden;
            tbCashIB.Visibility = System.Windows.Visibility.Hidden;
            lbOncardIB.Visibility = System.Windows.Visibility.Hidden;
            lbUSDIB.Visibility = System.Windows.Visibility.Hidden;
            btn1IB.IsEnabled = false;
            tb1IB.IsEnabled = false;
            cards = lObj.ReadCards();
            foreach (string item in cards)
            {
                cb1IB.Items.Add(item);  //заносим номера всех карт в ComboBox
            }
            
        }

        private void btn1_Click(object sender, RoutedEventArgs e)   //Снять наличные
        {
            long cardNumber = 0;    //значения cardNumber и summ нужны только для выведения сообщения о правильном проведении транзакции
            double summ = 0; ;
            string tempCashValue = tbCashIB.ToString();     //значение суммы наличных на счёте до снятия
            if (lbOncardIB.Visibility == System.Windows.Visibility.Visible)
            {
                try
                {
                    cardNumber = Convert.ToInt64(cb1IB.SelectedItem.ToString());
                    summ = Convert.ToDouble(tb1IB.Text);
                }
                catch { }
                try
                {
                    //если сумма на счету больше запрашиваемой и запрашиваемая сумма больше нуля
                    if ((Convert.ToInt32(tb1IB.Text) <= Convert.ToInt32(tbCashIB.Text)) && (Convert.ToInt32(tb1IB.Text) >= 0))
                    {
                        object[] temp = new object[2];  //запихиваем массив в object для отправки в метод в новом потоке
                        try
                        {

                            temp[0] = Convert.ToInt32(cb1IB.SelectedIndex);                                 //номер строки с суммой
                            temp[1] = (Convert.ToInt32(tbCashIB.Text) - Convert.ToInt32(tb1IB.Text));       //сумма для снятия
                            Thread th1 = new Thread(lObj.GetCash);
                            th1.Start(temp);                                                                //старт потока с входящим object
                            th1.Join();                                                             //ожидание завершения операции снятия наличных
                            errLblIB.Content = string.Format("{0} сумма снята со счёта {1}", summ, cardNumber);
                            tbCashIB.Text = lObj.ReadCash()[cb1IB.SelectedIndex];   //TextBoxу с доступными средствами присваиваем новое значение после транзакции
                        }
                        catch (Exception)
                        {
                            errLblIB.Content = "Проверьте входящие данные!";
                            Thread.Sleep(2000);
                        }
                    }
                    else
                    {   //если денег недостаточно или введена некорректная сумма для снятия
                        errLblIB.Content = "Недостаточно денег на вашем счету";
                        return;
                    }
                }
                catch
                {
                    errLblIB.Content = "Ошибка. Транзакция отменена.";
                    Thread.Sleep(2000);
                }
            }
        }

        private void cb1_SelectionChanged(object sender, SelectionChangedEventArgs e)   //Сменить номер карты
        {
            tbCashIB.Visibility = System.Windows.Visibility.Hidden;
            lbOncardIB.Visibility = System.Windows.Visibility.Hidden;
            lbUSDIB.Visibility = System.Windows.Visibility.Hidden;
            lbPinIB.Visibility = System.Windows.Visibility.Visible;
            tbPinIB.Visibility = System.Windows.Visibility.Visible;
            btnOkIB.Visibility = System.Windows.Visibility.Visible;
            btn1IB.IsEnabled = false;
            tb1IB.IsEnabled = false;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)  //"ОК" (проверка PIN)
        {
            string[] pinMass = lObj.ReadPin();  //массив PINов для всех карт
            try
            {
                if (Convert.ToInt32(pinMass[cb1IB.SelectedIndex]) == Convert.ToInt32(tbPinIB.Text)) //если PIN правильный
                {
                    errLblIB.Content = "PIN принят!";
                    tbCashIB.Visibility = System.Windows.Visibility.Visible;
                    lbOncardIB.Visibility = System.Windows.Visibility.Visible;
                    lbUSDIB.Visibility = System.Windows.Visibility.Visible;
                    btn1IB.IsEnabled = true;
                    tb1IB.IsEnabled = true;
                    tbCashIB.Text = lObj.ReadCash()[cb1IB.SelectedIndex];   //отображаем значение наличных для выбраной карты
                }
                else   //если PIN не правильный
                {
                    errLblIB.Content = "Неверный PIN!";
                    tbCashIB.Visibility = System.Windows.Visibility.Hidden;
                    lbOncardIB.Visibility = System.Windows.Visibility.Hidden;
                    lbUSDIB.Visibility = System.Windows.Visibility.Hidden;
                    btn1IB.IsEnabled = false;
                    tb1IB.IsEnabled = false;
                }
            }
            catch   //отрабатывает если поле PIN пустое
            {
                errLblIB.Content = "Введите PIN";
            }
        }

        private void MainWindowClosed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
