using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoReplier.Base;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace AutoReplier.Replier {
    public class CMessage : CDebugWrapper {
        public List<Message_t> Messages { get { return this.result; } }

        private List<Message_t> result;
        private CChat chat;

        public CMessage(CChat _chat, IWebElement cluster) {
            this.chat = _chat;
            this.result = new List<Message_t>();

            this.ProcessMessageCluster(cluster);
        }

        private void ProcessMessageCluster(IWebElement cluster) {
            // Grab the message element.
            IWebElement msg = cluster.FindElement(By.CssSelector(".msg"));

            // Currently, all div tags represent a message.
            // So just grab all the divs from msg!
            ReadOnlyCollection<IWebElement> messages = msg.FindElements(By.TagName("div"));
            if (messages.Count > 0) {
                for (int i = 0; i < messages.Count; i++) {
                    GenerateAndProcessStructure(chat, messages[i]);
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

        private void ProcessMessage(Message_t msg) {
            this.result.Add(msg);
        }
    }

    public struct Message_t {
        public CChat chat;
        public string sMessage;
        public long timeReceived;
    }
}
