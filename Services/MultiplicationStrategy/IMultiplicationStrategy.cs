using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using matrix_mul.Models;

namespace matrix_mul.Services.MultiplicationStrategy
{
    public interface IMultiplicationStrategy
    {
        SquareMatrix Multiply(SquareMatrix matrixA, SquareMatrix matrixB);
    }
}
