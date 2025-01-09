using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using matrix_mul.Models;


namespace matrix_mul.Services
{

    /// <summary>
    /// Identifies the matrix being accessed (A or B).
    /// </summary>
    enum MatrixIdentifier
    {
        A,
        B
    }

    /// <summary>
    /// Service to get data from the matrices.
    /// </summary>
    internal class MatrixService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private int _matrixSize;
        public MatrixService(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
        }

        /// <summary>
        /// Initializes the matrices by setting their size.
        /// </summary>
        /// <param name="matrixSize">The size of the square matrices.</param>
        public async Task InitMatrixes(int matrixSize)
        {
            if (matrixSize <= 0)
                throw new ArgumentException("Matrix size must be greater than 0.", nameof(matrixSize));

            _matrixSize = matrixSize;
            string url = $"{_baseUrl}/api/matrix/init";

            var requestBody = new { n = matrixSize };
            string jsonPayload = JsonSerializer.Serialize(requestBody);

            StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Matrix initialization failed: {response.StatusCode} - {errorMessage}");
            }

            Console.WriteLine($"Matrix initialized with size {matrixSize}x{matrixSize}");

        }

        /// <summary>
        /// Fetches a specific row from the given matrix.
        /// </summary>
        /// <param name="matrixIdentifier">The matrix identifier (A or B).</param>
        /// <param name="row">The row index (0-based).</param>
        /// <returns>The row data as an integer array.</returns>
        public async Task<int[]> GetRowAsync(MatrixIdentifier matrixIdentifier, int row)
        {
            ValidateIndex(row);

            string url = $"{_baseUrl}/api/matrix/{matrixIdentifier}/row/{row}";
            string response = await _httpClient.GetStringAsync(url);

            var rowResponse = JsonSerializer.Deserialize<MatrixRowResponse>(response);

            return rowResponse?.Row ?? Array.Empty<int>();
        }

        /// <summary>
        /// Fetches a specific column from the given matrix.
        /// </summary>
        /// <param name="matrixIdentifier">The matrix identifier (A or B).</param>
        /// <param name="col">The column index (0-based).</param>
        /// <returns>The column data as an integer array.</returns>
        public async Task<int[]> GetColAsync(MatrixIdentifier matrixIdentifier, int col)
        {
            ValidateIndex(col);

            string url = $"{_baseUrl}/api/matrix/{matrixIdentifier}/column/{col}";

            string response = await _httpClient.GetStringAsync(url);

            // Deserialize the response into an object and extract the "column" property
            var colResponse = JsonSerializer.Deserialize<MatrixColumnResponse>(response);
            return colResponse?.Column ?? Array.Empty<int>();
        }

        /// <summary>
        /// Validates the row or column index.
        /// </summary>
        /// <param name="index">The row or column index (1-based).</param>
        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= _matrixSize)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 1 and {_matrixSize}.");
        }
    }
}
