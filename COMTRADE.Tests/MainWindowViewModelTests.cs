using COMTRADEInsight.ViewModels;
using System.Numerics;
using Xunit;

namespace COMTRADE.Tests
{
    public class MainWindowViewModelTests
    {
        [Fact]
        public void DiscreteFourierEqual()
        {
            // Arrange
            MainWindowViewModel mainWindowVM = new MainWindowViewModel();

            Complex[] expected =
            [
                new Complex(1.2, 0),
                new Complex(0.1, -0.1),
                new Complex(0.2, 0.1)
            ];

            // Act
            Complex[] result = mainWindowVM.DiscreteFourierTransform([0.5, 0.4, 0.3]).Select(c =>
                new Complex(Math.Round(c.Real, 1), Math.Round(c.Imaginary, 1))
            ).ToArray();

            // Assert
            Assert.Equal(expected.Length, result.Length);

            for (int i = 0; i < result.Length; i++)
            {
                Assert.Equal(expected[i].Real, result[i].Real);
                Assert.Equal(expected[i].Imaginary, result[i].Imaginary);
            }
        }
    }
}
