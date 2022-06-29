using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Tools.Belt.Common.Extensions;
using Xunit;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class ObjectExtensionTests
    {
        [Fact]
        public void SerializeSucceeds()
        {
            var baseObject = new MyClass
            {
                Name = "Oceanmind",
                Country = "UK"
            };

            Assert.IsType<byte[]>(baseObject.Serialize());
            byte[] nullObject = null;
            Assert.Throws<ArgumentNullException>(() => nullObject.Serialize());
        }

        [Fact]
        public void DeserializeByteArraySucceeds()
        {
            var baseObject = new MyClass
            {
                Name = "Oceanmind",
                Country = "UK"
            };
            var serializedClass = baseObject.Serialize();
            Assert.IsType<MyClass>(serializedClass.Deserialize<MyClass>());
            Stream nullObject = null;
            Assert.Throws<ArgumentNullException>(() => nullObject.Deserialize<MyClass>());
        }

        [Fact]
        public void DeserializeStreamSucceeds()
        {
            var baseObject = new MyClass
            {
                Name = "Oceanmind",
                Country = "UK"
            };

            Stream serializedClass = objToStream(baseObject);
            Assert.IsType<MyClass>(serializedClass.Deserialize<MyClass>());
            byte[] nullObject = null;
            Assert.Throws<ArgumentNullException>(() => nullObject.Deserialize<MyClass>());
        }

        private MemoryStream objToStream(object o)
        {
            var stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, o);
            return stream;
        }

        [Serializable]
        private class MyClass
        {
            public string Name { get; set; }
            public string Country { get; set; }
        }
    }
}