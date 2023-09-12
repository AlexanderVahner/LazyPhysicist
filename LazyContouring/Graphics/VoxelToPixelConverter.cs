namespace LazyContouring.Graphics
{
    public sealed class VoxelToPixelConverter : IVoxelToPixelConverter
    {
        private int windowWidth = 1500;
        private int windowLevel = 1250;
        private int blackValue;
        private int whiteValue;
        private double pixelColorStep;
        public VoxelToPixelConverter()
        {
            SetupConverter();
        }

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
                pixel = (int)((value - blackValue) * pixelColorStep);
            }

            result = opacity << 24; // A
            result |= pixel << 16;  // R
            result |= pixel << 8;   // G
            result |= pixel << 0;   // B

            return result;
        }

        private void SetupConverter()
        {
            blackValue = windowLevel - windowWidth / 2;
            whiteValue = windowLevel + windowWidth / 2;
            pixelColorStep = 255.0 / windowWidth;
        }

        public int WindowWidth
        {
            get => windowWidth;
            set
            {
                windowWidth = value;
                SetupConverter();
            }
        }
        public int WindowLevel
        {
            get => windowLevel;
            set
            {
                windowLevel = value;
                SetupConverter();
            }
        }
    }
}
