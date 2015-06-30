using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.Web.Models
{
    public class TeamModel
    {
        public string Name { get; set; }
        public IEnumerable<UserModel> Members { get; set; }
        public int Id { get; set; }
    }
}