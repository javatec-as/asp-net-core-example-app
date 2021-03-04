using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XUnitTestProject
{
    /// <summary>
    /// Example test for testing Jenkins and xUnit
    /// </summary>
    public class ElementsTests
    {
        [Theory]
        [MemberData(nameof(GenerateTestData), "~element3~")]
        public void Test(Example example, string idInTest)
        {
            var elementToFind = example.Children
                .SelectMany(children => children.Elements)
                .FirstOrDefault(element => element.Id.Equals(idInTest));

            Assert.NotNull(elementToFind);
            Assert.Equal(idInTest, elementToFind.Id);
        }

        public static TheoryData<Example, string> GenerateTestData(string idInTest) => new()
        {
            {
                new Example()
                {
                    Children = new List<Child>
                    {
                        new()
                        {
                            Id = "~child1~", Elements = new List<Element>
                            {
                                new() {Id = "~element1~"},
                                new() {Id = "~element2~"}
                            }
                        },
                        new()
                        {
                            Id = "~child2~", Elements = new List<Element>
                            {
                                new() {Id = idInTest},
                                new() {Id = "~element4~"}
                            }
                        }
                    }
                },
                idInTest
            }
        };
    }

    public class Example
    {
        public List<Child> Children { get; set; }
    }

    public class Child
    {
        public string Id { get; set; }
        public List<Element> Elements { get; set; }
    }

    public class Element
    {
        public string Id { get; set; }
    }
}