using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoalQuest.Models
{
    public class Profile
    {
        public required string Name { get; set; }
        public required string DateOfBirth { get; set; }
        public required string Motivation { get; set; }
        public string ImagePath { get; set; } = "default_profile.png";
    }
}
