using AutoReplier.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoReplier;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace AutoReplier.Replier {
    public class CReplyController : CDebugWrapper {
        // To test, this class will just reverse each message and send it back.

        public CReplyController() {

        }

        public void MessageReceived(CChatController cChatController, CChat chat, List<IWebElement> messages) {
            // messages are a bunch of message 'clusters'. We must now create actual message objects out of these, then respond.
            for(int i = 0; i < messages.Count; i++) {
                ProcessMessageCluster(chat, messages[i]);
            }
        }

        private void ProcessMessageCluster(CChat _chat, IWebElement cluster) {
            // Grab the message element.
            IWebElement msg = cluster.FindElement(By.CssSelector(".msg"));

            // Currently, all div tags represent a message.
            // So just grab all the divs from msg!
            ReadOnlyCollection<IWebElement> messages = msg.FindElements(By.TagName("div"));
            if (messages.Count > 0) {
                for(int i = 0; i < messages.Count; i++) {
                    GenerateAndProcessStructure(_chat, messages[i]);
                }
            }
        }

        private void GenerateAndProcessStructure(CChat _chat, IWebElement msg) {
            // data-store now contains some JSON with cool info like timestamp
            // C# doesn't really have much support for JSON, but fortunately we're just looking for a single number.
            string dataStore = msg.GetAttribute("data-store");
            if (dataStore == null)
                return;

            Regex exp = new Regex("\"timestamp\":(\\d+)");
            Match time = Regex.Match(dataStore, "\"timestamp\":(\\d+)");

            long timeReceived = 0;
            if (time.Success) {
                timeReceived = long.Parse(time.Groups[1].Value);
            } else {
                timeReceived = 0;
            }

            IWebElement msgElement = msg.FindElement(By.TagName("span"));
            string msgString = msgElement.Text;

            Message_t result = new Message_t();
            result.chat = _chat;
            result.timeReceived = timeReceived;
            result.sMessage = msgString;

            ProcessMessage(result);
        }

        private void ProcessMessage(Message_t result) {
            DateTime friendly = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)
                .AddMilliseconds(result.timeReceived).ToLocalTime();

            WriteSuccess(result.chat.FullName + " said '" + result.sMessage + "' (" + friendly.ToString() + ")");
        }
    }

public struct Message_t {
        public CChat chat;
        public string sMessage;
        public long timeReceived;
    }
}
