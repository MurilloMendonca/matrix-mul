using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using matrix_mul.Models;

namespace matrix_mul.Services.MultiplicationStrategy
{
    internal class BlockMultiplicationStrategy : IMultiplicationStrategy
    {
        private readonly int _blockSize;

        public BlockMultiplicationStrategy(int blockSize = 64)
        {
            _blockSize = blockSize;
        }

        public SquareMatrix Multiply(SquareMatrix matrixA, SquareMatrix matrixB)
        {
            if (matrixA.Size != matrixB.Size)
                throw new ArgumentException("Matrix sizes must be equal.", nameof(matrixB));

            int size = matrixA.Size;
            SquareMatrix result = new SquareMatrix(size);

            for (int ii = 0; ii < size; ii += _blockSize)
            {
                for (int jj = 0; jj < size; jj += _blockSize)
                {
                    for (int kk = 0; kk < size; kk += _blockSize)
                    {
                        for (int i = ii; i < Math.Min(ii + _blockSize, size); i++)
                        {
                            for (int j = jj; j < Math.Min(jj + _blockSize, size); j++)
                            {
                                int sum = result.GetValue(i, j);
                                for (int k = kk; k < Math.Min(kk + _blockSize, size); k++)
                                {
                                    sum += matrixA.GetValue(i, k) * matrixB.GetValue(k, j);
                                }
                                result.SetValue(i, j, sum);
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
