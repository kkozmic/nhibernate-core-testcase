using System;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1234
{
    [TestFixture]
    public class SampleTest : BugTestCase
    {
        protected override void OnSetUp()
        {
            base.OnSetUp();
            using (ISession session = this.OpenSession())
            {
                var child1 = new Child
                                 {
                                     Id = 1,
                                     DateOfBirth = new DateTime(2010, 1, 1)
                                 };

                var p1 = new Parent {Id = 1};
                p1.Children.Add(child1);
                var p2 = new Parent {Id = 2};
                session.Save(p1);
                session.Save(p2);
                session.Flush();
            }
        }

        protected override void OnTearDown()
        {
            base.OnTearDown();
            using (ISession session = this.OpenSession())
            {
                string hql = "from System.Object";
                session.Delete(hql);
                session.Flush();
            }
        }

        [Test]
        public void BytePropertyShouldBeRetrievedCorrectly()
        {
            using (ISession session = OpenSession())
            {
                var query = from parent in session.Query<Parent>()
                where parent.Children == null ||
                      parent.Children.Any(c => c.DateOfBirth < new DateTime(2000, 1, 1))
                select parent;
                var result = query.ToList();

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(2, result[0].Id);
            }
        }
    }
}
