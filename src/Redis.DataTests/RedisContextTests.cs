using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Redis.Data;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Redis.Data.Tests
{
    [TestClass()]
    public class RedisContextTests
    {
        [TestMethod()]
        public void GetStringTest()
        {
            //Arrange
            string expected = "Teste";
            var mockConn = new Mock<IConnectionMultiplexer>();
            var mockDB = new Mock<IDatabase>();
            mockDB.Setup(m => m.StringGet(It.IsAny<RedisKey>(),It.IsAny<CommandFlags>())).Returns("Teste");
            mockConn.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDB.Object);
            RedisContext redisContext = new RedisContext(mockConn.Object);
            //Act
            string actual = redisContext.Get("variavel");
            //Assert
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void GetObjectTest()
        {
            //Arrange
            var expected = JsonSerializer.Deserialize<Test>("{\"Teste\":\"Teste\"}");
            var mockConn = new Mock<IConnectionMultiplexer>();
            var mockDB = new Mock<IDatabase>();
            mockDB.Setup(m => m.StringGet(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns("{\"Teste\":\"Teste\"}");
            mockConn.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDB.Object);
            RedisContext redisContext = new RedisContext(mockConn.Object);
            //Act
            var actual = redisContext.Get<Test>("variavel");
            //Assert
            Assert.AreEqual(expected.Teste, actual.Teste);
        }
        [TestMethod()]
        public async Task GetAsyncStringTest()
        {
            //Arrange
            string expected = "Teste";
            var mockConn = new Mock<IConnectionMultiplexer>();
            var mockDB = new Mock<IDatabase>();
            mockDB.Setup(m => m.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(new RedisValue("Teste")));
            mockConn.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDB.Object);
            RedisContext redisContext = new RedisContext(mockConn.Object);
            //Act
            string actual = await redisContext.GetAsync("variavel");
            //Assert
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public async Task GetAsyncObjectTest()
        {
            //Arrange
            var expected = JsonSerializer.Deserialize<Test>("{\"Teste\":\"Teste\"}");
            var mockConn = new Mock<IConnectionMultiplexer>();
            var mockDB = new Mock<IDatabase>();
            mockDB.Setup(m => m.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(new RedisValue("{\"Teste\":\"Teste\"}")));
            mockConn.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDB.Object);
            RedisContext redisContext = new RedisContext(mockConn.Object);
            //Act
            var actual = await redisContext.GetAsync<Test>("variavel");
            //Assert
            Assert.AreEqual(expected.Teste, actual.Teste);
        }
        [TestMethod]
        public async Task SetAsyncStringTest()
        {
            //Arrange
            Dictionary<string, string> keys = new Dictionary<string, string>();
            var expected = JsonSerializer.Deserialize<Test>("{\"Teste\":\"Teste\"}");
            var mockConn = new Mock<IConnectionMultiplexer>();
            var mockDB = new Mock<IDatabase>();
            mockDB.Setup(m => m.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), null, When.Always, CommandFlags.None))
                .Callback<RedisKey, RedisValue, TimeSpan?, When, CommandFlags>((key, valeu, timeSpan, when, commandFlags) =>
                {
                    keys.Add(key, valeu);
                });
            mockConn.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDB.Object);
            RedisContext redisContext = new RedisContext(mockConn.Object);
            //Act
            await redisContext.SetAsync("Teste", "Teste");
            //Assert
            Assert.IsTrue(keys.Count > 0);
        }
        [TestMethod]
        public void SetStringTest()
        {
            //Arrange
            Dictionary<string, string> keys = new Dictionary<string, string>();
            var expected = JsonSerializer.Deserialize<Test>("{\"Teste\":\"Teste\"}");
            var mockConn = new Mock<IConnectionMultiplexer>();
            var mockDB = new Mock<IDatabase>();
            mockDB.Setup(m => m.StringSet(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(),null,When.Always,CommandFlags.None))
                .Callback<RedisKey, RedisValue, TimeSpan?, When, CommandFlags>((key, valeu, timeSpan, when, commandFlags) =>
                {
                    keys.Add(key, valeu);
                });
            mockConn.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDB.Object);
            RedisContext redisContext = new RedisContext(mockConn.Object);
            //Act
            redisContext.Set("Teste", "Teste");
            //Assert
            Assert.IsTrue(keys.Count > 0);
        }
        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            //Arrange
            Dictionary<string, string> keys = new Dictionary<string, string>();
            var mockConn = new Mock<IConnectionMultiplexer>();
            var mockDB = new Mock<IDatabase>();
            mockDB.Setup(m => m.KeyDeleteAsync(It.IsAny<RedisKey>(), CommandFlags.None)).Returns(Task.FromResult(true));
            mockConn.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDB.Object);
            RedisContext redisContext = new RedisContext(mockConn.Object);
            //Act
            var actual = await redisContext.DeleteAsync("Teste");
            //Assert
            Assert.IsTrue(actual);
        }
        [TestMethod]
        public void DeleteTest()
        {
            //Arrange
            Dictionary<string, string> keys = new Dictionary<string, string>();
            var mockConn = new Mock<IConnectionMultiplexer>();
            var mockDB = new Mock<IDatabase>();
            mockDB.Setup(m => m.KeyDelete(It.IsAny<RedisKey>(), CommandFlags.None)).Returns(true);
            mockConn.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDB.Object);
            RedisContext redisContext = new RedisContext(mockConn.Object);
            //Act
            var actual = redisContext.Delete("Teste");
            //Assert
            Assert.IsTrue(actual);
        }
        public class Test
        {
            public string Teste { get; set; }
        }
    }
}