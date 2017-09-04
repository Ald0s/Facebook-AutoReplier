using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using AutoReplier.Base;

namespace AutoReplier.Sequences {
    public class CLogin : CBaseSequence {
        private string sCheckpointID = "checkpoint_title";
        private string sPostTextBox = "sPostTextBoxID";

        private string sEmailID = "m_login_email";
        private string sPasswordID = "m_login_password";
        private string sLoginBtnID = "u_0_5";

        private string sLoginError = "m_login_notice";

        private string sEmail;
        private string sPassword;

        public CLogin(ChromeDriver _driver, string _email, string _pass)
            : base(_driver, "Login") {

            this.sEmail = _email;
            this.sPassword = _pass;
        }

        public override void PerformSequence() {
            // Get the email and password inputs.
            IWebElement txtEmail = chrome.FindElementById(sEmailID);
            IWebElement txtPassword = chrome.FindElementById(sPasswordID);

            // Get the Log In button.
            IWebElement btnLogin = chrome.FindElementById(sLoginBtnID);

            if (txtEmail == null || txtPassword == null || btnLogin == null) {
                SetSequenceResult(false, "Failed to find either email, password or login button controls. Has the page changed?");
                return;
            }

            WriteInfo("Logging in user account '" + sEmail + "' ...");
            txtEmail.Clear();
            txtEmail.SendKeys(sEmail);

            try {
                txtPassword.Clear();
                txtPassword.SendKeys(sPassword);

                btnLogin.Click();
            } catch (InvalidElementStateException) {
                btnLogin.Click();
                WaitForAny(new string[] { sPasswordID });

                txtPassword.Clear();
                txtPassword.SendKeys(sPassword);

                btnLogin.Click();
            }

            if(!WaitForAny(new string[] { sLoginError, sCheckpointID, sPostTextBox })) {
                SetSequenceResult(false, "Failed to detect login change. Has the page changed?");
                return;
            }

            if(ElementExists(sLoginError)) {
                SetSequenceResult(false, "Login error! Please check the Chrome window for the reason.");
                return;
            }

            base.PerformSequence();
        }
    }
}
