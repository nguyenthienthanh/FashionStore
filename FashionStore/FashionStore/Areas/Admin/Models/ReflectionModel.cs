using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace FashionStore.Areas.Admin.Models
{
    public class ReflectionModel
    {
        //Lấy danh sách Controller
        public List<Type> GetControllers(string _namespace)
        {
            List<Type> controllers = new List<Type>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            IEnumerable<Type> types = assembly.GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type) &&
                type.Namespace.Contains(_namespace)).OrderBy(x => x.Name);
            return types.ToList();
        }

        //Lấy danh sách Action
        public List<string> GetActions(Type controler)
        {
            var actions = new List<string>();
            IEnumerable<MemberInfo> memberInfo = controler.GetMethods(BindingFlags.Instance | 
                BindingFlags.DeclaredOnly | BindingFlags.Public).Where(m => !m.GetCustomAttributes(
                    typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any()).OrderBy(x => x.Name);
            foreach (var method in memberInfo)
            {
                if (method.ReflectedType.IsPublic && !method.IsDefined(typeof(NonActionAttribute)))
                {
                    actions.Add(method.Name.ToString());
                }
            }
            return actions.Distinct().ToList();
        }
    }
}