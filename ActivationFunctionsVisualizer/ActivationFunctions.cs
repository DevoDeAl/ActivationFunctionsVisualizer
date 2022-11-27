using System;
using Newtonsoft.Json;
namespace NeoroCort
{
    #region Интерфейсы и абстракции

    public abstract class ActivationFunction : IActivationFunction
    {
        [JsonProperty("alpha")]
        public double alpha;
        [JsonProperty("beta")]
        public double beta;
        public ActivationFunction(double alpha) => this.alpha = alpha;
        #region Абстракции
        public abstract double Activate(double x);
        public abstract double Derivate(double x);
        public abstract string GetName();
        public abstract string GetDescription();
        #endregion
        /// <summary>
        /// Вычисление производной приблизительным способом
        /// </summary>
        /// <param name="x"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public double DerivateLight(double x, double offset = 0.001)
        {
            double a = Activate(x * alpha + offset);

            double b = Activate(x * alpha - offset);

            return (a + b)/(offset*2);
        }
        /// <summary>
        /// Активация с применение Softmax выпрямления
        /// </summary>
        /// <param name="x">Аргумент</param>
        /// <param name="itemsCount">Количество нейронов в слое</param>
        /// <returns>Выровненое значение</returns>
        /// <remarks>Применяется на выходных слоях для нормализации</remarks>
        //public double ActivateWithSoftmax(double x, IEnumerable<Neuron> neurons)
        //{
        //    double activatedX = Activate(x);
        //    double activatedExponent = Math.Exp(activatedX);
        //    double exponentialSum = neurons
        //        .Select(neuron => neuron.Output)
        //        .Aggregate((a,b) => Math.Exp(a) + Math.Exp(b));
        //    return activatedExponent / exponentialSum;
        //}
    }

    /// <summary>
    /// Функция активации
    /// </summary>
    public interface IActivationFunction
    {
        /// <summary>
        /// Наименование функции активации
        /// </summary>
        /// <returns>Имя функции в формате String</returns>
        string GetName();
        /// <summary>
        /// Функция активации
        /// </summary>
        /// <param name="x">Аргумент</param>
        /// <returns>Значение функции в точке</returns>
        double Activate(double x);
        /// <summary>
        /// Производная функции активации
        /// </summary>
        /// <param name="x">Аргумент</param>
        /// <returns>Значение производной в точке</returns>
        double Derivate(double x);
    }

    #endregion
    #region Функции активации
    /// <summary>
    /// Сигмоида
    /// </summary>
    /// <remarks>Обычно используется для моделей, где мы должны предсказать вероятность в качестве результата. Поскольку вероятность чего-либо существует только в диапазоне от 0 до 1, сигмоид является правильным выбором из-за его диапазона. Функция дифференцируема и обеспечивает плавный градиент, т.е. Предотвращает скачки выходных значений. Это представлено S-образной формой сигмовидной функции активации.</remarks>
    public class Sigmoid : ActivationFunction
    {
        public override string GetName() { return "Sigmoid"; }

        public override string GetDescription() { return "Обычно используется для моделей, где мы должны предсказать вероятность в качестве результата. Поскольку вероятность чего-либо существует только в диапазоне от 0 до 1, сигмоид является правильным выбором из-за его диапазона. Функция дифференцируема и обеспечивает плавный градиент, т.е. Предотвращает скачки выходных значений. Это представлено S-образной формой сигмовидной функции активации."; }

        public  Sigmoid(double alpha) : base(alpha) { }

        public override double Activate(double x) => 1 / (1 + Math.Exp(alpha * -x));

        //public double Activate(double outsignal) => outsignal * (alpha - outsignal);

        public override double Derivate(double x)
        {
            double f = Activate(x);
            return alpha * f * (1 - f);
        }
    }

    /// <summary>
    /// Гиперболический тангенс
    /// </summary>
    /// <remarks>Выходные данные функции активации tanh центрированы на нуле; следовательно, мы можем легко отобразить выходные значения как сильно отрицательные, нейтральные или сильно положительные. Обычно используется в скрытых слоях нейронной сети, поскольку его значения лежат в диапазоне от -1 до; следовательно, среднее значение для скрытого слоя оказывается равным 0 или очень близко к нему. Это помогает центрировать данные и значительно упрощает обучение для следующего уровня</remarks>
    public class Tanh : ActivationFunction
    {
        /// <summary>
        /// Удобные значения параметров:
        /// alpha = 1.7159 ;
        /// beta = 2 / 3 .
        /// </summary>
        public override string GetName() { return "Tanh"; }

        public override string GetDescription() { return "Обычно используется для моделей, где мы должны предсказать вероятность в качестве результата. Поскольку вероятность чего-либо существует только в диапазоне от 0 до 1, сигмоид является правильным выбором из-за его диапазона. Функция дифференцируема и обеспечивает плавный градиент, т.е. Предотвращает скачки выходных значений. Это представлено S-образной формой сигмовидной функции активации."; }

        public Tanh(double alpha, double beta) : base(alpha) => this.beta = beta;

        public Tanh(double alpha) : base(alpha) => this.beta = 2 / 3;

        //public override double Activate(double x) => alpha * Math.Tanh(beta * x);

        public override double Activate(double x) => (Math.Exp(alpha*x) - Math.Exp(alpha * -x)) / (Math.Exp(alpha * x) + Math.Exp(alpha * -x));

        public override double Derivate(double x)
        {
            //double f = Activate(x);
            //double c = beta / alpha;
            //return c * (alpha * alpha - f * f);

            return 1 - Math.Pow(Math.Tanh(x),2);
        }
    }

    /// <summary>
    /// Выпрямитель
    /// </summary>
    /// <remarks>Поскольку активируется только определенное количество нейронов, функция ReLU намного более эффективна с точки зрения вычислений по сравнению с сигмовидной и tanh функциями. ReLU ускоряет сходимость градиентного спуска к глобальному минимуму функции потерь благодаря своему линейному, ненасыщающему свойству</remarks>
    public class ReLU : ActivationFunction
    {
        public override string GetName() { return "ReLU"; }

        public override string GetDescription() { return "Поскольку активируется только определенное количество нейронов, функция ReLU намного более эффективна с точки зрения вычислений по сравнению с сигмовидной и tanh функциями. ReLU ускоряет сходимость градиентного спуска к глобальному минимуму функции потерь благодаря своему линейному, ненасыщающему свойству."; }

        public ReLU(double alpha) : base(alpha) { }

        public override double Activate(double x) => x < 0 ? alpha * x : x;

        public override double Derivate(double x) => x < 0 ? -alpha : 1;
    }

    /// <summary>
    /// Текучий выпрямитель
    /// </summary>
    /// <remarks>Как ReLU + обеспечивает обратное распространение даже при отрицательных входных значениях.</remarks>
    public class LeakyReLU : ActivationFunction
    {
        public override string GetName() { return "LeakyReLU"; }

        public override string GetDescription() { return "Как ReLU + обеспечивает обратное распространение даже при отрицательных входных значениях."; }

        public LeakyReLU(double alpha) : base(alpha) { }

        public override double Activate(double x) => x < x * 0.1 ? alpha * x : x;

        public override double Derivate(double x) => x < x * 0.1 ? -alpha : 1;
    }

    /// <summary>
    /// Параметрический выпрямитель
    /// </summary>
    /// <remarks>Как ReLU + обеспечивает обратное распространение даже при отрицательных входных значениях. Параметризованная функция ReLU используется, когда протекающая функция ReLU по-прежнему не справляется с решением проблемы мертвых нейронов, и соответствующая информация не передается успешно на следующий уровень.</remarks>
    public class ParametricReLU : ActivationFunction
    {
        public override string GetName() { return "ParametricReLU"; }

        public override string GetDescription() { return "Как ReLU + обеспечивает обратное распространение даже при отрицательных входных значениях. Параметризованная функция ReLU используется, когда протекающая функция ReLU по-прежнему не справляется с решением проблемы мертвых нейронов, и соответствующая информация не передается успешно на следующий уровень."; }

        public ParametricReLU(double alpha) : base(alpha) { }

        public override double Activate(double x) => x < x * 0.1 ? alpha * x : x;

        public override double Derivate(double x) => x < x * 0.1 ? -alpha : 1;
    }

    /// <summary>
    /// Функция экспоненциальных линейных единиц
    /// </summary>
    /// <remarks>ELU медленно сглаживается, пока его выходной сигнал не станет равным -α, тогда как RELU резко сглаживается.Позволяет избежать проблемы мертвого ReLU, введя логарифмическую кривую для отрицательных значений входных данных. Это помогает сети подталкивать веса и смещения в правильном направлении.</remarks>
    public class ELU : ActivationFunction, IActivationFunction
    {
        public override string GetName() { return "ELU"; }

        public override string GetDescription() { return "ELU медленно сглаживается, пока его выходной сигнал не станет равным -α, тогда как RELU резко сглаживается.Позволяет избежать проблемы мертвого ReLU, введя логарифмическую кривую для отрицательных значений входных данных. Это помогает сети подталкивать веса и смещения в правильном направлении."; }

        public ELU(double alpha) : base(alpha) { }

        public override double Activate(double x) => x < 0 ? alpha * (Math.Exp(x) - 1) : x;

        public override double Derivate(double x) => x < 0 ? (alpha * (Math.Exp(x) - 1)) + alpha : 1;
    }

    /// <summary>
    /// Смягчённый выпрямитель
    /// </summary>
    public class SoftPlus : ActivationFunction, IActivationFunction
    {
        public override string GetName() { return "SoftPlus"; }

        public override string GetDescription() { return "???"; }

        public SoftPlus(double alpha) : base(alpha) { }

        public override double Activate(double x) => Math.Log(1 + Math.Exp(alpha * x));

        public override double Derivate(double x) => alpha / (1 + Math.Exp(-x * alpha));
    }

    /// <summary>
    /// Взмах
    /// </summary>
    /// <remarks>Cаморегулируемая функция активации, разработанная исследователями из Google. Swish последовательно соответствует или превосходит функцию активации ReLU в глубоких сетях, применяемую к различным сложным областям, таким как классификация изображений, машинный перевод и т.д.</remarks>
    public class Swish : ActivationFunction, IActivationFunction
    {
        public override string GetName() { return "Swish"; }

        public override string GetDescription() { return "Cаморегулируемая функция активации, разработанная исследователями из Google. Swish последовательно соответствует или превосходит функцию активации ReLU в глубоких сетях, применяемую к различным сложным областям, таким как классификация изображений, машинный перевод и т.д."; }

        public Swish(double alpha) : base(alpha) { }

        public override double Activate(double x) => x / (1 + Math.Exp(alpha * -x));

        public override double Derivate(double x)
        {
            double f = Activate(x);

            return alpha * f * 1 / (1 + Math.Exp(alpha * -x))*(1 - f);
        }
    }

    /// <summary>
    /// Линейная единица гауссовой ошибки.
    /// </summary>
    /// <remarks>Нелинейность GELU лучше, чем активации ReLU и ELU, и обеспечивает повышение производительности во всех задачах в областях компьютерного зрения, обработки естественного языка и распознавания речи.</remarks>
    public class GeLU : ActivationFunction, IActivationFunction
    {
        public override string GetName() { return "GeLU"; }

        public override string GetDescription() { return "Нелинейность GELU лучше, чем активации ReLU и ELU, и обеспечивает повышение производительности во всех задачах в областях компьютерного зрения, обработки естественного языка и распознавания речи."; }

        public GeLU(double alpha) : base(alpha) { }

        public override double Activate(double x) => 0.5 * x * alpha * (1 + Math.Tanh(Math.Sqrt(2 / Math.PI) * (x + 0.044715 * Math.Pow(x,3))));

        public override double Derivate(double x) /// Поставить шаг поменьше - демонастрационный пример.
        {
            double a = Activate(x * alpha + 1);

            double b = Activate(x * alpha - 1);

            return (a + b) / (1 * 2);
        }
    }

    /// <summary>
    /// Масштабируемая экспоненциальная линейная единица.
    /// </summary>
    /// <remarks>SELU является относительно новой функцией активации и требует дополнительных работ по таким архитектурам, как CNN и RNN, где она сравнительно изучена.</remarks>
    public class SeLU : ActivationFunction, IActivationFunction
    {
        public override string GetName() { return "SeLU"; }

        public override string GetDescription() { return "SELU является относительно новой функцией активации и требует дополнительных работ по таким архитектурам, как CNN и RNN, где она сравнительно изучена."; }

        public SeLU(double alpha) : base(alpha) { }

        public override double Activate(double x) => x < 0 ? 1.0507 * 1.6733 * (Math.Exp(alpha * x) - 1) : 1.0507 * x * alpha;

        public override double Derivate(double x) => x < 0 ? 1.0507 * 1.6733 * Math.Exp(alpha * x) : 1.0507;
    }
    
    #endregion

}
