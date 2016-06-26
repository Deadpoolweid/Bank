using System;
using System.Linq;

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
        public ServicePaymentType ServicePaymentType;

        /// <summary>
        /// Величина комиссии
        /// </summary>
        public double SpValue;

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
        /// <param name="type">Тип платежа</param>
        /// <param name="sp">Величина комиссии</param>
        public Data(double s, int n, double p, ServicePaymentType type, double sp)
        {
            S = s;
            N = n;
            P = p;
            ServicePaymentType = type;
            SpValue = sp;

            Sp = new double[N];
            PaymentLeft = new double[N];
            GeneralPayment = new double[N];
            this.p = new double[N];
            Payment = new double[N];
            Dates = new DateTime[N];
        }

        /// <summary>
        /// Основной платёж
        /// </summary>
        public double[] GeneralPayment;

        /// <summary>
        /// Комисиия
        /// </summary>
        public double[] Sp;

        /// <summary>
        /// Остаток задолженности
        /// </summary>
        public double[] PaymentLeft;

        /// <summary>
        /// Начисленные проценты
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public double[] p;

        /// <summary>
        /// Полный платёж
        /// </summary>
        public double[] Payment;

        /// <summary>
        /// Полная сумма выплат
        /// </summary>
        public double PaymentsSum => Payment.Sum();

        /// <summary>
        /// Полная сумма начисленных процентов
        /// </summary>
        public double PSum => p.Sum();

        /// <summary>
        /// Все месяцы уплаты
        /// </summary>
        public DateTime[] Dates;
    }
}
