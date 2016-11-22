using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Chill.Tests.Shared
{
    namespace AsyncTestSpecs
    {
        public class When_there_is_no_subject_and_no_result : TestBase
        {
            private readonly AsyncTestContext test = new AsyncTestContext();
            private BlockingCollection<int> sideEffects;

            public When_there_is_no_subject_and_no_result()
            {
                test.Given(async () =>
                {
                    sideEffects = new BlockingCollection<int>();

#if NET45
                    await Task.Delay(10);
#else
                    await TaskEx.Delay(10);
#endif
                });

                test.When(async () =>
                {
                    foreach (int key in Enumerable.Range(0, 1000))
                    {
#if NET45
                        await Task.Delay(10);
#else
                        await TaskEx.Delay(10);                            
#endif
                        sideEffects.Add(key);
                    }
                });
            }

            [Fact]
            public async void Then_it_should_execute_asynchronously()
            {
                await test.ExecuteGiven();
                await test.ExecuteWhen();
                sideEffects.Should().HaveCount(1000);
            }
        }

        public class When_there_is_result_but_no_subject : TestBase
        {
            private readonly AsyncTestContext<string> test = new AsyncTestContext<string>();
            private BlockingCollection<int> sideEffects;

            public When_there_is_result_but_no_subject()
            {
                test.Given(async () =>
                {
                    sideEffects = new BlockingCollection<int>();

#if NET45
                    await Task.Delay(10);
#else
                    await TaskEx.Delay(10);
#endif
                });

                test.When(async () =>
                {
                    foreach (int key in Enumerable.Range(0, 1000))
                    {
#if NET45
                        await Task.Delay(10);
#else
                        await TaskEx.Delay(10);
#endif
                        sideEffects.Add(key);
                    }

                    return "DONE";
                });
            }

            [Fact]
            public async void Then_it_should_execute_asynchronously()
            {
                await test.ExecuteGiven();
                await test.ExecuteWhen();
                sideEffects.Should().HaveCount(1000);
            }

            [Fact]
            public async void Then_it_should_return_result()
            {
                await test.ExecuteGiven();
                string result = await test.ExecuteWhen();
                result.Should().Be("DONE");
            }
        }

        public class When_there_is_subject_but_no_result : TestBase
        {
            private readonly AsyncTestContextWithSubject<Subject> test;
            private BlockingCollection<int> sideEffects;
            private Subject oldSubject1;
            private Subject oldSubject2;
            private Subject whenSubject;

            public When_there_is_subject_but_no_result()
            {
                test = new AsyncTestContextWithSubject<Subject>(this);

                test.Given(async getOldSubject =>
                {
                    oldSubject1 = getOldSubject();
                    sideEffects = new BlockingCollection<int>();

#if NET45
                    await Task.Delay(10);
#else
                    await TaskEx.Delay(10);
#endif

                    return () => new Subject("SUBJ1");
                });

                test.Given(getOldSubject =>
                {
                    oldSubject2 = getOldSubject();
                    return () => new Subject("SUBJ2");
                });

                test.When(async subject =>
                {
                    whenSubject = subject;

                    foreach (int key in Enumerable.Range(0, 1000))
                    {
#if NET45
                        await Task.Delay(10);
#else
                        await TaskEx.Delay(10);
#endif
                        sideEffects.Add(key);
                    }
                });
            }

            [Fact]
            public async void Then_it_should_execute_asynchronously()
            {
                await test.ExecuteGiven();
                await test.ExecuteWhen();

                sideEffects.Should().HaveCount(1000);
            }

            [Fact]
            public async void Then_it_should_have_subject_from_the_last_given()
            {
                Subject subject = await test.ExecuteGiven();
                await test.ExecuteWhen();

                subject.Should().NotBeNull();
                subject.Value.Should().Be("SUBJ2");
            }

            [Fact]
            public async void Then_the_first_given_should_receive_subject_factory_building_default_subject()
            {
                await test.ExecuteGiven();
                await test.ExecuteWhen();

                oldSubject1.Should().NotBeNull();
                oldSubject1.Value.Should().Be(string.Empty);
            }

            [Fact]
            public async void Then_the_second_given_should_receive_subject_factory_from_the_first_given()
            {
                await test.ExecuteGiven();
                await test.ExecuteWhen();

                oldSubject2.Should().NotBeNull();
                oldSubject2.Value.Should().Be("SUBJ1");
            }

            [Fact]
            public async void Then_the_when_should_receive_subject_from_the_last_given()
            {
                await test.ExecuteGiven();
                await test.ExecuteWhen();

                whenSubject.Should().NotBeNull();
                whenSubject.Value.Should().Be("SUBJ2");
            }
        }

        public class When_there_is_subject_and_result : TestBase
        {
            private readonly AsyncTestContextWithSubject<Subject, string> test;
            private BlockingCollection<int> sideEffects;
            private Subject oldSubject1;
            private Subject oldSubject2;
            private Subject whenSubject;

            public When_there_is_subject_and_result()
            {
                test = new AsyncTestContextWithSubject<Subject, string>(this);

                test.Given(async getOldSubject =>
                {
                    oldSubject1 = getOldSubject();
                    sideEffects = new BlockingCollection<int>();

#if NET45
                    await Task.Delay(10);
#else
                    await TaskEx.Delay(10);
#endif

                    return () => new Subject("SUBJ1");
                });

                test.Given(getOldSubject =>
                {
                    oldSubject2 = getOldSubject();
                    return () => new Subject("SUBJ2");
                });

                test.When(async subject =>
                {
                    whenSubject = subject;

                    foreach (int key in Enumerable.Range(0, 1000))
                    {
#if NET45
                        await Task.Delay(10);
#else
                        await TaskEx.Delay(10);
#endif
                        sideEffects.Add(key);
                    }

                    return "DONE";
                });
            }

            [Fact]
            public async void Then_it_should_execute_asynchronously()
            {
                await test.ExecuteGiven();
                await test.ExecuteWhen();

                sideEffects.Should().HaveCount(1000);
            }

            [Fact]
            public async void Then_it_should_return_result()
            {
                await test.ExecuteGiven();
                string result = await test.ExecuteWhen();
                result.Should().Be("DONE");
            }

            [Fact]
            public async void Then_it_should_have_subject_from_the_last_given()
            {
                Subject subject = await test.ExecuteGiven();
                await test.ExecuteWhen();

                subject.Should().NotBeNull();
                subject.Value.Should().Be("SUBJ2");
            }

            [Fact]
            public async void Then_the_first_given_should_receive_subject_factory_building_default_subject()
            {
                await test.ExecuteGiven();
                await test.ExecuteWhen();

                oldSubject1.Should().NotBeNull();
                oldSubject1.Value.Should().Be(string.Empty);
            }

            [Fact]
            public async void Then_the_second_given_should_receive_subject_factory_from_the_first_given()
            {
                await test.ExecuteGiven();
                await test.ExecuteWhen();

                oldSubject2.Should().NotBeNull();
                oldSubject2.Value.Should().Be("SUBJ1");
            }

            [Fact]
            public async void Then_the_when_should_receive_subject_from_the_last_given()
            {
                await test.ExecuteGiven();
                await test.ExecuteWhen();

                whenSubject.Should().NotBeNull();
                whenSubject.Value.Should().Be("SUBJ2");
            }
        }

        public class Subject
        {
            public Subject()
                : this(string.Empty)
            {

            }

            public Subject(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }
    }
}