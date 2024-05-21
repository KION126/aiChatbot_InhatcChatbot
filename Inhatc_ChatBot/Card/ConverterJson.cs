using Microsoft.Bot.Builder;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.Bot.Schema;
using System.IO;
using System;
using AdaptiveCards;
using System.Collections.Generic;

namespace Inhatc_ChatBot.Card
{
    public class ConverterJson
    {
        public static IMessageActivity makeCard(String jasonFileName)
        {
            // JSON 파일을 UTF-8 인코딩으로 읽기
            string adaptiveCardJson;
            using (var reader = new StreamReader("./Card/JsonFile/"+ jasonFileName+".json", System.Text.Encoding.UTF8))
            {
                adaptiveCardJson = reader.ReadToEnd();
            }

            // JSON 문자열을 Adaptive Card로 변환
            var adaptiveCard = JsonConvert.DeserializeObject<AdaptiveCard>(adaptiveCardJson);

            // 카드 첨부물을 생성
            var attachment = new Attachment
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = adaptiveCard
            };

            // 환영 메시지 생성 및 전송
            var message = MessageFactory.Attachment(attachment);
            return message;
        }

        //학과명 Entity구분을 위한 메서드
        public static IMessageActivity MakeDepartmentCard(DepartmentInfo department)
        {
            var card = new
            {
                type = "AdaptiveCard",
                version = "1.2",
                body = new object[]
                {
                    new
                    {
                        type = "TextBlock",
                        text = department.Name,
                        weight = "bolder",
                        size = "medium"
                    },
                    new
                    {
                        type = "TextBlock",
                        text = department.Title,
                        wrap = true,
                        color = "accent",
                    },
                    new
                    {
                        type = "TextBlock",
                        text = department.Description,
                        wrap = true
                    }
                },
                actions = new object[]
                {
                    new
                    {
                        type = "Action.OpenUrl",
                        title = department.Name+" 홈페이지",
                        url = department.Url
                    }
                },
            };

            var adaptiveCardJson = JsonConvert.SerializeObject(card);

            var adaptiveCardAttachment = new Attachment
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };

            var message = MessageFactory.Attachment(adaptiveCardAttachment);
            return message;
        }

        //여러개의 Json파일을 슬라이드로 출력하기 위한..
        public static List<Attachment> MakeAllDepartmentCards()
        {
            var attachments = new List<Attachment>();

            // 카드 파일 목록
            var cardFiles = new List<string>
            {
                "./Card/JsonFile/DepartementCard/Card1.json",
                "./Card/JsonFile/DepartementCard/Card2.json",
                "./Card/JsonFile/DepartementCard/Card3.json",
                "./Card/JsonFile/DepartementCard/Card4.json",
                "./Card/JsonFile/DepartementCard/Card5.json"
            };

            foreach (var cardFile in cardFiles)
            {
                // JSON 파일을 UTF-8 인코딩으로 읽기
                string adaptiveCardJson;
                using (var reader = new StreamReader(cardFile, System.Text.Encoding.UTF8))
                {
                    adaptiveCardJson = reader.ReadToEnd();
                }

                // JSON 문자열을 Adaptive Card로 변환
                var adaptiveCard = JsonConvert.DeserializeObject<AdaptiveCard>(adaptiveCardJson);

                // 카드 첨부물을 생성
                var attachment = new Attachment
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = adaptiveCard
                };

                // 첨부물 목록에 추가
                attachments.Add(attachment);
            }

            return attachments;
        }
    }
}