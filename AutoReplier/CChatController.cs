using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoReplier.Base;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Threading;
using AutoReplier.Replier;

namespace AutoReplier {
    public class CChatController : CBaseObject {
        private string sThreadContainer = "threadlist_rows";
        private string sMessageConsistentSelector = "._55wp._4g33._5b6o._2ycx";
        private string sUnreadMessages = ".item.unreadMessage.acw.abb";

        private CReplyController replies;

        private int msgAmount = 10; // Default num messages to read and keep track of.
        private List<CChat> chats;

        public CChatController(ChromeDriver _driver)
            : base(_driver) {
            this.chats = new List<CChat>();
            this.replies = new CReplyController();

            WaitForAny(new string[] { sThreadContainer });

            while (true) {
                ReadMessages(msgAmount);

                if(UpdateMessages()) {
                    chats.Clear();
                    continue;
                }

                Thread.Sleep(1500);
            }
        }

        // Return true for renumeration required.
        private bool UpdateMessages() {
            for(int i = 0; i < chats.Count; i++) {
                if (chats[i].Update())
                    return true;
            }
            return false;
        }

        private void ReadMessages(int count) {
            if (!ElementExists(sThreadContainer)) {
                WriteError("Failed to find message container. Has the page changed?");
                return;
            }


            List<IWebElement> foundChats = new List<IWebElement>();
            // Get the container element.
            IWebElement container = chrome.FindElementById(sThreadContainer);

            // Get all messages.
            ReadOnlyCollection<IWebElement> threads = container.FindElements(By.CssSelector(sMessageConsistentSelector));

            // Get all 'unread' messages.
            ReadOnlyCollection<IWebElement> unread = container.FindElements(By.CssSelector(sUnreadMessages));

            if ((threads == null || threads.Count == 0) && (unread != null && threads.Count == 0)) {
                WriteError("Failed to get messages! Has the page changed? Do you even have any friends ... ???");
                return;
            }

            if(threads != null)
                foundChats.AddRange(threads);

            if(unread != null)
                foundChats.AddRange(unread);

            int c = Math.Min(foundChats.Count, 10);
            for (int i = 0; i < c; i++) {
                TrackMessage(foundChats.ElementAt(i));
            }
        }

        private void TrackMessage(IWebElement msg) {
            string id = msg.GetAttribute("id");
            if (ChatExists(id))
                return;

            CChat chat = new CChat(chrome, msg);
            chat.MessagesReceived += Chat_MessagesReceived;
            chats.Add(chat);
        }

        private void Chat_MessagesReceived(CChat chat, List<IWebElement> messages) {
            replies.MessageReceived(this, chat, messages);
        }

        private bool ChatExists(string _id) {
            for(int i = 0; i < chats.Count; i++) {
                if (chats[i].ID == _id)
                    return true;
            }
            return false;
        }
    }
}
