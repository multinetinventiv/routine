using NUnit.Framework;
using Routine.Core.Cache;

namespace Routine.Test.Core.Cache
{
    [TestFixture]
    public class DictionaryCacheTest
    {
        private DictionaryCache testing;

        [SetUp]
        public void SetUp()
        {
            testing = new DictionaryCache();
        }

        [Test]
        public void Indexer__returns_item_with_given_key()
        {
            var expected = new object();

            testing.Add("test", expected);

            var actual = testing["test"];

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void Indexer__returns_null_when_item_does_not_exist()
        {
            var actual = testing["test"];

            Assert.IsNull(actual);
        }

        [Test]
        public void ContainsKey__returns_true_when_item_exists__false_when_it_does_not()
        {
            testing.Add("exists", new object());

            Assert.IsTrue(testing.Contains("exists"));
            Assert.IsFalse(testing.Contains("doesn't exist"));
        }

        [Test]
        public void Add__adds_given_item_to_key()
        {
            var expected = new object();

            testing.Add("test", expected);

            Assert.AreEqual(expected, testing["test"]);
        }

        [Test]
        public void Add__overrides_existing_item_with_the_new_one()
        {
            var old = new object();
            var @new = new object();

            testing.Add("test", old);
            testing.Add("test", @new);

            Assert.AreEqual(@new, testing["test"]);
        }

        [Test]
        public void Remove__removes_item_at_key()
        {
            testing.Add("test", new object());

            testing.Remove("test");

            Assert.IsFalse(testing.Contains("test"));
        }

        [Test]
        public void Remove_does_not_do_anything_when_non_existing_key_is_removed()
        {
            Assert.DoesNotThrow(() => testing.Remove("non existing"));
        }
    }
}
