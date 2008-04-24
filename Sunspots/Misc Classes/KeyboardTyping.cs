using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace StarForce_PendingTitle_
{
    class KeyboardTyping
    {
        String CurrentString = "";
        bool uppercase = false;
        bool capsloack = false;
        TimeSpan backspacetime = new TimeSpan();
        TimeSpan spacetime = new TimeSpan();
        bool pressedspace= false;
        bool pressedbackspace = false;
        KeyboardState oldstate;

       
       
        public KeyboardTyping(KeyboardState keystate)
        {
            oldstate = keystate;
        }

        public String update(GameTime elapsedTime, KeyboardState keystate)
        {
            
            uppercase = false;
            if (capsloack) uppercase = true;
            if (keystate.GetPressedKeys().Length > 0)
            {
                if (keystate.IsKeyDown(Keys.LeftShift) || keystate.IsKeyDown(Keys.RightShift))
                {
                    uppercase = true;
                }
                if (keystate.IsKeyDown(Keys.Back))
                {

                    if (CurrentString.Length > 0)
                    {
                        if (elapsedTime.TotalGameTime >= backspacetime)
                        {
                            CurrentString = CurrentString.Substring(0, CurrentString.Length - 1);
                            if (!pressedbackspace)
                            {
                                backspacetime = elapsedTime.TotalGameTime + new TimeSpan(0, 0, 0,0,500);
                                pressedbackspace = true;

                            }
                            else
                                backspacetime = elapsedTime.TotalGameTime + new TimeSpan(0, 0, 0, 0, 100);
                            
                        }
                    }
                    return CurrentString;
                }
            
                if (keystate.IsKeyDown(Keys.Space) )
                {

                    if (elapsedTime.TotalGameTime> spacetime)
                    {
                        CurrentString += " ";
                        if (!pressedspace)
                        {
                            spacetime = elapsedTime.TotalGameTime + new TimeSpan(0, 0,0,0,500);
                            pressedspace = true;

                        }
                        else
                            spacetime = elapsedTime.TotalGameTime+ new TimeSpan(0, 0, 0, 0, 100);

                    }
                    return CurrentString;
                }
                if (keystate.IsKeyDown(Keys.CapsLock) && oldstate.IsKeyUp(Keys.CapsLock))
                {
                    if (capsloack == true)
                    {
                        capsloack = false;
                        uppercase = false;
                    }
                    else
                    {
                        capsloack = true;
                        uppercase = true;
                    }
                }                          
            

                foreach (Keys K in keystate.GetPressedKeys())
                {
                    

                    CurrentString+= ProcessKey(K, K.ToString(), elapsedTime);
    
                    
                }
            }
            if (keystate.IsKeyUp(Keys.Back))
            {
            pressedbackspace = false;
            backspacetime = new TimeSpan(0, 0, 0);
            }
            if (keystate.IsKeyUp(Keys.Space))
            {
            pressedspace = false;
            spacetime = new TimeSpan();
            }
            oldstate = keystate;
            return CurrentString;
        
        }
        public String ProcessKey(Keys Keys, String Key, GameTime elapsed)
        {
          

            String returnstring = "";
            Keys[] Keychecker = { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
                                  Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
                                  Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
                                  Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
                                  Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
                                  Keys.Z, Keys.D1, Keys.D2, Keys.D3, Keys.D4,
                                  Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9,
                                  Keys.D0, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2,
                                  Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6,
                                  Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.Decimal,
                                  Keys.Divide, Keys.Multiply, Keys.Add, Keys.Subtract
                                };

            int i = 0;
      
            while (i < Keychecker.Length && Keychecker[i] != Keys)
            {
                i++;
            }
       
           
            if (i>=Keychecker.Length && !Key.ToUpper().Contains("OEM") && !Key.ToUpper().Contains("NUMPAD")) return returnstring;
            
                    if (oldstate.IsKeyUp(Keys))
                    {
                        Key = convertText(Key, uppercase);
                        String thisletter = Key;
                        thisletter = thisletter.ToLower();
                        if (uppercase) thisletter = thisletter.ToUpper();
                        
                        returnstring += thisletter;

                    }
                    return returnstring;
        }
        
        public String convertText(String Key, bool uppercase)
        {
            if (Key.Length == 2 && Key[0] == 'D')
            {
                Key = Key[1].ToString();
                if (uppercase)
                {
                    if (Key == "1") Key = "!";
                    if (Key == "2") Key = "@";
                    if (Key == "3") Key = "$";
                    if (Key == "4") Key = "$";
                    if (Key == "5") Key = "%";
                    if (Key == "6") Key = "^";
                    if (Key == "7") Key = "&";
                    if (Key == "8") Key = "*";
                    if (Key == "9") Key = "(";
                    if (Key == "0") Key = ")";
                }
            }
            
            if (Key.ToLower() == "oemperiod")
                if (uppercase)
                    Key = ">";
                else
                    Key = ".";
            else if (Key.ToLower() == "oemcomma")
                if (uppercase)
                    Key = "<";
                else
                    Key = ",";
            else if (Key.ToLower() == "oemsemicolon")
                if (uppercase)
                    Key = ":";
                else
                    Key = ";";
            else if (Key.ToLower() == "oemquestion")
                if (uppercase)
                    Key = "?";
                else
                    Key = "/";
            else if (Key.ToLower() == "oemquotes")
                if (uppercase)
                    Key = "\"";
                else
                    Key = "'";
            

            if (Key.ToLower() == "numpad0") Key = "0";
            if (Key.ToLower() == "numpad1") Key = "1";
            if (Key.ToLower() == "numpad2") Key = "2";
            if (Key.ToLower() == "numpad3") Key = "3";
            if (Key.ToLower() == "numpad4") Key = "4";
            if (Key.ToLower() == "numpad5") Key = "5";
            if (Key.ToLower() == "numpad6") Key = "6";
            if (Key.ToLower() == "numpad7") Key = "7";
            if (Key.ToLower() == "numpad8") Key = "8";
            if (Key.ToLower() == "numpad9") Key = "9";

            if (Key.ToLower() == "decimal") Key = ".";
            if (Key.ToLower() == "multiply") Key = "*";
            if (Key.ToLower() == "divide") Key = "/";
            if (Key.ToLower() == "subtract") Key = "-";
            if (Key.ToLower() == "add") Key = "+";


            return Key;
        }


        public void clearCurrentString() { CurrentString = ""; }

            
            
        

    }
}
