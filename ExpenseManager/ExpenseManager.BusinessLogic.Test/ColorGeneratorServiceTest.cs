using ExpenseManager.BusinessLogic.CommonServices;
using NUnit.Framework;

namespace ExpenseManager.BusinessLogic.Test
{
    [TestFixture]
    internal class ColorGeneratorServiceTest
    {
        [Test]
        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(-1000)]
        public void ValidateModel_NegativeIndex_ReturnTheSameColorAsForPositiveIndex(int index)
        {
            var colorGeneratorService = new ColorGeneratorService();
            Assert.AreEqual(colorGeneratorService.GenerateColorForIndex(index),
                colorGeneratorService.GenerateColorForIndex(-index));
        }
    }
}