using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace StarForce_PendingTitle_
{
    [XmlRoot(Namespace = null, IsNullable = true, ElementName = "LevelData")]
    public class LevelData
    {
        /// <summary>
        /// An Abstract 3D Object class. Used to save 3D object Data. Expand upon if need be.
        /// </summary>
        public class Generic3DObject
        {
            [XmlAttribute]
            public string ContentName;
            public String Name;

            public Vector3 Position;
            public Vector3 Scale;
            public Vector3 Rotation;

            public Generic3DObject()
            {

            }
        }

        public class EnemyData : Generic3DObject
        {
            [XmlAttribute]
            public String EnemyType;

            public TriggerData Trigger;
            public EnemyData()
            {
                EnemyType = "Chase";
                Trigger = new TriggerData();
            }
        }

        public class TriggerData
        {

            public Vector3 Position;
            public Vector3 Scale;

            public TriggerData()
            {
                Position = new Vector3();
                Scale = Vector3.One;
            }
        }

        public class WaypointData
        {
            [XmlAttribute]
            public String Name;

            public Vector3 Position;

            public float Speed;

            public WaypointData()
            {
                Name = "";
                Position = Vector3.Zero;
                Speed = 1f;
            }
        }

        public class WaypointFolder
        {
            [XmlAttribute]
            public String FolderName;

            public List<WaypointData> WaypointData;
            //[XmlAttribute]
            public List<WaypointFolder> Folders;

            public WaypointFolder()
            {
                FolderName = "";
                WaypointData = new List<WaypointData>();
                Folders = new List<WaypointFolder>();
            }
        }



        /// <summary>
        /// Level Objects. These are giant Meshes placed in the level. Require Collision
        /// </summary>
        public List<Generic3DObject> LevelObjects;
        public List<EnemyData> LevelEnemies;

        ///<summary>
        /// Static Objects dont require collision. Things like Trees will go here.
        /// </summary>
        // public List<Generic3DObject> StaticObjects;

        /// <summary>
        /// List of Waypoints that a Railship will follow. Waypoints follow a directory format for organization
        /// </summary>
        public WaypointFolder Waypoints;

        public LevelData()
        {
            LevelObjects = new List<Generic3DObject>();
            // StaticObjects = new List<Generic3DObject>();new
            Waypoints = new WaypointFolder();

            LevelEnemies = new List<EnemyData>();
        }

        /// <summary>
        /// saves this class into a file
        /// </summary>
        /// <param name="Filename">the filename to save into</param>
        public void Save(string Filename)
        {
            Stream stream = null;
            try
            {
                stream = File.Create(Filename);
                XmlSerializer serializer = new XmlSerializer(typeof(LevelData));
                serializer.Serialize(stream, this);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        /// <summary>
        /// Loads Level Data from a file. If file does not exist or load fails, returns a new Level Data class
        /// </summary>
        /// <param name="filename">the filename of the file to load from</param>
        /// <returns>returns Level Data</returns>
        public static LevelData Load(string filename)
        {
            FileStream stream = null;
            LevelData LevelData = null;

            try
            {
                stream = File.OpenRead(filename);
                XmlSerializer serializer = new XmlSerializer(typeof(LevelData));
                LevelData = (LevelData)serializer.Deserialize(stream);
            }
            catch
            {
                LevelData = new LevelData();
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return LevelData;
        }
    }
}
