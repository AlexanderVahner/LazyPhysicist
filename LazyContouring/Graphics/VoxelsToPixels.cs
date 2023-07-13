namespace LazyContouring.Graphics
{
    public class VoxelsToPixels
    {
        private int minHu = 500;
        private int maxHu = 2000;

        public int GetBrga32(int value, byte opacity = 255)
        {
            int pixel;
            int result;

            if (value < minHu)
            {
                pixel = 0;
            }
            else if (value > maxHu)
            {
                pixel = 255;
            }
            else
            {
                pixel = (int)((value - minHu) * 255.0 / (maxHu - minHu));
            }

            result = opacity << 24; // A
            result |= pixel << 16; // R
            result |= pixel << 8;   // G
            result |= pixel << 0;   // B

            return result;
        }    
    }
}

