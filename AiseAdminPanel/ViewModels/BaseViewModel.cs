using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;


namespace BasketWebPanel.ViewModels
{
    public abstract class BaseViewModel
    {
        public string UserName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }
        public Utility.RoleTypes Role { get; set; }

        public void SetSharedData(IPrincipal User)
        {
            var claimIdentity = ((ClaimsIdentity)User.Identity);
            var fullName = claimIdentity.Claims.FirstOrDefault(x => x.Type == "FullName");
            //var profilePictureUrl = claimIdentity.Claims.FirstOrDefault(x => x.Type == "ProfilePictureUrl");
            UserName = fullName == null ? "John Doe" : fullName.Value;
            Email = claimIdentity.Name;
            //ProfilePictureUrl = string.IsNullOrEmpty(profilePictureUrl.Value) ? "~/Content/images/img.jpg" : profilePictureUrl.Value;
            UserId = Convert.ToInt32(claimIdentity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

            Role=(Utility.RoleTypes)Enum.Parse(typeof(Utility.RoleTypes), claimIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value);
            //StoreId = Convert.ToInt32(claimIdentity.Claims.FirstOrDefault(x => x.Type == "StoreId").Value);
        }
    }
}