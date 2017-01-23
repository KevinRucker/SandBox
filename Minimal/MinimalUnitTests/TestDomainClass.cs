using Minimal.Framework.BaseClasses;
using Minimal.StateTracking;
using Minimal.Utility;
using System;
using System.Data;

namespace MinimalUnitTests
{
    public class Department : PersistentEntityBase
    {
        private ValueStateTracker<string> _name = new ValueStateTracker<string>();
        private ValueStateTracker<string> _groupName = new ValueStateTracker<string>();
        private DateTime? _modifiedDate = default(DateTime?);

        [Flags]
        private enum DataColumns
        {
            DepartmentID,
            Name,
            GroupName,
            ModifiedDate
        }

        public Department()
        {

        }

        public string Name
        {
            get
            {
                return _name.CurrentValue;
            }

            set
            {
                _name.ChangeValue(value);
            }
        }

        public string GroupName
        {
            get
            {
                return _groupName.CurrentValue;
            }

            set
            {
                _groupName.ChangeValue(value);
            }
        }

        public DateTime? ModifiedDate
        {
            get
            {
                return _modifiedDate;
            }
        }

        public override bool IsChanged
        {
            get
            {
                return
                    _name.IsChanged ||
                    _groupName.IsChanged;
            }
        }

        public override void Load(IDataRecord record)
        {
            EntityHelper.SetEntityId(this, record.GetInt16((int)DataColumns.DepartmentID));
            Name = record.GetString((int)DataColumns.Name);
            GroupName = record.GetString((int)DataColumns.GroupName);
            EntityHelper.EntityModifiedDate(this, record.GetDateTime((int)DataColumns.ModifiedDate));
            EntityHelper.EntityAcceptChanges(this);
        }

        private void AcceptChanges()
        {
            _name.AcceptChanges();
            _groupName.AcceptChanges();
        }
    }
}
