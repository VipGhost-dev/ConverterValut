# ConverterValut

ConverterValut - само название говорит за себя.<br/>Простой конвертер валют с взятием курса онлайн с центрального банка России.

## Начало работы

Чтобы начать работать с данным приложением нужно скопировать ссылку на репозиторий GIT.<br/>
Это можно сделать двумя способами.<br/>
1 Способ:<br/>
* Скопировать в адрессной строке<br/>
![Клонирование репозитория](https://psv4.userapi.com/c235031/u216249055/docs/d34/e88a903c3a59/AdressString.jpg?extra=0ipp3gs8LwB0__WllamdqWQ1FoS3pxUAYwluzQNqb6LoZzsvYaSXR6F7q4qKqwKNuU08ATQrwLcQTQIFi9nb42uz3PGyZ2g23kSO4E_uJPiOO1v3q67pF6UHVG_FUBqtFyv7B1isMVJgYz3VMN1ldfcP)<br/>
2 Способ:<br/>
* Нажать на зеленую кнопку "Code"
* Нажать на кнопку "Скопировать"
![Клонирование репозитория](https://sun9-55.userapi.com/impg/l81t8FPIApiGfN9Ii1OMxCBjCvvqemvEQJOFGw/VN6pR6J0pUw.jpg?size=1045x337&quality=96&sign=d7bf3e231eb1d93ef3a010e65b021d36&type=album)<br/>
### Необходимые условия

Для установки приложения, необходимо:
* Операционная система Windows 10 или Windows 11
* Среда разработки [Visual Studio 2022](https://visualstudio.microsoft.com/ru/vs/) (*Если у вас она еще не установлена вы можете скачать и установить ее по этой ссылке*)
    * Запустите Visual Studio 2022
    * Убедитесь, что установлена рабочая нагрузка Разработка классических приложений .NET. Это можно проверить в Visual Studio Installer.

### Установка

После того как мы убедились, что у нас все установлено и все запускается можно переходить к открытию приложения. 

* На главном экране в Visual Studio 2022 нажать на кнопку "Клонирование репозитория"
![Клонирование репозитория](https://sun9-80.userapi.com/impg/OHm4zKfvat5fDZ9hApRoZsa5KWDeLn1k0IitxA/YyuqmDznxyc.jpg?size=1014x662&quality=96&sign=c259962108b76bc7abf09f39d1ac275c&type=album)<br/>
* Вставить в поле "Расположение репозитория" скопированную заранее ссылку и нажать кнопку "Клонировать"
![Клонирование репозитория](https://sun9-24.userapi.com/impg/3Ad2SeH7WCcRb3Yf5XQGzqU4typDhYDDBBIhng/QJZfCx0iS7M.jpg?size=1009x676&quality=96&sign=ac402578e52467fa137c21c65795e998&type=album)<br/>
* В открывшемся проекте из репозитория нажать на кнопку "Пуск" и приложение запустится
![Запуск приложения](https://sun9-8.userapi.com/impg/ssN5AiVcDSP8O3w7lvQocfS1pZ6uaxCPo_Ir2A/S9W-oWt6DUo.jpg?size=1204x92&quality=96&sign=094c7f0c363624c6c831d0163452dcf9&type=album)<br/>

### Работа программы

Данный код позволяет брать курсы валют в режиме реального времени с центрального банка России

```
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
```

## Автор

* **Егор Ледров** - [Vip_Ghost](https://github.com/VipGhost-dev) - [ВК](https://vk.com/vip_ghost_game)
