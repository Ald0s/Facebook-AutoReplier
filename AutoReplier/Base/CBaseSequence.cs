using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace AutoReplier.Base {
    public class CBaseSequence : CBaseObject {
        private string sName;
        private string sData;
        private bool bSuccess;

        public CBaseSequence(ChromeDriver _chrome, string _name)
            : base(_chrome) {
            this.sName = _name;
        }

        public virtual void PerformSequence() {
            SetSequenceResult(true, null);
        }

        protected void SetSequenceResult(bool success, string data) {
            this.sData = data;
            this.bSuccess = success;
        }

        public SequenceResult_t GetResult() {
            SequenceResult_t result = new SequenceResult_t();
            result.sSequenceName = sName;
            result.oSequenceData = sData;
            result.bSuccess = bSuccess;

            return result;
        }
    }

    public struct SequenceResult_t {
        public string sSequenceName;
        public object oSequenceData;
        public bool bSuccess;
    }
}
