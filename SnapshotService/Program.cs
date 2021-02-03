using System;

namespace SnapshotService
{
    class Program
    {
        static void Main(string[] args)
        {
            var workflow = new SnapshotWorkflow.SnapshotWorkflow();
            workflow.ExecuteWorkflow();
            Console.WriteLine("Done execution...");
            Console.Read();
        }
    }
}
