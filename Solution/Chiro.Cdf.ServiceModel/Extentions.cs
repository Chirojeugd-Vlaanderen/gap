/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿using System;
using System.Collections.Generic;
using System.Linq;

// LET OP! De klasses in dit project hebben niets te maken met ServiceHelper, wel met de
// implementatie van onze message queue services.

namespace Chiro.Cdf.ServiceModel
{
    public static class CollectionExtentions
    {
        public static IEnumerable<U> ConvertAll<T, U>(this IEnumerable<T> collection, Converter<T, U> converter)
        {
            return collection.Select(item => converter(item));
        }
    }

    public static class ArrayExtentions
    {
        public static U[] ConvertAll<T, U>(this T[] array, Converter<T, U> converter)
        {
            IEnumerable<T> enumerable = array;
            return enumerable.ConvertAll(converter).ToArray();
        }
    }
}
