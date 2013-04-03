/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Chiro.Gap.TestService
{
    public interface IMyDisposable : IDisposable
    {
        string Hello();
    }

    public class MyDisposable: IMyDisposable
    {
        public MyDisposable()
        {
            Debug.WriteLine(String.Format("Creating MyDisposable. {0}", base.GetHashCode()));           
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Debug.WriteLine(String.Format("Disposing MyDisposable. {0}", base.GetHashCode()));
        }

        public string Hello()
        {
            return String.Format("Hello world! {0}", base.GetHashCode());
        }
    }
}