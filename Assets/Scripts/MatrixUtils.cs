using UnityEngine;

public static class MatrixUtil
{
    public static void TestDetSolver()
    {
        float expected = -58f;
        float[,] testMatrix = {
            {  2,  7,  1  },
            {  3, -2,  0  },
            {  1,  5,  3  }
        };
        float result = GetDet(testMatrix);
        Debug.Log("Matrix Det Test: expected=" + expected + ", output=" + result + ", " + (expected == result));
    }

    public static float[,] IdentityMatrix(int size)
    {
        /*
            {  1,  0,  0  }
            {  0,  1,  0  }
            {  0,  0,  1  }
        */
        float[,] matrix = new float[size, size];

        for (int n = 0; n < size; n++)
        {
            matrix[n, n] = 1;
        }

        return matrix;
    }

    // TODO : GetDet
    // DETAIL: matrix row : i, col: j
    // float[i][j] - float[row, col]
    public static float GetDet(float[,] matrix)
    {
        float det = 0;
        int size = matrix.GetLength(0);

        float[,] U = matrix;
        float[,] L = IdentityMatrix(size);

        // For each row except the first one
        for (int row = 1; row < size; row++)
        {
            /*
                1 0 0  NaN
                x 1 0  (1, 0)
                x x 0  (2, 0), (2, 1)
            */
            int prevRow = row - 1; // previous row index
            for(int col = 0; col < row; col++)
            {
                float factor = U[row, col] / U[prevRow, col];
                for(int i = 0; i < size; i++)
                {
                    U[row, i] = U[row, i] - factor * U[prevRow, i];
                }
                L[row, col] = factor;
            }
        }

        string mat = "";
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < row; col++)
            {
                mat += L[row, col] + ", ";
            }
            Debug.Log(mat);
            mat = "";
        }
        det = GetDiagonalDet(L) * GetDiagonalDet(U);

        return det;
    }

    public static float GetDiagonalDet(float[,] mat)
    {
        float det = 0;
        for(int i = 0; i < mat.GetLength(0); i++)
        {
            if (i == 0) det = 1;
            det *= mat[i, i];
        }
        return det;
    }
}