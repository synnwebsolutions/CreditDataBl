using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CreditDataBl
{
    public class GenericAttributeHelper<TEntity> where TEntity : Attribute, new()
    {
        public List<TEntity> GetClassAttributes(object obj)
        {
            List<TEntity> entities = new List<TEntity>();
            var attributeType = typeof(TEntity);
            entities = obj.GetType().GetCustomAttributes(attributeType, false).Select(x => (TEntity)x).ToList();
            return entities;
        }

        public List<TEntity> GetPropertyAttributes(object obj)
        {
            List<TEntity> entities = new List<TEntity>();

            var objType = obj.GetType();
            var attributeType = typeof(TEntity);
            var properties = objType.GetProperties();
            foreach (PropertyInfo pr in properties)
            {
                GetPropertyAttribute(entities, attributeType, pr);
            }
            entities = objType.GetCustomAttributes(objType, false).Select(x => (TEntity)x).ToList();
            return entities;
        }

        private static void GetPropertyAttribute(List<TEntity> entities, Type attributeType, PropertyInfo pr)
        {
            var at = pr.GetCustomAttribute(attributeType);
            if (at != null)
                entities.Add((TEntity)at);
        }

        internal Dictionary<PropertyInfo, TEntity> GetPropertiesAttributes(object obj)
        {
            var data = new Dictionary<PropertyInfo, TEntity>();
            var objType = obj.GetType();
            var attributeType = typeof(TEntity);
            var properties = objType.GetProperties();
            foreach (PropertyInfo pr in properties)
            {
                var at = pr.GetCustomAttribute(attributeType);
                if (at != null)
                    data.Add(pr, (TEntity)at);
            }
            return data;
        }

    }

}
