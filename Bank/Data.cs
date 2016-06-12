using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    class Data
    {
        /// <summary>
        /// Сумма кредита
        /// </summary>
        public double S;

        /// <summary>
        /// Количество месяцев
        /// </summary>
        public double N;

        /// <summary>
        /// Годовая ставка
        /// </summary>
        public double P;

        /// <summary>
        /// Основной платёж
        /// </summary>
        public double b;

        /// <summary>
        /// Сохранение данныъ
        /// </summary>
        /// <param name="s">Сумма кредита</param>
        /// <param name="n">Количество месяцев</param>
        /// <param name="p">Годовая ставка</param>
        /// <param name="b">Основной платёж</param>
        public Data(double s, double n, double p, double b)
        {
            S = s;
            N = n;
            P = p;
            this.b = b;
        }

        /// <summary>
        /// Полная сумма выплат
        /// </summary>
        public double paymentsSum;

        /// <summary>
        /// Основной платёж
        /// </summary>
        public double generalPayment;
    }
}
