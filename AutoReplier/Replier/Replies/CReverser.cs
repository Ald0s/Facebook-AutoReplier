using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoReplier.Base;

namespace AutoReplier.Replier.Replies {
    public class CReverser : CBaseReply {
        public CReverser()
            :base(null, null) {
            
        }

        public override void PerformReply() {
            // Chat instance will still hold the chat we're targetting.
            // We can just explicity call SendMessage()

            foreach (Message_t msg in messages) {
                char[] str = msg.sMessage.ToArray().Reverse().ToArray();
                chat.SendMessage(new string(str));
            }

            base.PerformReply();
        }
    }
}
