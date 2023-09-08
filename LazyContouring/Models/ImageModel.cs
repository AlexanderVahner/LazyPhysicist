using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Models
{
    public sealed class ImageModel : Notifier
    {
        private readonly Image image;
        private readonly int[,] voxels;
        private readonly int xSize;
        private readonly int ySize;
        private readonly int zSize;

        public ImageModel(Image image)
        {
            this.image = image;
            if (image != null) 
            {
                xSize = image.XSize; 
                ySize = image.YSize; 
                zSize = image.ZSize;
                voxels = new int[xSize, ySize];
            }
        }

        public int XSize { get => xSize; }
        public int YSize { get => ySize; }
        public int ZSize { get => zSize; }
    }
}
