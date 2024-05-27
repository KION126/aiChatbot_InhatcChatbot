using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Globalization;

namespace Inhatc_Chatbot
{
    internal class WebCrolling
    {
        static public void crolling()
        {
            var _option = new ChromeOptions();
            _option.AddArgument("--headless");
            var _driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), _option);
            _driver.Navigate().GoToUrl("https://www.inhatc.ac.kr/kr/485/subview.do#this");
            String xpath = "//*[@id=\"menu485_obj8613\"]/div[2]/div/div[3]/table";
            var table = _driver.FindElement(By.XPath(xpath));
            var tbody = table.FindElement(By.TagName("tbody"));
            var trs = tbody.FindElements(By.TagName("tr"));
            String[] str = new String[5];
            int i = 1, j = 0;
            foreach (var tr in trs) {
                var tds = tr.FindElements(By.TagName("td"));
                foreach (var td in tds) {

                    str[j] = td.Text;
                    j += 1;
                    if (j == 5) break;
                }
                j = 0;

                var replaceDate = str[0].ToString().Replace(". ", ".");
                var adaptiveCard = new {
                    type = "AdaptiveCard",
                    version = "1.2",
                    body = new List<object>
                    {
                        new
                        {
                            type = "Container",
                            horizontalAlignment = "Center",
                            items = new List<object>
                            {
                                new
                                {
                                type = "TextBlock",
                                text = $"{replaceDate}{str[1]} \r\n \r\n",
                                weight = "Bolder",
                                size = "Medium"
                                },
                                new {
                                    type = "ColumnSet",
                                    width = "stretch",
                                    style = "default",
                                    columns = new List<object> {
                                        new {
                                            type = "Column",
                                            width = "stretch",
                                            items = new List<object>
                                            {
                                                new
                                                {
                                                    type = "TextBlock",
                                                    text = "조식", 
                                                },
                                                new
                                                {
                                                    type = "TextBlock",
                                                    text = $"{str[2].ToString().Replace("&","\r\n")}",
                                                }
                                            }
                                        },
                                        new {
                                            type = "Column",
                                            width = "stretch",
                                            items = new List<object>
                                            {
                                                new
                                                {
                                                    type = "TextBlock",
                                                    text = "중식(일반)",
                                                },
                                                new
                                                {
                                                    type = "TextBlock",
                                                    text = $"{str[3].ToString().Replace("&","\r\n")}",
                                                }
                                            }
                                        },
                                        new {
                                            type = "Column",
                                            width = "stretch",
                                            items = new List<object>
                                            {
                                                new
                                                {
                                                    type = "TextBlock",
                                                    text = "중식(특식)",
                                                },
                                                new
                                                {
                                                    type = "TextBlock",
                                                    text = $"{str[4].ToString().Replace("&","\r\n")}",
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
                string json = JsonConvert.SerializeObject(adaptiveCard, Formatting.Indented);
                Console.WriteLine(json); 
                string filePath = System.Environment.CurrentDirectory.ToString() + $"\\card\\JsonFile\\MenuJson\\MenuCard{i++}.json";
                File.WriteAllText(filePath, json);
            }

        }
    }
}
