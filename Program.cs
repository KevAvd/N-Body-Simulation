namespace SpaceSim
{
    static class Program
    {
        static void Main()
        {
            Simulation sim = new Simulation(800, 600, "My simulation");
            sim.Run();
        }
    }
}