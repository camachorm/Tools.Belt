using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Tools.Belt.Common.Extensions;
using Tools.Belt.Common.Tests.xUnit;
using Xunit;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class NewtonsoftExtensionTests : xUnitTestBase<NewtonsoftExtensionTests>
    {
        public NewtonsoftExtensionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void ToJObjectWithNullSourceFails()
        {
            Assert.Throws<ArgumentNullException>(() => NewtonsoftExtensions.ToJObject(null));
        }

        [Fact]
        public void ToJObjectWithValidSourceSucceeds()
        {
            var sourceJToken = JToken.Parse("{id:1}");
            Assert.IsType<JObject>(sourceJToken.ToJObject());
        }

        [Fact]
        public void ToJObjectWithGenericNullSourceFails()
        {
            Assert.Throws<ArgumentNullException>(() => NewtonsoftExtensions.ToJObject<string>(null));
        }

        [Fact]
        public void ToJObjectWithGenericValidSourceSucceeds()
        {
            var sourceJToken = JToken.Parse("{id:1}");
            Assert.IsType<JObject>(sourceJToken.ToJObject<JToken>());
        }

        [Fact]
        public void ReadJsonStringToObjectWithNullSourceFails()
        {
            Assert.Throws<ArgumentNullException>(() => NewtonsoftExtensions.ReadJsonStringToObject<string>(null));
        }

        [Fact]
        public void ReadJsonStringToObjectWithValidSourceSucceeds()
        {
            var expectedResult = JToken.Parse("{id:1}");
            var sourceString = "{id:1}";

            Assert.Equal(expectedResult, sourceString.ReadJsonStringToObject<JToken>());
            Assert.Throws<ArgumentNullException>(() => NewtonsoftExtensions.ReadJsonStringToObject<string>(null));
        }

        [Fact]
        public async void ToJObjectAsyncSucceeds()
        {
            var testJObject = new JObject();
            var result = await testJObject.ToJObjectAsync();

            Assert.IsType<JObject>(result);
        }

        [Fact]
        public void ToJsonSucceeds()
        {
            var t1 = JToken.Parse("{\"id\":234}");
            Assert.Throws<ArgumentNullException>(() => NewtonsoftExtensions.ToJson(null));
            Assert.IsType<string>(t1.ToJson());
        }

        [Fact]
        public void ToJsonWithObjectSucceeds()
        {
            var t1 = new
            {
                id = 1,
                name = "Oceanmind"
            };
            Assert.Throws<ArgumentNullException>(() => NewtonsoftExtensions.ToJson<dynamic>(null));
            Assert.IsType<string>(t1.ToJson());
        }

        [Fact]
        public void ToBooleanSucceeds()
        {
            var source = JToken.Parse("true");

            Assert.IsType<bool>(source.ToBoolean());
            Assert.Throws<ArgumentNullException>(() => NewtonsoftExtensions.ToBoolean(null));
        }

        [Fact]
        public void ToDateTimeSucceeds()
        {
            var source = JToken.Parse("\"1970-01-01T02:00:00.000Z\"");

            Assert.IsType<DateTime>(source.ToDateTime());
            Assert.Throws<ArgumentNullException>(() => NewtonsoftExtensions.ToDateTime(null));
        }

        [Fact]
        public void ChildTokenSucceeds()
        {
            var sourceObject = new
            {
                id = 1,
                name = "Parent",
                child = new
                {
                    id = 2,
                    name = "Child"
                }
            };

            var childObject = new
            {
                id = 2,
                name = "Child"
            };

            var sourceJToken = sourceObject.ToJToken();
            var childJToken = childObject.ToJToken();

            Assert.Equal(childJToken, sourceJToken.ChildToken("child"));
        }

        [Fact]
        public void ReadNameWithObjectSucceeds()
        {
            var sourceObject = new
            {
                Name = "Parent"
            };
            var sourceJToken = sourceObject.ToJToken();
            Assert.Equal("Name", sourceJToken.ReadName());
        }

        [Fact]
        public void ReadNameWithIntSucceeds()
        {
            var json = @"{testValue : 1234}";
            var obj = JObject.Parse(json);
            var token = obj["testValue"];
            Assert.Null(token.ReadName());
        }

        [Fact]
        public void ReadNameWithNullFails()
        {
            Assert.Throws<ArgumentNullException>(() => NewtonsoftExtensions.ReadName(null));
        }

        [Fact]
        public void AddPropertiesWithNullSourceFails()
        {
            Assert.Throws<ArgumentNullException>(() => NewtonsoftExtensions.AddProperties(null));
        }

        [Fact]
        public void AddPropertiesSucceeds()
        {
            var o = new JObject().AddProperties(
                new KeyValuePair<string, JToken>("Status", "Success"));
            Assert.Equal("Success", o["Status"]);
        }

        [Fact]
        public void FindTokenSucceeds()
        {
            var sourceObject = new
            {
                id = 1,
                name = "Parent",
                child = new
                {
                    id = 2,
                    name = "Child",
                    grandChild = new
                    {
                        name = "Grandchild",
                        age = 18
                    }
                }
            };


            var childObject = new
            {
                id = 2,
                name = "Child",
                grandChild = new
                {
                    name = "Grandchild",
                    age = 18
                }
            };


            var grandChildObject = new
            {
                name = "Grandchild",
                age = 18
            };

            var sourceJToken = sourceObject.ToJToken();
            var childJToken = childObject.ToJToken();
            var grandChildJToken = grandChildObject.ToJToken();

            Assert.Equal(childJToken, sourceJToken.FindToken("child"));
            Assert.Throws<ArgumentNullException>(() => NewtonsoftExtensions.FindToken(null, ""));

            Assert.Equal(grandChildJToken, sourceJToken.FindToken("grandChild")); //Tests recursively
        }

        [Fact]
        public void FindTokensSucceeds()
        {
            var o = JObject.Parse(@"{
                  'Stores': [
                    'Lambton Quay',
                    'Willis Street'
                  ],
                  'Manufacturers': [
                    {
                      'Name': 'Acme Co',
                      'Products': [
                        {
                          'Name': 'Anvil',
                          'Price': 50
                        }
                      ]
                    },
                    {
                      'Name': 'Contoso',
                      'Products': [
                        {
                          'Name': 'Elbow Grease',
                          'Price': 99.95
                        },
                        {
                          'Name': 'Headlight Fluid',
                          'Price': 4
                        }
                      ]
                    }
                  ]
                }");

            Assert.Equal(5,o.FindTokens("Name").Count());
        }

        [Fact]
        public void FindTokensWithMatchingNamesSucceeds()
        {

            var guid1 = Guid.NewGuid().ToString();
            var guid2 = Guid.NewGuid().ToString();

            string json = @"{
                    CPU: 'Intel',
                    Drives: [
                        {name: '" + guid1 + @"'},
                        {name: '" + guid2 + @"'} 
                    ]
                   }";
            
            JObject o = JObject.Parse(json);
            
            var drivesList = o.FindTokens("name").ToList();

            Assert.Contains(guid1, drivesList[0].SafeToString());
            Assert.Contains(guid2, drivesList[1].SafeToString());
            
        }
    }
}