using AutomatedEducationProgram.Areas.Data;

namespace AutomatedEducationProgram.Models
{
    public class Following
    {
        /// <summary>
        /// This entity's unique, auto-generated identifier
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The user in this relationship that is following the user 'Followed'
        /// </summary>
        public string Follower { get; set; }
        /// <summary>
        /// The user in this relationship that is followed by the user 'Follower'
        /// </summary>
        public string Followed { get; set; }
        /// <summary>
        /// True if this following has yet to be approved by the user 'Followed', false otherwise.
        /// </summary>
        public bool Pending { get; set; }
    }
}
