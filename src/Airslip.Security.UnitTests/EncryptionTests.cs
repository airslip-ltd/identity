using FluentAssertions;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace Airslip.Security.UnitTests
{
    public class EncryptionTests
    {
        private readonly ITestOutputHelper _output;

        public EncryptionTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void Can_generate_sha256_string()
        {
            Stopwatch timer = Stopwatch.StartNew();

            string encryptionKey = Cryptography.GenerateSHA256String("test@email.com");

            encryptionKey.Should().Be("73062D872926C2A556F17B36F50E328DDF9BFF9D403939BD14B6C3B7F5A33FC2");
            _output.WriteLine(timer.ElapsedMilliseconds.ToString());
        }
    }
}