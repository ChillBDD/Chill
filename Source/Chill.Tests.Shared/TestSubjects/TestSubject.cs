namespace Chill.Tests.TestSubjects
{
    public class TestSubject
    {
        private readonly ITestService _testService;

        public TestSubject(ITestService testService)
        {
            _testService = testService;
        }

        public bool DoSomething()
        {
            return _testService.TryMe();
        }

        public ITestService TestService
        {
            get { return _testService; }
        }
    }
}