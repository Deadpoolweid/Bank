﻿using System;
using System.Collections.Generic;
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
        }

        Core core = new Core();

        private void bCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (inputCheck() == false)
            {
                MessageBox.Show("Данные введены неверно.");
                return;
            }

            //MessageBox.Show("Данные введены верно.");

            core.SaveData(Convert.ToDouble(tS.Text,CultureInfo.InvariantCulture),Convert.ToDouble(tP.Text, CultureInfo.InvariantCulture),
                Convert.ToDouble(tN.Text,CultureInfo.InvariantCulture));

            ServicePaymentType type = ServicePaymentType.NoFee;
            double sum = 0;
            if (checkBox.IsChecked == true)
            {
                switch (cbServicePayment.SelectedIndex)
                {
                    case 0:
                        type = ServicePaymentType.ProcessingFee;
                        break;
                    case 1:
                        type = ServicePaymentType.MothlyFee;
                        break;
                    case 2:
                        type = ServicePaymentType.AnnualComission;
                        break;
                }
                sum = Double.Parse(tServiceSum.Text,CultureInfo.InvariantCulture);
            }

            core.SaveComission(type,sum);

            if (cbPayment.SelectedIndex == 0)
            {
                dgResults.ItemsSource = core.createPaymentGraph(PaymentType.Differentiated).DefaultView;
            }
            else
            {
                dgResults.ItemsSource = core.createPaymentGraph(PaymentType.Annuity).DefaultView;
            }
        }

        /// <summary>
        /// Проверка на правильность введённых данных во всех полях
        /// </summary>
        /// <returns>Результат проверки</returns>
        private bool inputCheck()
        {
            var check = Regex.IsMatch(tS.Text, "\\d*\\.?\\d+");


            if (!Regex.IsMatch(tN.Text, "\\d+"))
            {
                check = false;
            }

            if (!Regex.IsMatch(tP.Text, "[1]?[1-9]?\\d"))
            {
                check = false;
            }

            return check;
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
    }
}
