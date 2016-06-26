using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Bank
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            cbPayment.SelectedIndex = 0;
            cbServicePayment.SelectedIndex = 0;
            dpDate.DisplayDate = DateTime.Today;
        }

        Core core = new Core();

        private void bCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (CheckForNull() == false)
            {
                MessageBox.Show("Оставлены пустые поля.");
                return;
            }

            if (CheckInput() == false)
            {
                MessageBox.Show("Данные введены неверно.");
                return;
            }

            //MessageBox.Show("Данные введены верно.");

            ServicePaymentType type = ServicePaymentType.NoFee;
            double sum = 0;
            if (checkBox.IsChecked == true)
            {
                switch (cbServicePayment.SelectedIndex)
                {
                    case 0:
                        type = ServicePaymentType.Amount;
                        break;
                    case 1:
                        type = ServicePaymentType.Residual;
                        break;
                    default:
                        type = ServicePaymentType.NoFee;
                        break;
                }
                sum = Double.Parse(tServiceSum.Text,CultureInfo.InvariantCulture);
            }

            core.SaveData(Convert.ToDouble(tS.Text, CultureInfo.InvariantCulture), Convert.ToDouble(tP.Text, CultureInfo.InvariantCulture),
Convert.ToInt32(tN.Text, CultureInfo.InvariantCulture), type, sum, dpDate.DisplayDate,Convert.ToDouble(tEqualPayment.Text,CultureInfo.InvariantCulture));

            PaymentType pType;
            if (cbPayment.SelectedIndex == 0)
            {
                pType = PaymentType.Differentiated;
            }
            else if (cbPayment.SelectedIndex == 1)
            {
                pType = PaymentType.Annuity;
            }
            else
            {
                pType = PaymentType.Equal;
            }

            core.Calculate(pType);

            dgResults.ItemsSource = core.createPaymentGraph();
        }

        /// <summary>
        /// Проверка на правильность введённых данных во всех полях
        /// </summary>
        /// <returns>Результат проверки</returns>
        private bool CheckInput()
        {
            bool result = false;

            tS.Foreground = checkReal(tS.Text) ? Brushes.Green : Brushes.Red;

            tN.Foreground = checkInt(tN.Text) ? Brushes.Green : Brushes.Red;

            tP.Foreground = checkPercent(tP.Text) ? Brushes.Green : Brushes.Red;

            tServiceSum.Foreground = checkReal(tServiceSum.Text) ? Brushes.Green : Brushes.Red;

            tEqualPayment.Foreground = checkReal(tEqualPayment.Text) ? Brushes.Green : Brushes.Red;

            return checkReal(tS.Text) && checkInt(tN.Text) && checkPercent(tP.Text) && checkReal(tServiceSum.Text) 
                && checkReal(tEqualPayment.Text);
        }

        private bool checkReal(string line)
        {
            return Regex.IsMatch(line, "^\\d*\\.?\\d+\\z");
        }

        private bool checkInt(string line)
        {
            return Regex.IsMatch(line, "^\\d+\\z");
        }

        private bool checkPercent(string line)
        {
            return Regex.IsMatch(line, "^[1]?[1-9]?\\d\\.?\\d+\\z");
        }

        private bool CheckForNull()
        {
            tS.Background = tS.Text == "" ? Brushes.Yellow : Brushes.Transparent;

            tEqualPayment.Background = tEqualPayment.Text == "" ? Brushes.Yellow : Brushes.Transparent;

            tN.Background = tN.Text == "" ? Brushes.Yellow : Brushes.Transparent;

            tP.Background = tP.Text == "" ? Brushes.Yellow : Brushes.Transparent;

            tServiceSum.Background = tServiceSum.Text == "" ? Brushes.Yellow : Brushes.Transparent;

            List<string> texts = new List<string>(5);

            texts.Add(tS.Text);
            texts.Add(tEqualPayment.Text);
            texts.Add(tN.Text);
            texts.Add(tP.Text);
            texts.Add(tServiceSum.Text);

            return texts.All(t => t != "");
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            tServiceSum.IsEnabled = true;
            tServiceSum.Focusable = true;

            cbServicePayment.IsEnabled = true;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            tServiceSum.IsEnabled = false;
            tServiceSum.Focusable = false;

            cbServicePayment.IsEnabled = false;
        }

        private void cbPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPayment.SelectedIndex == 2)
            {
                tEqualPayment.IsEnabled = true;
            }
            else
            {
                tEqualPayment.IsEnabled = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                bCalculate_Click(e, new RoutedEventArgs());
            }
        }
    }
}
