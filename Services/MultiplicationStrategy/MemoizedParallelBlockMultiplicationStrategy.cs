using System.Collections.Concurrent;
using System.Threading.Tasks;
using matrix_mul.Models;
using matrix_mul.Services.MultiplicationStrategy;

public class MemoizedParallelBlockMultiplicationStrategy : IMultiplicationStrategy
{
    private readonly int _blockSize;
    private readonly ConcurrentDictionary<(int, int, int), int[,]> _cache;

    public MemoizedParallelBlockMultiplicationStrategy(int blockSize = 64)
    {
        _blockSize = blockSize;
        _cache = new ConcurrentDictionary<(int, int, int), int[,]>();
    }

    public SquareMatrix Multiply(SquareMatrix matrixA, SquareMatrix matrixB)
    {
        if (matrixA.Size != matrixB.Size)
            throw new ArgumentException("Matrix sizes must be equal.", nameof(matrixB));

        int size = matrixA.Size;
        SquareMatrix result = new SquareMatrix(size);

        Parallel.For(0, size / _blockSize + 1, ii =>
        {
            int rowStart = ii * _blockSize;
            int rowEnd = Math.Min(rowStart + _blockSize, size);

            for (int jj = 0; jj < size / _blockSize + 1; jj++)
            {
                int colStart = jj * _blockSize;
                int colEnd = Math.Min(colStart + _blockSize, size);

                for (int kk = 0; kk < size / _blockSize + 1; kk++)
                {
                    int blockStart = kk * _blockSize;
                    int blockEnd = Math.Min(blockStart + _blockSize, size);

                    // Fetch or compute the block
                    int[,] blockResult = MultiplyBlocksWithMemoization(matrixA, matrixB, rowStart, colStart, blockStart, rowEnd, colEnd, blockEnd);

                    // Add the block to the result matrix
                    for (int i = 0; i < blockResult.GetLength(0); i++)
                    {
                        for (int j = 0; j < blockResult.GetLength(1); j++)
                        {
                            int row = rowStart + i;
                            int col = colStart + j;
                            lock (result)
                            {
                                result.SetValue(row, col, result.GetValue(row, col) + blockResult[i, j]);
                            }
                        }
                    }
                }
            }
        });

        return result;
    }

    private int[,] MultiplyBlocksWithMemoization(SquareMatrix matrixA, SquareMatrix matrixB, int rowStart, int colStart, int blockStart, int rowEnd, int colEnd, int blockEnd)
    {
        var key = (rowStart, colStart, blockStart);

        // Try to fetch from cache
        if (_cache.TryGetValue(key, out int[,] cachedBlock))
        {
            return cachedBlock;
        }

        // Compute the block if not in cache
        int numRows = rowEnd - rowStart;
        int numCols = colEnd - colStart;
        int blockSize = blockEnd - blockStart;

        int[,] blockResult = new int[numRows, numCols];

        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                int sum = 0;
                for (int k = 0; k < blockSize; k++)
                {
                    sum += matrixA.GetValue(rowStart + i, blockStart + k) * matrixB.GetValue(blockStart + k, colStart + j);
                }
                blockResult[i, j] = sum;
            }
        }

        // Add the computed block to the cache
        _cache[key] = blockResult;

        return blockResult;
    }
}
