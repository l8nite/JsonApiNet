using System.Collections.Generic;

namespace JsonApiNet.TestSecondAssembly
{
    //  These classes are defined in a second assembly to test the
    //  ability of JsonApiNet to resolve types that are not defined
    //  in the same assembly that is running

    public class GraphicNovel
    {
        public string Title { get; set; }

        public Illustrator Illustrator { get; set; }

        public List<Illustration> Illustrations { get; set; }
    }

    public class Illustrator
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class Illustration
    {
        public string ImageUri { get; set; }

        public int HorizontalResolution { get; set; }

        public int VerticalResolution { get; set; }
    }
}
