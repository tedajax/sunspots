using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace StarForce_PendingTitle_
{
    class SystemSelect : GameWindow
    {
        SpriteFont InfoFont;
        List<string> SystemList;
        int SystemNumber = 0;
        bool ChangeNotPressed = true;

        public SystemSelect()
        {
            Mode = "Setup";
        }

        public override void LoadContent()
        {
            InfoFont = WindowManager.Content.Load<SpriteFont>("Content\\SpriteFont2");
        }

        public override void Update(GameTime gameTime)
        {
            if (Mode.Equals("Setup")) Setup();
            else if (Mode.Equals("Run")) Run();
            else if (Mode.Equals("Die")) Die();
        }

        private void Setup()
        {
            Mode = "Run";

            SystemList = new List<string>();

            StreamReader sr = new StreamReader("Systems\\SystemsList.DAT");
            while (!sr.EndOfStream)
            {
                SystemList.Add(sr.ReadLine());
            }
            sr.Close();
        }

        private void Run()
        {
            if (ChangeNotPressed)
            {
                if (WindowManager.Controls.GetMovement().X > 0.5f)
                {
                    SystemNumber++;
                    if (SystemNumber > SystemList.Count - 1)
                        SystemNumber = 0;

                    ChangeNotPressed = false;
                }
                else if (WindowManager.Controls.GetMovement().X < -0.5f)
                {
                    SystemNumber--;
                    if (SystemNumber < 0)
                        SystemNumber = SystemList.Count - 1;

                    ChangeNotPressed = false;
                }
                else
                {
                    ChangeNotPressed = true;
                }
            }

            if (WindowManager.Controls.getShoot() > 0)
                Mode = "Die";
        }

        private void Die()
        {
            if (SystemList.Count > 0)
            {
                WindowManager.findWindow(WindowManager.FindLastScreenPosition()).mode = "Hide";
                WindowManager.AddScreen(new LevelSelect(SystemList[SystemNumber]));
            }
            WindowManager.removeScreen(this);
        }

        public override void Draw(GameTime gameTime)
        {
            WindowManager.SpriteBatch.Begin();
            if (Mode.Equals("Run"))
            {
                if (SystemList.Count <= 0)
                    DrawCenterString(new Vector2(Game1.iWidth / 2, Game1.iHeight / 2), "Uh-oh no systems found!!!");
                else
                    DrawCenterString(new Vector2(Game1.iWidth / 2, Game1.iHeight / 2), SystemList[SystemNumber]);
            }
            WindowManager.SpriteBatch.End();
        }

        private void DrawCenterString(Vector2 Position, string str)
        {
            WindowManager.SpriteBatch.DrawString(InfoFont, str,
                                                 Position,
                                                 Color.White,
                                                 0f,
                                                 InfoFont.MeasureString(str) / 2,
                                                 1f,
                                                 SpriteEffects.None,
                                                 0);
        }
    }
}
