using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using matrix_mul.Models;

namespace matrix_mul.Services
{
    internal class MatrixMultiplicationValidationService(HttpClient httpClient, string baseUrl)
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        private readonly string _validateEndpoint = $"{baseUrl}/api/matrix/validate";

        /// <summary>
        /// Validates the matrix multiplication result by computing its MD5 hash,
        /// encoding it in Base64, and sending it to the validation endpoint.
        /// </summary>
        /// <param name="matrix">The result matrix from the multiplication.</param>
        /// <returns>True if validation is successful, otherwise false.</returns>
        public async Task<bool> ValidateAsync(SquareMatrix matrix)
        {
            ArgumentNullException.ThrowIfNull(matrix);

            string matrixHash = ComputeMatrixHash(matrix);

            var requestBody = new { Hash = matrixHash };
            string jsonPayload = JsonSerializer.Serialize(requestBody);

            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(_validateEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Validation successful.");
                return true;
            }

            string errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Validation failed: {errorMessage}");
            return false;
        }

        /// <summary>
        /// Computes the MD5 hash of a matrix and encodes it in Base64.
        /// </summary>
        /// <param name="matrix">The matrix to hash.</param>
        /// <returns>The MD5 hash encoded in Base64.</returns>
        private static string ComputeMatrixHash(SquareMatrix matrix)
        {

            using MemoryStream ms = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(ms);

            for (int row = 0; row < matrix.Size; row++)
            {
                for (int col = 0; col < matrix.Size; col++)
                {
                    writer.Write(matrix.GetValue(row, col));
                }
            }

            byte[] hashBytes = MD5.HashData(ms.ToArray());

            return Convert.ToBase64String(hashBytes);
        }
    }
}
