
namespace FlareSolverrSharp.Sample
{
    static class Program
    {
        static void Main()
        {
            ClearanceHandlerSample.SampleGet().Wait();
            ClearanceHandlerSample.SamplePostUrlEncoded().Wait();
        }
    }
}
