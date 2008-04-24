using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace StarForce_PendingTitle_
{
    [Serializable]
    public static class Level
    {
        //OH GEEZE A LIST OF WAYPOINTS
        public static List<Waypoint> Waypoints;


        //TESTING PURPOSES ONLY
        private static float testrate = 1f;
        private static float testturnrate = 1f;

        public static void Initialize()
        {
            Waypoints = new List<Waypoint>();

            Waypoints.Add(new Waypoint(new Vector3(50, 0, 0), new Vector3(0, 0, 0), testrate, testturnrate));
            Waypoints.Add(new Waypoint(new Vector3(80, -40,0), new Vector3(270,0,0), testrate, testturnrate));
            Waypoints.Add(new Waypoint(new Vector3(100, -40, 0), new Vector3(0, 0, 0), testrate, testturnrate));
            Waypoints.Add(new Waypoint(new Vector3(100, -40, -150), new Vector3(0, 270, 0), testrate, testturnrate));
            Waypoints.Add(new Waypoint(new Vector3(100, 0, -180), new Vector3(30, 270, 0), testrate, testturnrate));
            Waypoints.Add(new Waypoint(new Vector3(-100, 0, -180), new Vector3(0, 180, 0), testrate, testturnrate));
            Waypoints.Add(new Waypoint(new Vector3(-100, 0, 0), new Vector3(0, 90, 0), testrate, testturnrate));
          
            // Waypoints.Add(new Waypoint(new Vector3(50, 0, 50), new Vector3(0, 90, 0), testrate, testturnrate));
           // Waypoints.Add(new Waypoint(new Vector3(0, 0, 50), new Vector3(0, 180, 0), testrate, testturnrate));
            //Waypoints.Add(new Waypoint(new Vector3(0, 0, 0), new Vector3(0, 270, 0), testrate, testturnrate));

            


        }
       
    }
}
