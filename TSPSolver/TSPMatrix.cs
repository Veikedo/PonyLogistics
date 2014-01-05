using System;
using System.Linq;

namespace TSPSolver
{
  public class TSPMatrix
  {
    private const double Tolerance = 1e-5;
    private readonly TSPRoute[][] _routes;

    public TSPMatrix(TSPRoute[][] routes) : this(routes, 0)
    {
    }

    private TSPMatrix(TSPRoute[][] routes, double lowBound)
    {
      if (routes.Any(row => row.Length != routes.Length))
      {
        throw new ArgumentException("Matrix must be squared");
      }

      _routes = routes;
      Reduce();

      LowBound = lowBound;
    }

    public double LowBound { get; private set; }

    private void Reduce()
    {
      double reductionCoef = 0;

      foreach (var row in _routes)
      {
        double min = row.Min(x => x.Cost);
        reductionCoef += min;

        for (int i = 0; i < row.Length; i++)
        {
          TSPRoute t = row[i];
          row[i] = new TSPRoute(t.From, t.To, t.Cost - min, t.Duration);
        }
      }

      for (int i = 0; i < _routes.Length; i++)
      {
        double min = _routes.Min(row => row[i].Cost);
        reductionCoef += min;

        foreach (var row in _routes)
        {
          TSPRoute t = row[i];
          row[i] = new TSPRoute(t.From, t.To, t.Cost - min, t.Duration);
        }
      }

      LowBound += reductionCoef;
    }

    public Tuple<int, int> FindMaxPenyIndices()
    {
      Tuple<int, int> indices = null;
      double max = -1;

      for (int i = 0; i < _routes.Length; i++)
      {
        for (int j = 0; j < _routes.Length; j++)
        {
          if (Math.Abs(_routes[i][j].Cost) < Tolerance)
          {
            TSPRoute[] col = GetColumn(j);
            TSPRoute[] row = _routes[i];

            double peny = FindCoef(row, j) + FindCoef(col, i);
            if (peny > max)
            {
              max = peny;
              indices = new Tuple<int, int>(i, j);
            }
          }
        }
      }

      return indices;
    }

    private double FindCoef(TSPRoute[] routes, int exclude)
    {
      int start = exclude == 0 ? 1 : 0;
      double min = routes[start].Cost;

      for (int i = start; i < routes.Length; i++)
      {
        if (i == exclude)
        {
          continue;
        }

        if (routes[i].Cost < min)
        {
          min = routes[i].Cost;
        }
      }

      return min;
    }

    private TSPRoute[] GetColumn(int index)
    {
      return _routes.Select(row => row[index]).ToArray();
    }

    public TSPMatrix[] GetSubMatrices()
    {
      Tuple<int, int> slicePoint = FindMaxPenyIndices();

      TSPMatrix left = GetLeftBranch(slicePoint);
      TSPMatrix right = GetRightBranch(slicePoint);

      return new[] {left, right};
    }

    private TSPMatrix GetRightBranch(Tuple<int, int> slicePoint)
    {
      var newRoutes = new TSPRoute[_routes.Length][];
      for (int i = 0; i < newRoutes.Length; i++)
      {
        newRoutes[i] = new TSPRoute[newRoutes.Length];
      }

      for (int i = 0; i < newRoutes.Length; i++)
      {
        for (int j = 0; j < newRoutes.Length; j++)
        {
          newRoutes[i][j] = _routes[i][j];
        }
      }

      newRoutes[slicePoint.Item1][slicePoint.Item2] = new TSPRoute(-1, -1, double.MaxValue, double.MaxValue);
      return new TSPMatrix(newRoutes, LowBound);
    }

    private TSPMatrix GetLeftBranch(Tuple<int, int> slicePoint)
    {
      int newSize = _routes.Length - 1;
      var newRoutes = new TSPRoute[newSize][];

      for (int i = 0; i < newSize; i++)
      {
        newRoutes[i] = new TSPRoute[newSize];
      }

      for (int i = 0; i < newSize; i++)
      {
        if (i == slicePoint.Item1)
        {
          continue;
        }

        for (int j = 0; j < newSize; j++)
        {
          if (j == slicePoint.Item2)
          {
            continue;
          }

          newRoutes[i][j] = _routes[i][j];
        }
      }

      newRoutes[slicePoint.Item2][slicePoint.Item1] = new TSPRoute(-1, -1, double.MaxValue, double.MaxValue);

      return new TSPMatrix(newRoutes, LowBound);
    }
  }
}