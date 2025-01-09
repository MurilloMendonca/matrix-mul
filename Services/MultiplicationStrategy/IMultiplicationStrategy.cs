using matrix_mul.Models;

namespace matrix_mul.Services.MultiplicationStrategy
{
    public interface IMultiplicationStrategy
    {
        SquareMatrix Multiply(SquareMatrix matrixA, SquareMatrix matrixB);
    }
}
