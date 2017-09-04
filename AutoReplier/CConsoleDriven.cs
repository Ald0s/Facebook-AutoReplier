using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoReplier.Base;

namespace AutoReplier {
    public class CConsoleDriven : CDebugWrapper {
        private CAutoReplier auto;

        public CConsoleDriven() {
            Console.WriteLine("Starting Facebook AutoReplier - made by Alden Viljoen (http://github.com/ald0s)");

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            Console.Clear();

            StartAutoReplier(email, password);
        }

        private void StartAutoReplier(string email, string password) {
            // Open facebook and wait. This call is blocking.
            auto = new CAutoReplier();
            auto.OpenFacebook();

            Console.Clear();

            string login = auto.Login(email, password);
            if(login != null) {
                WriteError(login);
                return;
            }

            WriteSuccess("Facebook took our login!");

            string access = auto.GoToMessages();
            if(access != null) {
                WriteError(access);
                return;
            }
        }
    }
}
