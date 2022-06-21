using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using ResearchXAF.Module.BusinessObjects.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ResearchXAF.Module.BusinessObjects.Transaction
{
    [NavigationItem("Transaction")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Harvesting : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Harvesting(Session session)
            : base(session)
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

        private string _DocumentNumber;
        [Index(0), VisibleInListView(true), Size(10)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string DocumentNumber
        {
            get { return _DocumentNumber; }
            set { SetPropertyValue(nameof(DocumentNumber), ref _DocumentNumber, value); }
        }

        private string _DocumentDate;
        [Index(0), VisibleInListView(true), Size(10)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string DocumentDate
        {
            get { return _DocumentDate; }
            set { SetPropertyValue(nameof(DocumentDate), ref _DocumentDate, value); }
        }

        private Company _Company;
        [Persistent("CompanyOid"), RuleRequiredField(DefaultContexts.Save)]
        public Company Company
        {
            get { return _Company; }
            set
            {
                if (value != _Company) _Plant = null;
                SetPropertyValue(nameof(Company), ref _Company, value);
            }
        }

        private Plant _Plant;
        [Persistent("PlantOid"), RuleRequiredField(DefaultContexts.Save)]
        [DataSourceProperty("Company.Plants", DataSourcePropertyIsNullMode.SelectNothing)]
        public Plant Plant
        {
            get { return _Plant; }
            set
            {
                if (value != _Plant) _Division = null;
                SetPropertyValue(nameof(Plant), ref _Plant, value);
            }
        }

        private Division _Division;        
        [Persistent("DivisionOid"), RuleRequiredField(DefaultContexts.Save)]
        [DataSourceProperty("Plant.Divisions", DataSourcePropertyIsNullMode.SelectNothing)]
        public Division Division
        {
            get { return _Division; }
            set { SetPropertyValue(nameof(Division), ref _Division, value); }
        }

        [Association("Harvesting-Details")]
        public XPCollection<HarvestingDetail> Details
        {
            get { return GetCollection<HarvestingDetail>(nameof(Details)); }
        }

    }
}