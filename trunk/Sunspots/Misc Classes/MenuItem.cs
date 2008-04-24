using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace StarForce_PendingTitle_
{
    class MenuItem
    {
        Rectangle MouseOverRect;
        String Name;
        Vector2 OverlayPosition;
        /// <summary>
        /// Makes a New Menu Item
        /// </summary>
        /// <param name="MouseRect">The Rectangle that will be used to see if the mouse is hovering over it</param>
        /// <param name="Name">The name to reference the MenuItem by</param>
        /// <param name="OverlayPos"> The Position to place the overlay to show the user that they are selecting this item</param>
        public MenuItem(Rectangle MouseRect, String Name, Vector2 OverlayPos)
        {
            this.Name = Name;
            this.MouseOverRect = MouseRect;
            this.OverlayPosition = OverlayPos;
        }
        /// <summary>
        /// Checks if the mouse is over this Menu Item.
        /// </summary>
        /// <param name="Mouse">The Mouse's X Y Position in Vector2 Form</param>
        /// <returns>true if mouse is over this item, false otherwise</returns>
        public bool isMouseOver(Vector2 Mouse)
        {
            if (Mouse.X > MouseOverRect.X && Mouse.X < MouseOverRect.X + MouseOverRect.Width && Mouse.Y > MouseOverRect.Y && Mouse.Y < MouseOverRect.Y + MouseOverRect.Height)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// gets the position to place the overlay to match this menu item
        /// </summary>
        /// <returns>returns a vector for the top left corner to place the overlay</returns>
        public Vector2 getOverlayPosition() { return OverlayPosition; }

        public override string ToString()
        {
            return this.Name;
        }

    }
}
