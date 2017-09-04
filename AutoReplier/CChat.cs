using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoReplier.Base;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace AutoReplier {
    public class CChat : CBaseChatWrapper {
        public string FullName { get { return this.sFullName; } }
        public string ID { get { return this.sID; } }

        private string sMessageInput = "composerInput";
        private string sSubmitButton = "u_6_7";
        private string sNameSelector = "._52je._5tg_";
        private string sChatSelector = ".voice.acw.abt";

        private IWebElement chat;

        private string sFullName;
        private string sID;

        // While the Chrome window still holds focus on this chat, we fire an event which will pass messages to the processing engine.
        // This will perform whatever task we want such as string reverser, hangman etc etc.
        public delegate void MessagesReceived_Delegate(CChat chat, List<IWebElement> messages);
        public event MessagesReceived_Delegate MessagesReceived;

        public CChat(ChromeDriver _driver, IWebElement _chat)
            : base(_driver) {
            this.chat = _chat;

            sID = _chat.GetAttribute("id");

            ReadBasicInfo();
            WriteSuccess("Found your chat with '" + sFullName + "'");
        }

        public override bool Update() {
            if (IsUnread()) {
                // Grab all new messages.
                WriteInfo("Unread messages found in " + sFullName);

                GoToThisChat();

                // Now, grab the most recent message.
                ReadOnlyCollection<IWebElement> msgs = chrome.FindElements(By.CssSelector(sChatSelector));

                // Latest messages are on the bottom, so reverse the list so they're at the beginning.
                // Most recent will be theirs, so save the data store and retreive all messages until yours appears again.
                List<IWebElement> messages = new List<IWebElement>(msgs.Reverse());
                if (messages.Count != 0) {
                    string data_store = messages[0].GetAttribute("data-store");


                    List<IWebElement> their = new List<IWebElement>();
                    for(int i = 0; i < messages.Count; i++) {
                        if (messages[i].GetAttribute("data-store") != data_store)
                            break;
                        their.Add(messages[i]);
                    }

                    ProcessMessages(their);
                }

                // Post update and heading back to messages again will require a renumeration of messages.
                return base.Update();
            }

            return false;
        }

        // Send a message to this chat.
        // Function assumes chat has focus.
        public void SendMessage(string message) {
            if (!WaitForAny(new string[] { sMessageInput, sSubmitButton })) {
                WriteError("Error in chat with " + sFullName + ", chat doesn't have focus! We couldn't send the chat message.");
                return;
            }

            try {
                IWebElement input = chrome.FindElement(By.Id(sMessageInput));
                IWebElement submit = chrome.FindElement(By.Id(sSubmitButton));

                input.SendKeys(message);
                for(int i = 0; i < 5; i++) {
                    submit.Click(); // Spam that shiet.
                }
            }catch(Exception e) {
                if(e is StaleElementReferenceException || e is NoSuchElementException) {
                    WriteError("Error in chat with " + sFullName + ", chat doesn't have focus! We couldn't send the chat message.");
                }
            }
        }

        private void ProcessMessages(List<IWebElement> messages) {
            if(MessagesReceived != null) {
                MessagesReceived(this, messages);
            }
        }

        private void ReadBasicInfo() {
            // Firstly, read the person's name and display a notification.
            if (!ElementExists(By.CssSelector(sNameSelector))) {
                WriteError("Couldn't find user's name selector. Has the page changed?");
                return;
            }

            IWebElement span = chat.FindElement(By.CssSelector(sNameSelector));
            IWebElement strong = span.FindElement(By.TagName("strong"));

            sFullName = strong.Text;
        }

        private void GoToThisChat() {
            // Click onto the chat, then wait.
            chat.Click();

            WaitForAny(new string[] { sMessageInput });
        }

        public bool IsUnread() {
            string c = chat.GetAttribute("class");
            if (c.Contains("aclb")) // It's currently aclb for unread or acw for read.
                return true;
            return false;
        }
    }
}
