﻿using System;
using System.Collections.Generic;
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

            core.SaveData(Convert.ToDouble(tS.Text),Convert.ToDouble(tP.Text),Convert.ToDouble(tN.Text));

            dgResults.ItemsSource = core.createPaymentGraph().DefaultView;
        }

        private bool inputCheck()
        {
            bool check = true;

            if (!Regex.IsMatch(tS.Text, "\\d*\\.?\\d+"))
            {
                check = false;
            }

            

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
    }
}