using Microsoft.Bot.Schema;
using System.Collections.Generic;

namespace Inhatc_ChatBot.Card
{
    public class Video
    {
        public static VideoCard GetVideoCard()
        {
            var videoCard = new VideoCard
            {
                Title = "학교소개영상",
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = "https://www.youtube.com/watch?v=ZUUcGTWfe8k",
                    },
                }
            };

            return videoCard;
        }
    }
}
