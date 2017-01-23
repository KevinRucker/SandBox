using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimal.Configuration;
using System;

namespace MinimalUnitTests
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void ConfigurationTestMethod1()
        {
            var expectedGuid = Guid.Parse("e4c8a9d4-6105-4e2d-87bf-ece64f64e292");
            var expectedQueue = @".\private$\TestQueue";
            var expectedEncryptedAppSetting = "AppSetting encrypted test value.";
            var expectedTestSetting1 = default(string);
            var expectedTestSetting2 = default(TimeSpan);
            var expectedTestSetting3 = default(string);

            var applicationId = ConfigManager.AppSetting<Guid>("ApplicationId");
            Assert.IsTrue(expectedGuid.Equals(applicationId), "ApplicationId check failed.");
            var queue = ConfigManager.AppSetting<string>("TestingQueue");
            Assert.IsTrue(expectedQueue.Equals(queue), "TestingQueue check failed.");
            var encryptedAppSetting = ConfigManager.AppSetting<string>("EncryptedTestValue", true);
            Assert.IsTrue(expectedEncryptedAppSetting.Equals(encryptedAppSetting), "EncryptedTestValue check failed.");

            var environment = ConfigManager.ConfigValue<string>("Environment");
            var testSetting1 = ConfigManager.ConfigValue<string>("TestSetting1");
            var testSetting2 = ConfigManager.ConfigValue<TimeSpan>("TestSetting2");
            var testSetting3 = ConfigManager.ConfigValue<string>("TestSetting3");

            switch (environment)
            {
                case "Local":
                    expectedTestSetting1 = "Local setting 1";
                    expectedTestSetting2 = TimeSpan.Parse("00:00:05");
                    expectedTestSetting3 = "Local - Now is the time for all good men to come to the aid of their country. 0123456789";
                    break;
                case "Development":
                    expectedTestSetting1 = "Development setting 1";
                    expectedTestSetting2 = TimeSpan.Parse("00:00:10");
                    expectedTestSetting3 = "Development - Now is the time for all good men to come to the aid of their country. 0123456789";
                    break;
                case "Test":
                    expectedTestSetting1 = "Test setting 1";
                    expectedTestSetting2 = TimeSpan.Parse("00:00:15");
                    expectedTestSetting3 = "Test - Now is the time for all good men to come to the aid of their country. 0123456789";
                    break;
                case "Production":
                    expectedTestSetting1 = "Production setting 1";
                    expectedTestSetting2 = TimeSpan.Parse("00:00:20");
                    expectedTestSetting3 = "Production - Now is the time for all good men to come to the aid of their country. 0123456789";
                    break;
            }

            Assert.IsTrue(expectedTestSetting1.Equals(testSetting1), "TestSetting1 chack failed.");
            Assert.IsTrue(expectedTestSetting2.Equals(testSetting2), "TestSetting2 chack failed.");
            Assert.IsTrue(expectedTestSetting3.Equals(testSetting3), "TestSetting3 chack failed.");
        }
    }
}
