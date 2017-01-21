using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bank
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            CbPayment.SelectedIndex = 0;
            CbServicePayment.SelectedIndex = 0;
            DpDate.DisplayDate = DateTime.Today;


            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(@"/Images/Bank.png", UriKind.Relative);
            b.EndInit();


            System.Windows.Controls.Image img = new Image
            {
                Source = b,
                Width = 40
            };

            MessageBox.Show("Количество imageй ", "ImageChoose", MessageBoxButton.OK, MessageBoxImage.Asterisk);

            spBody.Children.Add(img);
        }

        readonly Core _core = new Core();

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

            var type = ServicePaymentType.NoFee;
            double sum = 0;
            if (CheckBox.IsChecked == true)
            {
                switch (CbServicePayment.SelectedIndex)
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
                sum = double.Parse(TServiceSum.Text,CultureInfo.InvariantCulture);
            }

            _core.SaveData(Convert.ToDouble(Ts.Text, CultureInfo.InvariantCulture), Convert.ToDouble(Tp.Text, CultureInfo.InvariantCulture),
Convert.ToInt32(Tn.Text, CultureInfo.InvariantCulture), type, sum, DpDate.DisplayDate,Convert.ToDouble(TEqualPayment.Text,CultureInfo.InvariantCulture));

            var pType = (PaymentType)CbPayment.SelectedIndex;

            if (pType == PaymentType.Equal)
            {
                double x = _core.calc_x(Convert.ToDouble(Ts.Text, CultureInfo.InvariantCulture),
                    Convert.ToDouble(Tp.Text, CultureInfo.InvariantCulture)/100,
                    Convert.ToInt32(Tn.Text, CultureInfo.InvariantCulture));
                if (Convert.ToDouble(TEqualPayment.Text, CultureInfo.InvariantCulture) < x)
                {
                    MessageBox.Show("Сумма платежа должна быть не меньше " + x + " руб.");
                    TEqualPayment.Foreground = Brushes.Red;
                    return;
                }
                else
                {
                    TEqualPayment.Foreground = Brushes.Green;
                }
            }

            _core.Calculate(pType);

            DgResults.ItemsSource = _core.CreatePaymentGraph();
        }

        /// <summary>
        /// Проверка на правильность введённых данных во всех полях
        /// </summary>
        /// <returns>Результат проверки</returns>
        private bool CheckInput()
        {
            Ts.Foreground = CheckReal(Ts.Text) ? Brushes.Green : Brushes.Red;

            Tn.Foreground = CheckInt(Tn.Text) ? Brushes.Green : Brushes.Red;

            Tp.Foreground = CheckPercent(Tp.Text) ? Brushes.Green : Brushes.Red;

            TServiceSum.Foreground = CheckReal(TServiceSum.Text) ? Brushes.Green : Brushes.Red;

            TEqualPayment.Foreground = CheckReal(TEqualPayment.Text) ? Brushes.Green : Brushes.Red;

            return CheckReal(Ts.Text) && CheckInt(Tn.Text) && CheckPercent(Tp.Text) && CheckReal(TServiceSum.Text) 
                && CheckReal(TEqualPayment.Text);
        }

        /// <summary>
        /// Проверяет строку на соответствие вещественному числу
        /// </summary>
        /// <param name="line">Проверяемая строка</param>
        /// <returns>Результат проверки</returns>
        private static bool CheckReal(string line)
        {
            return Regex.IsMatch(line, "^\\d*\\.?\\d+\\z");
        }

        /// <summary>
        /// Проверяет строку на соответствие целому числу
        /// </summary>
        /// <param name="line">Проверяемая строка</param>
        /// <returns>Результат проверки</returns>
        private static bool CheckInt(string line)
        {
            return Regex.IsMatch(line, "^\\d+\\z");
        }

        /// <summary>
        /// Проверяет строку на соответствие процентам (в том числе дробным)
        /// </summary>
        /// <param name="line">Проверяемая строка</param>
        /// <returns>Результат проверки</returns>
        private static bool CheckPercent(string line)
        {
            return Regex.IsMatch(line, "^[1]?[1-9]?\\d\\.?\\d+\\z");
        }

        /// <summary>
        /// Проверяет все Textbox элементы на пустую строку
        /// </summary>
        /// <returns>Результат проверки</returns>
        private bool CheckForNull()
        {
            Ts.Background = Ts.Text == "" ? Brushes.Yellow : Brushes.Transparent;

            TEqualPayment.Background = TEqualPayment.Text == "" ? Brushes.Yellow : Brushes.Transparent;

            Tn.Background = Tn.Text == "" ? Brushes.Yellow : Brushes.Transparent;

            Tp.Background = Tp.Text == "" ? Brushes.Yellow : Brushes.Transparent;

            TServiceSum.Background = TServiceSum.Text == "" ? Brushes.Yellow : Brushes.Transparent;

            var texts = new List<string>(5) {Ts.Text, TEqualPayment.Text, Tn.Text, Tp.Text, TServiceSum.Text};


            return texts.All(t => t != "");
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            TServiceSum.IsEnabled = true;
            TServiceSum.Focusable = true;

            CbServicePayment.IsEnabled = true;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TServiceSum.IsEnabled = false;
            TServiceSum.Focusable = false;

            CbServicePayment.IsEnabled = false;
        }

        private void cbPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TEqualPayment.IsEnabled = CbPayment.SelectedIndex == 2;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                bCalculate_Click(e, new RoutedEventArgs());
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Автор: Глухов Владимир 2016");
        }
    }
}
