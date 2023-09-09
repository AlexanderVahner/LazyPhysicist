using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LazyContouring.Graphics
{
    public sealed class VoxelToPixelConverter
    {
        private int windowWidth = 2000;
        private int windowLevel = 2000;
        private int blackValue;
        private int whiteValue;

        public int VoxelToBgra32(int value, byte opacity = 255)
        {
            int pixel;
            int result;

            if (value < blackValue)
            {
                pixel = 0;
            }
            else if (value > whiteValue)
            {
                pixel = 255;
            }
            else
            {
                pixel = (int)((value - blackValue) * 255.0 / (whiteValue - blackValue));
            }

            result = opacity << 24; // A
            result |= pixel << 16;  // R
            result |= pixel << 8;   // G
            result |= pixel << 0;   // B

            return result;
        }

        public void TuneConverter()
        {
            blackValue = windowLevel - windowWidth / 2;
        }

        public int WindowWidth 
        { 
            get => windowWidth;
            set
            {
                windowWidth = value;
                TuneConverter();
            }
        }
        public int WindowLevel 
        { 
            get => windowLevel;
            set
            {
                windowLevel = value;
                TuneConverter();
            }
        }
    }
}
