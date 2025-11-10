using System;

namespace LazyContouring.Operations
{
    public static class OperationCreator
    {
        public static Operation CreateFromString(string operationName)
        {
            Operation result = null;

            if (Enum.TryParse(operationName, out OperationType operationType))
            {
                result = CreateFromType(operationType);
            }
            else if (operationName == "CropInner")
            {
                result = new CropOperation() { CropPart = CropPart.Inside };
            }
            else if (operationName == "CropOuter")
            {
                result = new CropOperation() { CropPart = CropPart.Outside };
            }

            return result;
        }

        public static Operation CreateFromType(OperationType operationType)
        {
            Operation result = null;

            switch (operationType)
            {
                case OperationType.Empty:
                    result = new EmptyOperation();
                    break;
                case OperationType.Assign:
                    result = new AssignOperation();
                    break;
                case OperationType.And:
                    result = new AndOperation();
                    break;
                case OperationType.Or:
                    result = new OrOperation();
                    break;
                case OperationType.Not:
                    result = new NotOperation();
                    break;
                case OperationType.Sub:
                    result = new SubOperation();
                    break;
                case OperationType.Xor:
                    result = new XorOperation();
                    break;
                case OperationType.Wall:
                    result = new WallOperation();
                    break;
                case OperationType.Margin:
                    result = new MarginOperation();
                    break;
                case OperationType.AsymmetricMargin:
                    result = new AsymmetricMarginOperation();
                    break;
                case OperationType.Crop:
                    result = new CropOperation();
                    break;
            }

            return result;
        }
    }
}
