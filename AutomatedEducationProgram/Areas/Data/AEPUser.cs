using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Bcpg;

namespace AutomatedEducationProgram.Areas.Data
{
    /// <summary>
    /// This class represents a user in the Automated Education Program system
    /// </summary>
    public class AEPUser : IdentityUser
    {
        /// <summary>
        /// This is the name that the user has chosen to go by in the system
        /// </summary>
        public string UserID {  get; set; }
        public string Major {  get; set; }
        /// <summary>
        /// The user's description of their interests, the kinds of notes they share on their profile, etc.
        /// </summary>
        public string SelfDescription {  get; set; }
        public List<AEPUser> followees { get; set; }
        public List<AEPUser> followers { get; set; }
    }
}
