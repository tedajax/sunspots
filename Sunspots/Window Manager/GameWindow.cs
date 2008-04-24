#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace StarForce_PendingTitle_
{
    public abstract class GameWindow
    {
        public WindowManager WindowManager
        {
            get { return windowManager;  }
            internal set { windowManager = value; }
        }


        WindowManager windowManager;

        protected String Mode;

        public String mode
        {
            get { return Mode; }
            set { Mode = value; }
        }
        

        public KeyboardState NewState
        {
            get { return windowManager.NewState; }
        }

        public KeyboardState OldState
        {
            get { return WindowManager.OldState; }
        }

        public virtual void Init() { }

        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }

        public virtual void Update(GameTime gametime) { }

        public virtual void Draw(GameTime gameTime) { }

        /// <summary>
        /// This is used to unpause a window in a window created by the paused window
        /// </summary>
        public virtual void UnPauseWindow() 
        {
            Mode = "UnPause";
        }
    }
}
