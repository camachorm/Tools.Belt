using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Text;
using Tools.Belt.Common.Extensions;
using Tools.Belt.Common.Tests.xUnit;
using Xunit;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class StringExtensionTests : xUnitTestBase<StringExtensionTests>
    {
        public StringExtensionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void AddUriSegmentAddsForwardSlashIfNeeded()
        {
            var baseURL = "https://www.oceanmind.global";
            Assert.Equal("https://www.oceanmind.global/Media", baseURL.AddUriSegment("Media"));
        }

        [Fact]
        public void AddUriSegmentAddsNoForwardSlashIfNotNeeded()
        {
            var baseURLWithSlash = "https://www.oceanmind.global/";
            Assert.Equal("https://www.oceanmind.global/Media", baseURLWithSlash.AddUriSegment("Media"));
        }

        [Fact]
        public void AddUriSegmentReturnsValidUri()
        {
            var baseURL = "https://www.oceanmind.global/";
            var baseUri = new Uri(baseURL);

            Assert.Equal(baseURL + "Media", baseUri.AddUriSegment("Media"));
        }

        [Fact]
        public void AddUriSegmentWithInvalidSourceFails()
        {
            Assert.Throws<UriFormatException>(() => "thi is not a valid URL".AddUriSegment("Media"));
        }

        [Fact]
        public void AddUriSegmentWithEmptySourceFails()
        {
            Assert.Throws<ArgumentNullException>(() => "".AddUriSegment("Media"));
        }

        [Fact]
        public void AddUriSegmentFailsWithNullSource()
        {
            Assert.Throws<ArgumentNullException>(() => ((Uri) null).AddUriSegment("Media"));
        }

        [Fact]
        public void EndsWithDirectorySeparatorCharFailsOnEmptySource()
        {
            Assert.Throws<ArgumentNullException>(() => "".EndsWithDirectorySeparatorChar());
        }

        [Fact]
        public void EndsWithDirectorySeparatorCharReturnsTrueWhenExists()
        {
            Assert.True("c:\\temp\\OM.Cloud\\".EndsWithDirectorySeparatorChar());
        }

        [Fact]
        public void EndsWithDirectorySeparatorWithoutCharFails()
        {
            Assert.False("c:\\temp\\OM.Cloud".EndsWithDirectorySeparatorChar());
        }

        [Fact]
        public void ToPrintableSecretReturnsSourceWithOneChar()
        {
            Assert.Equal("O", "O".ToPrintableSecret());
        }

        [Fact]
        public void ToPrintableSecretReturnsCensuredSourceWithMoreThanOneChar()
        {
            Assert.Equal("ocea*****mind",
                "oceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmind"
                    .ToPrintableSecret());
        }

        [Fact]
        public void ToPrintableSecretReturnsCensuredSourceWithThreeChars()
        {
            Assert.Equal("o*****a", "ola".ToPrintableSecret());
        }

        [Fact]
        public void ToPrintableSecretReturnsCensuredSourceWithFourChars()
        {
            Assert.Equal("o*****a", "ola".ToPrintableSecret(100));
        }

        [Fact]
        public void ToPrintableSecretReturnsCensuredSourceWithMaxCharCountSet()
        {
            Assert.Equal("ocean*****nmind",
                "oceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmindoceanmind"
                    .ToPrintableSecret(5, 5));
        }

        [Fact]
        public void ToIntReturnsIntWithNumberAsString()
        {
            Assert.Equal(123, "123".ToInt());
        }

        [Fact]
        public void ToIntReturnsNullWithNull()
        {
            Assert.Null(StringExtensions.ToInt(null));
        }

        [Fact]
        public void ToLongReturnsLongWithNumberAsString()
        {
            Assert.Equal(1234, "1234".ToLong());
        }

        [Fact]
        public void ToLongReturnsNullWithNull()
        {
            Assert.Null(StringExtensions.ToLong(null));
        }

        [Fact]
        public void ToDecimalReturnsDecimalWithNumberAsString()
        {
            decimal decimalNumber = 1234.56m;
            string expectedResult = "1234.56";
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Assert.Equal(decimalNumber, expectedResult.ToDecimal());
        }

        [Fact]
        public void ToDecimalReturnsNullWithNull()
        {
            Assert.Null(StringExtensions.ToDecimal(null));
        }

        [Fact]
        public void ToUriWithNullFails()
        {
            Assert.Throws<ArgumentNullException>(() => "".ToUri());
        }

        [Fact]
        public void ToUriWithValidSourceSucceeds()
        {
            Assert.Equal(new Uri("http://www.oceanmind.global"), "http://www.oceanmind.global".ToUri());
        }

        [Fact]
        public void SecureStringWithNullSourceFails()
        {
            Assert.Throws<ArgumentNullException>(() => StringExtensions.ToSecureString(null));
        }

        [Fact]
        public void SecureStringSucceeds()
        {
            Assert.IsType<SecureString>("test".ToSecureString());
        }

        [Fact]
        public void ToGuidSucceeds()
        {
            Assert.IsType<Guid>(new Guid().ToString().ToGuid());
        }

        [Fact]
        public void ToDateTimeSucceeds()
        {
            Assert.IsType<DateTime>(DateTime.Now.ToString(CultureInfo.InvariantCulture).ToDateTime());
        }

        [Fact]
        public void ToDateTimeWithNullSourceFails()
        {
            Assert.Throws<ArgumentNullException>(() => StringExtensions.ToDateTime(null));
        }

        [Fact]
        public void ToDateTimeWithInvalidDateReturnsNull()
        {
            Assert.Null("invalid date".ToDateTime());
        }

        [Fact]
        public void IndentAddsGivenNumberOfTabs()
        {
            Assert.Equal("\t\toceanmind", "oceanmind".Indent(2));
        }

        [Fact]
        public void IndentWithNullSourceFails()
        {
            Assert.Throws<ArgumentNullException>(() => StringExtensions.Indent(null));
        }

        [Fact]
        public void IndentWithNegativeIndentationFails()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => "oceanmind".Indent(-1));
        }

        [Fact]
        public void GetBytesWithNullSourceFails()
        {
            Assert.Throws<ArgumentNullException>(() => StringExtensions.GetBytes(null));
        }

        [Fact]
        public void GetBytesWithNullEncodingSourceFails()
        {
            Assert.Throws<ArgumentNullException>(() => "oceanmind".GetBytes(null));
        }

        [Fact]
        public void GetBytesSucceeds()
        {
            byte[] omBytes = {111, 109};
            Assert.Equal(omBytes, "om".GetBytes(Encoding.UTF8));
        }

        [Fact]
        public void SafeToBooleanSucceeds()
        {
            Assert.True("yes".SafeToBoolean());
        }

        [Fact]
        public void SafeToLongSucceeds()
        {
            Assert.Equal(123, "123".SafeToLong());
        }

        [Fact]
        public void SafeToDecimalSucceeds()
        {
            Assert.Equal((decimal) 123.2, "123,2".SafeToDecimal());
        }

        [Fact]
        public void SafeToDecimalIntSucceeds()
        {
            Assert.Equal(123, "123".SafeToInt());
        }

        [Fact]
        public void FilterOutIConfigurationProviderSucceeds()
        {
            Assert.Equal("oceanmind", ":: oceanmind".FilterOutIConfigurationProvider());
        }

        [Fact]
        public void FilterOutIConfigurationProviderReturnsSourceIfEmpty()
        {
            Assert.Equal("", string.Empty.FilterOutIConfigurationProvider());
        }

        [Fact]
        public void ToNullableBooleanWithNullAgreementsFails()
        {
            Assert.Throws<ArgumentNullException>(() => "oceanmind".ToNullableBoolean(null, null));
        }

        [Fact]
        public void ToNullableBooleanWithNullRejetionFails()
        {
            IList<string> x = new List<string>();
            Assert.Throws<ArgumentNullException>(() => "oceanmind".ToNullableBoolean(x, null));
        }

        [Fact]
        public void ToNullableBooleanWithNulSourceReturnsNull()
        {
            Assert.Null(StringExtensions.ToNullableBoolean(null, null, null));
        }

        [Fact]
        public void ToNullableBooleanWithBoolValueSucceeds()
        {
            IList<string> x = new List<string>();
            Assert.True("true".ToNullableBoolean(x, x));
        }

        [Fact]
        public void ToNullableBooleanWithNonBoolValueReturnsNull()
        {
            IList<string> x = new List<string>();
            Assert.Null("this is not a boolean value".ToNullableBoolean(x, x));
        }

        [Fact]
        public void ToNullableBooleanWithRejectionReturnsFalse()
        {
            IList<string> x = new List<string>();
            IList<string> rejecetionList = new List<string>();
            rejecetionList.Add("ocean");
            rejecetionList.Add("phishery");

            Assert.False("ocean".ToNullableBoolean(x, rejecetionList));
        }

        [Fact]
        public void ReadKeyValueFromStringRetursnKeyValuePair()
        {
            const string sKVPair = "name=oceanmind";
            var kvPair = new KeyValuePair<string, string>("name", "oceanmind");
            Assert.Equal(kvPair, sKVPair.ReadKeyValueFromString("name"));
        }

        [Fact]
        public void ReadKeyValueFromStringRetursnNullOnEmptySource()
        {
            Assert.Null("".ReadKeyValueFromString("name"));
        }

        [Fact]
        public void ReadKeyValueFromStringRetursnNullWithInvalidKey()
        {
            const string sKVPair = "name=oceanmind;";
            Assert.Null(sKVPair.ReadKeyValueFromString("other"));
        }

        [Fact]
        public void ReadKeyValueFromStringRetursnNullWithInvalidSeparator()
        {
            const string sKVPair = "name=oceanmind;";
            Assert.Null(sKVPair.ReadKeyValueFromString("name", '|'));
        }

        [Fact]
        public void EncryptDecrypt_Normal_StringRetrieved()
        {
            const string key = "mykey12345678901";
            const string IV = "myiv123456789012";
            string guid = Guid.NewGuid().ToString();
            string encrypted = guid.Encrypt(key, IV);
            string decrypted = encrypted.Decrypt(key, IV);

            Assert.Equal(guid, decrypted);
        }

        [Fact]
        public void EncryptDecrypt_DifferentKey_Exception()
        {
            const string keyA = "mykey12345678901";
            const string keyB = "mykey12345678900";
            const string IV = "myiv123456789012";
            string guid = Guid.NewGuid().ToString();
            string encrypted = guid.Encrypt(keyA, IV);

            Assert.Throws<System.Security.Cryptography.CryptographicException>(() => encrypted.Decrypt(keyB, IV));
        }

        [Fact]
        public void EncryptDecrypt_DifferentIV_StringDifferent()
        {
            const string key = "mykey12345678901";
            const string IVa = "myiv123456789012";
            const string IVb = "myiv123456789011";
            string guid = Guid.NewGuid().ToString();
            string encrypted = guid.Encrypt(key, IVa);
            string decrypted = encrypted.Decrypt(key, IVb);

            Assert.NotEqual(guid, decrypted);
        }
    }
}