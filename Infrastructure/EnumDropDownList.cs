using System;
using System.Reflection;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MyAdmin.Mvc.Infrastructure
{
    public static class EnumDropDownList
    {
        public static HtmlString EnumDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> modelExpression, string firstElement)
        {
            var typeOfProperty = modelExpression.ReturnType;
            if (typeOfProperty.GetTypeInfo().IsEnum)
            {
                throw new ArgumentException(string.Format("Type {0} is not an enum", typeOfProperty));
            }
            var enumValues = new SelectList(Enum.GetValues(typeOfProperty));
            return htmlHelper.DropDownListFor(modelExpression, enumValues, firstElement) as HtmlString;
        }
    }
}