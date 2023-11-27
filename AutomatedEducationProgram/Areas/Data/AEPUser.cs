using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Bcpg;

namespace AutomatedEducationProgram.Areas.Data
{
    public class AEPUser : IdentityUser
    {
        public string UserID {  get; set; }
        public string Major {  get; set; }
        public string SelfDescription {  get; set; }
        public List<AEPUser> followees { get; set; }
        public List<AEPUser> followers { get; set; }
    }
}
