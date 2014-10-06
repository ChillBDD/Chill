namespace Chill.Examples.Tests.TestSubjects
{
    public class View
    {
        public object Model { get; set; }

        public View(object model)
        {
            Model = model;
        }
    }
}