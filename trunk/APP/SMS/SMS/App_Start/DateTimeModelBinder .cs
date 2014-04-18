﻿using System;
using System.Globalization;
using System.Web.Mvc;

namespace SMS.App_Start
{
    public class DateTimeModelBinder : IModelBinder  
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("vi-VN");
            var date = DateTime.ParseExact(value.AttemptedValue, "dd/MM/yyyy", cultureinfo);
            return date;
        }
    }

    public class NullableDateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value != null)
            {
                System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("vi-VN");
                var  date = DateTime.Now;
                if (!value.Culture.ToString().Equals("vi-VN"))
                {
                    try
                    {
                        string s = value.AttemptedValue;
                        if (!String.IsNullOrEmpty(s) && s.Length > 10)
                        {
                            s = s.Substring(0, 10);
                        }
                        date = DateTime.ParseExact(s, "MM/dd/yyyy", cultureinfo);
                        return date;
                    }
                    catch
                    {
                        return DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyy"), "dd/MM/yyyy", cultureinfo);                       
                    }
                    
                }
                else
                {
                    date = DateTime.ParseExact(value.AttemptedValue, "dd/MM/yyyy", cultureinfo);
                    return date;
                }
            }
            return null;
        }
    }
}
