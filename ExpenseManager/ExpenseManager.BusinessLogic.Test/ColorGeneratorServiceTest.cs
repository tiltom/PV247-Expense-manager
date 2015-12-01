using NUnit.Framework;

namespace ExpenseManager.BusinessLogic.Test
{
    [TestFixture]
    internal class ColorGeneratorServiceTest
    {
        [Test]
        public void ValidateModel_ColorHave7Char_Color()
        {
            var colorGeneratorService = new ColorGeneratorService();
            var result = colorGeneratorService.GenerateColor();
            Assert.AreEqual(7, result.Length, "Expected to have length of 7 chars (#RRGGBB)");
        }

        [Test]
        public void ValidateModel_ColorStartsWithSharp_Color()
        {
            var colorGeneratorService = new ColorGeneratorService();
            var result = colorGeneratorService.GenerateColor();
            Assert.AreEqual(result[0], '#', "Expected to start with sharp (#RRGGBB)");
        }

        [Test]
        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(-1000)]
        public void ValidateModel_NegativeIndex_ReturnTheSameColorAsForPositiveIndex(int index)
        {
            var colorGeneratorService = new ColorGeneratorService();
            Assert.AreEqual(colorGeneratorService.GenerateColorForIndex(index),
                colorGeneratorService.GenerateColorForIndex(index), "Expected to generate same color.");
        }

        [Test]
        public void ValidateModel_NewColor_ReturnNewColorWhenCalledMultipleTimes()
        {
            var colorGeneratorService = new ColorGeneratorService();
            var firstColor = colorGeneratorService.GenerateColor();
            var secondColor = colorGeneratorService.GenerateColor();
            Assert.AreNotEqual(firstColor, secondColor, "Expected to generate different color");
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(1)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public void ValidateModel_SameIndexSameColor_ReturnSameColorForSameIndex(int index)
        {
            var colorGeneratorService = new ColorGeneratorService();
            Assert.AreEqual(colorGeneratorService.GenerateColorForIndex(index),
                colorGeneratorService.GenerateColorForIndex(index), "Expected to generate same color for same index");
        }
    }
}