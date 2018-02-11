using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TemplateTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void EuropaDateTimeTemplateTest()
        {
            var __inputStr =
                "1088;10.02.2018 8:00:47;AZS37;11.02.2018 2:02:00;Установлено время 11.02.2018 2:02:00; расчитанное время 11.02.2018 1:00:17;18";
            var __searchPattern = "(\\d{2}).(\\d{2}).(\\d{4})\\s(\\d{1,2}):(\\d{2}):(\\d{2})";
            var __outputStr = Regex.Replace(__inputStr, __searchPattern, "$3-$2-$1 $4:$5:$6");
            Assert.AreEqual(__outputStr, "1088;2018-02-10 8:00:47;AZS37;2018-02-11 2:02:00;Установлено время 2018-02-11 2:02:00; расчитанное время 2018-02-11 1:00:17;18");

            __searchPattern = "(\\d{4}-\\d{2}-\\d{2})\\s(\\d{1}):(\\d{2}:\\d{2})";
            __outputStr = Regex.Replace(__outputStr, __searchPattern, "$1 0$2:$3");
            Assert.AreEqual(__outputStr, "1088;2018-02-10 08:00:47;AZS37;2018-02-11 02:02:00;Установлено время 2018-02-11 02:02:00; расчитанное время 2018-02-11 01:00:17;18");
        }
    }
}
