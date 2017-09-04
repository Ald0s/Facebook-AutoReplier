using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoReplier.Base {
    public class CBaseMessageWrapper : CBaseObject {
        private string sMessagesSearchID = "messages_search_box";

        public CBaseMessageWrapper(ChromeDriver _driver)
            : base(_driver) {

        }

        protected void GoBackToMessages() {
            // Head to messages.
            chrome.Navigate().GoToUrl("https://m.facebook.com/messages/?more");
            WaitForAny(new string[] { sMessagesSearchID });

            // Verify we made it to messages.
            if (!ElementExists(sMessagesSearchID)) {
                WriteError("We failed to go back to the messages screen! The page has most likely changed.");
                return;
            }
        }

        public virtual bool Update() {
            GoBackToMessages();
            return true;
        }
    }
}
