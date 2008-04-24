using System;
using System.Collections.Generic;
using System.Text;

namespace StarForce_PendingTitle_
{
    /// <summary>
    /// this is the profile class. Currently it only contains the name of the profile but later it will contain button configs, etc
    /// </summary>
    class Profile
    {
        String Name;
        /// <summary>
        /// creates a new profile
        /// </summary>
        /// <param name="name">the name of the profile</param>
        public Profile(String name)
        {
            Name = name;
        }
        /// <summary>
        /// returns the name of the profile
        /// </summary>
        /// <returns> the name of the profile</returns>
        public String getName() { return Name; }

    }
}
