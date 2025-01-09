using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using matrix_mul.Services;
using matrix_mul.Services.MultiplicationStrategy;

class Program
{
    static async Task Main(string[] args)
    {
        string baseUrl = Environment.GetEnvironmentVariable("MATRIX_BASE_URL") ?? "http://jbi-takehome.app:8080";
        int matrixSize = args.Length > 0 && int.TryParse(args[0], out int parsedSize) ? parsedSize : 1000;

        using HttpClient httpClient = new HttpClient();

        MatrixMultiplicationExecutor executor = new MatrixMultiplicationExecutor(httpClient, baseUrl, new ParallelMultiplicationStrategy());
        await executor.Run(matrixSize);
    }
}
