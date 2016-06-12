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

        /// <summary>
        /// Расчёт основного платежа
        /// </summary>
        /// <param name="S">Размер кредита</param>
        /// <param name="N">Количество месяцев</param>
        /// <returns>Основной платёж</returns>
        public double calc_b(double S, double N) => S/N;

        /// <summary>
        /// Расчёт остатка задолженности
        /// </summary>
        /// <param name="S">Размер кредита</param>
        /// <param name="b">Основной платёж</param>
        /// <param name="n">Количество прошедших периодов</param>
        /// <returns>Остаток задолженности</returns>
        public double calc_Sn(double S, double b, double n) => S - (b*n);

        /// <summary>
        /// Расчёт начисленных процентов
        /// </summary>
        /// <param name="Sn">Остаток задолженности</param>
        /// <param name="P">Годовая ставка</param>
        /// <returns>Начисленные проценты</returns>
        public double calc_p(double Sn, double P) => Sn*P/12;

        /// <summary>
        /// Получить полную сумму выплат
        /// </summary>
        /// <returns>Полная сумма выплат</returns>
        public double getPaymentInfo()
        {
            return Data.paymentsSum;
        }

        /// <summary>
        /// Создаёт таблицу "График выплат"
        /// </summary>
        /// <returns>График выплат</returns>
        public DataTable createPaymentGraph()
        {
            DataTable table = new DataTable("График выплат");

            table.Columns.Add("Месяц №");
            table.Columns.Add("Остаток");
            table.Columns.Add("Основной платёж");
            table.Columns.Add("Проценты");
            table.Columns.Add("Всего за платёж");

            string format = "#.##";

            //for (int i = 1; i <= 4; i++)
            //{
            //    table.Columns[i].Expression = format;
            //}


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

            return table;
        }
    }
}
