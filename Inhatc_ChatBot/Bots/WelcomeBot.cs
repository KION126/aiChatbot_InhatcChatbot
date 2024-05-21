using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using Inhatc_ChatBot.Card;

namespace Inhatc_ChatBot.Bots
{
    public class WelcomeBot<T> : EchoBot<T>
        where T : Dialog
    {

        public WelcomeBot(ConversationState conversationState, UserState userState, T dialog, ILogger<EchoBot<T>> logger)
            : base(conversationState, userState, dialog, logger)
        {
        }
        

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var reply = ConverterJson.makeCard("MainCard");
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}
