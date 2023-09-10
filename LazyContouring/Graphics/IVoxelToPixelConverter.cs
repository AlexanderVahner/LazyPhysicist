namespace LazyContouring.Graphics
{
    public interface IVoxelToPixelConverter
    {
        int WindowLevel { get; set; }
        int WindowWidth { get; set; }

        int VoxelToBgra32(int value, byte opacity = 255);
    }
}