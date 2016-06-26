using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    class Data
    {
        #region Ввведённые данные

        /// <summary>
        /// Сумма кредита
        /// </summary>
        public double S;

        /// <summary>
        /// Количество месяцев
        /// </summary>
        public int N;

        /// <summary>
        /// Годовая ставка
        /// </summary>
        public double P;

        /// <summary>
        /// Дата получения кредита
        /// </summary>
        public DateTime StartDate;

        /// <summary>
        /// Тип платежа
        /// </summary>
        public PaymentType PaymentType;

        /// <summary>
        /// Тип обслуживания счёта
        /// </summary>
        public ServicePaymentType ServicePaymentType = ServicePaymentType.NoFee;

        /// <summary>
        /// Величина комиссии
        /// </summary>
        public double spValue;

        /// <summary>
        /// Величина равного платежа
        /// </summary>
        public double EqualPayment;
        #endregion

        /// <summary>
        /// Сохранение данных
        /// </summary>
        /// <param name="s">Сумма кредита</param>
        /// <param name="n">Количество месяцев</param>
        /// <param name="p">Годовая ставка</param>
        /// <param name="b">Основной платёж</param>
        /// <param name="type">Тип платежа</param>
        /// <param name="sp">Величина комиссии</param>
        public Data(double s, int n, double p, ServicePaymentType type, double sp)
        {
            S = s;
            N = n;
            P = p;
            ServicePaymentType = type;
            spValue = sp;

            this.sp = new double[N];
            PaymentLeft = new double[N];
            generalPayment = new double[N];
            this.p = new double[N];
            payment = new double[N];
            Dates = new DateTime[N];
        }

        /// <summary>
        /// Основной платёж
        /// </summary>
        public double[] generalPayment;

        /// <summary>
        /// Комисиия
        /// </summary>
        public double[] sp;

        /// <summary>
        /// Остаток задолженности
        /// </summary>
        public double[] PaymentLeft;

        /// <summary>
        /// Начисленные проценты
        /// </summary>
        public double[] p;

        /// <summary>
        /// Полный платёж
        /// </summary>
        public double[] payment;

        /// <summary>
        /// Полная сумма выплат
        /// </summary>
        public double paymentsSum => payment.Sum();

        /// <summary>
        /// Полная сумма начисленных процентов
        /// </summary>
        public double pSum => p.Sum();

        /// <summary>
        /// Все месяцы уплаты
        /// </summary>
        public DateTime[] Dates;
    }
}
