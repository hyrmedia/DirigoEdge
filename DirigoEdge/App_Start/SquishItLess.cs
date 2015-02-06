[assembly: WebActivator.PreApplicationStartMethod(typeof(DirigoEdge.App_Start.SquishItLess), "Start")]

namespace DirigoEdge.App_Start
{
    using SquishIt.Framework;
    using SquishIt.Less;

    public class SquishItLess
    {
        public static void Start()
        {
            Bundle.RegisterStylePreprocessor(new LessPreprocessor());
        }
    }
}