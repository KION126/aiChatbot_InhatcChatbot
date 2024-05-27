using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inhatc_ChatBot.Clu;
using Inhatc_ChatBot.CognitiveModels;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Inhatc_ChatBot.Card;

namespace Inhatc_ChatBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly InhatcRecognizer _cluRecognizer;
        protected readonly ILogger Logger;
        private readonly List<DepartmentInfo> _departments;

        public MainDialog(InhatcRecognizer cluRecognizer, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _cluRecognizer = cluRecognizer;
            Logger = logger;
            _departments = DepartmentInfo.LoadDepartments("Card/JsonFile/Departments.json");

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
            }));

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

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var cluResult = await _cluRecognizer.RecognizeAsync<Inhatc>(stepContext.Context, cancellationToken);
            if (cluResult.GetTopIntent().score > 0.9) {
                switch (cluResult.GetTopIntent().intent) {
                    case Inhatc.Intent.인사말:
                        var helloCard = ConverterJson.makeCard("MainCard");
                        await stepContext.Context.SendActivityAsync(helloCard, cancellationToken);
                        break;

                    case Inhatc.Intent.학교소개:
                        var IntroductionCard = ConverterJson.makeCard("IntroductionCard");
                        await stepContext.Context.SendActivityAsync(IntroductionCard, cancellationToken);
                        break;

                    case Inhatc.Intent.전체학과소개:
                        var AllEopCard = ConverterJson.makeCard("AllDepartmentCard");
                        await stepContext.Context.SendActivityAsync(AllEopCard, cancellationToken);

                        var depIntroCards = ConverterJson.MakeAllDepartmentCards();
                        var message = MessageFactory.Carousel(depIntroCards);
                        await stepContext.Context.SendActivityAsync(message, cancellationToken);
                        break;

                    case Inhatc.Intent.학과소개:
                        var 학과Entity = cluResult.Entities.GetDepartment();

                        if (!string.IsNullOrEmpty(학과Entity)) {
                            var normalizedEntity = 학과Entity.Trim().ToLower();
                            var department = _departments.FirstOrDefault(d =>
                                d.Name.Trim().ToLower() == normalizedEntity ||
                                d.Aliases.Any(alias => alias.Trim().ToLower() == normalizedEntity));

                            if (department != null) {
                                var cardMessage = ConverterJson.MakeDepartmentCard(department);
                                await stepContext.Context.SendActivityAsync(cardMessage, cancellationToken);
                            } else {
                                var DeptNotFoundCard = ConverterJson.makeCard("DepartmentNotFoundCard");
                                await stepContext.Context.SendActivityAsync(DeptNotFoundCard, cancellationToken);
                            }
                        } else {
                            var 학과소개Mess = MessageFactory.Text("학과를 인식하지 못했습니다. 다시 시도해주세요.", inputHint: InputHints.IgnoringInput);
                            await stepContext.Context.SendActivityAsync(학과소개Mess, cancellationToken);
                        }
                        break;

                    case Inhatc.Intent.학사일정:
                        var ASCard = ConverterJson.makeCard("AcademicSchedule");
                        await stepContext.Context.SendActivityAsync(ASCard, cancellationToken);
                        break;
                        
                    case Inhatc.Intent.입학안내:
                        var AdmissionCard = ConverterJson.makeCard("AdmissionCard");
                        await stepContext.Context.SendActivityAsync(AdmissionCard, cancellationToken);
                        break;
                        
                    case Inhatc.Intent.교내연락처:
                        var SchoolCICard = ConverterJson.makeCard("SchoolCICard");
                        await stepContext.Context.SendActivityAsync(SchoolCICard, cancellationToken);
                        break;
                        
                    case Inhatc.Intent.캠퍼스맵:
                        var CampusMapCard = ConverterJson.makeCard("CampusMapCard");
                        await stepContext.Context.SendActivityAsync(CampusMapCard, cancellationToken);
                        break;

                    case Inhatc.Intent.식당:
                        var RestaurantMenu = ConverterJson.makeCard("RestaurantMenu");
                        await stepContext.Context.SendActivityAsync(RestaurantMenu, cancellationToken);
                        var meunCards = ConverterJson.MakeAllMenuCards();
                        var message2 = MessageFactory.Carousel(meunCards);
                        await stepContext.Context.SendActivityAsync(message2, cancellationToken);
                        break;
                        
                    case Inhatc.Intent.공지사항:
                        var NoticeCard = ConverterJson.makeCard("NoticeCard");
                        await stepContext.Context.SendActivityAsync(NoticeCard, cancellationToken);
                        break;

                    case Inhatc.Intent.수강신청:
                        var EnrolmentCard = ConverterJson.makeCard("EnrolmentCard");
                        await stepContext.Context.SendActivityAsync(EnrolmentCard, cancellationToken);
                        break;

                    default:
                        var didntUnderstandMessageText = $"Sorry, I didn't get that. Please try asking in a different way (intent was {cluResult.GetTopIntent().intent})";
                        var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                        await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                        break;
                }
            } else {
                var didntUnderstandMessageText = $"무슨 말인지 모르겠어요 다시 질문해 주세요";
                var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                var helloCard = ConverterJson.makeCard("MainCard");
                await stepContext.Context.SendActivityAsync(helloCard, cancellationToken);

            }
            return await stepContext.NextAsync(null, cancellationToken);
        }
    }
}
