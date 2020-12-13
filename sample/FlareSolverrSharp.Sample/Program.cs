
namespace FlareSolverrSharp.Sample
{
    static class Program
    {
        static void Main(string[] args)
        {
            ClearanceHandlerSample.SampleGet().Wait();
            ClearanceHandlerSample.SamplePostUrlEncoded().Wait();
        }
    }
}