using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace Ardalis.Specification.EFCore.Tests
{
    public class TwitchUser : BaseEntity
    {
        public string Name { get; set; }
        public DateTime? DateFollowed { get; set; }
    }
}
