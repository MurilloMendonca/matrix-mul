﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using matrix_mul.Models;
using matrix_mul.Services.MultiplicationStrategy;

namespace matrix_mul.Services
{
    internal class MatrixMultiplicationService
    {
        private IMultiplicationStrategy _strategy;

        public MatrixMultiplicationService(IMultiplicationStrategy strategy)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        public void SetStrategy(IMultiplicationStrategy strategy)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        public SquareMatrix Multiply(SquareMatrix matrixA, SquareMatrix matrixB)
        {
            return _strategy.Multiply(matrixA, matrixB);
        }
    }
}
