using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace Ardalis.Specification.EFCore.Tests
{
    public class SpecificationEvaluatorGetQuery
    {
        [Fact]
        public void ReturnsQueryWithFilterApplied()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            optionsBuilder.UseInMemoryDatabase("Testcontext");

            using (var context = new TestDbContext(optionsBuilder.Options))
            {
                var user1 = new TwitchUser { Id = 1, Name = "ardalis" };
                var user2 = new TwitchUser { Id = 2, Name = "panjkov", DateFollowed = DateTime.Today };
                context.TwitchUsers.Add(user1);
                context.TwitchUsers.Add(user2);
                context.SaveChanges();

                var spec = new CurrentTwitchFollowersSpec();
                var query = SpecificationEvaluator<TwitchUser>
                    .GetQuery(context.TwitchUsers.AsQueryable(), spec);

                var result = query.ToList();

                Assert.Single(result);
                Assert.Equal(2, result.First().Id);

            }

        }
    }
}
