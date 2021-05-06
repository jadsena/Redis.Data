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
        private Mock<IConnectionMultiplexer> MockConn { get; }
        private Mock<IDatabase> MockDB { get; }
        private Mock<IServer> MockServer { get; }

        public RedisContextTests()
        {
            MockConn = new Mock<IConnectionMultiplexer>();
            MockDB = new Mock<IDatabase>();
            MockServer = new Mock<IServer>();
            MockConn.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(MockDB.Object);
            MockConn.Setup(m => m.GetServer(It.IsAny<string>(), It.IsAny<object>())).Returns(MockServer.Object);
        }
        [TestMethod()]
        public void GetStringTest()
        {
            //Arrange
            string expected = "Teste";
            MockDB.Setup(m => m.StringGet(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns("Teste");
            RedisContext redisContext = new RedisContext(MockConn.Object);
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
            MockDB.Setup(m => m.StringGet(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns("{\"Teste\":\"Teste\"}");
            RedisContext redisContext = new RedisContext(MockConn.Object);
            //Act
            var actual = redisContext.Get<Test>("variavel");
            //Assert
            Assert.AreEqual(expected.Teste, actual.Teste);
        }
        [TestMethod]
        public void GetObjectNullTest()
        {
            //Arrange
            MockDB.Setup(m => m.StringGet(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns("");
            RedisContext redisContext = new RedisContext(MockConn.Object);
            //Act
            var actual = redisContext.Get<Test>("variavel");
            //Assert
            Assert.IsTrue(actual is null);
        }
        [TestMethod()]
        public async Task GetAsyncStringTest()
        {
            //Arrange
            string expected = "Teste";
            MockDB.Setup(m => m.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(new RedisValue("Teste")));
            RedisContext redisContext = new RedisContext(MockConn.Object);
            //Act
            string actual = await redisContext.GetAsync("variavel");
            //Assert
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public async Task GetAsyncStringEmptyTest()
        {
            //Arrange
            MockDB.Setup(m => m.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(new RedisValue("")));
            RedisContext redisContext = new RedisContext(MockConn.Object);
            //Act
            string actual = await redisContext.GetAsync("variavel");
            //Assert
            Assert.IsTrue(string.IsNullOrWhiteSpace(actual));
        }
        [TestMethod]
        public async Task GetAsyncObjectTest()
        {
            //Arrange
            var expected = JsonSerializer.Deserialize<Test>("{\"Teste\":\"Teste\"}");
            MockDB.Setup(m => m.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(new RedisValue("{\"Teste\":\"Teste\"}")));
            RedisContext redisContext = new RedisContext(MockConn.Object);
            //Act
            var actual = await redisContext.GetAsync<Test>("variavel");
            //Assert
            Assert.AreEqual(expected.Teste, actual.Teste);
        }
        [TestMethod]
        public async Task GetAsyncObjectNullTest()
        {
            //Arrange
            MockDB.Setup(m => m.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(new RedisValue("")));
            RedisContext redisContext = new RedisContext(MockConn.Object);
            //Act
            var actual = await redisContext.GetAsync<Test>("variavel");
            //Assert
            Assert.IsTrue(actual is null);
        }
        [TestMethod]
        public async Task SetAsyncStringTest()
        {
            //Arrange
            Dictionary<string, string> keys = new Dictionary<string, string>();
            var expected = JsonSerializer.Deserialize<Test>("{\"Teste\":\"Teste\"}");
            MockDB.Setup(m => m.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), null, When.Always, CommandFlags.None))
                .Callback<RedisKey, RedisValue, TimeSpan?, When, CommandFlags>((key, valeu, timeSpan, when, commandFlags) =>
                {
                    keys.Add(key, valeu);
                });
            RedisContext redisContext = new RedisContext(MockConn.Object);
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
            MockDB.Setup(m => m.StringSet(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), null, When.Always, CommandFlags.None))
                .Callback<RedisKey, RedisValue, TimeSpan?, When, CommandFlags>((key, valeu, timeSpan, when, commandFlags) =>
                {
                    keys.Add(key, valeu);
                });
            RedisContext redisContext = new RedisContext(MockConn.Object);
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
            MockDB.Setup(m => m.KeyDeleteAsync(It.IsAny<RedisKey>(), CommandFlags.None)).Returns(Task.FromResult(true));
            RedisContext redisContext = new RedisContext(MockConn.Object);
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
            MockDB.Setup(m => m.KeyDelete(It.IsAny<RedisKey>(), CommandFlags.None)).Returns(true);
            RedisContext redisContext = new RedisContext(MockConn.Object);
            //Act
            var actual = redisContext.Delete("Teste");
            //Assert
            Assert.IsTrue(actual);
        }
        public class Test
        {
            public string Teste { get; set; }
        }

        [TestMethod()]
        public void SetDatabaseDefaultDatabaseTest()
        {
            //Arrange
            int expected = -1;
            Dictionary<string, string> keys = new Dictionary<string, string>();
            MockDB.SetupGet(m => m.Database).Returns(-1);
            IRedisContext redisContext = new RedisContext(MockConn.Object);
            //Act
            redisContext.SetDatabase();
            var actual = redisContext.Database.Database;
            //Assert
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void SetDatabaseDiferentDatabaseTest()
        {
            //Arrange
            int expected = 0;
            int _db = -1;
            Dictionary<string, string> keys = new Dictionary<string, string>();
            MockDB.Setup(m => m.Database).Returns(() => { return _db; });
            MockConn.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns<int, object>((db, asyncState) =>
            {
                _db = db;
                return MockDB.Object;
            });
            IRedisContext redisContext = new RedisContext(MockConn.Object);
            //Act
            redisContext.SetDatabase(0);
            var actual = redisContext.Database.Database;
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            //Arrange
            string expected = "Teste";
            IEnumerable<RedisKey> keys = new List<RedisKey> { new RedisKey("Teste") };
            MockServer.Setup(m => m.Keys(It.IsAny<int>(),It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
                .Returns(keys);
            MockDB.Setup(m => m.StringGet(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns("Teste");
            RedisContext redisContext = new RedisContext(MockConn.Object);
            //Act
            var actual = redisContext.GetAll();
            //Assert
            Assert.IsTrue(actual.ContainsKey(expected));
        }

        [TestMethod()]
        public async Task GetAllAsyncTest()
        {
            //Arrange
            string expected = "Teste";
            IEnumerable<RedisKey> keys = new List<RedisKey> { new RedisKey("Teste") };
            MockServer.Setup(m => m.Keys(It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
                .Returns(keys);
            MockDB.Setup(m => m.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).Returns(Task.FromResult(new RedisValue("Teste")));
            RedisContext redisContext = new RedisContext(MockConn.Object);
            //Act
            var actual = await redisContext.GetAllAsync();
            //Assert
            Assert.IsTrue(actual.ContainsKey(expected));
        }
    }
}