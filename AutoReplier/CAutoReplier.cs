using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using AutoReplier.Sequences;
using AutoReplier.Base;

namespace AutoReplier {
    public class CAutoReplier : CDebugWrapper {
        private ChromeDriver chrome;
        private CChatController chatController;

        public CAutoReplier() {

        }

        public void OpenFacebook() {
            this.chrome = new ChromeDriver();
            this.chrome.Manage().Timeouts().PageLoad = new TimeSpan(0, 0, 10);
            this.chrome.Navigate().GoToUrl("https://m.facebook.com");
        }

        public void CloseWindow() {
            chrome.Close();
        }

        public string Login(string email, string password) {
            WriteInfo("Opening Facebook and navigating to 'https://m.facebook.com' ...");

            CLogin login = new CLogin(chrome, email, password);
            login.PerformSequence();

            SequenceResult_t loginResult = login.GetResult();
            if (!loginResult.bSuccess)
                return (string)loginResult.oSequenceData;
            
            return null;
        }

        public string GoToMessages() {
            WriteInfo("Attempting to locate messages, we may need your help here ...");

            CAccessMessages access = new CAccessMessages(chrome);
            access.PerformSequence();

            SequenceResult_t accessResult = access.GetResult();
            if (!accessResult.bSuccess)
                return (string)accessResult.oSequenceData;

            WriteSuccess("Found messages! ...");

            // Now we can start taking control of the messages.
            chatController = new CChatController(chrome);
            return null;
        }
    }
}
