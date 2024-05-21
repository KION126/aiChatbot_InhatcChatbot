// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Inhatc_ChatBot.Card;
using Inhatc_ChatBot.Clu;
using Inhatc_ChatBot.CognitiveModels;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Inhatc_ChatBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly InhatcRecognizer _cluRecognizer;
        protected readonly ILogger Logger;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(InhatcRecognizer cluRecognizer, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _cluRecognizer = cluRecognizer;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_cluRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: CLU is not configured. To enable all capabilities, add 'CluProjectName', 'CluDeploymentName', 'CluAPIKey' and 'CluAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }

            // Use the text provided in FinalStepAsync or the default if it is the first time.
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {


            // Call CLU and gather any potential booking details. (Note the TurnContext has the response to the prompt.)
            var cluResult = await _cluRecognizer.RecognizeAsync<Inhatc>(stepContext.Context, cancellationToken);
            string intentMess;
            switch (cluResult.GetTopIntent().intent)
            {
                case Inhatc.Intent.인사말:
                    var reply = MainCard.makeMainCard();
                    await stepContext.Context.SendActivityAsync(reply, cancellationToken);

                    break;

                case Inhatc.Intent.학교소개:
                    intentMess = $"학교소개 Intent";
                    var 학교소개Mess = MessageFactory.Text(intentMess,
                        intentMess, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(학교소개Mess, cancellationToken);
                    break;

                case Inhatc.Intent.학과소개:
                    intentMess = $"학과소개 Intent";
                    var 학과Entity = cluResult.Entities.GetDepartment();

                    if (!string.IsNullOrEmpty(학과Entity))
                    {
                        intentMess = $"{학과Entity}에 대한 정보를 제공합니다.";
                    }
                    else
                    {
                        intentMess = "학과를 인식하지 못했습니다. 다시 시도해주세요.";
                    }
                    var 학과소개Mess = MessageFactory.Text(intentMess, intentMess, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(학과소개Mess, cancellationToken);
                    break;

                    var 학과소개Messt = MessageFactory.Text(intentMess,
                        intentMess, InputHints.IgnoringInput);

                    await stepContext.Context.SendActivityAsync(학과소개Messt, cancellationToken);
                    break;

                default:
                    // Catch all for unhandled intents
                    var didntUnderstandMessageText = $"Sorry, I didn't get that. Please try asking in a different way (intent was {cluResult.GetTopIntent().intent})";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }
    }
}