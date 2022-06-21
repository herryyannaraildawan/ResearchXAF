using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using ResearchXAF.Module.BusinessObjects;
using ResearchXAF.Module.BusinessObjects.Master;

namespace ResearchXAF.Module.DatabaseUpdate;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
public class Updater : ModuleUpdater {
    public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) 
    {
    
    }

    public override void UpdateDatabaseBeforeUpdateSchema()
    {
        base.UpdateDatabaseBeforeUpdateSchema();
        //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
        //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
        //}
    }

    public override void UpdateDatabaseAfterUpdateSchema() 
    {
        base.UpdateDatabaseAfterUpdateSchema();

        CreateCompany();
        CreatePlant();
        CreateDivision();

        CreateUserAdministrator();
        CreateUserCompanyLevel();
        CreateUserPlantLevel();
    }

    

    private void CreateUserAdministrator()
    {
        ApplicationUser userAdmin = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == "Admin");
        if (userAdmin == null)
        {
            userAdmin = ObjectSpace.CreateObject<ApplicationUser>();
            userAdmin.UserName = "Admin";
            // Set a password if the standard authentication type is used
            userAdmin.SetPassword("");

            // The UserLoginInfo object requires a user object Id (Oid).
            // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
            ObjectSpace.CommitChanges(); //This line persists created object(s).
            ((ISecurityUserWithLoginInfo)userAdmin).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(userAdmin));
        }

        // If a role with the Administrators name doesn't exist in the database, create this role
        PermissionPolicyRole adminRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "Administrators");
        if (adminRole == null)
        {
            adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            adminRole.Name = "Administrators";
        }

        adminRole.IsAdministrative = true;
        userAdmin.Roles.Add(adminRole);
        ObjectSpace.CommitChanges(); //This line persists created object(s).
    }

    private void CreateUserCompanyLevel()
    {
        PermissionPolicyRole role = CreateRoleCompanyLevel();

        string username = "";
        ApplicationUser sampleUser;
        Company company;

        company = ObjectSpace.FirstOrDefault<Company>(u => u.Code == "WNLL");

        // add user 
        username = "adm.wnll.01";
        sampleUser = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == username);
        if (sampleUser == null)
        {
            sampleUser = ObjectSpace.CreateObject<ApplicationUser>();
            sampleUser.UserName = username;
            // Set a password if the standard authentication type is used
            sampleUser.SetPassword("");
            sampleUser.Company = company;

            // The UserLoginInfo object requires a user object Id (Oid).
            // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
            ObjectSpace.CommitChanges(); //This line persists created object(s).
            ((ISecurityUserWithLoginInfo)sampleUser).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(sampleUser));
        }      
        sampleUser.Roles.Add(role);

        // add user
        company = ObjectSpace.FirstOrDefault<Company>(u => u.Code == "WNAL");
        username = "adm.wnal.01";
        sampleUser = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == username);
        if (sampleUser == null)
        {
            sampleUser = ObjectSpace.CreateObject<ApplicationUser>();
            sampleUser.UserName = username;
            // Set a password if the standard authentication type is used
            sampleUser.SetPassword("");
            sampleUser.Company = company;

            // The UserLoginInfo object requires a user object Id (Oid).
            // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
            ObjectSpace.CommitChanges(); //This line persists created object(s).
            ((ISecurityUserWithLoginInfo)sampleUser).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(sampleUser));
        }
        sampleUser.Roles.Add(role);

        ObjectSpace.CommitChanges(); //This line persists created object(s).
    }

    private PermissionPolicyRole CreateRoleCompanyLevel() {
        string name = "Role User Company";
        PermissionPolicyRole role = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(x => x.Name == name);

        if(role == null) {
            role = ObjectSpace.CreateObject<PermissionPolicyRole>();
            role.Name = name;

			role.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.Read, cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            role.AddNavigationPermission(@"Application/NavigationItems/Items/Organization Structure", SecurityPermissionState.Allow);

			role.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
			role.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "StoredPassword", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            role.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
            role.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
            role.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
			role.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
            role.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
        }
        return role;
    }

    private void CreateUserPlantLevel()
    {
        PermissionPolicyRole role = CreateRolePlantLevel();

        string username = "";
        ApplicationUser sampleUser;
        Company company;
        Plant plant;

        company = ObjectSpace.FirstOrDefault<Company>(u => u.Code == "WNLL");
        plant = ObjectSpace.FirstOrDefault<Plant>(x => x.Code == "PNBE" & x.Company == company);

        // add user 
        username = "adm.wnll.pnbe.01";
        sampleUser = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == username);
        if (sampleUser == null)
        {
            sampleUser = ObjectSpace.CreateObject<ApplicationUser>();
            sampleUser.UserName = username;
            // Set a password if the standard authentication type is used
            sampleUser.SetPassword("");
            sampleUser.Company = company;
            sampleUser.Plant = plant;

            // The UserLoginInfo object requires a user object Id (Oid).
            // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
            ObjectSpace.CommitChanges(); //This line persists created object(s).
            ((ISecurityUserWithLoginInfo)sampleUser).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(sampleUser));
        }
        sampleUser.Roles.Add(role);

        // add user
        company = ObjectSpace.FirstOrDefault<Company>(u => u.Code == "WNAL");
        plant = ObjectSpace.FirstOrDefault<Plant>(x => x.Code == "SBHE" & x.Company == company);

        username = "adm.wnal.sbhe.01";
        sampleUser = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == username);
        if (sampleUser == null)
        {
            sampleUser = ObjectSpace.CreateObject<ApplicationUser>();
            sampleUser.UserName = username;
            // Set a password if the standard authentication type is used
            sampleUser.SetPassword("");
            sampleUser.Company = company;
            sampleUser.Plant = plant;

            // The UserLoginInfo object requires a user object Id (Oid).
            // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
            ObjectSpace.CommitChanges(); //This line persists created object(s).
            ((ISecurityUserWithLoginInfo)sampleUser).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(sampleUser));
        }
        sampleUser.Roles.Add(role);

        ObjectSpace.CommitChanges(); //This line persists created object(s).
    }
    private PermissionPolicyRole CreateRolePlantLevel()
    {
        string name = "Role User Plant";
        PermissionPolicyRole role = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(x => x.Name == name);

        if (role == null)
        {
            role = ObjectSpace.CreateObject<PermissionPolicyRole>();
            role.Name = name;

            role.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.Read, cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            role.AddNavigationPermission(@"Application/NavigationItems/Items/Organization Structure", SecurityPermissionState.Allow);

            role.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            role.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "StoredPassword", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            role.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
            role.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
            role.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
            role.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
            role.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
        }
        return role;
    }

    private void CreateCompany()
    {
        string code = "";
        string name = "";
        Company company;

        // create company WINDU NABATINDO LESTARI
        code = "WNLL";
        name = "WINDU NABATINDO LESTARI";
        company = ObjectSpace.FirstOrDefault<Company>(r => r.Code == code);
        if (company == null)
        {
            company = ObjectSpace.CreateObject<Company>();
            company.Code = code;
            company.Name = name;
        }

        code = "WNAL";
        name = "WINDU NABATINDO ABADI";
        company = ObjectSpace.FirstOrDefault<Company>(r => r.Code == code);
        if (company == null)
        {
            company = ObjectSpace.CreateObject<Company>();
            company.Code = code;
            company.Name = name;
        }

        ObjectSpace.CommitChanges(); //This line persists created object(s).
    }

    private void CreatePlant()
    {
        string code = "";
        string name = "";
        Company company;
        Plant plant;

        company = ObjectSpace.FirstOrDefault<Company>(r => r.Code == "WNLL");

        code = "PNBE";
        name = "PUNDU NABATINDO ESTATE";
        plant = ObjectSpace.FirstOrDefault<Plant>(r => r.Code == code & r.Company == company);
        if (plant == null)
        {
            plant = ObjectSpace.CreateObject<Plant>();
            plant.Code = code;
            plant.Name = name;
            plant.Company = company;
        }

        code = "PNRE";
        name = "PANAGA RAYA ESTATE";
        plant = ObjectSpace.FirstOrDefault<Plant>(r => r.Code == code & r.Company == company);
        if (plant == null)
        {
            plant = ObjectSpace.CreateObject<Plant>();
            plant.Code = code;
            plant.Name = name;
            plant.Company = company;
        }

        code = "PNBM";
        name = "PUNDU NABATINDO MILL";
        plant = ObjectSpace.FirstOrDefault<Plant>(r => r.Code == code & r.Company == company);
        if (plant == null)
        {
            plant = ObjectSpace.CreateObject<Plant>();
            plant.Code = code;
            plant.Name = name;
            plant.Company = company;
        }


        company = ObjectSpace.FirstOrDefault<Company>(r => r.Code == "WNAL");

        code = "SBHE";
        name = "SUNGAI BAHAUR ESTATE";
        plant = ObjectSpace.FirstOrDefault<Plant>(r => r.Code == code & r.Company == company);
        if (plant == null)
        {
            plant = ObjectSpace.CreateObject<Plant>();
            plant.Code = code;
            plant.Name = name;
            plant.Company = company;
        }

        code = "SAGM";
        name = "SELUCING AGRO MILL";
        plant = ObjectSpace.FirstOrDefault<Plant>(r => r.Code == code & r.Company == company);
        if (plant == null)
        {
            plant = ObjectSpace.CreateObject<Plant>();
            plant.Code = code;
            plant.Name = name;
            plant.Company = company;
        }


        ObjectSpace.CommitChanges(); //This line persists created object(s).
    }

    private void CreateDivision()
    {
        string code = "";
        string name = "";

        Plant plant;
        Division division;

        #region PNBE        
        plant = ObjectSpace.FirstOrDefault<Plant>(r => r.Code == "PNBE");

        code = "01";
        name = "DIVISI 01";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;
        }

        code = "02";
        name = "DIVISI 02";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;
        }

        code = "03";
        name = "DIVISI 03";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;
        }

        code = "04";
        name = "DIVISI 04";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;
        }

        code = "05";
        name = "DIVISI 05";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;
        }

        code = "06";
        name = "DIVISI 06";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;

        }

        code = "20";
        name = "DIVISI BIBITAN";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;

        }

        code = "30";
        name = "DIVISI ADMINISTRASI";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;

        }

        code = "40";
        name = "DIVISI TRAKSI";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;

        }
        #endregion

        #region SBHE        
        plant = ObjectSpace.FirstOrDefault<Plant>(r => r.Code == "SBHE");

        code = "01";
        name = "DIVISI 01";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;

        }

        code = "02";
        name = "DIVISI 02";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;

        }

        code = "03";
        name = "DIVISI 03";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;

        }

        code = "04";
        name = "DIVISI 04";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;

        }

        code = "05";
        name = "DIVISI 05";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;

        }

        code = "06";
        name = "DIVISI 06";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;

        }

        code = "20";
        name = "DIVISI BIBITAN";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;

        }

        code = "30";
        name = "DIVISI ADMINISTRASI";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;

        }

        code = "40";
        name = "DIVISI TRAKSI";
        division = ObjectSpace.FirstOrDefault<Division>(r => r.Code == code & r.Plant == plant);
        if (division == null)
        {
            division = ObjectSpace.CreateObject<Division>();
            division.Code = code;
            division.Name = name;
            division.Company = plant.Company;
            division.Plant = plant;

        }
        #endregion

        ObjectSpace.CommitChanges(); //This line persists created object(s).
    }
}
