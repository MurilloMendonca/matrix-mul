using System.Collections.Generic;
using matrix_mul.Models;
using matrix_mul.Services.MultiplicationStrategy;

public class MemoizedBlockMultiplicationStrategy : IMultiplicationStrategy
{
    private readonly int _blockSize;
    private readonly Dictionary<(int, int, int), int[,]> _cache;

    public MemoizedBlockMultiplicationStrategy(int blockSize = 64)
    {
        _blockSize = blockSize;
        _cache = new Dictionary<(int, int, int), int[,]>();
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
                    int[,] blockResult = MultiplyBlocksWithMemoization(matrixA, matrixB, ii, jj, kk);
                    for (int i = 0; i < blockResult.GetLength(0); i++)
                    {
                        for (int j = 0; j < blockResult.GetLength(1); j++)
                        {
                            int row = ii + i;
                            int col = jj + j;
                            result.SetValue(row, col, result.GetValue(row, col) + blockResult[i, j]);
                        }
                    }
                }
            }
        }

        return result;
    }

    private int[,] MultiplyBlocksWithMemoization(SquareMatrix matrixA, SquareMatrix matrixB, int ii, int jj, int kk)
    {
        if (_cache.TryGetValue((ii, jj, kk), out int[,] cachedResult))
        {
            return cachedResult;
        }

        int blockSize = _blockSize;
        int sizeA = Math.Min(blockSize, matrixA.Size - ii);
        int sizeB = Math.Min(blockSize, matrixB.Size - jj);
        int sizeK = Math.Min(blockSize, matrixA.Size - kk);

        int[,] blockResult = new int[sizeA, sizeB];

        for (int i = 0; i < sizeA; i++)
        {
            for (int j = 0; j < sizeB; j++)
            {
                int sum = 0;
                for (int k = 0; k < sizeK; k++)
                {
                    sum += matrixA.GetValue(ii + i, kk + k) * matrixB.GetValue(kk + k, jj + j);
                }
                blockResult[i, j] = sum;
            }
        }

        _cache[(ii, jj, kk)] = blockResult;
        return blockResult;
    }
}
