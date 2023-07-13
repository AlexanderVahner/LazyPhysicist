namespace LazyContouring.Graphics
{
    public sealed class ImageSlice
    {
        private readonly int[,] voxels;

        public ImageSlice(int[,] voxels)
        {
            this.voxels = voxels;
        }

        public int[,] Voxels => voxels;
    }
}

