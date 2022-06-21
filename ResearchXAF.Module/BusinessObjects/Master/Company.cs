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

    public class Company : BasePersistentObject
    { 
        public Company(Session session) : base(session)
        {

        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        //private string _PersistentProperty;
        //[XafDisplayName("My display name"), ToolTip("My hint message")]
        //[ModelDefault("EditMask", "(000)-00"), Index(0), VisibleInListView(false)]
        //[Persistent("DatabaseColumnName"), RuleRequiredField(DefaultContexts.Save)]
        //public string PersistentProperty {
        //    get { return _PersistentProperty; }
        //    set { SetPropertyValue(nameof(PersistentProperty), ref _PersistentProperty, value); }
        //}

        //[Action(Caption = "My UI Action", ConfirmationMessage = "Are you sure?", ImageName = "Attention", AutoCommit = true)]
        //public void ActionMethod() {
        //    // Trigger a custom business logic for the current record in the UI (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112619.aspx).
        //    this.PersistentProperty = "Paid";
        //}

        private string _Code;       
        [ModelDefault("EditMask", ">"), Index(0), VisibleInListView(true), Size(10)]
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

        [Association("Company-Plants")]
        public XPCollection<Plant> Plants
        {
            get { return GetCollection<Plant>(nameof(Plants)); }
        }

        [Association("Company-ApplicationUsers")]
        public XPCollection<ApplicationUser> ApplicationUsers
        {
            get { return GetCollection<ApplicationUser>(nameof(ApplicationUsers)); }
        }

    }
}