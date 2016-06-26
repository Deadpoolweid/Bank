using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    class Core
    {
        /// <summary>
        /// Хранилище
        /// </summary>
        private Data Data;

        /// <summary>
        /// Сохранение введённых данных
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="type">Тип платежа</param>
        /// <param name="sType">Тип комиссии</param>
        /// <param name="sPayment">Размер комиссии</param>
        public void SaveData(double S, double P, int N, ServicePaymentType type = ServicePaymentType.NoFee, double sPayment = 0, DateTime date = default(DateTime), double EqualPayment = 0)
        {
            Data data = new Data(S,N,P/100, type, sPayment);
            data.StartDate = date;
            data.EqualPayment = EqualPayment;
            Data = data;
        }

        #region Расчёты по дифференцированному платёжу

        /// <summary>
        /// Расчёт основного платежа по дифф. платежу
        /// </summary>
        /// <param name="S">Размер кредита</param>
        /// <param name="N">Количество месяцев</param>
        /// <returns>Основной платёж</returns>
        public double calc_b(double S, int N) => (double)S/N;

        /// <summary>
        /// Расчёт остатка задолженности по дифф. платежу
        /// </summary>
        /// <param name="S">Размер кредита</param>
        /// <param name="b">Основной платёж</param>
        /// <param name="n">Количество прошедших периодов</param>
        /// <returns>Остаток задолженности</returns>
        public double calc_Sn(double S, double b, double n) => S - (b*n);

        /// <summary>
        /// Расчёт начисленных процентов по дифф. платежу
        /// </summary>
        /// <param name="Sn">Остаток задолженности</param>
        /// <param name="P">Годовая ставка</param>
        /// <returns>Начисленные проценты</returns>
        public double calc_p(double Sn, double P) => Sn*P/12;
        #endregion

        #region Расчёты по аннуитетному платёжу

        /// <summary>
        /// Расчёт месячного платежа по аннуитету
        /// </summary>
        /// <param name="S">Сумма кредита</param>
        /// <param name="P">Процентная ставка</param>
        /// <param name="N">Срок кредита(в месяцах)</param>
        /// <returns>Месячный платёж</returns>
        public double calc_x(double S, double P, int N)
        {
            double p = P / 12;
            return S * (p + ((p) / (Math.Pow(1 + p, N) - 1)));
        }

        /// <summary>
        /// Расчёт начисленных процентов по аннуитету
        /// </summary>
        /// <param name="Sn">Остаток задолженности</param>
        /// <param name="P">Процентная ставка</param>
        /// <returns>Начисленные проценты</returns>
        public double calc_Pn(double Sn, double P)
        {
            return Sn * P / 12;
        }

        /// <summary>
        /// Расчёт основного платежа по аннуитету
        /// </summary>
        /// <param name="x">Месячный платёж</param>
        /// <param name="Pn">Проценты на момент n-ой выплаты</param>
        /// <returns>Выплата на погашение долга</returns>
        public double calc_s(double x, double Pn)
        {
            return x - Pn;
        }

        #endregion

        #region Расчёты по платежам равными частями


        #endregion

        /// <summary>
        /// Расчёт комиссии на указанный месяц(Начиная с нулевого)
        /// </summary>
        /// <param name="type">Тип комиссии</param>
        /// <param name="i">Месяц</param>
        /// <returns>Комиссия на указанный месяц</returns>
        private double calc_sp(ServicePaymentType type = ServicePaymentType.NoFee, int i = 0)
        {
            if (Data.ServicePaymentType == ServicePaymentType.NoFee)
            {
                return 0;
            }
            else if (Data.ServicePaymentType == ServicePaymentType.Amount)
            {
                return Data.spValue * Data.S / 100;
            }
            else if (Data.ServicePaymentType == ServicePaymentType.Residual)
            {
                return Data.spValue * Data.PaymentLeft[i] / 100;
            }
            else
            {
                throw new ArgumentException("Тип комиссии в неверном формате.");
            }
        }

        /// <summary>
        /// Расчитывает все необходимые значения и заносит их в хранилище
        /// </summary>
        /// <param name="type"></param>
        public void Calculate(PaymentType type)
        {
            Data.PaymentType = type;
            for (int i = 0; i < Data.N; i++)
            {
                // Остатока долга
                if (i == 0)
                {
                    Data.PaymentLeft[0] = Data.S;
                }
                else
                {
                    Data.PaymentLeft[i] = Data.PaymentLeft[i - 1] - Data.generalPayment[i - 1]; // Остаток долга
                }

                // Даты платежа
                if (i == 0)
                {
                    Data.Dates[0] = Data.StartDate;
                }
                else
                {
                    Data.Dates[i] = Data.Dates[i - 1].AddMonths(1);
                }

                Data.sp[i] = calc_sp(Data.ServicePaymentType, i);   // Комиссия

                if (type == PaymentType.Differentiated)
                {
                    Data.generalPayment[i] = calc_b(Data.S, Data.N);    // Основной платёж
                    Data.p[i] = calc_p(Data.PaymentLeft[i], Data.P);    // Начисленные проценты
                    Data.payment[i] = Data.generalPayment[i] + Data.p[i] + Data.sp[i];  // Полный платёж за месяц
                }
                else if (type == PaymentType.Annuity)
                {
                    Data.generalPayment[i] = calc_s(calc_x(Data.S,Data.P,Data.N),calc_Pn(Data.PaymentLeft[i],Data.P));    // Основной платёж
                    Data.p[i] = calc_Pn(Data.PaymentLeft[i],Data.P);    // Начисленные проценты
                    Data.payment[i] = calc_x(Data.S, Data.P, Data.N) + Data.sp[i];  // Полный платёж за месяц
                }
                else if (type == PaymentType.Equal)
                {
                    Data.p[i] = calc_p(Data.PaymentLeft[i], Data.P);    // Начисленные проценты
                    Data.payment[i] = Data.PaymentLeft[i] > Data.EqualPayment ? Data.EqualPayment : Data.PaymentLeft[i] + Data.p[i];    // Полный платёж за месяц
                    Data.generalPayment[i] = Data.payment[i] - Data.p[i] - Data.sp[i];  // Основной платёж

                    if (Data.PaymentLeft[i] == 0)
                    {
                        Data.N = i;
                    }
                }
            }
        }

        /// <summary>
        /// Создаёт таблицу "График выплат"
        /// </summary>
        /// <returns>График выплат</returns>
        public DataView createPaymentGraph()
        {
            DataTable table = new DataTable("График выплат");

            table.Columns.Add("Месяц №");
            table.Columns.Add("Дата");
            table.Columns.Add("Остаток");
            table.Columns.Add("Основной платёж");
            table.Columns.Add("Проценты");
            table.Columns.Add("Обслуживание");
            table.Columns.Add("Всего за платёж");

            string format = "#.##";

            //table.Columns[0].AutoIncrement = true;
            //table.Columns[0].AutoIncrementSeed = 1;
            //table.Columns[0].AutoIncrementStep = 1;

            for (int i = 0; i < Data.N; i++)
            {
                DataRow dr = table.NewRow();

                dr.ItemArray = new object[]
                {
                        i + 1,  // Номер месяца
                        Data.Dates[i].Date.ToShortDateString(),
                        Data.PaymentLeft[i].ToString(format),  //Остаток платежа
                        Data.generalPayment[i].ToString(format),    // Основной платёж
                        Data.p[i].ToString(format),    // Начисленные проценты
                        Data.sp[i].ToString(format), // Комиссия
                        Data.payment[i].ToString(format) // Всего за платёж
                };

                table.Rows.Add(dr);
            }

            // Последняя строка
            DataRow drEnd = table.NewRow();
            drEnd.ItemArray = new object[]
            {
                    "Итого",
                    "-",
                    "-",
                    Data.generalPayment.Sum().ToString(format), // Сумма основных платежей
                    Data.pSum.ToString(format), // Сумма начисленных процентов
                    Data.sp.Sum().ToString(format), // Сумма комиссионных выплат
                    Data.paymentsSum.ToString(format)   // Всего уплачено
            };

            table.Rows.Add(drEnd);

            return table.DefaultView;
        }
    }
}
