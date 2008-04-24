using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace StarForce_PendingTitle_
{
    public class Waypoint
    {
        //These three are basic, used everywhere
        Vector3 WaypointPosition;
        Vector3 WaypointRotation;
        float WaypointRate;
        float WaypointTurnRate;

        List<Command> CommandList;

        public Vector3 GetWaypointPosition() { return WaypointPosition; }
        public Vector3 GetWaypointRotation() { return WaypointRotation; }
        public float GetWaypointRate() { return WaypointRate; }
        public float GetWaypointTurnRate() { return WaypointTurnRate; }

        public Waypoint(Vector3 Pos, Vector3 Rot, float Rate, float TurnRate)
        {
            CommandList = new List<Command>();

            WaypointPosition = Pos;
            WaypointRotation = Rot;
            WaypointRate = Rate;
            WaypointTurnRate = TurnRate;
        }

        public void AddCommand(Command cmd)
        {
            CommandList.Add(cmd);
        }
    }
}
