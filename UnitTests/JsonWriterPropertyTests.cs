using Moq;
using NUnit.Framework;
using RazorEngineCore;
using RazorEngineCore.Writers.Interfaces;

namespace UnitTests
{
    // Useless bunch of tests!
    public class JsonWriterPropertyTests
    {
        private RazorEngineCorePageModel _pageModel;
        private Mock<IJsonWriter> _mockJsonWriter;

        [SetUp]
        public void Setup()
        {
            _mockJsonWriter = new Mock<IJsonWriter>();

            _pageModel = new RazorEngineCorePageModel
            {
                Json = _mockJsonWriter.Object
            };
        }

        [Test]
        public void DefaultBufferSize_TimesOnce()
        {
            int? defaultBufferSize = null;
            
            _mockJsonWriter.Setup(writer => writer.DefaultBufferSize(It.IsAny<int>())).Returns(_mockJsonWriter.Object).Callback((int value) =>
            {
                defaultBufferSize = value;
            }).Verifiable();
            
            _pageModel.Json.DefaultBufferSize(16 * 1024);
            
            _mockJsonWriter.Verify(writer => writer.DefaultBufferSize(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(16 * 1024, defaultBufferSize);
        }

        [Test]
        public void MaxDepth_TimesOnce()
        {
            int? maxDepth = null;

            _mockJsonWriter.Setup(writer => writer.MaxDepth(It.IsAny<int>())).Returns(_mockJsonWriter.Object).Callback((int value) =>
            {
                maxDepth = value;
            }).Verifiable();
            
            _pageModel.Json.MaxDepth(64);
            
            _mockJsonWriter.Verify(writer => writer.MaxDepth(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(64, maxDepth);
        }

        [Test]
        public void WriteIndented_TimesOnce()
        {
            bool? writeIndented = null;

            _mockJsonWriter.Setup(writer => writer.WriteIndented(It.IsAny<bool>())).Returns(_mockJsonWriter.Object).Callback((bool value) =>
            {
                writeIndented = value;
            }).Verifiable();

            _pageModel.Json.WriteIndented(true);
            
            _mockJsonWriter.Verify(writer => writer.WriteIndented(It.IsAny<bool>()), Times.Once);
            Assert.AreEqual(true, writeIndented);

        }

        [Test]
        public void AllowTrailingCommas_TimesOnce()
        {
            bool? allowTrailingCommas = null;

            _mockJsonWriter.Setup(writer => writer.AllowTrailingCommas(It.IsAny<bool>())).Returns(_mockJsonWriter.Object).Callback((bool value) =>
            {
                allowTrailingCommas = value;
            }).Verifiable();

            _pageModel.Json.AllowTrailingCommas(true);
            
            _mockJsonWriter.Verify(writer => writer.AllowTrailingCommas(It.IsAny<bool>()), Times.Once);
            Assert.AreEqual(true, allowTrailingCommas);
        }

        [Test]
        public void IgnoreNullValues_TimesOnce()
        {
            bool? ignoreNullValues = null;

            _mockJsonWriter.Setup(writer => writer.IgnoreNullValues(It.IsAny<bool>())).Returns(_mockJsonWriter.Object).Callback((bool value) =>
            {
                ignoreNullValues = value;
            }).Verifiable();
            
            _pageModel.Json.IgnoreNullValues(true);
            
            _mockJsonWriter.Verify(writer => writer.IgnoreNullValues(It.IsAny<bool>()), Times.Once);
            Assert.AreEqual(true, ignoreNullValues);
        }

        [Test]
        public void IgnoreReadOnlyProperties_TimesOnce()
        {
            bool? ignoreReadOnlyProperties = null;

            _mockJsonWriter.Setup(writer => writer.IgnoreReadOnlyProperties(It.IsAny<bool>())).Returns(_mockJsonWriter.Object).Callback((bool value) =>
            {
                ignoreReadOnlyProperties = value;
            }).Verifiable();

            _pageModel.Json.IgnoreReadOnlyProperties(true);
            
            _mockJsonWriter.Verify(writer => writer.IgnoreReadOnlyProperties(It.IsAny<bool>()), Times.Once);
            Assert.AreEqual(true, ignoreReadOnlyProperties);
        }

        [Test]
        public void PropertyNameCaseInsensitive_TimesOnce()
        {
            bool? propertyNameCaseInsensitive = null;

            _mockJsonWriter.Setup(writer => writer.PropertyNameCaseInsensitive(It.IsAny<bool>())).Returns(_mockJsonWriter.Object).Callback((bool value) =>
            {
                propertyNameCaseInsensitive = value;
            }).Verifiable();

            _pageModel.Json.PropertyNameCaseInsensitive(true);
            
            _mockJsonWriter.Verify(writer => writer.PropertyNameCaseInsensitive(It.IsAny<bool>()), Times.Once);
            Assert.AreEqual(true, propertyNameCaseInsensitive);
        }
    }
}