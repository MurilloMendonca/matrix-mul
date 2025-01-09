# Matrix Multiplication CLI Application

## Overview

This project is a console application designed to perform matrix multiplication with various strategies and validate the results using a hashing mechanism. It demonstrates different matrix multiplication strategies and allows for runtime configuration using environment variables and command-line arguments.

## Getting Started

## Prerequisites

- **.NET 8.0 SDK**
- **Docker** : If you want to run as a docker application

## How to Run

### Running from the terminal

1. Clone the repository:
```bash
git clone https://github.com/MurilloMendonca/matrix-mul.git
cd matrix-mul
```
2. Build the application:
```bash
dotnet build -c Release
```
3. Run the application:
```bash
 ./bin/Release/net8.0/./matrix-mul 
```

### Using Docker

1. Build the Docker Image:
```bash
docker build -t matrix-mul .
```
2. Run the Docker container:
```bash
docker run --rm -e MATRIX_BASE_URL=http://example.com -it matrix-mul <matrix-size>
```
- Replace `<matrix-size>` with the desired size of the matrix.
- Optionally set the `MATRIX_BASE_URL` environment variable if the default URL needs to be overridden.

## Example Output
```text
Initializing matrices of size 1000x1000...
Matrix initialized with size 1000x1000
Matrix initialization completed in 0.42 seconds.

Fetching and filling matrices A and B...
Matrices A and B populated in 1.86 seconds.

Multiplying matrices A and B...
Matrix multiplication completed in 0.57 seconds.

Validating the result...
Validation successful.
Validation completed in 2.22 seconds.

Validation succeeded. The result is correct!

Total time: 5.07 seconds.
```

## Performance Results

### Machine Details

- OS: Windows 11
- CPU: Ryzen 5600 (6 cores, 12 threads)
- Memory: 32Gb 3200MT/s

### Methodology

Each test was run 5 times, with a matrix of size `1000x1000` using the Release configuration of Visual Studio.

### Strategy Results

- ParallelMultiplicationStrategy: 0.612 seconds
- MemoizedBlockMultiplicationStrategy: 2.654 seconds (block size of `64`)
- MemoizedParallelBlockMultiplicationStrategy: 0.732 seconds (block size of `64`)
- NaiveMultiplicationStrategy: 2.830 seconds

### Overall Results

The medium of 5 execution of the total time was:  5.674 seconds

With the output looking like [this](#Example-Output).