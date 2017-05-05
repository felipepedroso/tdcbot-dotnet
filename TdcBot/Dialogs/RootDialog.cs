using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace TdcBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private Trivia currentTrivia;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var message = activity.CreateReply();

            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage("http://s3-sa-east-1.amazonaws.com/website-systextil/wp-content/uploads/20160225190252/thedeconf-e1455217447553-495x480.png"));

            CardAction cardAction = new CardAction {
                Type = "openUrl",
                Value = "http://www.thedevelopersconference.com.br",
                Title = "The Developers Conference"
            };

            var heroCard = new HeroCard
            {
                Images = cardImages,
                Title = "The Developers Conference",
                Subtitle = "Florianópolis - 2017",
                Tap = cardAction
            };

            message.Attachments.Add(heroCard.ToAttachment());

            await context.PostAsync(message);

            await context.PostAsync("Hello, my name is TdcBot and my mission is to show the power of bots to you! :)");

            PromptDialog.Confirm(context, HandleConfirm, "Do you want to see a trivia?", "I didn't understand.");
        }

        private async Task HandleConfirm(IDialogContext context, IAwaitable<bool> result)
        {
            bool selectedOption = await result;

            if (selectedOption)
            {

                currentTrivia = await GetTrivia();

                await context.PostAsync("Category: " + currentTrivia.category);

                await context.PostAsync("Difficulty: " + currentTrivia.difficulty);

                List<string> answers = new List<string>();

                answers.AddRange(currentTrivia.incorrect_answers);

                int randomIndex = new Random().Next(answers.Count);
                answers.Insert(randomIndex, currentTrivia.correct_answer);

                PromptDialog.Choice<string>(context, HandleAnswerChoice, answers, currentTrivia.question);
            }
            else
            {

                await context.PostAsync("OK... Maybe next time.");
            }
        }

        private async Task HandleAnswerChoice(IDialogContext context, IAwaitable<string> result)
        {
            string selectedAnswer = await result;

            if (selectedAnswer.Equals(currentTrivia.correct_answer))
            {
                await context.PostAsync("Congratulations! You chose the right answer");
            }
            else
            {
                await context.PostAsync("You chose the wrong answer... :(");
                await context.PostAsync("The right answer was: " + currentTrivia.correct_answer);
            }

            PromptDialog.Confirm(context, HandleConfirm, "Do you want to see another trivia?", "I didn't understand.");
        }

        private async Task<Trivia> GetTrivia()
        {
            string url = "https://opentdb.com/api.php?amount=1&type=multiple";

            string triviaJsonRaw;

            using (WebClient webClient = new WebClient()) {
                triviaJsonRaw = await webClient.DownloadStringTaskAsync(url).ConfigureAwait(false);
            }

            var serializer = new JavaScriptSerializer();

            Results results = serializer.Deserialize<Results>(triviaJsonRaw);

            return results.results[0];
        }
    }
}