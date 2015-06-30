using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.Web.Models
{
    public class TeamListModel
    {
        public IEnumerable<TeamModel> Teams { get; set; }
        public AddTeamModel NewTeam { get; set; }
    }
}