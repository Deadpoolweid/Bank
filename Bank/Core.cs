using System;
using System.Data;
using System.Linq;

namespace Bank
{
    class Core
    {
        /// <summary>
        /// Хранилище
        /// </summary>
        private Data _data;

        /// <summary>
        /// Сохранение введённых данных
        /// </summary>
        /// <param name="n">Срок кредита</param>
        /// <param name="type">Тип платежа</param>
        /// <param name="sPayment">Размер комиссии</param>
        /// <param name="s">Размер кредита</param>
        /// <param name="p">Годовая ставка</param>
        /// <param name="date">Дата получения кредита</param>
        /// <param name="equalPayment">Величина уплаты при платеже равными суммами</param>
        public void SaveData(double s, double p, int n, ServicePaymentType type = ServicePaymentType.NoFee, double sPayment = 0, DateTime date = default(DateTime),
            double equalPayment = 0)
        {
            var data = new Data(s, n, p/100, type, sPayment)
            {
                StartDate = date,
                EqualPayment = equalPayment
            };
            _data = data;
        }

        #region Расчёты по дифференцированному платёжу

        /// <summary>
        /// Расчёт основного платежа по дифф. платежу
        /// </summary>
        /// <param name="s">Размер кредита</param>
        /// <param name="n">Количество месяцев</param>
        /// <returns>Основной платёж</returns>
        public double calc_b(double s, int n) => s/n;

        /// <summary>
        /// Расчёт остатка задолженности по дифф. платежу
        /// </summary>
        /// <param name="s">Размер кредита</param>
        /// <param name="b">Основной платёж</param>
        /// <param name="n">Количество прошедших периодов</param>
        /// <returns>Остаток задолженности</returns>
        public double calc_Sn(double s, double b, double n) => s - (b*n);
        #endregion

        #region Расчёты по аннуитетному платёжу

        /// <summary>
        /// Расчёт месячного платежа по аннуитету
        /// </summary>
        /// <param name="s">Сумма кредита</param>
        /// <param name="p">Процентная ставка</param>
        /// <param name="n">Срок кредита(в месяцах)</param>
        /// <returns>Месячный платёж</returns>
        public double calc_x(double s, double p, int n)
        {
            p = p / 12;
            return s * (p + ((p) / (Math.Pow(1 + p, n) - 1)));
        }

        /// <summary>
        /// Расчёт основного платежа по аннуитету
        /// </summary>
        /// <param name="x">Месячный платёж</param>
        /// <param name="pn">Проценты на момент n-ой выплаты</param>
        /// <returns>Выплата на погашение долга</returns>
        public double calc_s(double x, double pn) => x - pn;

        #endregion

        /// <summary>
        /// Расчёт начисленных процентов
        /// </summary>
        /// <param name="sn">Остаток задолженности</param>
        /// <param name="p">Годовая ставка</param>
        /// <returns>Начисленные проценты</returns>
        public double calc_p(double sn, double p) => sn * p / 12;

        /// <summary>
        /// Расчёт комиссии на указанный месяц(Начиная с нулевого)
        /// </summary>
        /// <param name="i">Месяц</param>
        /// <returns>Комиссия на указанный месяц</returns>
        private double calc_sp(int i = 0)
        {
            switch (_data.ServicePaymentType)
            {
                case ServicePaymentType.NoFee:
                    return 0;
                case ServicePaymentType.Amount:
                    return _data.SpValue * _data.S / 100;
                case ServicePaymentType.Residual:
                    return _data.SpValue * _data.PaymentLeft[i] / 100;
                default:
                    throw new ArgumentException("Тип комиссии в неверном формате.");
            }
        }

        /// <summary>
        /// Расчитывает все необходимые значения и заносит их в хранилище
        /// </summary>
        /// <param name="type"></param>
        public void Calculate(PaymentType type)
        {
            _data.PaymentType = type;
            for (var i = 0; i < _data.N; i++)
            {
                // Остатока долга
                if (i == 0)
                {
                    _data.PaymentLeft[0] = _data.S;
                }
                else
                {
                    _data.PaymentLeft[i] = _data.PaymentLeft[i - 1] - _data.GeneralPayment[i - 1]; // Остаток долга
                }

                // Даты платежа
                if (i == 0)
                {
                    _data.Dates[0] = _data.StartDate;
                }
                else
                {
                    _data.Dates[i] = _data.Dates[i - 1].AddMonths(1);
                }

                _data.Sp[i] = calc_sp(i); // Комиссия

                if (type == PaymentType.Differentiated)
                {
                    _data.GeneralPayment[i] = calc_b(_data.S, _data.N); // Основной платёж
                    _data.p[i] = calc_p(_data.PaymentLeft[i], _data.P); // Начисленные проценты
                    _data.Payment[i] = _data.GeneralPayment[i] + _data.p[i] + _data.Sp[i]; // Полный платёж за месяц
                }
                else if (type == PaymentType.Annuity)
                {
                    _data.GeneralPayment[i] = calc_s(calc_x(_data.S, _data.P, _data.N), calc_p(_data.PaymentLeft[i], _data.P)); // Основной платёж
                    _data.p[i] = calc_p(_data.PaymentLeft[i], _data.P); // Начисленные проценты
                    _data.Payment[i] = calc_x(_data.S, _data.P, _data.N) + _data.Sp[i]; // Полный платёж за месяц
                }
                else if (type == PaymentType.Equal)
                {
                    _data.p[i] = calc_p(_data.PaymentLeft[i], _data.P); // Начисленные проценты
                    _data.Payment[i] = _data.PaymentLeft[i] > _data.EqualPayment ? _data.EqualPayment : _data.PaymentLeft[i] + _data.p[i]; // Полный платёж за месяц
                    _data.GeneralPayment[i] = _data.Payment[i] - _data.p[i] - _data.Sp[i]; // Основной платёж

                    if (Math.Abs(_data.PaymentLeft[i]) < double.Epsilon)
                    {
                        _data.N = i;
                    }
                }
            }
        }

        /// <summary>
        /// Создаёт таблицу "График выплат"
        /// </summary>
        /// <returns>График выплат</returns>
        public DataView CreatePaymentGraph()
        {
            var table = new DataTable("График выплат");

            table.Columns.Add("Месяц №");
            table.Columns.Add("Дата");
            table.Columns.Add("Остаток");
            table.Columns.Add("Основной платёж");
            table.Columns.Add("Проценты");
            table.Columns.Add("Обслуживание");
            table.Columns.Add("Всего за платёж");

            var format = "#.##";

            //table.Columns[0].AutoIncrement = true;
            //table.Columns[0].AutoIncrementSeed = 1;
            //table.Columns[0].AutoIncrementStep = 1;

            for (var i = 0; i < _data.N; i++)
            {
                var dr = table.NewRow();

                dr.ItemArray = new object[]
                {
                    i + 1, // Номер месяца
                    _data.Dates[i].Date.ToShortDateString(), _data.PaymentLeft[i].ToString(format), //Остаток платежа
                    _data.GeneralPayment[i].ToString(format), // Основной платёж
                    _data.p[i].ToString(format), // Начисленные проценты
                    _data.Sp[i].ToString(format), // Комиссия
                    _data.Payment[i].ToString(format) // Всего за платёж
                };

                table.Rows.Add(dr);
            }

            // Последняя строка
            var drEnd = table.NewRow();
            drEnd.ItemArray = new object[]
            {
                "Итого", "-", "-", _data.GeneralPayment.Sum().ToString(format), // Сумма основных платежей
                _data.PSum.ToString(format), // Сумма начисленных процентов
                _data.Sp.Sum().ToString(format), // Сумма комиссионных выплат
                _data.PaymentsSum.ToString(format) // Всего уплачено
            };

            table.Rows.Add(drEnd);

            return table.DefaultView;
        }
    }
}
