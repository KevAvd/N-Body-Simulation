using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace NBodySim
{
    internal static class InputHandler
    {
        static bool[] oldState = new bool[(int)Keyboard.Key.KeyCount];
        static bool[] actState = new bool[(int)Keyboard.Key.KeyCount];

        static public void Update()
        {
            for(int i = 0; i < (int)Keyboard.Key.KeyCount; i++)
            {
                if (Keyboard.IsKeyPressed((Keyboard.Key)i))
                {
                    actState[i] = true;
                }
                else
                {
                    actState[i] = false;
                }
            }
        }

        static public void UpdateOld()
        {
            Array.Copy(actState, oldState, actState.Length);
        }

        static public bool IsKeyPressed(Keyboard.Key key)
        {
            if (actState[(int)key])
            {
                return true;
            }

            return false;
        }

        static public bool IsKeyClicked(Keyboard.Key key)
        {
            if(!oldState[(int)key] && actState[(int)key])
            {
                return true;
            }

            return false;
        }

        static public bool IsKeyReleased(Keyboard.Key key)
        {
            if (oldState[(int)key] && !actState[(int)key])
            {
                return true;
            }

            return false;
        }
    }
}
