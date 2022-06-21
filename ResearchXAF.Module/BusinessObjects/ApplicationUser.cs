using System.ComponentModel;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using ResearchXAF.Module.BusinessObjects.Master;

namespace ResearchXAF.Module.BusinessObjects;

[MapInheritance(MapInheritanceType.ParentTable)]
[DefaultProperty(nameof(UserName))]
public class ApplicationUser : PermissionPolicyUser, IObjectSpaceLink, ISecurityUserWithLoginInfo 
{
    public ApplicationUser(Session session) : base(session) { }

    [Browsable(false)]
    [Aggregated, Association("User-LoginInfo")]
    public XPCollection<ApplicationUserLoginInfo> LoginInfo 
    {
        get { return GetCollection<ApplicationUserLoginInfo>(nameof(LoginInfo)); }
    }

    IEnumerable<ISecurityUserLoginInfo> IOAuthSecurityUser.UserLogins => LoginInfo.OfType<ISecurityUserLoginInfo>();

    IObjectSpace IObjectSpaceLink.ObjectSpace { get; set; }

    ISecurityUserLoginInfo ISecurityUserWithLoginInfo.CreateUserLoginInfo(string loginProviderName, string providerUserKey) 
    {
        ApplicationUserLoginInfo result = ((IObjectSpaceLink)this).ObjectSpace.CreateObject<ApplicationUserLoginInfo>();
        result.LoginProviderName = loginProviderName;
        result.ProviderUserKey = providerUserKey;
        result.User = this;
        return result;
    }

    private Company _Company;
    [Persistent("CompanyOid"), RuleRequiredField(DefaultContexts.Save)]
    [Association("Company-ApplicationUsers")]
    public Company Company
    {
        get { return _Company; }
        set
        {
            //if (value != _Company) _Plant = null;
            SetPropertyValue(nameof(Company), ref _Company, value);
        }
    }

    private Plant _Plant;
    [Persistent("PlantOid"), RuleRequiredField(DefaultContexts.Save)]
    [Association("Plant-ApplicationUsers")]
    public Plant Plant
    {
        get { return _Plant; }
        set
        {
            //if (value != _Plant) _Plant = null;
            SetPropertyValue(nameof(Plant), ref _Plant, value);
        }
    }
}
