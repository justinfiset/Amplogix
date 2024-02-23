using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System.Dynamic;
using UnityEditor.SceneTemplate;
using System;

public class MatrixEquationSystem
{
    private int meshCount;
    private Matrix<float> resistanceMatrix;
    private Vector<float> meshVoltage;

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

    MatrixEquationSystem(Matrix<float> resistanceMatrix, Vector<float> meshVoltage)
    {
        this.resistanceMatrix = resistanceMatrix;
        this.meshVoltage = meshVoltage;
        meshCount = resistanceMatrix.RowCount;

        // Si on a pas une matrice carré ou que la matrice des voltages n'est pas de meme longueur
        if(meshCount != resistanceMatrix.ColumnCount && meshCount != meshVoltage.Count)
        {
            throw new IncorrectCircuitException("Matrices de tailles incompatibles fournisent"); 
        }
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
