using System.Collections.Generic;

namespace TaskManager.Web.Models.Teams
{
    public class TeamListModel
    {
        public IEnumerable<TeamModel> Teams { get; set; }
        public AddTeamModel NewTeam { get; set; }
    }
}