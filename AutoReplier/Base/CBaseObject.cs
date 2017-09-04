using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace AutoReplier.Base {
    public class CBaseObject :CDebugWrapper {
        protected ChromeDriver chrome;

        public CBaseObject(ChromeDriver _driver) {
            this.chrome = _driver;
        }

        protected bool WaitForAny(string[] element_ids, bool infinite = false) {
            DateTime start = DateTime.Now;

            while (true) {
                for (int i = 0; i < element_ids.Length; i++) {
                    if (DateTime.Now.Subtract(start).Seconds > 10 && !infinite) {
                        return false; // None of these elements exist.
                    }

                    if (ElementExists(element_ids[i]))
                        return true;
                }
            }
        }

        protected bool ElementExists(string id) {
            try {
                chrome.FindElementById(id);
                return true;
            } catch (NoSuchElementException) {
                return false;
            }
        }

        protected bool ElementExists(By by) {
            try {
                chrome.FindElement(by);
                return true;
            } catch (NoSuchElementException) {
                return false;
            }
        }
    }
}
