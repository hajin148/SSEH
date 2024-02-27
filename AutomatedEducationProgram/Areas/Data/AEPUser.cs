using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;

namespace AutomatedEducationProgram.Areas.Data
{
    /// <summary>
    /// This class represents a user in the Automated Education Program system
    /// </summary>
    [Index(nameof(AEPUser.UserID), IsUnique = true)]
    public class AEPUser : IdentityUser
    {
        /// <summary>
        /// This is the name that the user has chosen to go by in the system. Not to be confused with the ID field, which is 
        /// an auto-generated string provided by the Identity system
        /// </summary>
        public string UserID {  get; set; }
        /// <summary>
        /// The user's declared major
        /// </summary>
        public string Major {  get; set; }
        /// <summary>
        /// The user's description of their interests, the kinds of notes they share on their profile, etc.
        /// </summary>
        public string SelfDescription {  get; set; }
        public List<AEPUser> followees { get; set; }
        public List<AEPUser> followers { get; set; }
    }
}
