using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

public class MatrixEquationSystem
{
    public Dictionary<int, List<ElectricComponent>> meshList { get; private set; }
    public int meshCount                    { get; private set; }
    public Matrix<float> resistanceMatrix   { get; private set; }
    public Vector<float> meshVoltage        { get; private set; }
    public Vector<float> meshCurrent        { get; private set; }

    /*
    public static void Test()
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


    public MatrixEquationSystem(Dictionary<int, List<ElectricComponent>> meshList, Matrix<float> resistanceMatrix, Vector<float> meshVoltage, int currentModifier)
    {
        this.meshList = meshList;
        this.resistanceMatrix = resistanceMatrix;
        this.meshVoltage = meshVoltage;
        meshCount = resistanceMatrix.RowCount;

        // Si on a pas une matrice carr� ou que la matrice des voltages n'est pas de meme longueur
        if(meshCount != resistanceMatrix.ColumnCount && meshCount != meshVoltage.Count)
        {
            throw new IncorrectCircuitException("Matrices de tailles incompatibles fournisent"); 
        }

        this.meshCurrent = GetCalculatedCurrents(currentModifier);
    }

    public Vector<float> GetCalculatedCurrents(int modifier)
    {
        Vector<float> result = Vector<float>.Build.Dense(meshCount);

        float det = resistanceMatrix.Determinant();
        if (det != 0)
        {
            for (int col = 0; col < meshCount; col++)
            {
                Matrix<float> temp = Matrix<float>.Build.DenseOfMatrix(resistanceMatrix);
                temp.SetColumn(col, meshVoltage);
                float current = (temp.Determinant() / det);
                current *= modifier;
                //current = current * Mathf.Sign(meshVoltage[col]); // On vient donner le signe positif ou n�gatif au courrant
                result[col] = current;
            }
        } else
        {
            throw new IncorrectCircuitException("Cannot be calculated - matrix det is 0");
        }

        return result;
    }
}
