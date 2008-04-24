using System;
using System.Collections.Generic;
using System.Text;

namespace StarForce_PendingTitle_
{
    public abstract class Command
    {
        private string CommandName;

        public virtual void SetCommand(string cmdline)
        {
            CommandName = cmdline;
        }

        public string GetCommandName() { return CommandName; }

        public override string ToString()
        {
            return CommandName;
        }
    }
}
