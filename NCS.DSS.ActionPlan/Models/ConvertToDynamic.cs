using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace NCS.DSS.ActionPlan.Models
{
    public class ConvertToDynamic : IConvertToDynamic
    {
        public ExpandoObject RenameAndExcludeProperty(ActionPlan actionPlan, string oldname, string newName, string exclName)
        {
            var updatedObject = new ExpandoObject();
            foreach (var item in typeof(Models.ActionPlan).GetProperties())
            {
                if (item.Name == exclName)
                    continue;
                var itemName = item.Name;
                if (itemName == oldname)
                    itemName = newName;
                AddProperty(updatedObject, itemName, item.GetValue(actionPlan));
            }
            return updatedObject;
        }
        public ExpandoObject RenameProperty(ActionPlan actionPlan, string name, string newName)
        {
            var updatedObject = new ExpandoObject();
            foreach (var item in typeof(ActionPlan).GetProperties())
            {
                var itemName = item.Name;
                if (itemName == name)
                    itemName = newName;
                AddProperty(updatedObject, itemName, item.GetValue(actionPlan));
            }
            return updatedObject;
        }
        public IList<ExpandoObject> RenameProperty(IList<ActionPlan> actionPlans, string name, string newName)
        {
            var updatedObjects = new List<ExpandoObject>();
            foreach (var actionPlan in actionPlans)
            {
                updatedObjects.Add(RenameProperty(actionPlan, name, newName));
            }

            return updatedObjects;
        }
        public ExpandoObject ExcludeProperty(ActionPlan actionPlan, string name)
        {
            dynamic updatedObject = new ExpandoObject();
            foreach (var item in typeof(ActionPlan).GetProperties())
            {
                if (item.Name == name)
                    continue;
                AddProperty(updatedObject, item.Name, item.GetValue(actionPlan));
            }
            return updatedObject;
        }
        public ExpandoObject ExcludeProperty(Exception exception, string[] names)
        {
            dynamic updatedObject = new ExpandoObject();
            foreach (var item in typeof(Exception).GetProperties())
            {
                if (names.Contains(item.Name))
                    continue;

                AddProperty(updatedObject, item.Name, item.GetValue(exception));
            }
            return updatedObject;
        }
        public IList<ExpandoObject> ExcludeProperty(IList<ActionPlan> actionPlans, string name)
        {
            var updatedObjects = new List<ExpandoObject>();
            foreach (var actionPlan in actionPlans)
            {
                updatedObjects.Add(ExcludeProperty(actionPlan, name));
            }

            return updatedObjects;
        }
        public void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
    }
}
