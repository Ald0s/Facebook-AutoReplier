using AutoReplier.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoReplier;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using AutoReplier.Replier.Replies;

namespace AutoReplier.Replier {
    public class CReplyController : CDebugWrapper {
        private CBaseReply reply;

        public CReplyController() {
            // Default, set reply to the reverser. It's the funniest.
            reply = new CReverser();
        }

        public void MessageReceived(CChatController cChatController, CChat chat, List<IWebElement> messages) {
            // messages are a bunch of message 'clusters'. We must now create actual message objects out of these, then respond.
            for(int i = 0; i < messages.Count; i++) {
                ProcessMessageCluster(chat, messages[i]);
            }
        }

        private void ProcessMessageCluster(CChat _chat, IWebElement cluster) {
            // Here we want to turn a 'cluster' into a series of actual messages.
            // CMessage will expose  a list of structures for each message within the cluster.
            CMessage message = new CMessage(_chat, cluster);

            // According to the reply mode, we can now process all these messages.
            reply.TargetChat(_chat, message.Messages);
            reply.PerformReply();

            // Event finishes here, execution continues in CChat.
        }
    }
}
