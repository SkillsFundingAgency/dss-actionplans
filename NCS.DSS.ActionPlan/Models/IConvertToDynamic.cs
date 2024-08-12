using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;

namespace NCS.DSS.ActionPlan.Models
{
    public interface IConvertToDynamic
    {
        public ExpandoObject RenameAndExcludeProperty(ActionPlan actionPlan, string oldname, string newName, string exclName);

        public ExpandoObject RenameProperty(ActionPlan actionPlan, string name, string newName);

        public IList<ExpandoObject> RenameProperty(IList<ActionPlan> actionPlans, string name, string newName);

        public ExpandoObject ExcludeProperty(ActionPlan actionPlan, string name);

        public IList<ExpandoObject> ExcludeProperty(IList<ActionPlan> actionPlans, string name);

        public void AddProperty(ExpandoObject expando, string propertyName, object propertyValue);
        public ExpandoObject ExcludeProperty(Exception exception, string[] names);

    }
}
