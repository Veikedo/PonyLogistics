namespace TSPSolver
{
  public class TSPRoute
  {
    public TSPRoute(int from, int to, double cost, double duration = 0)
    {
      Duration = duration;
      Cost = cost;
      To = to;
      From = from;
    }

    public int From { get; private set; }
    public int To { get; private set; }
    public double Cost { get; private set; }
    public double Duration { get; private set; }
  }
}