using SpaceSim;

namespace SpaceSim
{
    static class Program
    {
        static void Main()
        {
            Simulation mySim = new Simulation(800, 600, "My simulation");
            mySim.Run();
        }
    }
}