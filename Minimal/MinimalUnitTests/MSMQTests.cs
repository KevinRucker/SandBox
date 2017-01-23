using System;
using System.Configuration;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimal.Network;

namespace MinimalUnitTests
{
    [TestClass]
    public class MSMQTests
    {
        private string _queue = default(string);
        private Encoding encoder = new UTF8Encoding();

        [TestInitialize]
        public void init()
        {
            _queue = ConfigurationManager.AppSettings["TestingQueue"];
        }

        [TestMethod]
        public void MSMQInstalledTest()
        {
            Assert.IsTrue(MSMQ.isMSMQInstalled());
        }

        [TestMethod]
        public void MSMQIsRunningTest()
        {
            Assert.IsTrue(MSMQ.isMSMQServiceRunning());
        }

        [TestMethod]
        public void MSMQQueueExistsTest()
        {
            Assert.IsTrue(MSMQ.QueueExists(_queue), "Failed to detect existing queue.");
            Assert.IsFalse(MSMQ.QueueExists(".\\private$\\MadeUpName"), "Failed to detect absence of nonexistant queue.");
        }

        [TestMethod]
        public void MSMQQueueMessageTest()
        {
            var message = TestDataFactory.GetLoremIpsumString(1000, encoder);
            MSMQ.queueMessage(message, null, _queue);
            var test = MSMQ.readQueue<string>(_queue);
            Assert.AreEqual(message, test);
        }

        [TestMethod]
        public void MSMQReadQueueTest()
        {
            var message = TestDataFactory.GetLoremIpsumString(1000, encoder);
            MSMQ.queueMessage(message, null, _queue);
            var test = MSMQ.readQueue<string>(_queue);
            Assert.AreEqual(message, test);
        }

        [TestMethod]
        public void MSMQReadQueueByIdTest()
        {
            var Id = MSMQ.GetCorrelationId();
            var message = TestDataFactory.GetLoremIpsumString(1000, encoder);
            MSMQ.queueMessage(message, Id, _queue);
            var test = MSMQ.readQueueById<string>(Id, _queue);
            Assert.AreEqual(message, test);
        }

        [TestMethod]
        public void MSMQPeekQueueTest()
        {
            var message = TestDataFactory.GetLoremIpsumString(1000, encoder);
            MSMQ.queueMessage(message, null, _queue);
            var test1 = MSMQ.peekQueue<string>(_queue);
            Assert.AreEqual(message, test1, "peekQueue failed.");
            var test2 = MSMQ.readQueue<string>(_queue);
            Assert.AreEqual(message, test2, "readQueue failed, test message not removed from queue.");
        }

        [TestMethod]
        public void MSMQQueueDepthTest()
        {
            var message = TestDataFactory.GetLoremIpsumString(1000, encoder);
            MSMQ.queueMessage(message, null, _queue);
            var depth = MSMQ.queueDepth(_queue);
            Assert.AreEqual(1u, depth, "queueDepth failed.");
            MSMQ.readQueue<string>(_queue);
            depth = MSMQ.queueDepth(_queue);
            Assert.AreEqual(0u, depth, "Failed to remove test message from queue.");
        }
    }
}
