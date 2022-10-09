using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ConverterValut
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public decimal usdValue = 0, eurValue = 0, jpyValue = 0;
        public MainWindow()
        {
            InitializeComponent();
            //Инициализируем объекта типа XmlTextReader и
            //загружаем XML документ с сайта центрального банка
            XmlTextReader reader = new XmlTextReader("http://www.cbr.ru/scripts/XML_daily.asp");
            //В эти переменные будем сохранять куски XML
            //с определенными валютами (EUR, USD, JPY)
            string USDXml = "";
            string EURXml = "";
            string JPYXml = "";
            //Перебираем все узлы в загруженном документе
            while (reader.Read())
            {
                //Проверяем тип текущего узла
                switch (reader.NodeType)
                {
                    //Если этого элемент Valute, то начинаем анализировать атрибуты
                    case XmlNodeType.Element:

                        if (reader.Name == "Valute")
                        {
                            if (reader.HasAttributes)
                            {
                                //Метод передвигает указатель к следующему атрибуту
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "ID")
                                    {
                                        //Если значение атрибута равно R01235, то перед нами информация о курсе доллара
                                        if (reader.Value == "R01235")
                                        {
                                            //Возвращаемся к элементу, содержащий текущий узел атрибута
                                            reader.MoveToElement();
                                            //Считываем содержимое дочерних узлом
                                            USDXml = reader.ReadOuterXml();
                                        }
                                    }

                                    //Аналогичную процедуру делаем для ЕВРО
                                    if (reader.Name == "ID")
                                    {
                                        if (reader.Value == "R01239")
                                        {
                                            reader.MoveToElement();
                                            EURXml = reader.ReadOuterXml();
                                        }
                                    }

                                    //И для японсикх иен
                                    if (reader.Name == "ID")
                                    {
                                        if (reader.Value == "R01820")
                                        {
                                            reader.MoveToElement();
                                            JPYXml = reader.ReadOuterXml();
                                        }
                                    }
                                }
                            }
                        }

                        break;
                }
            }

            //Из выдернутых кусков XML кода создаем новые XML документы
            XmlDocument usdXmlDocument = new XmlDocument();
            usdXmlDocument.LoadXml(USDXml);
            XmlDocument eurXmlDocument = new XmlDocument();
            eurXmlDocument.LoadXml(EURXml);
            XmlDocument jpyXmlDocument = new XmlDocument();
            jpyXmlDocument.LoadXml(JPYXml);
            //Метод возвращает узел, соответствующий выражению XPath
            XmlNode xmlNode = usdXmlDocument.SelectSingleNode("Valute/Value");

            //Считываем значение и конвертируем в decimal. Курс валют получен
            usdValue = Convert.ToDecimal(xmlNode.InnerText);
            xmlNode = eurXmlDocument.SelectSingleNode("Valute/Value");
            eurValue = Convert.ToDecimal(xmlNode.InnerText);
            xmlNode = jpyXmlDocument.SelectSingleNode("Valute/Value");
            jpyValue = Convert.ToDecimal(xmlNode.InnerText);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            StartPanel.Visibility = Visibility.Hidden;
            Curs.Visibility = Visibility.Visible;
        }

        private void CountBtn_Click(object sender, RoutedEventArgs e)
        {
           
            try
            {
                decimal Count = Convert.ToDecimal(CountBox.Text);
                decimal result;

                if (IN.SelectedIndex == 1)
                {
                    result = Count / usdValue;
                    ResultText.Content = $"{Count} RUB = {Math.Round(result, 2)} USD";
                }
                else if (IN.SelectedIndex == 2)
                {
                    result = Count / eurValue;
                    ResultText.Content = $"{Count} RUB = {Math.Round(result, 2)} EUR";
                }
                else if (IN.SelectedIndex == 3)
                {
                    result = Count / jpyValue;
                    ResultText.Content = $"{Count} RUB = {Math.Round(result, 2)} JPY";
                }
                else if (OUT.SelectedIndex == 1)
                {
                    result = Count * usdValue;
                    ResultText.Content = $"{Count} USD = {Math.Round(result, 2)} RUB";
                }
                else if (OUT.SelectedIndex == 2)
                {
                    result = Count * eurValue;
                    ResultText.Content = $"{Count} EUR = {Math.Round(result, 2)} RUB";
                }
                else if (OUT.SelectedIndex == 3)
                {
                    result = Count * jpyValue;
                    ResultText.Content = $"{Count} JPY = {Math.Round(result, 2)} RUB";
                }
            }
            catch 
            {
                MessageBox.Show("Неверный формат количества. Введите правильно. Пример: 100 или 55,55", "Неверный формат количества", MessageBoxButton.OK, MessageBoxImage.Error);
            }   
        }

        private void IN_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IN.SelectedIndex != -1 && (IN.SelectedIndex == 1 || IN.SelectedIndex == 2 || IN.SelectedIndex == 3))
            {
                OUT.SelectedIndex = 0;
            }
            if ((IN.SelectedIndex != -1 && IN.SelectedIndex == 0 && IN.SelectedIndex == 1) || (IN.SelectedIndex != -1 && IN.SelectedIndex == 1 && OUT.SelectedIndex == 0))
            {
                CURSRes.Content = $"{usdValue}";
            }
            else if ((IN.SelectedIndex != -1 && IN.SelectedIndex == 0 && IN.SelectedIndex == 2) || (IN.SelectedIndex != -1 && IN.SelectedIndex == 2 && OUT.SelectedIndex == 0))
            {
                CURSRes.Content = $"{eurValue}";
            }
            else if ((IN.SelectedIndex != -1 && IN.SelectedIndex == 0 && IN.SelectedIndex == 3) || (IN.SelectedIndex != -1 && IN.SelectedIndex == 3 && OUT.SelectedIndex == 0))
            {
                CURSRes.Content = $"{jpyValue}";
            }
            else if (IN.SelectedIndex == 0 && OUT.SelectedIndex == 0)
            {
                MessageBox.Show("Нельзя конвертировать в одну и туже валюту", "Ошибка выбора валют", MessageBoxButton.OK, MessageBoxImage.Error);
                IN.SelectedIndex = 1;
            }
        }

        public void OUT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OUT.SelectedIndex != -1 && (OUT.SelectedIndex == 1 || OUT.SelectedIndex == 2 || OUT.SelectedIndex == 3))
            {
                IN.SelectedIndex = 0;
            }
            if ((OUT.SelectedIndex != -1 && OUT.SelectedIndex == 0 && IN.SelectedIndex == 1) || (OUT.SelectedIndex != -1 && OUT.SelectedIndex == 1 && IN.SelectedIndex == 0))
            {
                CURSRes.Content = $"{usdValue}";
            }
            else if ((OUT.SelectedIndex != -1 && OUT.SelectedIndex == 0 && IN.SelectedIndex == 2) || (OUT.SelectedIndex != -1 && OUT.SelectedIndex == 2 && IN.SelectedIndex == 0))
            {
                CURSRes.Content = $"{eurValue}";
            }
            else if ((OUT.SelectedIndex != -1 && OUT.SelectedIndex == 0 && IN.SelectedIndex == 3) || (OUT.SelectedIndex != -1 && OUT.SelectedIndex == 3 && IN.SelectedIndex == 0))
            {
                CURSRes.Content = $"{jpyValue}";
            }
            else if (IN.SelectedIndex == 0 && OUT.SelectedIndex == 0)
            {
                MessageBox.Show("Нельзя конвертировать в одну и туже валюту", "Ошибка выбора валют", MessageBoxButton.OK, MessageBoxImage.Error);
                OUT.SelectedIndex = 1;
            }
        }
    }
}
