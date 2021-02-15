﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace newsroom.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(propertyName); 

            if( valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask; 
            }

            try
            {
                var deserializedValue = JsonConvert.DeserializeObject<T>(valueProviderResult.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(deserializedValue); 
            }
            catch
            {
                bindingContext.ModelState.TryAddModelError(propertyName, "value is invalid for type int ");  
            }

            return Task.CompletedTask; 
        }
    }
}
