using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class MatrixEquationSystem
{
    public int meshCount                    { get; private set; }
    public Matrix<float> resistanceMatrix   { get; private set; }
    public Vector<float> meshVoltage        { get; private set; }
    public Vector<float> meshCurrent        { get; private set; }
    /*
    private static void Test()
    {
        float[,] resistances = {
            { 18, -12 },
            {-12,  23 }
        };
        var resMat = Matrix<float>.Build.DenseOfArray(resistances);

        float[] voltages = {
            45,
            -30
        };
        var voltVec = Vector<float>.Build.DenseOfArray(voltages);

        MatrixEquationSystem system = new MatrixEquationSystem(resMat, voltVec);
        Debug.Log(system.GetCalculatedCurrents().ToString());
    }
    */

    public MatrixEquationSystem(Matrix<float> resistanceMatrix, Vector<float> meshVoltage)
    {
        this.resistanceMatrix = resistanceMatrix;
        this.meshVoltage = meshVoltage;
        meshCount = resistanceMatrix.RowCount;

        // Si on a pas une matrice carré ou que la matrice des voltages n'est pas de meme longueur
        if(meshCount != resistanceMatrix.ColumnCount && meshCount != meshVoltage.Count)
        {
            throw new IncorrectCircuitException("Matrices de tailles incompatibles fournisent"); 
        }

        this.meshCurrent = GetCalculatedCurrents();
    }

    public Vector<float> GetCalculatedCurrents()
    {
        Vector<float> result = Vector<float>.Build.Dense(meshCount);

        float det = resistanceMatrix.Determinant();
        if (det != 0)
        {
            for(int col = 0; col < meshCount; col++)
            {
                Matrix<float> temp = Matrix<float>.Build.DenseOfMatrix(resistanceMatrix);
                temp.SetColumn(col, meshVoltage);
                result[col] = temp.Determinant() / det;
            }
        } else
        {
            throw new IncorrectCircuitException("Cannot be calculated - matrix det is 0");
        }

        return result;
    }
}
