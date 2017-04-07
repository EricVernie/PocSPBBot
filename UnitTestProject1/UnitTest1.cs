using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Tests;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;
using System.Threading;
using Microsoft.Bot.Builder.Test;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1 : DialogTestBase
    {
        [TestMethod]
        public async Task Test_RootDialog()
        {
            await EchoDialogFlow(new PocSPBBot.Dialogs.RootDialog());
        }
        private async Task EchoDialogFlow(IDialog<object> echoDialog)
        {
            // arrange
            var toBot = DialogTestBase.MakeTestMessage();
            toBot.From.Id = Guid.NewGuid().ToString();
            toBot.Text = "Test";

            Func<IDialog<object>> MakeRoot = () => echoDialog;

            using (new FiberTestBase.ResolveMoqAssembly(echoDialog))
            using (var container = Build(Options.MockConnectorFactory | Options.ScopedQueue, echoDialog))
            {
                // act: sending the message
                IMessageActivity toUser = await GetResponse(container, MakeRoot, toBot);

                // assert: check if the dialog returned the right response
                Assert.IsTrue(toUser.Text.StartsWith("1"));
                Assert.IsTrue(toUser.Text.Contains("Test"));

                // act: send the message 10 times
                for (int i = 0; i < 10; i++)
                {
                    // pretend we're the intercom switch, and copy the bot data from message to message
                    toBot.Text = toUser.Text;
                    toUser = await GetResponse(container, MakeRoot, toBot);
                }

                // assert: check the counter at the end
                Assert.IsTrue(toUser.Text.StartsWith("11"));

                toBot.Text = "reset";
                toUser = await GetResponse(container, MakeRoot, toBot);

                // checking if there is any cards in the attachment and promote the card.text to message.text
                if (toUser.Attachments != null && toUser.Attachments.Count > 0)
                {
                    var card = (HeroCard)toUser.Attachments.First().Content;
                    toUser.Text = card.Text;
                }
                Assert.IsTrue(toUser.Text.ToLower().Contains("are you sure"));

                toBot.Text = "yes";
                toUser = await GetResponse(container, MakeRoot, toBot);
                Assert.IsTrue(toUser.Text.ToLower().Contains("reset count"));

                //send a random message and check count
                toBot.Text = "test";
                toUser = await GetResponse(container, MakeRoot, toBot);
                Assert.IsTrue(toUser.Text.StartsWith("1"));

                toBot.Text = "/deleteprofile";
                toUser = await GetResponse(container, MakeRoot, toBot);
                Assert.IsTrue(toUser.Text.ToLower().Contains("deleted"));
                using (var scope = DialogModule.BeginLifetimeScope(container, toBot))
                {
                    var botData = scope.Resolve<IBotData>();
                    await botData.LoadAsync(default(CancellationToken));
                    var stack = scope.Resolve<IDialogStack>();
                    Assert.AreEqual(0, stack.Frames.Count);
                }
            }
        }

        private async Task<IMessageActivity> GetResponse(IContainer container, Func<IDialog<object>> makeRoot, IMessageActivity toBot)
        {
            using (var scope = DialogModule.BeginLifetimeScope(container, toBot))
            {
                DialogModule_MakeRoot.Register(scope, makeRoot);

                // act: sending the message
                //await Conversation.SendAsync(scope, toBot);
                await Conversation.SendAsync(toBot,makeRoot,CancellationToken.None);
                return scope.Resolve<Queue<IMessageActivity>>().Dequeue();
            }
        }
    }
}
