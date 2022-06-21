using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ResearchXAF.Module.BusinessObjects.Master
{
    [NavigationItem("Organization Structure")]
    [ObjectCaptionFormat(@"{0:Description}"), DefaultProperty("Description")]
    public class Division : BasePersistentObject
    { 
        public Division(Session session) : base(session)
        {

        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();            
        }
        
        private string _Code;       
        [Index(0), VisibleInListView(true), Size(10)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string Code
        {
            get { return _Code; }
            set { SetPropertyValue(nameof(Code), ref _Code, value); }
        }

        private string _Name;        
        [Index(1), VisibleInListView(true), Size(100)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string Name
        {
            get { return _Name; }
            set { SetPropertyValue(nameof(Name), ref _Name, value); }
        }

        [VisibleInListView(false)]
        [PersistentAlias("Concat(Code, ' - ', Name)")]
        public string Description
        {
            get
            {
                string namePart = string.Format("{0} - {1}", Code, Name);
                return namePart;
            }
        }

        private Company _Company;
        [Persistent("CompanyOid"), RuleRequiredField(DefaultContexts.Save)]        
        public Company Company
        {
            get
            {                                
                return _Company;
            }
            set 
            {
                if (value != _Company) _Plant = null;
                SetPropertyValue(nameof(Company), ref _Company, value); 
            }
        }

        private Plant _Plant;
        [Association("Plant-Divisions")]
        [Persistent("PlantOid"), RuleRequiredField(DefaultContexts.Save)]
        [DataSourceProperty("Company.Plants", DataSourcePropertyIsNullMode.SelectNothing)]
        public Plant Plant
        {
            get { return _Plant; }
            set { SetPropertyValue(nameof(Plant), ref _Plant, value); }
        }

        [Association("Division-Employees")]
        public XPCollection<Employee> Employees
        {
            get { return GetCollection<Employee>(nameof(Employees)); }
        }

        [Association("Division-ApplicationUsers")]
        public XPCollection<ApplicationUser> ApplicationUsers
        {
            get { return GetCollection<ApplicationUser>(nameof(ApplicationUsers)); }
        }

    }
}