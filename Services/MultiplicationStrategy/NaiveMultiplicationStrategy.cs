using matrix_mul.Models;

namespace matrix_mul.Services.MultiplicationStrategy
{
    internal class NaiveMultiplicationStrategy : IMultiplicationStrategy
    {
        public SquareMatrix Multiply(SquareMatrix matrixA, SquareMatrix matrixB)
        {
            if (matrixA.Size != matrixB.Size)
                throw new ArgumentException("Matrix sizes must be equal.", nameof(matrixB));

            int size = matrixA.Size;
            SquareMatrix result = new SquareMatrix(size);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int sum = 0;
                    for (int k = 0; k < size; k++)
                    {
                        sum += matrixA.GetValue(i, k) * matrixB.GetValue(k, j);
                    }
                    result.SetValue(i, j, sum);
                }
            }

            return result;
        }
    }
}
