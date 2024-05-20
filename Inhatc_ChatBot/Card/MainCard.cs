using Microsoft.Bot.Builder;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.Bot.Schema;
using System.IO;
using System;
using AdaptiveCards;

namespace Inhatc_ChatBot.Card
{
    public class MainCard
    {

        public static IMessageActivity makeMainCard()
        {
            // JSON 파일을 UTF-8 인코딩으로 읽기
            string adaptiveCardJson;
            using (var reader = new StreamReader("./Card/MainCard.json", System.Text.Encoding.UTF8))
            {
                adaptiveCardJson = reader.ReadToEnd();
            }

            // JSON 문자열을 Adaptive Card로 변환
            var adaptiveCard = JsonConvert.DeserializeObject<AdaptiveCards.AdaptiveCard>(adaptiveCardJson);

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
    }
}
