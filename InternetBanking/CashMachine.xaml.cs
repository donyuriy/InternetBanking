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
using System.Windows.Shapes;
using System.Threading;

namespace InternetBanking
{
    /// <summary>
    /// Interaction logic for CashMachine.xaml
    /// </summary>
    public partial class CashMachine : Window       //этот код дублируется из класса MainWindow (все комменты можно найти там)
    {
        //все компоненты формы CashMachine имеют в своём названии окончание CM
        string[] cards;
        Logics lObj = new Logics();
        public CashMachine()
        {
            InitializeComponent();

            lbPinCM.Visibility = System.Windows.Visibility.Hidden;
            tbPinCM.Visibility = System.Windows.Visibility.Hidden;
            btnOKCM.Visibility = System.Windows.Visibility.Hidden;
            tbCashCM.Visibility = System.Windows.Visibility.Hidden;
            lbOncardCM.Visibility = System.Windows.Visibility.Hidden;
            lbUSDCM.Visibility = System.Windows.Visibility.Hidden;
            btn1CM.IsEnabled = false;
            tb1CM.IsEnabled = false;
            cards = lObj.ReadCards();
            foreach (string item in cards)
            {
                cb1CM.Items.Add(item);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)   //снять наличные
        {
            long cardNumber = 0;
            double summ = 0; ;
            string tempCashValue = tbCashCM.ToString();
            if (lbOncardCM.Visibility == System.Windows.Visibility.Visible)
            {
                try
                {
                    cardNumber = Convert.ToInt64(cb1CM.SelectedItem.ToString());
                    summ = Convert.ToDouble(tb1CM.Text);
                }
                catch { }
                try
                {
                    if ((Convert.ToInt32(tb1CM.Text) <= Convert.ToInt32(tbCashCM.Text)) && (Convert.ToInt32(tb1CM.Text) >= 0))
                    {
                        object[] temp = new object[2];  //запихиваем массив в object для отправки в метод
                        try
                        {

                            temp[0] = Convert.ToInt32(cb1CM.SelectedIndex); //номер строки с суммой
                            temp[1] = (Convert.ToInt32(tbCashCM.Text) - Convert.ToInt32(tb1CM.Text));     //сумма для снятия
                            Thread th2 = new Thread(lObj.GetCash);
                            th2.Start(temp);    //старт потока с входящим object
                            th2.Join();
                            errLblCM.Content = string.Format("{0} сумма снята со счёта {1}", summ, cardNumber);
                            tbCashCM.Text = lObj.ReadCash()[cb1CM.SelectedIndex];
                        }
                        catch (Exception)
                        {
                            errLblCM.Content = "Проверьте входящие данные!";
                            Thread.Sleep(2000);
                        }
                    }
                    else
                    {
                        errLblCM.Content = "Недостаточно денег на вашем счету";
                        return;
                    }
                }
                catch
                {
                    errLblCM.Content = "Ошибка. Транзакция отменена.";
                    Thread.Sleep(3000);
                }
            }
        }

        private void cb1CM_SelectionChanged(object sender, SelectionChangedEventArgs e)     //сменить номер карты
        {
            errLblCM.Content = "Internet Banking is avaliable";
            tbCashCM.Visibility = System.Windows.Visibility.Hidden;
            lbOncardCM.Visibility = System.Windows.Visibility.Hidden;
            lbUSDCM.Visibility = System.Windows.Visibility.Hidden;
            lbPinCM.Visibility = System.Windows.Visibility.Visible;
            tbPinCM.Visibility = System.Windows.Visibility.Visible;
            btnOKCM.Visibility = System.Windows.Visibility.Visible;
            btn1CM.IsEnabled = false;
            tb1CM.IsEnabled = false;
        }

        private void btnOKCM_Click(object sender, RoutedEventArgs e)    //"ОК" (проверка PIN)
        {
            string[] pinMass = lObj.ReadPin();
            try
            {
                if (Convert.ToInt32(pinMass[cb1CM.SelectedIndex]) == Convert.ToInt32(tbPinCM.Text))
                {
                    errLblCM.Content = "PIN принят!";
                    tbCashCM.Visibility = System.Windows.Visibility.Visible;
                    lbOncardCM.Visibility = System.Windows.Visibility.Visible;
                    lbUSDCM.Visibility = System.Windows.Visibility.Visible;
                    btn1CM.IsEnabled = true;
                    tb1CM.IsEnabled = true;
                    tbCashCM.Text = lObj.ReadCash()[cb1CM.SelectedIndex];
                }
                else
                {
                    errLblCM.Content = "Неверный PIN!";
                    tbCashCM.Visibility = System.Windows.Visibility.Hidden;
                    lbOncardCM.Visibility = System.Windows.Visibility.Hidden;
                    lbUSDCM.Visibility = System.Windows.Visibility.Hidden;
                    btn1CM.IsEnabled = false;
                    tb1CM.IsEnabled = false;
                }
            }
            catch
            {
                errLblCM.Content = "Введите PIN";
            }
        }
    }
}
