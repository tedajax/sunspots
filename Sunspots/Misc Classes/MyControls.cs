using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Soopah.Xna.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace StarForce_PendingTitle_
{
    [XmlRoot(Namespace = null, IsNullable = true, ElementName = "Controls")]
    public class MyControls
    {
        [XmlAttribute()]
        public String ControllerType;
        
        public Movement MyMovement;

        private KeyboardState KeyboardState;
                
        public struct KeyOrButton
        {
            [XmlAttribute]
            public Keys Key;
            [XmlAttribute]
            public int ButtonNumber;
        }


        public class Movement
        {
            [XmlAttribute()]
            public Keys Up;
            [XmlAttribute()]
            public Keys Down;
            [XmlAttribute()]
            public Keys Left;
            [XmlAttribute()]
            public Keys Right;
            public KeyOrButton BankLeft;
            public KeyOrButton BankRight;
            public KeyOrButton Boost;
            public KeyOrButton Shoot;
            public KeyOrButton Missle;

            
            public Keys Loop;
            public Keys Reverse;
            public Keys LeftLoop;
            public Keys RightLoop;

            [XmlAttribute()]
            public int Gamepad;
            public Movement()
            {
                Gamepad = 0;
            }
            [XmlAttribute]
            public int InvertYAxis;
        }

        public MyControls()
        {
            MyMovement = new Movement();
            ControllerType = "Keyboard";
            MyMovement.Up = Keys.Up;
            MyMovement.Down = Keys.Down;
            MyMovement.Left = Keys.Left;
            MyMovement.Right = Keys.Right;
            
            MyMovement.Boost.Key = Keys.Space;
            MyMovement.BankLeft.Key = Keys.Z;
            MyMovement.BankRight.Key = Keys.X;
            MyMovement.Shoot.Key = Keys.LeftControl;
            MyMovement.Missle.Key = Keys.LeftShift;
            MyMovement.Reverse = Keys.S;
            MyMovement.Loop = Keys.W;
            MyMovement.LeftLoop = Keys.A;
            MyMovement.RightLoop = Keys.D;

            MyMovement.Boost.ButtonNumber = 1;
            MyMovement.BankLeft.ButtonNumber = 6;
            MyMovement.BankRight.ButtonNumber = 7;
            MyMovement.Shoot.ButtonNumber = 2;
            MyMovement.Missle.ButtonNumber = 0;

            MyMovement.InvertYAxis = 1;

            MyMovement.Gamepad = 0;

            KeyboardState = Keyboard.GetState();
        }

        public Vector2 getBanking()
        {
            KeyboardState = Keyboard.GetState();
            Vector2 returnVector = new Vector2();
            if (ControllerType == "Keyboard")
            {
                if (KeyboardState.IsKeyDown(MyMovement.BankLeft.Key))
                {
                    returnVector.X = 1f;
                } 
                if (KeyboardState.IsKeyDown(MyMovement.BankRight.Key))
                {
                    returnVector.Y = 1f;
                }
            }
            else if (ControllerType == "Gamepad")
            {
                if (DirectInputGamepad.Gamepads[MyMovement.Gamepad].Buttons.List[MyMovement.BankLeft.ButtonNumber] == ButtonState.Pressed)
                {
                    returnVector.X = 1f;
                }
                if (DirectInputGamepad.Gamepads[MyMovement.Gamepad].Buttons.List[MyMovement.BankRight.ButtonNumber] == ButtonState.Pressed)
                {
                    returnVector.Y = 1f;
                }
            }
            else if (ControllerType == "XBox Controller")
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
                {
                    returnVector.X = 1f;
                }

                if (GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed)
                {
                    returnVector.Y = 1f;
                }
            }
            return returnVector;
        }

        public float getBoost()
        {
            KeyboardState = Keyboard.GetState();
            float returnfloat = 0f;
            if (ControllerType == "Keyboard")
            {
                if (KeyboardState.IsKeyDown(MyMovement.Boost.Key))
                {
                    returnfloat = 1f;
                }
            }
            else if (ControllerType == "Gamepad")
            {
                if (DirectInputGamepad.Gamepads[MyMovement.Gamepad].Buttons.List[MyMovement.Boost.ButtonNumber] == ButtonState.Pressed)
                {
                    returnfloat = 1f;
                }
            }
            else if (ControllerType == "XBox Controller")
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed)
                {
                    returnfloat = 1f;
                }
            }
            return returnfloat;
                

        }

        public Vector2 getRightThumbStick()
        {
            KeyboardState = Keyboard.GetState();
            if (ControllerType == "Keyboard")
            {
                Vector2 ReturnVector = new Vector2(); ;
                if (KeyboardState.IsKeyDown(MyMovement.Loop))
                {
                    ReturnVector.Y = 1;
                }
                else
                {
                    if (KeyboardState.IsKeyDown(MyMovement.Reverse))
                    {
                        ReturnVector.Y = -1;
                    }
                    else
                    {
                        if (KeyboardState.IsKeyDown(MyMovement.LeftLoop))
                        {
                            ReturnVector.X = -1;
                        }
                        else if (KeyboardState.IsKeyDown(MyMovement.RightLoop))
                        {
                            ReturnVector.X = 1;
                        }
                    }
                }
                
                return ReturnVector;
            }

                
            if (ControllerType == "Gamepad")
            {
                Vector2 Values = DirectInputGamepad.Gamepads[MyMovement.Gamepad].ThumbSticks.Right;;

                return new Vector2(Values.Y, Values.X) * new Vector2(1,-1);

            }
            if (ControllerType == "XBox Controller")
            {
                return GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;
            }

            return new Vector2();
        }

        public float getLock()
        {
            KeyboardState = Keyboard.GetState();
            if (KeyboardState.IsKeyDown(Keys.LeftAlt))
            {
                return 1f;
            }
            return 0f;
        }

        public float getShoot()
        {
            KeyboardState = Keyboard.GetState();
            float returnfloat = 0f;
            if (ControllerType == "Keyboard")
            {
                if (KeyboardState.IsKeyDown(MyMovement.Shoot.Key))
                {
                    returnfloat = 1f;
                }
            }
            else if (ControllerType == "Gamepad")
            {
                if (DirectInputGamepad.Gamepads[MyMovement.Gamepad].Buttons.List[MyMovement.Shoot.ButtonNumber] == ButtonState.Pressed)
                {
                    returnfloat = 1f;
                }
            }
            else if (ControllerType == "XBox Controller")
            {
                
                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                {
                    returnfloat = 1f;
                }
            }
            return returnfloat;
        }

        public float getMissle()
        {
            KeyboardState = Keyboard.GetState();
            float returnfloat = 0f;

            if (ControllerType == "Keyboard")
            {
                if (KeyboardState.IsKeyDown(MyMovement.Missle.Key)) returnfloat = 1f;
            }
            else if (ControllerType == "Gamepad")
            {
                if (DirectInputGamepad.Gamepads[MyMovement.Gamepad].Buttons.List[MyMovement.Missle.ButtonNumber] == ButtonState.Pressed)
                    returnfloat = 1f;
            }
            else if (ControllerType == "XBox Controller")
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed)
                    returnfloat = 1f;
            }

            return returnfloat;
        }

        public Vector2 GetMovement()
        {
            Vector2 returnvector = new Vector2() ;
            KeyboardState = Keyboard.GetState();
            if (ControllerType == "Gamepad")
            {
                returnvector = DirectInputGamepad.Gamepads[MyMovement.Gamepad].ThumbSticks.Left;
                //returnvector.Y *= MyMovement.InvertYAxis;
            }
            else if (ControllerType == "Keyboard")
            {
                if (KeyboardState.IsKeyDown(MyMovement.Up))
                {
                    returnvector.Y = -1;
                }
                else if (KeyboardState.IsKeyDown(MyMovement.Down))
                {
                    returnvector.Y = 1;
                }
                if (KeyboardState.IsKeyDown(MyMovement.Left))
                {
                    returnvector.X = -1;
                }
                if (KeyboardState.IsKeyDown(MyMovement.Right))
                {
                    returnvector.X = 1;
                }
               
            }
            else if (ControllerType == "XBox Controller")
            {
                returnvector = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
                returnvector.Y *= MyMovement.InvertYAxis;
            }
            return returnvector;
        }

      
        public void Save(string filename)
        {
            Stream stream = null;
            try
            {
                stream = File.Create(filename);
                XmlSerializer serializer = new XmlSerializer(typeof(MyControls));
                serializer.Serialize(stream, this);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        public static MyControls Load(string filename)
        {
            FileStream stream = null;
            MyControls Controls = null;

            try
            {
                stream = File.OpenRead("Controls.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(MyControls));
                Controls = (MyControls)serializer.Deserialize(stream);
            }
            catch
            {
                Controls = new MyControls();
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            if (Controls.ControllerType == "Gamepad" && DirectInputGamepad.Gamepads.Count == 0)
            {
                Controls.ControllerType = "Keyboard";
            }
            return Controls;
        }

        

    }
}
