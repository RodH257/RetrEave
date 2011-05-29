using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Retreave.Domain.Models;

namespace Retreave.Models
{
    public class UserProfileViewModel
    {
        public RegisteredUser User { get; set; }
        public IEnumerable<RetreaveIndex> Indexes { get; set; }
    }
}