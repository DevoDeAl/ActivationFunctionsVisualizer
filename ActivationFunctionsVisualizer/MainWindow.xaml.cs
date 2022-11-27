using LiveCharts.Wpf;
using LiveCharts;
using NeoroCort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ActivationFunctionsVisualizer
{
    /// <summary>
    /// Окно визуализации функций активации из CS - файла.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Поля

        /// <summary>
        /// Коллекция графиков
        /// </summary>
        private SeriesCollection _seriesCollection
            = new SeriesCollection();

        /// <summary>
        /// Класс рандомизации
        /// </summary>
        private Random _random
            = new Random();

        #endregion

        #region Конструктор

        public MainWindow()
        {
            InitializeComponent();

            _chart.Series = _seriesCollection;

            List<ActivationFunction> functions = GetAllActivationsFunctions();

            foreach (ActivationFunction function in functions)
            {
                string name = function.GetName();

                string description = function.GetDescription();

                (LineSeries activation, LineSeries derive) seriesTuple = AddNewChart(name, function);

                CheckBox checkBox = new CheckBox()
                {
                    Content = name,

                    ToolTip = description,

                    DataContext = seriesTuple,

                    Margin = new Thickness(5, 0, 0, 0)
                };

                stackPanel.Children.Add(checkBox);

                checkBox.Click += CheckBox_Click;

            }

            #region Настройка осей

            _chart.AxisX.Clear();

            _chart.AxisY.Clear();

            _chart.AxisX.Add(new Axis
            {
                Title = "X",

                LabelFormatter = x => ((20 - x) / 2).ToString(),

                Separator = new LiveCharts.Wpf.Separator() { StrokeThickness = 1, Step = 20, Stroke = new SolidColorBrush(Colors.LightGray) },

            });

            _chart.AxisY.Add(new Axis
            {
                Title = "Y",

                LabelFormatter = y => (Math.Round(y,1)).ToString(),

                Separator = new LiveCharts.Wpf.Separator() { StrokeThickness = 1, Step = 1, Stroke = new SolidColorBrush(Colors.LightGray) },

            });

            #endregion

            Show();
        }

        #endregion

        #region Реализация событий

        /// <summary>
        /// Действия при щелчке по чек-боксу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.IsChecked == true)
            {
                (LineSeries activation, LineSeries derive) dataContext
                    = ((LineSeries activation, LineSeries derive))checkBox.DataContext;

                dataContext.derive.Visibility = Visibility.Visible;

                dataContext.activation.Visibility = Visibility.Visible;
            }
            else
            {
                (LineSeries activation, LineSeries derive) dataContext
                    = ((LineSeries activation, LineSeries derive))checkBox.DataContext;

                dataContext.derive.Visibility = Visibility.Collapsed;

                dataContext.activation.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавить новый график
        /// </summary>
        /// <param name="name"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public (LineSeries activation, LineSeries derive) AddNewChart(string name, ActivationFunction func)
        {
            (LineSeries activation, LineSeries derive) result;

            #region Прямой проход
            Color color
            = Color.FromRgb((byte)_random.Next(0, 255),
            (byte)_random.Next(0, 255),
            (byte)_random.Next(0, 255));

            ChartValues<double> chartValue = new ChartValues<double>();

            LineSeries lineSeries = new LineSeries
            {

                Title = $"Активация {name}",

                Values = chartValue,

                PointGeometry = DefaultGeometries.Diamond,

                PointGeometrySize = 10,

                Stroke = new SolidColorBrush(color),

                Fill = new SolidColorBrush(color)
                {
                    Opacity = 0.1
                },
            };

            for (double i = -10; i < 10; i += 0.5)
            {
                chartValue.Add(Math.Round(func.Activate(i)));
            }

            _seriesCollection.Add(lineSeries);

            lineSeries.Visibility = Visibility.Collapsed;

            #endregion

            #region Обратный проход
            Color color_d
            = Color.FromRgb((byte)_random.Next(0, 255),
            (byte)_random.Next(0, 255),
            (byte)_random.Next(0, 255));

            ChartValues<double> chartValue_d = new ChartValues<double>();

            LineSeries lineSeries_d = new LineSeries
            {

                Title = $"Производная {name}",

                Values = chartValue_d,

                PointGeometry = DefaultGeometries.Diamond,

                PointGeometrySize = 10,

                Stroke = new SolidColorBrush(color_d),

                Fill = new SolidColorBrush(color_d)
                {
                    Opacity = 0.1
                },
            };

            for (double i = -10; i < 10; i += 0.5)
            {
                chartValue_d.Add(Math.Round(func.Derivate(i),3));
            }

            _seriesCollection.Add(lineSeries_d);

            lineSeries_d.Visibility = Visibility.Collapsed;

            #endregion

            result = (lineSeries, lineSeries_d);

            return result;
        }

        /// <summary>
        /// Получить все функции активации из файла
        /// </summary>
        /// <returns></returns>
        private List<ActivationFunction> GetAllActivationsFunctions()
        {
            List<ActivationFunction> result = new List<ActivationFunction>();

            IEnumerable<Type> functions = Assembly.GetExecutingAssembly()
            .GetExportedTypes()
            .Where(t => t.IsSubclassOf(typeof(ActivationFunction)));

            foreach (Type func in functions)
            {
                ActivationFunction function = Activator.CreateInstance(func, 1) as ActivationFunction;

                result.Add(function);
            }

            return result;
        }

        #endregion
    }
}
