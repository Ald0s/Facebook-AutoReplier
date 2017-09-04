using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using AutoReplier.Base;

namespace AutoReplier.Sequences {
    public class CAccessMessages : CBaseSequence {
        private string sCheckpointID = "checkpoint_title";
        private string sPostTextBoxID = "u_0_k";
        private string sMessagesSearchID = "messages_search_box";

        public CAccessMessages(ChromeDriver _driver)
            : base(_driver, "AccessMessages") {

        }

        public override void PerformSequence() {
            if(!WaitForAny(new string[] { sCheckpointID, sPostTextBoxID })) {
                SetSequenceResult(false, "Operation failed! (timed out waiting for initial login)");
                return;
            }

            // Check if we're actually logged in.
            // Most importantly, check for any checkpoints - like 2 factor auth and stuff.
            if(ElementExists(sCheckpointID)) {
                chrome.FindElementById(sCheckpointID);
                WriteError("Please complete the Facebook check and continue your login, we'll wait.");

                // We must first wait for the user to complete the checkpoint.
                WaitForAny(new string[] { sPostTextBoxID }, true);
            }

            // Check once more for the text box.
            // If this check fails, somethings changed.
            if (!ElementExists(sPostTextBoxID)) {
                SetSequenceResult(false, "We failed to locate the 'create post' text input! The page has most likely changed.");
                return;
            }

            // Head to messages.
            chrome.Navigate().GoToUrl("https://m.facebook.com/messages/?more");
            WaitForAny(new string[] { sMessagesSearchID });

            // Verify we made it to messages.
            if (!ElementExists(sMessagesSearchID)) {
                SetSequenceResult(false, "We failed to find your messages. The page has most likely changed.");
                return;
            }

            // We're here!
            base.PerformSequence();
        }
    }
}
