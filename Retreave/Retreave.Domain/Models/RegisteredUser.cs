using System;
using System.Collections.Generic;
using Retreave.Domain.Enums;

namespace Retreave.Domain.Models
{
    /// <summary>
    /// A registered user
    /// </summary>
    [PetaPoco.TableName("Users")]
    [PetaPoco.PrimaryKey("UserId", autoIncrement = true)]
    public class RegisteredUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int TwitterId { get; set; }

        [PetaPoco.Ignore]
        public TwitterAuthentication AuthDetails { get; set; }
   
    }
}
