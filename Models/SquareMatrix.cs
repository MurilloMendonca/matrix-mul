using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace matrix_mul.Models
{

    /// <summary>
    /// Represents the API response for a matrix row.
    /// </summary>
    internal class MatrixRowResponse
    {
        [JsonPropertyName("row")]
        public int[] Row { get; set; }
    }

    /// <summary>
    /// Represents the API response for a matrix column.
    /// </summary>
    internal class MatrixColumnResponse
    {
        [JsonPropertyName("column")]
        public int[] Column { get; set; }
    }

    /// <summary>
    /// Represents a square matrix with utility methods for accessing rows and columns.
    /// </summary>
    public class SquareMatrix
    {
        private readonly int[,] _data;
        public int Size { get; }

        /// <summary>
        /// Initializes a new instance of the Matrix class with the specified size.
        /// </summary>
        /// <param name="size">The size of the square matrix (NxN).</param>
        public SquareMatrix(int size)
        {
            if (size <= 0)
                throw new ArgumentException("Matrix size must be greater than 0.", nameof(size));

            Size = size;
            _data = new int[size, size];
        }

        /// <summary>
        /// Sets the value at the specified row and column.
        /// </summary>
        /// <param name="row">The row index (0-based).</param>
        /// <param name="col">The column index (0-based).</param>
        /// <param name="value">The value to set.</param>
        public void SetValue(int row, int col, int value)
        {
            ValidateIndex(col);
            ValidateIndex(row);
            _data[row, col] = value;
        }

        /// <summary>
        /// Gets the value at the specified row and column.
        /// </summary>
        /// <param name="row">The row index (0-based).</param>
        /// <param name="col">The column index (0-based).</param>
        /// <returns>The value at the specified position.</returns>
        public int GetValue(int row, int col)
        {
            ValidateIndex(col);
            ValidateIndex(row);
            return _data[row, col];
        }

        /// <summary>
        /// Gets a specific row as an array.
        /// </summary>
        /// <param name="row">The row index (0-based).</param>
        /// <returns>An array containing the values in the specified row.</returns>
        public int[] GetRow(int row)
        {
            ValidateIndex(row);
            int[] result = new int[Size];
            for (int col = 0; col < Size; col++)
            {
                result[col] = _data[row, col];
            }
            return result;
        }

        /// <summary>
        /// Sets the values of a specific row.
        /// </summary>
        /// <param name="row">The row index (0-based).</param>
        /// <param name="values">The values to set in the row.</param>
        public void SetRow(int row, int[] values)
        {
            if (values.Length != Size)
            {
                Console.WriteLine("Row size does not match matrix size.");
                Console.WriteLine("Row size: " + values.Length);
                Console.WriteLine("Matrix size: " + Size);
                Console.WriteLine("Values: " + values);
                Console.WriteLine("Row: " + row);
                throw new ArgumentException("Row size does not match matrix size.", nameof(values));
            }

            ValidateIndex(row);
            for (int col = 0; col < Size; col++)
            {
                _data[row, col] = values[col];
            }
        }

        /// <summary>
        /// Gets a specific column as an array.
        /// </summary>
        /// <param name="col">The column index (0-based).</param>
        /// <returns>An array containing the values in the specified column.</returns>
        public int[] GetColumn(int col)
        {
            ValidateIndex(col);
            int[] result = new int[Size];
            for (int row = 0; row < Size; row++)
            {
                result[row] = _data[row, col];
            }
            return result;
        }

        /// <summary>
        /// Sets the values of a specific column.
        /// </summary>
        /// <param name="col">The column index (0-based).</param>
        /// <param name="values">The values to set in the column.</param>
        public void SetColumn(int col, int[] values)
        {
            if (values.Length != Size)
                throw new ArgumentException("Column size does not match matrix size.", nameof(values));

            ValidateIndex(col);
            for (int row = 0; row < Size; row++)
            {
                _data[row, col] = values[row];
            }
        }

        public void Print()
        {
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    Console.Write($"{_data[row, col]} ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Validates the index.
        /// </summary>
        /// <param name="index">The index.</param>
        private void ValidateIndex(int index)
        {
            if ( index < 0 || index >= Size)
                throw new ArgumentOutOfRangeException($"Indices must be between 0 and {Size - 1}.");
        }

        
    }
}
