using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace Ardalis.Specification.EFCore.Tests
{
    public class CurrentTwitchFollowersSpec : BaseSpecification<TwitchUser>
    {
        public CurrentTwitchFollowersSpec() : base(t => t.DateFollowed.HasValue)
        {
        }
    }
}
