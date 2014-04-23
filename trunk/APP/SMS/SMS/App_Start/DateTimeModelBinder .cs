using System;
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
            var date = DateTime.Now;
            if (value.Culture.ToString().Equals("vi-VN"))
            {
                try
                {
                    return DateTime.ParseExact(value.AttemptedValue, "dd/MM/yyyy", cultureinfo);
                }
                catch
                {
                    return null;
                }

            }
            else if (value.Culture.ToString().Equals("en-US"))
            {
                try
                {
                    date = DateTime.ParseExact(value.AttemptedValue, "dd/MM/yyyy", cultureinfo);
                    return date;
                }
                catch
                {
                    try
                    {
                        date = DateTime.ParseExact(value.AttemptedValue, "MM/dd/yyyy", cultureinfo);
                        return date;
                    }
                    catch
                    {
                        return null;
                    }
                }

            }
            else
            {
                try
                {
                    date = DateTime.ParseExact(value.AttemptedValue, "yyyy/MM/dd", cultureinfo);
                    return date;
                }
                catch
                {
                    return null;
                }
            }
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
                if (value.Culture.ToString().Equals("vi-VN"))
                {
                    try
                    {
                        return DateTime.ParseExact(value.AttemptedValue, "dd/MM/yyyy", cultureinfo);                       
                    }
                    catch
                    {
                        return null;
                    }

                }
                else if (value.Culture.ToString().Equals("en-US"))
                {
                    try
                    {
                        date = DateTime.ParseExact(value.AttemptedValue, "dd/MM/yyyy", cultureinfo);
                        return date;
                    }
                    catch
                    {
                        try
                        {
                            date = DateTime.ParseExact(value.AttemptedValue, "MM/dd/yyyy", cultureinfo);
                            return date;
                        }
                        catch
                        {
                            return null;
                        }
                    }
                   
                }
                else
                {
                    try
                    {
                        date = DateTime.ParseExact(value.AttemptedValue, "yyyy/MM/dd", cultureinfo);
                        return date;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            return null;
        }
    }
}
