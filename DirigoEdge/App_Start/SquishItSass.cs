[assembly: WebActivator.PreApplicationStartMethod(typeof(DirigoEdge.App_Start.SquishItSass), "Start")]

namespace DirigoEdge.App_Start
{
    using SquishIt.Framework;
    using SquishIt.Sass;

    public class SquishItSass
    {
        public static void Start()
        {
            Bundle.RegisterStylePreprocessor(new SassPreprocessor());
        }
    }
}