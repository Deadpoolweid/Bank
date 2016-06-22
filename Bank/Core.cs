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
        private Data Data;

        /// <summary>
        /// Сохранение введённых данных
        /// </summary>
        /// <param name="data">Данные</param>
        public void SaveData(double S, double P, double N)
        {
            Data data = new Data(S,N,P/100,calc_b(S,N));
            Data = data;
        }

        #region Дифференцированный платёж

        /// <summary>
        /// Расчёт основного платежа по дифф. платежу
        /// </summary>
        /// <param name="S">Размер кредита</param>
        /// <param name="N">Количество месяцев</param>
        /// <returns>Основной платёж</returns>
        public double calc_b(double S, double N) => S/N;

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

        // TODO возможно лишнее
        /// <summary>
        /// Получить полную сумму выплат по дифф. платежу
        /// </summary>
        /// <returns>Полная сумма выплат</returns>
        public double getPaymentInfo()
        {
            return Data.paymentsSum;
        }



        #endregion


        #region Аннуитетный платёж

        /// <summary>
        /// Расчёт месячного платежа по аннуитету
        /// </summary>
        /// <param name="S">Сумма кредита</param>
        /// <param name="P">Процентная ставка</param>
        /// <param name="N">Срок кредита(в месяцах)</param>
        /// <returns>Месячный платёж</returns>
        public double calc_x(double S, double P, double N)
        {
            double p = P/12;
            return S*(p + ((p)/(Math.Pow(1 + p, N) - 1)));
        }

        /// <summary>
        /// Расчёт начисленных процентов по аннуитету
        /// </summary>
        /// <param name="Sn">Остаток задолженности</param>
        /// <param name="P">Процентная ставка</param>
        /// <returns>Начисленные проценты</returns>
        public double calc_Pn(double Sn, double P)
        {
            return Sn*P/12;
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

        /// <summary>
        /// Создаёт таблицу "График выплат"
        /// </summary>
        /// <returns>График выплат</returns>
        public DataTable createPaymentGraph(PaymentType type)
        {
            DataTable table = new DataTable("График выплат");

            table.Columns.Add("Месяц №");
            table.Columns.Add("Остаток");
            table.Columns.Add("Основной платёж");
            table.Columns.Add("Проценты");
            table.Columns.Add("Всего за платёж");

            string format = "#.##";

            if (type == PaymentType.Differentiated)
            {
                double left = Data.S;

                for (int i = 0; i < Data.N; i++)
                {
                    DataRow dr = table.NewRow();
                    dr.ItemArray = new object[]
                    {
                        i + 1,
                        left.ToString(format),
                        Data.b.ToString(format),
                        calc_p(calc_Sn(Data.S, Data.b, i), Data.P).ToString(format),
                        (Data.b + calc_p(calc_Sn(Data.S, Data.b, i), Data.P)).ToString(format)
                    };

                    table.Rows.Add(dr);
                    left -= Data.b;
                }
            }
            else
            {
                double left = Data.S;

                for (int i = 0; i < Data.N; i++)
                {
                    DataRow dr = table.NewRow();
                    dr.ItemArray = new object[]
                    {
                        i + 1,
                        left.ToString(format),
                        calc_s(calc_x(Data.S,Data.P,Data.N),calc_Pn(left,Data.P)).ToString(format),
                        calc_Pn(left,Data.P).ToString(format),
                        calc_x(Data.S,Data.P,Data.N).ToString(format)
                    };

                    table.Rows.Add(dr);
                    left -= calc_s(calc_x(Data.S,Data.P,Data.N),calc_Pn(left,Data.P));
                }
            }



            return table;
        }
    }
}
