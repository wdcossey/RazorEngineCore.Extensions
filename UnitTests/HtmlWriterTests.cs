using Moq;
using NUnit.Framework;
using RazorEngineCore.Writers.Interfaces;

namespace UnitTests
{
    public class HtmlWriterTests
    {
        private Mock<IHtmlWriter> _mockHtmlWriter;

        [SetUp]
        public void Setup()
        {
            _mockHtmlWriter = new Mock<IHtmlWriter>();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}