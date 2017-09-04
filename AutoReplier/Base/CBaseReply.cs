using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoReplier.Replier;

namespace AutoReplier.Base {
    public class CBaseReply : CDebugWrapper {
        protected CChat chat;
        protected List<Message_t> messages;

        private Dictionary<string, object> state;

        public CBaseReply(CChat _chat, List<Message_t> msgs) {
            this.chat = _chat;
            this.messages = msgs;

            this.state = new Dictionary<string, object>();
        }

        public void TargetChat(CChat _chat, List<Message_t> msgs) {
            this.chat = _chat;
            this.messages = msgs;
        }

        public virtual void PerformReply() {
            // Finish up replying here.
        }

        protected void Store(string key, object value) {
            state[key] = value;
        }

        protected object Retreive(string key) {
            if (state.ContainsKey(key))
                return state[key];
            return null;
        }
    }
}
