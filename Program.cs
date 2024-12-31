using System.Data;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Markup;

class Program
{
    public static void Main(string[] args)
    {
        List<List<Double>> matrixDefault = [
            [0, 2, -3, 0, -5, 0, 0, 0],
            [8, -1, 1, -1, -1, 1, 0, 0],
            [10, 2, 4, 0, 0, 0, 1, 0],
            [3,  0,  0,  1,  1, 0, 0, 1]
        ];

        List<List<Double>> matrixPersonal = [
            [0,  2, -3,  0, -5,  0,  0,  0],
            [5, -1,  1, -1, -1,  1,  0,  0],
            [2,  2,  4,  0,  0,  0,  1,  0],
            [6,  0,  0,  1,  1,  0,  0,  1]
        ];

        Console.WriteLine("\n### Default matrix ###\n");
        var resultDefault = SolveSimplex(matrixDefault);
        Console.WriteLine("   Result: " + string.Join(" ", resultDefault.Select(x => x.ToString("0.##").PadLeft(5))));
        
        Console.WriteLine("\n### Personal matrix ###\n");
        var resultPersonal = SolveSimplex(matrixPersonal);
        Console.WriteLine("   Result: " + string.Join(" ", resultPersonal.Select(x => x.ToString("0.##").PadLeft(5))));
        
        Console.WriteLine("\nEnd");


    }
    private static List<Double> SolveSimplex(List<List<Double>> matrix)
    {
        List<Double> baseVals = [5, 6, 7];
        while (true){
            // see, what the current matrix is, to know what we're working with
            Console.WriteLine("Base matrix:");
            foreach (List<Double> row in matrix){
                Console.WriteLine(string.Join(" ", row.Select(x => x.ToString("0.##").PadLeft(5))));
            }

            // find the index which will be add to base
            var newBaseColumnId = FindNewBaseColumnId(matrix[0]);
            Console.WriteLine($"New base id: {newBaseColumnId}");
            Console.WriteLine("Base: " + string.Join(", ", baseVals));

            // in the case, that there is no negative base, additional logic
            if (newBaseColumnId == -1){
                Double x1 = 0, x2 = 0, x3 = 0, x4 = 0;
                for (int i = 0; i < baseVals.Count(); i++){
                    switch (baseVals[i]){
                        case 1: x1 = matrix[i + 1][0]; break;
                        case 2: x2 = matrix[i + 1][0]; break;
                        case 3: x3 = matrix[i + 1][0]; break;
                        case 4: x4 = matrix[i + 1][0]; break;
                    }
                }
                return new List<Double> {-1*matrix[0][0], x1, x2, x3, x4};
            }

            var removeBaseColumnId = FindRemoveBaseColumnId(matrix, baseVals, newBaseColumnId);
            Console.WriteLine($"Remove base id: {removeBaseColumnId}");
            (matrix, baseVals) = SwitchBases(matrix, baseVals, newBaseColumnId, removeBaseColumnId);
        }
    }

    private static int FindNewBaseColumnId(List<Double> row)
    {
        var i = 0;
        foreach (var val in row){
            if (val < 0 && i >= 0)
                return i;
            i++;
        }
        return -1;
    }

    private static int FindRemoveBaseColumnId(List<List<Double>> matrix, List<Double> baseVals, int newBaseColumnId)
    {
        Double[] remove = [-1, Double.MaxValue];
        for(int i = 0; i < baseVals.Count(); i++){
            if(matrix[i+1][newBaseColumnId]>0){
                if(matrix[i + 1][0]/matrix[i + 1][newBaseColumnId] < remove[1]){
                    remove = [i, matrix[i + 1][0]/matrix[i + 1][newBaseColumnId]];
                }
            }
        }
        return (int)remove[0];
    }

    private static (List<List<Double>>, List<Double>) SwitchBases(List<List<Double>> matrix, List<Double> baseVals, int newBaseColumnId, int removeBaseColumnId)
    {
        var divisor = matrix[removeBaseColumnId + 1][newBaseColumnId];
        List<Double> updatedRow = new List<Double>();

        foreach(var x in matrix[removeBaseColumnId + 1]){
            var num = x/divisor;
            updatedRow.Add(num);
        }

        matrix[removeBaseColumnId + 1] = updatedRow;

        for(int i = 0; i < matrix.Count(); i++){
            updatedRow = new List<Double>();
            if(i != removeBaseColumnId + 1){
                var rowMultiplier = matrix[i][newBaseColumnId];
                for(int j = 0; j < matrix[i].Count(); j++){
                    updatedRow.Add(matrix[i][j] - (matrix[removeBaseColumnId + 1][j] * rowMultiplier));
                }
                matrix[i] = updatedRow;
            }
        }

        baseVals[removeBaseColumnId] = newBaseColumnId;
        return (matrix, baseVals);
    }

}