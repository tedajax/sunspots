using System;
using System.Collections.Generic;
using System.Text;

namespace StarForce_PendingTitle_
{
    class WaypointCommand : Command
    {
        int DestinationWaypointNumber;

        public WaypointCommand(string cmdline, int wpnumber)
        {
            base.SetCommand(cmdline);
            DestinationWaypointNumber = wpnumber;
        }

        public int GetDestinationNumber() { return DestinationWaypointNumber; }

        public override string ToString()
        {
            return "WaypointCMD";
        }
    }
}
