using System.Diagnostics;
using matrix_mul.Models;
using matrix_mul.Services.MultiplicationStrategy;

namespace matrix_mul.Services
{
    public class MatrixMultiplicationExecutor
    {
        private readonly MatrixService _matrixService;
        private readonly MatrixMultiplicationService _multiplicationService;
        private readonly MatrixMultiplicationValidationService _validationService;

        /// <summary>
        /// Default constructor using a parallel multiplication strategy.
        /// </summary>
        public MatrixMultiplicationExecutor(HttpClient httpClient, string baseUrl)
        {
            _matrixService = new MatrixService(httpClient, baseUrl);
            _multiplicationService = new MatrixMultiplicationService(new ParallelMultiplicationStrategy());
            _validationService = new MatrixMultiplicationValidationService(httpClient, baseUrl);
        }

        /// <summary>
        /// Constructor allowing a custom multiplication strategy.
        /// </summary>
        public MatrixMultiplicationExecutor(HttpClient httpClient, string baseUrl, IMultiplicationStrategy strategy)
        {
            _matrixService = new MatrixService(httpClient, baseUrl);
            _multiplicationService = new MatrixMultiplicationService(strategy);
            _validationService = new MatrixMultiplicationValidationService(httpClient, baseUrl);
        }

        public async Task Run(int matrixSize)
        {
            Stopwatch totalStopwatch = Stopwatch.StartNew();

            try
            {
                Console.WriteLine($"Initializing matrices of size {matrixSize}x{matrixSize}...");
                Stopwatch stepStopwatch = Stopwatch.StartNew();

                await _matrixService.InitMatrices(matrixSize);

                stepStopwatch.Stop();
                Console.WriteLine($"Matrix initialization completed in {stepStopwatch.Elapsed.TotalSeconds:F2} seconds.\n");

                Console.WriteLine("Fetching and filling matrices A and B...");
                stepStopwatch.Restart();

                (SquareMatrix matrixA, SquareMatrix matrixB) = await FetchAndFillMatrices(matrixSize);

                stepStopwatch.Stop();
                Console.WriteLine($"Matrices A and B populated in {stepStopwatch.Elapsed.TotalSeconds:F2} seconds.\n");

                Console.WriteLine("Multiplying matrices A and B...");
                stepStopwatch.Restart();

                SquareMatrix resultMatrix = MultiplyMatrices(matrixA, matrixB);

                stepStopwatch.Stop();
                Console.WriteLine($"Matrix multiplication completed in {stepStopwatch.Elapsed.TotalSeconds:F2} seconds.\n");

                Console.WriteLine("Validating the result...");
                stepStopwatch.Restart();

                bool isValid = await ValidateResult(resultMatrix);

                stepStopwatch.Stop();
                Console.WriteLine($"Validation completed in {stepStopwatch.Elapsed.TotalSeconds:F2} seconds.\n");

                Console.WriteLine(isValid ? "Validation succeeded. The result is correct!" : "Validation failed. The result is incorrect.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                totalStopwatch.Stop();
                Console.WriteLine($"\nTotal time: {totalStopwatch.Elapsed.TotalSeconds:F2} seconds.");
            }
        }

        private async Task<(SquareMatrix, SquareMatrix)> FetchAndFillMatrices(int matrixSize)
        {
            SquareMatrix matrixA = new SquareMatrix(matrixSize);
            SquareMatrix matrixB = new SquareMatrix(matrixSize);

            var rowTasks = new Task<int[]>[matrixSize];
            var colTasks = new Task<int[]>[matrixSize];

            for (int i = 0; i < matrixSize; i++)
            {
                rowTasks[i] = _matrixService.GetRowAsync(MatrixIdentifier.A, i);
                colTasks[i] = _matrixService.GetColAsync(MatrixIdentifier.B, i);
            }

            await Task.WhenAll(rowTasks);
            await Task.WhenAll(colTasks);

            for (int i = 0; i < matrixSize; i++)
            {
                matrixA.SetRow(i, rowTasks[i].Result);
                matrixB.SetColumn(i, colTasks[i].Result);
            }

            return (matrixA, matrixB);
        }

        private SquareMatrix MultiplyMatrices(SquareMatrix matrixA, SquareMatrix matrixB)
        {
            return _multiplicationService.Multiply(matrixA, matrixB);
        }

        private async Task<bool> ValidateResult(SquareMatrix resultMatrix)
        {
            return await _validationService.ValidateAsync(resultMatrix);
        }
    }
}
